﻿#region Copyright

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharacterSelectViewModel.cs">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Libraries;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Regions;
    using Microsoft.Practices.Unity;
    using Models;
    using Utilities;
    using Views;

    #endregion

    /// <summary>
    ///     This View model only serves to allow a user to select a character.
    ///     Fires off 'CharacterSelectedLoginEvent' when the user has selected their character.
    ///     Responds to the 'LoginCompletedEvent'
    /// </summary>
    public class CharacterSelectViewModel : ViewModelBase
    {
        #region Constants

        internal const string CharacterSelectViewName = "CharacterSelectView";

        #endregion

        #region Fields

        private readonly IAccount model;

        private bool isConnecting;

        private string relayMessage = "Next, Select a Character ...";

        private RelayCommand @select;
        private ICharacter selectedCharacter = new CharacterModel();

        #endregion

        #region Constructors and Destructors

        public CharacterSelectViewModel(
            IUnityContainer contain, IRegionManager regman, IEventAggregator events, IAccount acc, IChatModel cm,
            ICharacterManager manager)
            : base(contain, regman, events, cm, manager)
        {
            try
            {
                model = acc.ThrowIfNull("acc");

                Events.GetEvent<LoginCompleteEvent>()
                    .Subscribe(HandleLoginComplete, ThreadOption.UIThread, true);
            }
            catch (Exception ex)
            {
                ex.Source = "Character Select ViewModel, init";
                Exceptions.HandleException(ex);
            }
        }

        #endregion

        #region Public Properties

        public IEnumerable<ICharacter> Characters
        {
            get
            {
                return model.Characters.Select(
                    c =>
                        {
                            var temp = new CharacterModel {Name = c};
                            temp.GetAvatar();
                            return temp;
                        });
            }
        }

        public bool IsConnecting
        {
            get { return isConnecting; }

            set
            {
                isConnecting = value;
                OnPropertyChanged("IsConnecting");
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

        public ICommand SelectCharacterCommand
        {
            get
            {
                return @select
                       ?? (@select =
                           new RelayCommand(
                               param => SendCharacterSelectEvent(), param => CanSendCharacterSelectEvent()));
            }
        }

        public ICharacter CurrentCharacter
        {
            get { return selectedCharacter; }

            set
            {
                selectedCharacter = value;
                OnPropertyChanged("CurrentCharacter");
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void Initialize()
        {
            try
            {
                Container.RegisterType<object, CharacterSelectView>(CharacterSelectViewName);
            }
            catch (Exception ex)
            {
                ex.Source = "Character Select ViewModel, init";
                Exceptions.HandleException(ex);
            }
        }

        #endregion

        #region Methods

        private bool CanSendCharacterSelectEvent()
        {
            return !string.IsNullOrWhiteSpace(CurrentCharacter.Name);
        }

        private void SendCharacterSelectEvent()
        {
            RelayMessage = "Awesome! Connecting to F-Chat ...";
            IsConnecting = true;
            Events.GetEvent<CharacterSelectedLoginEvent>().Publish(CurrentCharacter.Name);
        }

        private void HandleLoginComplete(bool should)
        {
            if (!should)
                return;

            RelayMessage = "Done! Now pick a character.";
            RegionManager.RequestNavigate(Shell.MainRegion, new Uri(CharacterSelectViewName, UriKind.Relative));
        }

        #endregion
    }
}