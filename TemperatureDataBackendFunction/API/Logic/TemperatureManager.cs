using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Logic {
    public class TemperatureManager {

        public TemperatureManager() {

        }

        public async Task<Model.TemperatureModel> GetTemperature(Core.Adapters.DataReaderAdapter dataReader, DateTime dateTime) {
            var telemetry = await dataReader.GetTelemetry(dateTime);

            var mapper = new Mapper();

            return mapper.Map(telemetry);
        }
    }
}
