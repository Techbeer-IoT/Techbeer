using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;

namespace Storage {
    internal class TelemetryMaper {

        /// <summary>
        /// Map from the Core Entity to Azure Table Storage Entity
        /// </summary>
        /// <param name="telemetry"></param>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public TableTelemetryEntity MapToAzure(TemperatureTelemetry telemetry, string date, string time) =>
            new TableTelemetryEntity(date, time) {
                temperature = telemetry.temperature,
                temperatureScale = telemetry.temperatureScale,
                peltierStatus = (int)telemetry.peltierStatus,
                peltierCOOL = telemetry.peltierCOOL,
                peltierHEAT = telemetry.peltierHEAT,
                peltierOFF = telemetry.peltierOFF
            };

        /// <summary>
        /// Map from Azure Table Storage Entity to Core Entity
        /// </summary>
        /// <param name="telemetry"></param>
        /// <returns></returns>
        public TemperatureTelemetry MapToCommon(TableTelemetryEntity telemetry) =>
            new TemperatureTelemetry() {
                temperature = (float)telemetry.temperature,
                temperatureScale = (float)telemetry.temperatureScale,
                peltierStatus = (Core.PeltiesStatusTy)telemetry.peltierStatus,
                peltierCOOL = telemetry.peltierCOOL,
                peltierHEAT = telemetry.peltierHEAT,
                peltierOFF = telemetry.peltierOFF
            };
    }
}
