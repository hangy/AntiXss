﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRestartable.cs" company="Microsoft Corporation">
//   Copyright (c) 2008, 2009, 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
//
// </copyright>
// <summary>
//    Interface declaration for classes that are restartable.
// </summary>

namespace Microsoft.Exchange.Data.TextConverters
{
    /// <summary>
    /// Interface declaration for classes that are restartable.
    /// </summary>
    internal interface IRestartable
    {
        /// <summary>
        /// Determines whether this instance can restart.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can restart; otherwise, <c>false</c>.
        /// </returns>
        bool CanRestart();

        /// <summary>
        /// Restarts this instance.
        /// </summary>
        void Restart();

        /// <summary>
        /// Disables the restart.
        /// </summary>
        void DisableRestart();
    }
}
