using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Core.Util;

namespace Storage {
    internal class CloudStorageManager {

        private ILog _log;

        public CloudStorageManager(ILog log) {
            _log = log;
        }

        private CloudStorageAccount CreateStorageAccountFromConnectionString(string connectionString) {

            try {
                return CloudStorageAccount.Parse(connectionString);
            } catch (FormatException e1) {
                _log.LogError(e1.Message);
                throw;
            } catch (ArgumentException e2) {
                _log.LogError(e2.Message);
                throw;
            }
        }

        public async Task<CloudTable> GetCloudTableAsync(string tableName, string connectionString) {
            var storageAccount = CreateStorageAccountFromConnectionString(connectionString);

            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var table = tableClient.GetTableReference(tableName);
            bool isTableCreate = await table.CreateIfNotExistsAsync();

            if (isTableCreate) {
                _log.LogInfo($"New Azure Storage Table has been created: {tableName}");
            }

            return table;
        }

    }
}
