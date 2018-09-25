using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using BPC.Http;
using System.Configuration;

namespace BPC.Authenticate
{
    public enum IsTokenValidResult
    {
        Valid, Invalid, UnableToConnect
    }

    public interface IAuthorizor
    {
        MontelTicket VerifyCredentials(string username, string password, string clientId);
        Task<IsTokenValidResult> IsTokenValid(string token);
    }

    public class Authorizor : IAuthorizor
    {
        private readonly IProxyHandler _proxyHandler;
        private readonly IHttpClientBuilder _httpClientBuilder;
        private readonly IUrlGenerator _urlGenerator;
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Authorizor( IProxyHandler proxyHandler, IHttpClientBuilder httpClientBuilder, IUrlGenerator urlGenerator)
        {
            _proxyHandler = proxyHandler;
            _httpClientBuilder = httpClientBuilder;
            _urlGenerator = urlGenerator;
       }

        public MontelTicket VerifyCredentials(string username, string password, string clientId)
        {
            var ticket = new MontelTicket();

            try
            {
                logger.Trace("VerifyCredentials...");
                logger.Trace("Username:" + username);
                logger.Trace("PW:" + password);
                logger.Trace("ClientID:" + clientId);

                HttpClientHandler httpClientHandler = new HttpClientHandler();
                
                if (_proxyHandler.IsProxyUsed())
                {
                    httpClientHandler.Proxy = _proxyHandler.GetProxySettings();
                    httpClientHandler.UseProxy = true;
                }
                
                using (var client = new HttpClient(httpClientHandler))
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["MontelWebApiUri"]);
                    client.DefaultRequestHeaders.Accept.Clear();

                    var req = new HttpRequestMessage(HttpMethod.Post, "GetToken")
                    {
                        Content = new StringContent("grant_type=password&client_Id=" + clientId + "&username=" + username + "&password=" + password),
                    };
 
                    client.Timeout = new TimeSpan(0, 0, 0, 60);
                
                    Task<HttpResponseMessage> responseMessage = client.SendAsync(req);

                    HttpResponseMessage httpResponse = responseMessage.Result;
                    
                    ticket.HttpStatusCode = httpResponse.StatusCode;

                    Task<string> readContentTask = httpResponse.Content.ReadAsStringAsync();
                    
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        dynamic data = JObject.Parse(readContentTask.Result);

                        string expiresInString = data.expires_in;
                        int expiresInSeconds = int.Parse(expiresInString);
                        DateTime expireTime = DateTime.Now.AddSeconds(expiresInSeconds);

                        ticket.ExpireTime = expireTime;
                        ticket.Token = data.access_token;
                        ticket.Status = "OK";
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        dynamic data = JObject.Parse(readContentTask.Result);
                        ticket.Status = data.error + ". " + data.error_description;
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        ticket.Status = "Authentication failed. This could be related to incorrect proxy configuration settings. Please contact your system administrator.";
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.ProxyAuthenticationRequired)
                    {
                        ticket.Status = "Proxy authentication failed. Please contact your system administrator.";
                    }
                    else
                    {
                        ticket.Status = httpResponse.StatusCode.ToString();
                    }
                }
            }
            catch (AggregateException aggregateException)
            {
                AggregateException flattenedAggregateException = aggregateException.Flatten();

                Exception exception = flattenedAggregateException.InnerExceptions.First();

                if (exception is HttpRequestException)
                {
                    if (exception.InnerException != null)
                        ticket.Status = "An HTTP exception occured: " + exception.InnerException.Message;
                    else
                        ticket.Status = "An HTTP exception occured: " + exception.Message;

                    ticket.InnerException = exception;
                }
                else
                {
                    ticket.Status = "An exception has occured: " + exception.Message;
                    ticket.InnerException = exception;
                }

            }
            catch (HttpRequestException httpRequestException)
            {
                if (httpRequestException.InnerException != null)
                    ticket.Status = "An HTTP exception occured: " + httpRequestException.InnerException.Message;
                else
                    ticket.Status = "An HTTP exception occured: " + httpRequestException.Message;

                ticket.InnerException = httpRequestException;

            }
            catch (Exception exception)
            {
                ticket.Status = "An exception has occured: " + exception.Message;
                ticket.InnerException = exception;

            }

            return ticket;
        }

        public async Task<IsTokenValidResult> IsTokenValid(string token)
        {
            try
            {
                HttpClient httpClient = _httpClientBuilder.GenerateHttpClient(token);

                string validateTokenUrl = _urlGenerator.GetValidateTokenUrl();
                HttpResponseMessage httpResponse = await httpClient.GetAsync(validateTokenUrl);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    return IsTokenValidResult.Valid;
                }

                return IsTokenValidResult.Invalid;
            }
            catch (HttpRequestException)
            {
                return IsTokenValidResult.UnableToConnect;
            }
            catch (Exception)
            {
                return IsTokenValidResult.Invalid;
            }
        }
    }
}
