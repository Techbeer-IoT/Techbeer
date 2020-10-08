using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Adapters;
using Core.Entities;
using Microsoft.Azure.Cosmos.Table;

namespace Storage {
    public class DataReader : DataReaderAdapter {

        public DataReader(Core.AppSettings appSettings, Core.Util.ILog log) : base(appSettings, log) { }

        public async override Task<TemperatureTelemetry> GetTelemetry(DateTime date) {
            var storageManager = new CloudStorageManager(Log);
            var table = await storageManager.GetCloudTableAsync(Settings.Storage.StorageTableName, Settings.Storage.ConnectionString);

            var telemetry = GetTelemetryData(table, date);

            return new TelemetryMaper().MapToCommon(telemetry);
        }

        private TableTelemetryEntity GetTelemetryData(CloudTable table, DateTime targetDate) {
            TableTelemetryEntity telemetry = null;

            while(telemetry == null) {
                var tableKeys = TableKeysCreator.CreateFromDate(targetDate);

                var tableManager = new StorageTableManager(Log);

                var query = GetTableQuery(tableKeys.PartitionKey, targetDate);

                telemetry = tableManager.QueryData(table, query);

                if(telemetry == null) {
                    targetDate = targetDate.AddDays(-1);
                    targetDate = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, 23, 59, 59);
                }
            }

            return telemetry;
        }

        private TableQuery<TableTelemetryEntity> GetTableQuery(string partitionKey, DateTime targetDate) =>
            new TableQuery<TableTelemetryEntity>()
                .Where(TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThanOrEqual, targetDate)))
                .OrderByDesc("RowKey");
    }
}
