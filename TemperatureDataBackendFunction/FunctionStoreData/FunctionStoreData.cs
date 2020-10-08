using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using Core.Entities;

namespace FunctionStoreData {
    public class FunctionStoreData {
        private Core.Adapters.DataCreatorAdapter DataCreator;
        private Core.Adapters.DataReaderAdapter DataReader;

        [FunctionName("FunctionStoreData")]
        [return: Table("TTemperature", Connection = "StorageConnectionString")]
        public async Task<Data.TableTelemetryEntity> Run([IoTHubTrigger("messages/events", Connection = "IoTHubEndPoint", ConsumerGroup = "techbeer")]EventData message, ILogger log) {
            var payload = Encoding.UTF8.GetString(message.Body.Array);
            log.LogInformation($"IoTHub Message: {payload}");

            InitSettings(log);

            var telemetry = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.Entities.TemperatureTelemetry>(payload);

            await SendNotification(telemetry, log);

            log.LogInformation("Send request to PowerBi...");
            var response = SendData(Util.ApiFactory.APIType.POWERBI, telemetry);
            log.LogInformation($"PowerBi Response: {response}");
            response = SendData(Util.ApiFactory.APIType.POWERBI_HYBRID, telemetry);
            log.LogInformation($"PowerBi Hybrid Response: {response}");


            var mapper = new Data.TelemetryMapper(DateTime.UtcNow.AddHours(9));

            return mapper.MapToStorage(telemetry);

            //await DataCreator.InsertOrMergeTelemetry(telemetry, DateTime.UtcNow.AddHours(9));
        }

        private void InitSettings(ILogger log) {
            var settings = new Core.AppSettings();
            var myLogger = new Util.MyLogger(log);

            settings.Storage = new Core.AppSettings.StorageSection() {
                StorageTableName = Settings.StorageTableName(),
                ConnectionString = Settings.StorageConnectionString()
            };

            DataCreator = new Storage.DataCreator(settings, myLogger);
            DataReader = new Storage.DataReader(settings, myLogger);
        }

        private async Task SendNotification(Core.Entities.TemperatureTelemetry telemetry, ILogger log) {
            var lastData = await DataReader.GetTelemetry(DateTime.Now);

            log.LogInformation($"CurrentStatus: {lastData.peltierStatus}");
            log.LogInformation($"TelemetryStatus: {telemetry.peltierStatus}");

            if (telemetry.peltierStatus != lastData.peltierStatus && Settings.LineSendMessage()) {
                log.LogInformation("Send request to LINE...");
                var response = SendData(Util.ApiFactory.APIType.LINE, telemetry);
                log.LogInformation($"Line Notification Response: {response}");
            }
        }
    
        private string SendData(Util.ApiFactory.APIType apiType, Core.Entities.TemperatureTelemetry telemetry) {
            var endpoint = Util.ApiFactory.GetEndpoint(apiType);
            endpoint.SetDataForRequest(telemetry);

            var postRequest = new Core.Util.HttpRequest(endpoint);
            return postRequest.MakeRequest();
        }
    }
}