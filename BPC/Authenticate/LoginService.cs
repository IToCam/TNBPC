using BPC.Annotations;
using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;


namespace BPC.Authenticate
{
    public class LoginService : INotifyPropertyChanged
    {

        private readonly IAuthorizor _authorizor;
            private readonly ITicketHolder _ticketHolder;
            private readonly IConnectionHandler _connectionHandler;

            private string _token;
            private DateTime _tokenExpireTime;

            public string Status { get; set; }
            public bool SaveCredentials { get; set; }
            public string ClientId { get; set; }
            public string Username { get; set; }

            public string ClearCredentialsVisible { get; set; }

            public bool UsernameEnabled { get; set; }
            public string PasswordBoxVisibility { get; set; }
            public string TxtClientIdVisibility { get; set; }

            //public LoginWindowViewModel(IAuthorizor authorizor, ITicketHolder ticketHolder, IConnectionHandler connectionHandler)
            //{
            //    _authorizor = authorizor;
            //    _ticketHolder = ticketHolder;
            //    _connectionHandler = connectionHandler;

            //    LoginEnabled = true;

            //    SaveCredentials = BPC_TestApp.Properties.Settings.Default.SaveCredentials;

            //    ClientId = BPC_TestApp.Properties.Settings.Default.ClientId;
            //    Username = BPC_TestApp.Properties.Settings.Default.Username;
            //    _tokenExpireTime = BPC_TestApp.Properties.Settings.Default.TokenExpireTime;

            //    if (_tokenExpireTime < DateTime.Now)
            //    {
            //        PasswordBoxVisibility = "visible";
            //        TxtClientIdVisibility = "visible";
            //        ClearCredentialsVisible = "hidden";
            //        UsernameEnabled = true;
            //    }
            //    else
            //    {
            //        _token = BPC_TestApp.Properties.Settings.Default.Token;
            //        PasswordBoxVisibility = "hidden";
            //        TxtClientIdVisibility = "hidden";
            //        ClearCredentialsVisible = "visible";
            //        UsernameEnabled = false;
            //    }
            //}

            private ICommand _clickCommand;
            public ICommand LoginCommand => _clickCommand ?? (_clickCommand = new CommandHandler(Login));

            private ICommand _clearCredentialsCommand;

            public ICommand ClearCredentialsCommand => _clearCredentialsCommand ?? (_clearCredentialsCommand = new ClearCredentialsCommandHandler(ClearCredentials));

            public bool LoginEnabled { get; private set; }

            public void ClearCredentials()
            {
                _token = null;

                ClearCredentialsVisible = "hidden";
                UsernameEnabled = true;
                PasswordBoxVisibility = "visible";
                TxtClientIdVisibility = "visible";

                OnPropertyChanged(nameof(Username));
                OnPropertyChanged(nameof(ClearCredentialsVisible));
                OnPropertyChanged(nameof(UsernameEnabled));
                OnPropertyChanged(nameof(PasswordBoxVisibility));
                OnPropertyChanged(nameof(TxtClientIdVisibility));
            }

            public async void Login(string password)
            {
                MontelTicket montelTicket = null;

                await Task.Factory.StartNew(() =>
                {
                    LoginEnabled = false;
                    OnPropertyChanged(nameof(LoginEnabled));

                    Status = "Authenticating user...";
                    OnPropertyChanged(nameof(Status));

                    if (string.IsNullOrEmpty(_token))
                    {
                        montelTicket = _authorizor.VerifyCredentials(Username, password, ClientId);
                    }
                    else
                    {

                        IsTokenValidResult isTokenValidResult = _authorizor.IsTokenValid(_token).Result;

                        if (isTokenValidResult == IsTokenValidResult.Invalid)
                        {
                            PasswordBoxVisibility = "visible";
                            TxtClientIdVisibility = "visible";
                            ClearCredentialsVisible = "hidden";
                            UsernameEnabled = true;
                            Status = "Token is no longer valid. Please enter password";
                            _token = "";
                            _tokenExpireTime = new DateTime(1900, 1, 1);

                            OnPropertyChanged(nameof(PasswordBoxVisibility));
                            OnPropertyChanged(nameof(TxtClientIdVisibility));
                            OnPropertyChanged(nameof(ClearCredentialsVisible));
                            OnPropertyChanged(nameof(UsernameEnabled));
                            OnPropertyChanged(nameof(Status));

                            return;
                        }

                        if (isTokenValidResult == IsTokenValidResult.UnableToConnect)
                        {
                            Status = "Unable to connect to server";
                            OnPropertyChanged(nameof(Status));
                            return;
                        }

                        montelTicket = new MontelTicket();
                        montelTicket.Token = _token;
                        montelTicket.ExpireTime = _tokenExpireTime;
                        montelTicket.HttpStatusCode = HttpStatusCode.OK;
                        montelTicket.Status = "OK";
                    }
                }).ContinueWith(completedTask =>
                {
                    if (montelTicket != null)
                    {
                        if (montelTicket.HttpStatusCode == HttpStatusCode.OK)
                        {
                            if (SaveCredentials)
                            {
                                BPC.Properties.Settings.Default["SaveCredentials"] = true;
                                BPC.Properties.Settings.Default["Username"] = Username.Trim();
                                BPC.Properties.Settings.Default["ClientId"] = ClientId;
                                BPC.Properties.Settings.Default["Token"] = montelTicket.Token;
                                BPC.Properties.Settings.Default["TokenExpireTime"] = montelTicket.ExpireTime;
                                BPC.Properties.Settings.Default.Save();
                            }
                            else
                            {
                                BPC.Properties.Settings.Default["SaveCredentials"] = false;
                                BPC.Properties.Settings.Default.Save();
                            }

                            Status = "";
                            OnPropertyChanged(nameof(Status));

                            _ticketHolder.SetTicket(montelTicket);
                            _connectionHandler.FireConnectedEvent();
                        }
                        else
                        {
                            Status = montelTicket.Status;
                            OnPropertyChanged(nameof(Status));
                        }
                    }

                    LoginEnabled = true;
                    OnPropertyChanged(nameof(LoginEnabled));

                },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
            }



            private class ClearCredentialsCommandHandler : ICommand
            {
                private readonly Action _action;

                public ClearCredentialsCommandHandler(Action action)
                {
                    _action = action;
                }

                public bool CanExecute(object parameter)
                {
                    return true;
                }

                public void Execute(object parameter)
                {
                    _action();
                }

                public event EventHandler CanExecuteChanged;
            }

            private class CommandHandler : ICommand
            {
                private Action<string> _action;



                public CommandHandler(Action<string> action)
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
                //var passwordBox = parameter as PasswordBox;

                //if (passwordBox != null)
                //{
                //    var password = passwordBox.Password;

                //    _action(password);
                //}
            }
        }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
}
