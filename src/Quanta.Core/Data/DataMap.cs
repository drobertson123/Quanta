using System;

namespace Quanta.Core.Data
{
    public class DataMap<TData, TStruct> 
            where TStruct : struct
    {
        private Func<TStruct, IMetaData, TData> _toData;
        private Func<TData, TStruct> _toStruct;

        protected void SetCreateDataFunc(Func<TStruct, IMetaData, TData> toData) => _toData = toData;
        protected void SetCreateStructFunc(Func<TData, TStruct> toStruct) => _toStruct = toStruct;
        public virtual TData ToData(TStruct data, IMetaData meta) => _toData(data, meta);
        public virtual TStruct ToStruct(TData data) => _toStruct(data);
    }
}
