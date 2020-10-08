using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureController : ControllerBase {

        private readonly Core.Adapters.DataReaderAdapter DataReader;

        public TemperatureController(IOptions<Core.AppSettings> settings) {
            DataReader = new Storage.DataReader(settings.Value, new Logic.MyLogger());
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<Model.TemperatureModel>> Get() {
            var tempManager = new Logic.TemperatureManager();
            var currentTemp = await tempManager.GetTemperature(DataReader, DateTime.Now);

            return currentTemp;
        }
    }
}
