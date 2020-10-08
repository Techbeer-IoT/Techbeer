using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Adapters;
using System.Net;
using System.IO;


namespace Core.Util {
    public class HttpRequest {

        public enum MethodTy {
            GET,
            POST,
            PUT,
            DELETE
        }

        private readonly APIEndpoint _apiEndpoint;

        public HttpRequest(APIEndpoint apiEndpoint) {
            _apiEndpoint = apiEndpoint;
        }

        public string MakeRequest() {
            string result = "";

            _apiEndpoint.GetEndpoint().ForEach(async endP => {
                if(!string.IsNullOrEmpty(endP))
                    result += await DoRequest(endP) + Environment.NewLine;
            });

            return result;
        }

        private async Task<string> DoRequest(string endpoint) {
            var request = WebRequest.Create(endpoint);

            request.Method = _apiEndpoint.GetMethodTy().ToString();
            SetHeaders(request);

            var model = _apiEndpoint.GetBodyModel();
            var json = JsonConvert.SerializeObject(model);
            var byteData = Encoding.UTF8.GetBytes(json);

            var dataStream = await request.GetRequestStreamAsync();
            dataStream.Write(byteData, 0, byteData.Length);
            dataStream.Close();

            var response = await request.GetResponseAsync();
            string responseFromServer = "";

            using (dataStream = response.GetResponseStream()) {
                var reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
            }

            return responseFromServer;
        }

        private void SetHeaders(WebRequest request) {
            var headers = _apiEndpoint.GetHeaders();

            foreach (var item in headers) {
                request.Headers[item.Key] = item.Value;
            }
        }
    }
}
