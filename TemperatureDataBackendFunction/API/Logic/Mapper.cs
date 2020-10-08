using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Logic {
    public class Mapper {

        public Mapper() {

        }

        public Model.TemperatureModel Map(Core.Entities.TemperatureTelemetry telemetry) =>
            new Model.TemperatureModel() {
                temperature = telemetry.temperature.ToString(),
                peltierStatus = telemetry.peltierStatus.ToString()
            };
    }
}
