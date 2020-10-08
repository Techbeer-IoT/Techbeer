using System;
using System.Threading.Tasks;
using Core.Adapters;
using Core.Entities;
using Microsoft.Azure.Cosmos.Table;

namespace Storage {
    public class DataCreator : DataCreatorAdapter {

        public DataCreator(Core.AppSettings appSettings, Core.Util.ILog log) : base(appSettings, log) { }

        public async override Task InsertOrMergeTelemetry(TemperatureTelemetry telemetry, DateTime date) {
            var storageManager = new CloudStorageManager(Log);
            var tableManager = new StorageTableManager(Log);
            var table = await storageManager.GetCloudTableAsync(Settings.Storage.StorageTableName, Settings.Storage.ConnectionString);

            var tableKeys = TableKeysCreator.CreateFromDate(date);

            var mapper = new TelemetryMaper();
            var azureTelemetry = mapper.MapToAzure(telemetry, tableKeys.PartitionKey, tableKeys.RowKey);

            var operation = TableOperation.InsertOrMerge(azureTelemetry);
            await tableManager.ExecuteOperation<TableTelemetryEntity>(table, operation);
        }
    }
}
