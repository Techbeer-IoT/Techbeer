using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FunctionStoreData {
    public static class Settings {

        public static List<string> LineEndpoint() =>
            GetSettings("LineEndpointAPI").Split('|').ToList();

        public static string LineToken() =>
            GetSettings("LineToken");

        public static bool LineIsNotificationOFF() =>
            GetSettings("IsNotificationOFF").ToLower().Trim() == "true";

        public static bool LineSendMessage() =>
            GetSettings("IsSendLineMessage").ToLower().Trim() == "true";        

        public static List<string> PowerBIDSEndpoint() =>
            GetSettings("PowerBIDSEndpointAPI").Split('|').ToList();

        public static List<string> PowerBIHybridEndpoint() =>
            GetSettings("PowerBIHybridEndpointAPI").Split('|').ToList();

        public static string StorageConnectionString() =>
            GetSettings("StorageConnectionString");

        public static string StorageTableName() =>
            GetSettings("StorageTableName");

        private static string GetSettings(string name) =>
            System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }
}
