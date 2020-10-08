using Core.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionStoreData.API {
    public class LineAPI : Core.Adapters.APIEndpoint {

        private Core.Entities.TemperatureTelemetry _telemetry;

        public override List<string> GetEndpoint() =>
            Settings.LineEndpoint();

        public override HttpRequest.MethodTy GetMethodTy() =>
            HttpRequest.MethodTy.POST;

        public override void SetDataForRequest<T>(T data) =>
            _telemetry = data as Core.Entities.TemperatureTelemetry;

        public override Dictionary<string, string> GetHeaders() =>
            new Dictionary<string, string>() {
                {"Content-Type", "application/json"},
                {"Authorization", $"Bearer {Settings.LineToken()}"},
            };

        public override APIEndpointModel GetBodyModel() =>
            new MessageList() {
                messages = new Message[] { GetMessage() },
                notificationDisabled = Settings.LineIsNotificationOFF()
            };

        private Message GetMessage() =>
            new Message() { type = "text", text = CreateMessage() };

        private string CreateMessage() {
            string message = "ペルチェの状態は変わりました‼️";
            
            message += Environment.NewLine;
            message += Environment.NewLine;
            message += "現在の様子は：";
            message += Environment.NewLine;
            message += Environment.NewLine;
            message += $"温度：{_telemetry.temperature}℃";
            message += Environment.NewLine;
            message += $"ペルチェ：{GetPeltierEmoji(_telemetry.peltierStatus)}";

            return message;
        }

        private string GetPeltierEmoji(Core.PeltiesStatusTy peltiesStatusTy) =>
            peltiesStatusTy == Core.PeltiesStatusTy.OFF ? "😐" :
            peltiesStatusTy == Core.PeltiesStatusTy.COOL ? "🥶" : "🥵";

        #region Nested Class
        private class MessageList : APIEndpointModel {
            public Message[] messages { get; set; }
            public bool notificationDisabled { get; set; }
        }

        private class Message {
            public string type { get; set; }
            public string text { get; set; }
        }
        #endregion
    }
}
