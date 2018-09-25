using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using System.Collections.Generic;
using BPC.Authenticate;
using BPC.Config;
using BPC.DB;

namespace BPC.SignalR
{
    public interface ISignalRHandler : IDisposable
    {
        bool IsSignalrConnected { get; }
        bool ConnectToSignalR(MontelTicket ticket);
        void DisconnectSignalR();
    }

    public class SignalRHandler : ISignalRHandler
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IProxyHandler _proxyHandler;
        private readonly BlockingCollection<dynamic> _messageQueue;

        private HubConnection _hubConnection;
        private IHubProxy _hub;

        static GetInstruments gt = new GetInstruments();
        Dictionary<string, Instrument> subs = gt.GiveInstruments();

        public SignalRHandler(IProxyHandler proxyHandler)
        {
            _proxyHandler = proxyHandler;
            InitializeSignalrServer();
            _messageQueue = new BlockingCollection<dynamic>();
        }

        private void InitializeSignalrServer()
        {
            try
            {
                _hubConnection = new HubConnection(ConfigurationManager.AppSettings["MontelWebApiUri"]);
                _hubConnection.Closed += HubConnectionOnClosed;
                _hubConnection.Error += HubConnectionOnError;
                _hubConnection.StateChanged += HubConnectionOnStateChanged;
                _hubConnection.Reconnecting += HubConnectionOnReconnecting;
                _hubConnection.Reconnected += HubConnectionOnReconnected;
                _hub = _hubConnection.CreateHubProxy("MontelDataHub");
                _hub.On("Message", m => OnNewMessage( m ) );
                _hub.On("Info", x => OnInfo(x));
            }
            catch (Exception ex)
            {
                logger.Error("Error:" + ex.Message);
            }
        }

        public bool IsSignalrConnected => _hubConnection.State == ConnectionState.Connected;

        public bool ConnectToSignalR(MontelTicket ticket)
        {
            logger.Trace("SignalR - Connecting to SignalR");

            if (!_hubConnection.Headers.ContainsKey("Authorization"))
                _hubConnection.Headers.Add("Authorization", "Bearer " + ticket.Token);
            else
                _hubConnection.Headers["Authorization"] = "Bearer " + ticket.Token;

            if (_proxyHandler.IsProxyUsed())
                _hubConnection.Proxy = _proxyHandler.GetProxySettings();
            
            try
            {
                var startConnnectionTask = _hubConnection.Start();

                startConnnectionTask.Wait(30 * 1000);

                if (!startConnnectionTask.IsCompleted)
                {
                    
                    logger.Trace("SignalR - Connection timed out");
                    return false;
                }

                _hub.Invoke("Connect");
            }
            catch (Exception exception)
            {
                while (exception.InnerException != null)
                    exception = exception.InnerException;

                logger.Trace("SignalR - Exception: " + exception.Message);
                
                return false;
            }

         
            logger.Trace($"SignalR - Connection succeeded. Transport method={_hubConnection.Transport.Name}, SupportsKeepAlive={_hubConnection.Transport.SupportsKeepAlive}.");
            Task.Factory.StartNew(ReadFromMessageQueue);

            return true;
        }

        public void DisconnectSignalR()
        {
            logger.Trace("SignalR - Disconnecting...");
            _hubConnection.Stop();
        }

        private void OnNewMessage(dynamic message)
        {
          _messageQueue.Add(message);
        }

        private void OnInfo(dynamic message)
        {
            _messageQueue.Add(message);
        }

        private void ReadFromMessageQueue()
        {
            foreach (dynamic message in _messageQueue.GetConsumingEnumerable())
            {
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(message);

                try
                {
                    if (jsonString.Contains("SpotKey"))
                    {
                        logger.Trace("ReadFromMessageQueue: " + message.ToString());
                        Trade[] montelTrades = Newtonsoft.Json.JsonConvert.DeserializeObject<Trade[]>(jsonString);
                        DatabaseHandler databaseHandler = new DatabaseHandler();

                        foreach (Trade item in montelTrades)
                        {
                            foreach (TradeElement element in item.Elements)
                            {
                                int instrumentId = subs[item.SpotName].InstrumentId;
                                byte vendorId = subs[item.SpotName].VendorId;

                                databaseHandler.InsertTrade(
                                                                instrumentId
                                                                , vendorId
                                                                , element.Date.Add(DateTime.UtcNow.TimeOfDay).AddDays(-1)
                                                                , element.Base
                                                                , DateTime.UtcNow
                                                            );
                                string logmessage = string.Format("Insert was made for Instrument: {0} VendorId: {1} Datetime: {2} Price: {3}", instrumentId, vendorId, element.Date.Add(DateTime.UtcNow.TimeOfDay).AddDays(-1), element.Base);
                                logger.Trace(logmessage);
                                
                            }
                        }
                    }
                    else
                    {
                        logger.Trace("ReadFromMessageQueue: " + message.ToString());
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error:" + ex.Message);
                }
            }
        }

        class Trade
        {
            public string SubscriptionId { get; set; }
            public string SymbolKey { get; set; }
            public string SpotName { get; set; }
            public string Denomination { get; set; }
            public List<TradeElement> Elements { get; set; }
        }

        class TradeElement
        {
            public DateTime Date { get; set; }
            public decimal Base { get; set; }
            public object[] TimeSpans { get; set; }
      }

        private void HubConnectionOnReconnected()
        {
            logger.Trace("SignalR - Reconnected");
        }

        private void HubConnectionOnReconnecting()
        {
            logger.Trace("SignalR - Reconnecting");
        }

        private void HubConnectionOnStateChanged(StateChange stateChange)
        {
            logger.Trace("SignalR - HubConnectionOnStateChanged");
        }

        private void HubConnectionOnError(Exception exception)
        {
            logger.Trace("SignalR - Error: " + exception.Message);
        }

        private void HubConnectionOnClosed()
        {
            logger.Trace("SignalR - Connection closed");
        }

        public void Dispose()
        {
            _messageQueue.Dispose();
        }
    }

    public class NotConnectedToSignalrException : Exception {};
}
