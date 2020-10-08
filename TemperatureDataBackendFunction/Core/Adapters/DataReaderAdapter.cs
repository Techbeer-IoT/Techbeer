using Core.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Adapters {
    public abstract class DataReaderAdapter : DataManager {

        public DataReaderAdapter(AppSettings appSettings, ILog log) : base(appSettings, log) { }

        public abstract Task<Entities.TemperatureTelemetry> GetTelemetry(DateTime date);

    }
}
