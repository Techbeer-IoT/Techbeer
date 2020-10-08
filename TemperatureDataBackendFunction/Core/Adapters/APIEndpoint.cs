using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Adapters {
    public abstract class APIEndpoint {

        public abstract List<string> GetEndpoint();

        public abstract Util.HttpRequest.MethodTy GetMethodTy();

        public abstract void SetDataForRequest<T>(T data) where T : new();

        public abstract Dictionary<string, string> GetHeaders();

        public abstract APIEndpointModel GetBodyModel();

        public class APIEndpointModel { }
    }
}
