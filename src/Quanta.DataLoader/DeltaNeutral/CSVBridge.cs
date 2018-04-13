using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
//using OptionsEDU.Data.Exceptions;

namespace Quanta.DataLoader.DeltaNeutral
{
    public class CSVBridge<T>
    {
        protected ReaderWriterLockSlim _syncLock = new ReaderWriterLockSlim();

        protected CsvHelper.Configuration.Configuration _config;
        protected ClassMap _map;

        public CSVBridge(ClassMap map = null, bool hasHeader = true)
        {
            _map = map;
            _config = new Configuration() { HasHeaderRecord = hasHeader };

            //override IgnoreReadingExceptions so we can do internal error management
            //_config.IgnoreReadingExceptions = true;
        }

        public CSVBridge(Configuration config, ClassMap map = null)
        {
            _map = map;
            _config = config;

            //override IgnoreReadingExceptions so we can do internal error management
            //_config.IgnoreReadingExceptions = true;
        }

        void HandleError(Exception ex)
        {
            throw ex;
            //if (ex is CsvHelper.MissingFieldException) errors.AddFetchException($"CSV Missing Field Error: {ex.Message}", this, ex);
            //else if (ex is TypeConverterException) errors.AddFetchException($"CSV TypeConverter Error: {ex.Message} on data field {ex.Data}", this, ex);
            //else if (ex is CsvHelperException) errors.AddFetchException($"CSV Helper Error: {ex.Message}", this, ex);
            //else if (ex is BadDataException) errors.AddFetchException($"CSV Bad Data Error: {ex.Message}", this, ex);
            //else errors.AddFetchException($"CSV Error: {ex.Message}", this, ex);
        }

        public virtual  IEnumerable<T> GetData(string FilePath) => GetData(FilePath, null);
        public virtual  IEnumerable<T> GetData(string FilePath, Func<CsvReader, bool> filter)
        {
            IEnumerable<T> info = (IEnumerable<T>)null;
            //ExceptionList errors = new ExceptionList();

            List<T> quotes = new List<T>();

            try
            {
                _syncLock.EnterReadLock();

                _config.ReadingExceptionOccurred = (ex) => HandleError(ex);
                //_config.BadDataCallback = (ex, row) => HandleError(ex, ref errors);

                if (_map != null) _config.RegisterClassMap(_map);
                else
                    _config.AutoMap<T>();

                using (TextReader textReader = File.OpenText(FilePath))
                using (CsvReader reader = new CsvReader(textReader, _config))
                {
                    if (filter is null) quotes.AddRange(reader.GetRecords<T>().ToList());
                    else
                        while (reader.Read())
                            if (filter(reader)) quotes.Add(reader.GetRecord<T>());
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //errors.AddOperationException($"CSV could get get info from {FilePath}.", FilePath, ex);
            }
            finally { _syncLock.ExitReadLock(); }

            //info = await Task.FromResult(quotes);
            //errors = await Task.FromResult(errors);

            return quotes.AsEnumerable<T>();
        }


        public Configuration Config => _config;
        public ClassMap Map => _map;
        public void Dispose()
        {
            _syncLock.Dispose();
        }

    }
}

