using BPC.Authenticate;
using System;
using System.Configuration;
using System.Net;


namespace BPC
{
    public interface IProxyHandler
    {
        IWebProxy GetProxySettings();
        bool IsProxyUsed();
    }

    public class ProxyHandler : IProxyHandler
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private bool? _cachedIsProxyInUse;
        private IWebProxy _cachedProxySettings;
        private readonly string _montelWebApiUri;

        public ProxyHandler()
        {
            _montelWebApiUri = ConfigurationManager.AppSettings["MontelWebApiUri"];
        }

        public IWebProxy GetProxySettings()
        {
            if (_cachedProxySettings != null)
            {
                return _cachedProxySettings;
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ProxyUri"]))
                _cachedProxySettings = GetProxySettingsFromConfigFile();
            else
                _cachedProxySettings = GetProxySettingsFromWindows();


            return _cachedProxySettings;
        }

        private IWebProxy GetProxySettingsFromConfigFile()
        {
            WebProxy webProxy = new WebProxy();
            try
            {
                string proxyUriFromConfigurationFile = ConfigurationManager.AppSettings["ProxyUri"];

                if (string.IsNullOrEmpty(proxyUriFromConfigurationFile))
                    return null;

                logger.Trace("Authentication: Proxy is set from config file: " + proxyUriFromConfigurationFile);
                
                webProxy.Address = new Uri(proxyUriFromConfigurationFile);
                webProxy.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ProxyUsername"], ConfigurationManager.AppSettings["ProxyPassword"]);

            }
            catch (Exception)
            {
                
            }
           
            return webProxy;
        }

        public bool IsProxyUsed()
        {
            if (_cachedIsProxyInUse.HasValue )
                return _cachedIsProxyInUse.Value;

            IWebProxy webProxy = WebRequest.GetSystemWebProxy();
            Uri proxyUri = webProxy.GetProxy(new Uri(_montelWebApiUri));
            //_montelLogger.Info("Authentication: ProxyUri is " + proxyUri + " for " + _xlfServerUrl);
            _cachedIsProxyInUse = proxyUri.AbsoluteUri.TrimEnd('/') != _montelWebApiUri.TrimEnd('/');

            return _cachedIsProxyInUse.Value;
        }

        private IWebProxy GetProxySettingsFromWindows()
        {
            
            IWebProxy webProxy = WebRequest.GetSystemWebProxy();

            Uri proxyUri = webProxy.GetProxy(new Uri(_montelWebApiUri));

            Credential credentials = CredentialManager.ReadCredential(proxyUri.Host);

            if (credentials != null)
            {
                webProxy.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);
            }

            return webProxy;
        }
    
    }
}
