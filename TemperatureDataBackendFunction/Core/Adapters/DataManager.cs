using System;
using System.Collections.Generic;
using System.Text;
using Core.Util;

namespace Core.Adapters {
    public class DataManager {
        protected AppSettings Settings { get; set; }
        protected ILog Log { get; set; }

        public DataManager(AppSettings appSettings, ILog log) {
            Settings = appSettings;
            Log = log;
        }
    }
}
