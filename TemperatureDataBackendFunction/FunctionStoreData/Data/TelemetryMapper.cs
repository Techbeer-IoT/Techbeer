using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionStoreData.Data {
    internal class TelemetryMapper {
        private readonly Util.TableKeysCreator.TableKeys _tableKeys;

        public TelemetryMapper(DateTime dateTime) {
            _tableKeys = Util.TableKeysCreator.CreateFromDate(dateTime);
        }

        public TableTelemetryEntity MapToStorage(Core.Entities.TemperatureTelemetry telemetry) =>
            new TableTelemetryEntity(_tableKeys.PartitionKey, _tableKeys.RowKey) {
                temperature = telemetry.temperature,
                temperatureScale = telemetry.temperatureScale,
                peltierStatus = (int)telemetry.peltierStatus,
                peltierCOOL = telemetry.peltierCOOL,
                peltierHEAT = telemetry.peltierHEAT,
                peltierOFF = telemetry.peltierOFF
            };
    }
}
