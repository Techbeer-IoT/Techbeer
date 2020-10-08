using System;
using System.Collections.Generic;
using System.Text;

namespace Core {
    public class AppSettings {

        public AppSettings() {
        }

        public StorageSection Storage { get; set; }
        public IoTDeviceSection IoTDevice { get; set; }
        
        public class StorageSection {
            public string StorageTableName { get; set; }
            public string ConnectionString { get; set; }
        }

        public class IoTDeviceSection {
            public string TempDataFilePath { get; set; }
            public string IoTDeviceConnectionString { get; set; }
            public int StartHeatTemp { get; set; }
            public int StartCoolTemp { get; set; }
            public int StopHeatTemp { get; set; }
            public int StopCoolTemp { get; set; }
        }
    }
}
