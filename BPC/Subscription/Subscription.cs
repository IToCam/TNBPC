using BPC.Annotations;
using BPC.Http;
using BPC.SignalR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BPC.Subscription
{
    public class Subscription
    {
        public SubscriptionType SubscriptionType { get; set; }
        public Guid Guid { get; private set; }

        public Subscription(Guid guid, SubscriptionType subscriptionType)
        {
            Guid = guid;
            SubscriptionType = subscriptionType;
        }
    }
    public interface ISubscriptions : IDisposable
    {
        void AddSubscription(Guid subscriptionId, System.Net.Http.HttpClient httpClient, string subscribeUrl, SubscriptionType subscriptionType);
        void StopSubscription(Guid subscription, System.Net.Http.HttpClient httpClient, string stopUrl);
        void ChangeSubscription(Guid subscriptionId, System.Net.Http.HttpClient httpClient, string changeUrl, SubscriptionType subscriptionType);
    }

    public enum SubscriptionType { Ohlc, SpotPrice, SpotVolume, Quote, Trade, Fundamental }


    public class Subscriptions : INotifyPropertyChanged, ISubscriptions
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ISignalRHandler _signalRHandler;
        //private readonly IOutputViewModel _outputViewModel;
        private readonly ITicketHolder _ticketHolder;
        private readonly IHttpSubscriptionHandler _httpSubscriptionHandler;
        public bool IsSignalrConnected { get; set; }

        public string SignalrConnectionButtonText
        {
            get
            {
                if (IsSignalrConnected)
                    return "Disconnect";
                else
                    return "Connect";

            }

        }

        private ICommand _connectSignalrCommand;
        public ICommand ConnectSignalrCommand => _connectSignalrCommand ?? (_connectSignalrCommand = new CommandHandler(ConnectSignalR));


        public ObservableCollection<Subscription> ObservableCollection { get; set; }

        public Subscriptions(ISignalRHandler signalRHandler,  ITicketHolder ticketHolder, IHttpSubscriptionHandler httpSubscriptionHandler)
        {
            _signalRHandler = signalRHandler;
            _ticketHolder = ticketHolder;
            _httpSubscriptionHandler = httpSubscriptionHandler;

            ObservableCollection = new ObservableCollection<Subscription>();
        }

        private void ConnectSignalR()
        {
            if (IsSignalrConnected)
                _signalRHandler.DisconnectSignalR();
            else
            {
                var ticket = _ticketHolder.GetTicket();
                _signalRHandler.ConnectToSignalR(ticket);
            }

            IsSignalrConnected = _signalRHandler.IsSignalrConnected;
            OnPropertyChanged(nameof(SignalrConnectionButtonText));
        }


        public void AddSubscription(Guid subscriptionId, HttpClient httpClient, string subscribeUrl, SubscriptionType subscriptionType)
        {
            if (!_signalRHandler.IsSignalrConnected)
            {
                //_outputViewModel.WriteLine("Not connected to server");
                Console.WriteLine("Not connected to server");
                return;
            }
            try
            {

                Task<SubscribeResult> subscribeTask = _httpSubscriptionHandler.Subscribe(httpClient, subscribeUrl);

                SubscribeResult subscribeResult = subscribeTask.Result;

                //_outputViewModel.WriteLine(subscribeResult.Result);
                Console.WriteLine(subscribeResult.Result);

                if (subscribeResult.Result == "Subscription initalized")
                {
                    if (ObservableCollection.All(s => s.Guid != subscriptionId))
                    {
                        ObservableCollection.Add(new Subscription(subscriptionId, subscriptionType));
                        OnPropertyChanged(nameof(ObservableCollection));
                    }
                }
            }
            catch (NotConnectedToSignalrException)
            {
                //_outputViewModel.WriteLine("Error: Not connected to signal R");
                Console.WriteLine("Error: Not connected to signal R");
            }

        }

        public void ChangeSubscription(Guid subscriptionId, HttpClient httpClient, string changeUrl, SubscriptionType subscriptionType)
        {
            if (!_signalRHandler.IsSignalrConnected)
            {
                //_outputViewModel.WriteLine("Not connected to server");
                return;
            }
            try
            {
                Task<SubscribeResult> subscribeTask = _httpSubscriptionHandler.Subscribe(httpClient, changeUrl);

                SubscribeResult subscribeResult = subscribeTask.Result;

                //_outputViewModel.WriteLine(subscribeResult.Result);

                if (subscribeResult.Result == "Change initalized")
                {

                    if (ObservableCollection.All(s => s.Guid != subscriptionId))
                    {
                        ObservableCollection.Remove(ObservableCollection.FirstOrDefault(s => s.Guid == subscriptionId));
                        ObservableCollection.Add(new Subscription(subscriptionId, subscriptionType));
                        OnPropertyChanged(nameof(ObservableCollection));
                    }
                }
            }
            catch (NotConnectedToSignalrException)
            {
                //_outputViewModel.WriteLine("Error: Not connected to signal R");
            }

        }


        public void StopSubscription(Guid subscription, HttpClient httpClient, string stopUrl)
        {
            Subscription stopSubscription = ObservableCollection.FirstOrDefault(s => s.Guid == subscription);

            if (!_signalRHandler.IsSignalrConnected)
            {
                //_outputViewModel.WriteLine("Not connected to server");
                Console.WriteLine("Not connected to server");
                return;
            }
            try
            {

                if (stopSubscription != null)
                {
                    Task<SubscribeResult> subscribeTask = _httpSubscriptionHandler.StopSubscription(httpClient, stopUrl);
                    SubscribeResult subscribeResult = subscribeTask.Result;
                    //_outputViewModel.WriteLine(subscribeResult.Result);
                    Console.WriteLine(subscribeResult.Result);
                    ObservableCollection.Remove(stopSubscription);
                }
            }
            catch (NotConnectedToSignalrException)
            {
                //_outputViewModel.WriteLine("Error: Not connected to signal R");
                Console.WriteLine("Error: Not connected to signal R");
            }

        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private class CommandHandler : ICommand
        {
            private Action _action;

            public CommandHandler(Action action)
            {
                _action = action;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {


                _action();
            }
        }

        public void Dispose()
        {
            _signalRHandler.Dispose();
        }
    }
}