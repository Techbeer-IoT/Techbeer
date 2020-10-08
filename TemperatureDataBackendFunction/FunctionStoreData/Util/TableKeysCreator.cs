using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionStoreData.Util {
    internal class TableKeysCreator {
        public static TableKeys CreateFromDate(DateTime date) =>
            new TableKeys() {
                PartitionKey = date.ToString("yyyy/MM/dd").Replace("/", "_"),
                RowKey = date.ToString("HH:mm:ss").Replace(":", "_")
            };

        internal class TableKeys {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
        }
    }
}
