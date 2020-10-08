using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Structure of the Azure Table Storage
/// </summary>
namespace FunctionStoreData.Data {
    internal class TableTelemetryEntity : TableEntity { 

        public TableTelemetryEntity(string partitionKey, string rowKey) {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public double temperature { get; set; }
        public double temperatureScale { get; set; }
        public int peltierStatus { get; set; }
        public int peltierOFF { get; set; }
        public int peltierHEAT { get; set; }
        public int peltierCOOL { get; set; }
    }
}
