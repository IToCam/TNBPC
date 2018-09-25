using BPC.Properties;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;



namespace BPC.Http
{
    public interface IHttpClientBuilder
    {
        HttpClient GenerateHttpClient(string token);
    }

    public class HttpClientBuilder : IHttpClientBuilder
    {
        private readonly IProxyHandler _proxyHandler;
        private string _httpCompress;
        public HttpClientBuilder(IProxyHandler proxyHandler)
        {
            _proxyHandler = proxyHandler;
            
            _httpCompress = Settings.Default.HttpCompress;
            
            if (_httpCompress != "" && _httpCompress != "deflate")
                _httpCompress = "";
        }

        public HttpClient GenerateHttpClient(string token)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();

            if (_proxyHandler.IsProxyUsed())
            {
                httpClientHandler.Proxy = _proxyHandler.GetProxySettings();
                httpClientHandler.UseProxy = true;
            }

            var client = new HttpClient(httpClientHandler);
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (!string.IsNullOrEmpty(_httpCompress))
                client.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse(_httpCompress));

            return client;
        }


      
    }
}