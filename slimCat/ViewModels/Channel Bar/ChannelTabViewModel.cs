﻿#region Copyright

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelTabViewModel.cs">
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
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Regions;
    using Microsoft.Practices.Unity;
    using Models;
    using Views;

    #endregion

    /// <summary>
    ///     The channels tab view model.
    /// </summary>
    public class ChannelsTabViewModel : ChannelbarViewModelCommon
    {
        #region Constants

        public const string ChannelsTabView = "ChannelsTabView";

        #endregion

        #region Fields

        /// <summary>
        ///     The show only alphabet.
        /// </summary>
        public bool ShowOnlyAlphabet;

        private bool showPrivate = true;

        private bool showPublic = true;

        private bool sortByName;

        private int thresh = 5;

        #endregion

        #region Constructors and Destructors

        public ChannelsTabViewModel(
            IChatModel cm, IUnityContainer contain, IRegionManager reggman, IEventAggregator events,
            ICharacterManager manager)
            : base(contain, reggman, events, cm, manager)
        {
            Container.RegisterType<object, ChannelTabView>(ChannelsTabView);

            SearchSettings.Updated += (s, e) =>
                {
                    OnPropertyChanged("SearchSettings");
                    OnPropertyChanged("SortedChannels");
                };
        }

        #endregion

        #region Public Properties

        public bool ShowPrivateRooms
        {
            get { return showPrivate; }

            set
            {
                if (showPrivate == value)
                    return;

                showPrivate = value;
                OnPropertyChanged("SortedChannels");
            }
        }

        public bool ShowPublicRooms
        {
            get { return showPublic; }

            set
            {
                if (showPublic == value)
                    return;

                showPublic = value;
                OnPropertyChanged("SortedChannels");
            }
        }

        public bool SortByName
        {
            get { return sortByName; }

            set
            {
                if (sortByName == value)
                    return;

                sortByName = value;
                OnPropertyChanged("SortedChannels");
            }
        }

        public IEnumerable<GeneralChannelModel> SortedChannels
        {
            get
            {
                Func<GeneralChannelModel, bool> containsSearchString =
                    channel =>
                        channel.Id.ToLower().Contains(SearchSettings.SearchString)
                        || channel.Title.ToLower().Contains(SearchSettings.SearchString);

                Func<GeneralChannelModel, bool> meetsThreshold = channel => channel.UserCount >= Threshold;

                Func<GeneralChannelModel, bool> meetsTypeFilter =
                    channel =>
                        ((channel.Type == ChannelType.Public) && showPublic)
                        || ((channel.Type == ChannelType.Private) && showPrivate);


                Func<GeneralChannelModel, bool> meetsFilter =
                    channel => containsSearchString(channel) && meetsTypeFilter(channel) && meetsThreshold(channel);

                return SortByName
                    ? ChatModel.AllChannels.Where(meetsFilter).OrderBy(channel => channel.Title)
                    : ChatModel.AllChannels.Where(meetsFilter).OrderByDescending(channel => channel.UserCount);
            }
        }

        public int Threshold
        {
            get { return thresh; }

            set
            {
                if (thresh == value || value <= 0 || value >= 1000)
                    return;

                thresh = value;
                OnPropertyChanged("SortedChannels");
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void Initialize()
        {
        }

        #endregion
    }
}