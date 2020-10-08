using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Structure of the Azure Table Storage
/// </summary>
namespace Storage {
    internal class TableTelemetryEntity : TableEntity { 

        public TableTelemetryEntity() {

        }

        public TableTelemetryEntity(string date, string time) {
            PartitionKey = date;
            RowKey = time;
        }

        public double temperature { get; set; }
        public double temperatureScale { get; set; }
        public int peltierStatus { get; set; }
        public int peltierOFF { get; set; }
        public int peltierHEAT { get; set; }
        public int peltierCOOL { get; set; }
    }
}
