﻿#region Copyright

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SysProp.cs">
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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Threading;

    #endregion

    /// <summary>
    ///     This is a master class which provides access to the Dispather and NotifyPropertyChanged
    /// </summary>
    public abstract class SysProp : DispatcherObject, IDisposable, INotifyPropertyChanged
    {
        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Verifies the name of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <exception cref="System.Exception">Thrown if the name is not valid for the class.</exception>
        [Conditional("DEBUG")]
        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] != null)
                return;

            var msg = "Invalid property name: " + propertyName;

            throw new Exception(msg);
        }

        #endregion

        #region Methods

        protected void OnPropertyChanged(string propertyName)
        {
            // this.VerifyPropertyName(propertyName);
            var handler = PropertyChanged;
            if (handler == null)
                return;

            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);
        }


        protected virtual void Dispose(bool isManaged)
        {
            if (!isManaged)
                return;

            PropertyChanged = null;
        }

        #endregion
    }
}