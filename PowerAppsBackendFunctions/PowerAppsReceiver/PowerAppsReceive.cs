using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PowerAppsReceiver
{
    public static class PowerAppsReceive
    {
        [FunctionName("PowerAppsReceive")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // IoT Hub (Service) Connection Key
            var connectionString = "";

            string date = DateTime.Now.ToString("yyyyMMddHHmmss");
            var message = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(date));
            try
            {
                var sc = ServiceClient.CreateFromConnectionString(connectionString);
                await sc.SendAsync("myDeviceTest1", message);
            }
            catch (Exception e)
            {
                log.LogInformation(e.ToString());
            }

            log.LogInformation("request images");


            return new OkObjectResult("");
        }
    }
}
