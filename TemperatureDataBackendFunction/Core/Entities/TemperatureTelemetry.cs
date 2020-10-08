using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities {
    public class TemperatureTelemetry {
        public float temperature { get; set; }
        public float temperatureScale { get; set; }
        public PeltiesStatusTy peltierStatus { get; set; }
        public int peltierOFF { get; set; }
        public int peltierHEAT { get; set; }
        public int peltierCOOL { get; set; }
    }
}
