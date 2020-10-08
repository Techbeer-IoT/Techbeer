using System;
using System.Collections.Generic;
using System.Text;
using FunctionStoreData.API;

namespace FunctionStoreData.Util {
    public static class ApiFactory {
        public enum APIType {
            LINE,
            POWERBI,
            POWERBI_HYBRID
        }

        public static Core.Adapters.APIEndpoint GetEndpoint(APIType type) {
            switch (type) {
                case APIType.LINE:
                    return new LineAPI();
                case APIType.POWERBI:
                    return new PowerBIAPI();
                case APIType.POWERBI_HYBRID:
                    return new PowerBIHybridAPI();
                default:
                    return new PowerBIAPI();
            }
        }
            
    }
}
