using Quanta.Core.Data;
using System;

namespace Quanta.Core.Repository
{
    public interface IRouter<TMeta>
       where TMeta : IMetaData
    {
        string GetRoute(TMeta meta);
        string GetKey(TMeta meta);
    }
    public class Router<TMeta> : IRouter<TMeta>
       where TMeta : IMetaData
    {
        protected Func<TMeta, string> _routeMap = (_) => throw new NotImplementedException();
        protected Func<TMeta, string> _routeKey = (_) => throw new NotImplementedException();

        public Router() { }
        public Router(Func<TMeta, string> map, Func<TMeta, string> key)
        {
            _routeMap = map;
            _routeKey = key;
        }
        protected void SetRouteFunc(Func<TMeta, string> map) => _routeMap = map;
        protected void SetKeyFunc(Func<TMeta, string> key) => _routeKey = key;
        public virtual string GetRoute(TMeta meta) => _routeMap(meta);
        public virtual string GetKey(TMeta meta) => _routeMap(meta);
    }
}
