using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;
using Core.Util;

namespace Storage {
    internal class StorageTableManager {

        private ILog _log;

        public StorageTableManager(ILog log) {
            _log = log;
        }

        public async Task ExecuteOperation<T>(CloudTable table, TableOperation operation) where T : TableEntity {
            try {
                await table.ExecuteAsync(operation);                
            } catch (StorageException e) {
                _log.LogError(e.Message);
                throw;
            }
        }

        public T QueryData<T>(CloudTable table, TableQuery<T> query) where T : TableEntity, new() {
            try {
                var result = table.ExecuteQuery(query).ToList();

                T resultItem = null;

                if (result.Count > 0) {
                    resultItem = result.OrderByDescending(x => x.Timestamp).ElementAt<T>(0);
                }

                return resultItem;

            } catch (StorageException e) {
                _log.LogError(e.Message);
                throw;
            }
        }
    }
}
