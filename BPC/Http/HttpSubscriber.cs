using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BPC.Http
{
    public class SubscribeResult
    {
        public string Result { get; set; }
    }

    public interface IHttpSubscriptionHandler
    {
       Task<SubscribeResult> Subscribe(HttpClient httpClient, string requestUrl);
        Task<SubscribeResult> StopSubscription(HttpClient httpClient, string url);
    }

    public class HttpSubscriptionHandler : IHttpSubscriptionHandler
    {
        private readonly IProxyHandler _proxyHandler;
        private string _httpCompress;

        public HttpSubscriptionHandler(IProxyHandler proxyHandler)
        {
            _proxyHandler = proxyHandler;

            _httpCompress = ConfigurationManager.AppSettings["HttpCompress"];
            if (_httpCompress != "" && _httpCompress != "deflate")
                _httpCompress = "";
        }

        public async Task<SubscribeResult> Subscribe(HttpClient httpClient, string requestUrl)
        {
            HttpResponseMessage response;
            SubscribeResult subscribeResult = new SubscribeResult();

            try
            {

                response = httpClient.GetAsync(requestUrl).Result;
                
                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(_httpCompress))
                    {
                        using (var responseStream = await response.Content.ReadAsStreamAsync())
                        using (var decompressedStream = new DeflateStream(responseStream, CompressionMode.Decompress))
                        using (var streamReader = new StreamReader(decompressedStream))
                        {
                            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                            {
                                JsonSerializer jsonSerializer = new JsonSerializer();
                                subscribeResult = jsonSerializer.Deserialize<SubscribeResult>(jsonTextReader);
                            }
                        }
                    }
                    else
                    {
                        using (var responseStream = await response.Content.ReadAsStreamAsync())
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                            {
                                JsonSerializer jsonSerializer = new JsonSerializer();
                                subscribeResult = jsonSerializer.Deserialize<SubscribeResult>(jsonTextReader);
                            }
                        }
                    }
                }
                else
                {
                    
                    subscribeResult.Result = "Server returned status code " + response.StatusCode;
                }

                return subscribeResult;
            }
            catch (Exception)
            {
                subscribeResult.Result = "An exception occured";

                return subscribeResult;
            }
        }

        public async Task<SubscribeResult> StopSubscription(HttpClient httpClient, string url)
        {
            
            HttpResponseMessage response;
            SubscribeResult subscribeResult = new SubscribeResult();

            try
            {
                response = httpClient.GetAsync(url).Result;
                
                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(_httpCompress))
                    {
                        //_montelLogger.Info("DAL:  Start handling compressed data");
                        using (var responseStream = await response.Content.ReadAsStreamAsync())
                        using (var decompressedStream = new DeflateStream(responseStream, CompressionMode.Decompress))
                        using (var streamReader = new StreamReader(decompressedStream))
                        {
                            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                            {
                                JsonSerializer jsonSerializer = new JsonSerializer();
                                subscribeResult = jsonSerializer.Deserialize<SubscribeResult>(jsonTextReader);
                            }
                        }
                    }
                    else
                    {
                        using (var responseStream = await response.Content.ReadAsStreamAsync())
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                            {
                                JsonSerializer jsonSerializer = new JsonSerializer();
                                subscribeResult = jsonSerializer.Deserialize<SubscribeResult>(jsonTextReader);
                            }
                        }
                    }
                }
                else
                {
                    subscribeResult.Result = "Server returned an error";
                }

                return subscribeResult;
            }
            catch (Exception)
            {
                subscribeResult.Result = "An exception occured";

                return subscribeResult;
            }
        }
    }
}
