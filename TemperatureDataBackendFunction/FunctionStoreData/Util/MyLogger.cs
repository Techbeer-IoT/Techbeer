using System;
using System.Collections.Generic;
using System.Text;
using Core.Util;
using Microsoft.Extensions.Logging;

namespace FunctionStoreData.Util {
    public class MyLogger : ILog {

        private ILogger _logger;

        public MyLogger(ILogger logger) {
            _logger = logger;
        }

        public void LogError(string message) {
            _logger.LogError(message);
        }

        public void LogInfo(string message) {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message) {
            _logger.LogWarning(message);
        }
    }
}
