using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Quanta.Core.Data;

namespace Quanta.Core.Repository
{
    public struct Header
    {
        public int MetaLen;
        public int ItemCount;
    }

    public enum WriteType
    {
        Update,     // Updates current equivalent values and adds new items
        Append,     // Appends new items but does not change current items
        Overwrite   // Completely replaces current list with new data 
    }
    /// <summary>
    /// A Session is the connection to one file that contains one type of data element. 
    /// The data elements may be defined by metadata that applies to all the data elements in the file. 
    /// </summary>
    public unsafe class Session<TSession, TSet, TData, TStruct, TMap, TMeta>
        where TSession : Session<TSession, TSet, TData, TStruct, TMap, TMeta>, new()
        where TSet : Set<TData, TMeta>, new()
        where TStruct : struct
        where TMap : DataMap<TData, TStruct>, new()
        where TMeta : IMetaData
    {
        private ReaderWriterLockSlim _syncLock = new ReaderWriterLockSlim();

        private string _fileName;
        private string _mapName;
        private TSet _data;
        private bool _initialized = false;

        public Session() { }
        public Session(string fileName, string mapKey, bool preLoad = false)
        {
            Initialize(fileName, mapKey);
            if (preLoad) Read();
        }

        public void Initialize(string fileName, string mapKey)
        {
            _fileName = fileName;
            _mapName = mapKey;
            _data = null;

            _initialized = true;
        }

        public virtual TSession CreateSession(string fileName, string mapKey, bool preLoad = false)
        {
            TSession session = new TSession();
            session.Initialize(fileName, mapKey);
            return session;
        }

        public virtual TSet Read()
        {
            if (!_initialized) throw new NotSupportedException("Reading from an uninitialized Session is not supported");

            if (_data != null) return _data;
            _data = new TSet();

            if (!File.Exists(_fileName)) return _data;

            using (MemoryMappedFile memFile = MemoryMappedFile.CreateFromFile(_fileName, FileMode.Open, _mapName, 0, MemoryMappedFileAccess.Read))
            using (MemoryMappedViewStream view = memFile.CreateViewStream(0, 0, MemoryMappedFileAccess.Read))
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                long headerSize = Unsafe.SizeOf<Header>();
                TMap map = new TMap();
                byte* ptr = (byte*)0;

                try
                {
                    view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                    byte* loc = ptr;

                    Header info = Unsafe.Read<Header>(loc);
                    loc += headerSize;

                    byte[] metaBytes = new byte[info.MetaLen];

                    Marshal.Copy((IntPtr)loc, metaBytes, 0, info.MetaLen);
                    loc += info.MetaLen;
                    _data.MetaData = MetaFormatter.Deserialize<TMeta>(metaBytes);

                    int len = Unsafe.SizeOf<TStruct>();

                    for (int i = 0; i < info.ItemCount; ++i)
                    {
                        _data.Add(map.ToData(Unsafe.Read<TStruct>(loc), _data.MetaData));
                        loc += len;
                    }

                }
                finally
                {
                    if (ptr != null)
                        view.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }

            return _data;
        }
        public void Write(TSet items, WriteType writeType = WriteType.Update)
        {
            if (!_initialized) throw new NotSupportedException("Writing to an uninitialized Session is not supported");

            _syncLock.EnterWriteLock();
            try
            {
                _data = Read();
                if (_data.Count == 0 || writeType == WriteType.Overwrite)
                {
                    _data = items;
                }
                else
                {
                    if (writeType == WriteType.Update)
                    {
                        _data.Update(items);
                        _data.MetaData = items.MetaData;
                    }
                    else if (writeType == WriteType.Append)
                    {
                        _data.Append(items);
                        _data.MetaData = items.MetaData;
                    }
                }

                long fileSize = CalculateSize(_data.MetaData, _data.Count);

                byte[] metaBytes = MetaFormatter.Serialize<TMeta>(_data.MetaData);

                long headerSize = Unsafe.SizeOf<Header>();
                long dataStart = Unsafe.SizeOf<Header>() + metaBytes.Length;
                long dataEnd = dataStart + (Unsafe.SizeOf<TStruct>() * items.Count);

                Header info = new Header
                {
                    MetaLen = metaBytes.Length,
                    ItemCount = items.Count
                };


                using (MemoryMappedFile memFile = MemoryMappedFile.CreateFromFile(_fileName, FileMode.Create, _mapName, fileSize, MemoryMappedFileAccess.ReadWrite))
                using (MemoryMappedViewStream view = memFile.CreateViewStream(0, 0, MemoryMappedFileAccess.Write))
                {
                    TMap map = new TMap();

                    RuntimeHelpers.PrepareConstrainedRegions();
                    byte* ptr = (byte*)0;
                    try
                    {

                        view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                        byte* start = ptr;
                        byte* end = ptr + fileSize;

                        Unsafe.Write<Header>(start, info);
                        start += headerSize;

                        Marshal.Copy(metaBytes, 0, IntPtr.Add(new IntPtr(ptr), (int)headerSize), metaBytes.Length);
                        start += metaBytes.Length;

                        int len = Unsafe.SizeOf<TStruct>();

                        foreach (TData item in items)
                        {
                            TStruct strct = map.ToStruct(item);
                            Unsafe.Write<TStruct>(start, strct);
                            start += len;
                        }
                    }
                    finally
                    {
                        if (ptr != null)
                            view.SafeMemoryMappedViewHandle.ReleasePointer();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _syncLock.ExitWriteLock();
            }
        }

        public void Delete()
        {
            if (!_initialized) throw new NotSupportedException("Deleting an uninitialized Session is not supported");
            // Caution this deletes the entire file - no rollback
            _syncLock.EnterWriteLock();
            try
            {
                if (File.Exists(_fileName)) File.Delete(_fileName);
                _data = default(TSet);
            }
            finally { _syncLock.ExitWriteLock(); }
        }
        public void Delete(TSet items)
        {
            if (!_initialized) throw new NotSupportedException("Deleting from an uninitialized Session is not supported");

            _data.Remove(items);
            _data.MetaData = items.MetaData;

            Write(_data, WriteType.Overwrite);
        }

        #region Utilities
        internal long CalculateSize(TMeta metadata, int count)
        {
            long metaSize = MetaFormatter.Serialize<TMeta>(metadata).Length;
            long headerSize = Unsafe.SizeOf<Header>();
            long dataSize = Unsafe.SizeOf<TStruct>() * count;

            return metaSize + headerSize + dataSize;
        }
        #endregion Utilities
    }
}
