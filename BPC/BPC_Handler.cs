using BPC.Authenticate;
using BPC.Http;
using BPC.SignalR;
using BPC.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPC.Config;
using System.Net.Http;

namespace BPC
{
    /// <summary>
    /// Top object for all Base Price Collection
    /// </summary>
    public class BPC_Handler
    {

        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static IProxyHandler proxyHandler = new ProxyHandler();
        static MontelTicket montelTicket = new MontelTicket();
        static ITicketHolder ticketHolder = new TicketHolder();
        static HttpClientBuilder httpClientBuilder = new HttpClientBuilder(BPC_Handler.proxyHandler);
        static ISignalRHandler signalRHandler = new SignalRHandler(proxyHandler);
        static HttpSubscriptionHandler httpSubscriptionHandler = new HttpSubscriptionHandler(proxyHandler);
        Subscriptions subscriptions = new Subscriptions(BPC_Handler.signalRHandler, ticketHolder, httpSubscriptionHandler);
        static UrlGenerator urlGenerator = new UrlGenerator();
        Authorizor authorizor = new Authorizor(proxyHandler, httpClientBuilder, urlGenerator);
        static List<Guid> Subscriptions = new List<Guid>();
        
       

        public void GetTicket()
        {
            try
            {
                logger.Trace("...in i GetTicket");
                ConfigHandler configHandler = new ConfigHandler();
                Authorizor authorizor = new Authorizor(proxyHandler, httpClientBuilder, urlGenerator);
                montelTicket = authorizor.VerifyCredentials(configHandler.GetMontelUsername(), configHandler.GetMontelPassword(), configHandler.GetMontelClientId());
                signalRHandler.ConnectToSignalR(montelTicket);
                ticketHolder.SetTicket(montelTicket);
                logger.Trace("...allt gjort i GetTicket()");
            }
            catch (Exception ex)
            {
                logger.Error("Error in GetTicket: " + ex.Message);
            }
        }

        public void Subscribe()
        {
            try
            {
                logger.Trace("...in i Subscribe");
                GetInstruments gt = new GetInstruments();
                var subs = gt.GiveInstruments();

                logger.Trace("Number of instruments in subs:" + subs.Values.Count());

                string FieldsSubscribe = "Base";
                HttpClient httpClient = httpClientBuilder.GenerateHttpClient(ticketHolder.GetTicket().Token);

                foreach (var item in subs)
                {
                    Guid subscriptionId = Guid.NewGuid();
                    Subscriptions.Add(subscriptionId);
                    string subscribeUrl = urlGenerator.SubscribeSpotPricesUrl(subscriptionId, item.Value.SpotSymbol, FieldsSubscribe.Split(',').ToList(), item.Value.Currency);
                    logger.Trace("Subscribed to:" + subscribeUrl);
                    subscriptions.AddSubscription(subscriptionId, httpClient, subscribeUrl, SubscriptionType.SpotPrice);
                }
                logger.Trace("...allt gjort i Subscribe()");
            }
            catch (Exception ex)
            {
                logger.Error("Error in Subscribe: " + ex.Message);
            }
        }

        public void UnSubscribe()
        {
            try
            {
                logger.Trace("...in i UnSubscribe()");
                HttpClient httpClient = httpClientBuilder.GenerateHttpClient(ticketHolder.GetTicket().Token);
                string stopSpotSubscriptionUrl = string.Empty;//urlGenerator.StopSpotSubscriptionUrl(subscriptionId);
                
                foreach (var item in Subscriptions)
                {
                    stopSpotSubscriptionUrl = urlGenerator.StopSpotSubscriptionUrl(item);
                    subscriptions.StopSubscription(item, httpClient, stopSpotSubscriptionUrl);
                    logger.Trace("Stopping spot subscription with url: " + stopSpotSubscriptionUrl);
                }
                logger.Trace("...allt gjort i UnSubscribe()");
            }
            catch (Exception ex)
            {

                logger.Error("Error in UnSubscribe: " + ex.Message);
            }
        }


    }
}
