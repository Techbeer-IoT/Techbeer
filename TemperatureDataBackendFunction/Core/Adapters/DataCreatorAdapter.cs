using Core.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Adapters {
    public abstract class DataCreatorAdapter : DataManager {

        public DataCreatorAdapter(AppSettings appSettings, ILog log) : base(appSettings, log) { }

        public abstract Task InsertOrMergeTelemetry(Entities.TemperatureTelemetry telemetry, DateTime time);

    }
}
