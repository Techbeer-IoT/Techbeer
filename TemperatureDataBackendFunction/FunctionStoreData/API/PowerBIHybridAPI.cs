﻿using System;
using System.Collections.Generic;
using System.Text;
using Core.Util;

namespace FunctionStoreData.API {
    public class PowerBIHybridAPI  : Core.Adapters.APIEndpoint {

        private Core.Entities.TemperatureTelemetry _telemetry;

        public override List<string> GetEndpoint() =>
            Settings.PowerBIHybridEndpoint();

        public override HttpRequest.MethodTy GetMethodTy() =>
            HttpRequest.MethodTy.POST;

        public override void SetDataForRequest<T>(T data) =>
             _telemetry = data as Core.Entities.TemperatureTelemetry;

        public override Dictionary<string, string> GetHeaders() =>
            new Dictionary<string, string>() {
                { "Content-Type", "application/json" }
            };

        public override APIEndpointModel GetBodyModel() =>
            new PowerBIHybridBodyModel() {
                rows = new HybridItem[] { GetHybridItem() }
            };


        public HybridItem GetHybridItem() =>
            new HybridItem() {
                temperature = _telemetry.temperature,
                temperatureScale = _telemetry.temperatureScale,
                peltierStatus = (int)_telemetry.peltierStatus,
                peltierHEAT = _telemetry.peltierHEAT,
                peltierOFF = _telemetry.peltierOFF,
                peltierCOOL = _telemetry.peltierCOOL,
                japanTime = DateTime.UtcNow.AddHours(9)
            };

        public class PowerBIHybridBodyModel : APIEndpointModel {
            public HybridItem[] rows { get; set; }
        }

        public class HybridItem {
            public float temperature { get; set; }
            public float temperatureScale { get; set; }
            public int peltierStatus { get; set; }
            public int peltierOFF { get; set; }
            public int peltierHEAT { get; set; }
            public int peltierCOOL { get; set; }
            public DateTime japanTime { get; set; }
        }

    }
}
