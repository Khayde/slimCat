﻿#region Copyright

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginViewModel.cs">
//    Copyright (c) 2013, Justin Kadrovach, All rights reserved.
//   
//    This source is subject to the Simplified BSD License.
//    Please see the License.txt file for more information.
//    All other rights reserved.
//    
//    THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//    PARTICULAR PURPOSE.
// </copyright>
//  --------------------------------------------------------------------------------------------------------------------

#endregion

namespace slimCat.ViewModels
{
    #region Usings

    using System;
    using System.Windows.Input;
    using Libraries;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Regions;
    using Microsoft.Practices.Unity;
    using Models;
    using Properties;
    using Utilities;
    using Views;

    #endregion

    /// <summary>
    ///     The LoginViewModel is responsible for displaying login details to the user.
    ///     Fires off 'LoginEvent' when the user clicks the connect button.
    ///     Responds to the 'LoginCompletedEvent'.
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        #region Constants

        internal const string LoginViewName = "LoginView";

        #endregion

        #region Fields

        private readonly IAccount model; // the model to interact with

        private RelayCommand login;

        private string relayMessage = Constants.FriendlyName; // message relayed to the user

        private bool requestIsSent; // used for determining Login UI state

        #endregion

        #region Constructors and Destructors

        public LoginViewModel(
            IUnityContainer contain, IRegionManager regman, IAccount acc, IEventAggregator events, IChatModel cm,
            ICharacterManager lists)
            : base(contain, regman, events, cm, lists)
        {
            try
            {
                model = acc.ThrowIfNull("acc");
            }
            catch (Exception ex)
            {
                ex.Source = "Login ViewModel, Init";
                Exceptions.HandleException(ex);
            }
        }

        #endregion

        #region Public Properties

        public string AccountName
        {
            get { return model.AccountName; }

            set
            {
                if (model.AccountName == value)
                    return;

                model.AccountName = value;
                OnPropertyChanged("AccountName");
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                return login
                       ?? (login = new RelayCommand(param => SendTicketRequest(), param => CanLogin()));
            }
        }

        public string Password
        {
            get { return model.Password; }

            set
            {
                if (model.Password == value)
                    return;

                model.Password = value;
                OnPropertyChanged("Password");
            }
        }

        public string RelayMessage
        {
            get { return relayMessage; }

            set
            {
                relayMessage = value;
                OnPropertyChanged("RelayMessage");
            }
        }

        public bool RequestSent
        {
            get { return requestIsSent; }

            set
            {
                requestIsSent = value;
                OnPropertyChanged("RequestSent");
            }
        }

        public bool SaveLogin
        {
            get { return Settings.Default.SaveLogin; }

            set
            {
                Settings.Default.SaveLogin = value;
                Settings.Default.Save();
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void Initialize()
        {
            try
            {
                Container.RegisterType<object, LoginView>(LoginViewName);

                RegionManager.RequestNavigate(Shell.MainRegion, new Uri(LoginViewName, UriKind.Relative));
            }
            catch (Exception ex)
            {
                ex.Source = "Login ViewModel, init";
                Exceptions.HandleException(ex);
            }
        }

        #endregion

        #region Methods

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(AccountName) && !string.IsNullOrWhiteSpace(Password)
                   && !RequestSent;
        }

        private void SendTicketRequest()
        {
            RelayMessage = "Logging in ...";
            RequestSent = true;
            Events.GetEvent<LoginEvent>().Publish(true);
            Events.GetEvent<LoginCompleteEvent>().Subscribe(HandleLogin, ThreadOption.UIThread);
        }

        private void HandleLogin(bool gotTicket)
        {
            Events.GetEvent<LoginCompleteEvent>().Unsubscribe(HandleLogin);

            if (!gotTicket)
            {
                RequestSent = false;
                RelayMessage = "Oops!" + " " + model.Error;
            }
            else
            {
                if (SaveLogin)
                {
                    Settings.Default.UserName = model.AccountName;
                    Settings.Default.Password = model.Password;
                    Settings.Default.Save();
                }
                else
                {
                    Settings.Default.UserName = null;
                    Settings.Default.Password = null;
                    Settings.Default.Save();
                }
            }
        }

        #endregion
    }
}