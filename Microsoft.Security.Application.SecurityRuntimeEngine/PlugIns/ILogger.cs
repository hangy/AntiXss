// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Microsoft Corporation">
//   Copyright (c) 2010 All Rights Reserved, Microsoft Corporation
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
//   Defines methods used for logging from the SRE and from within plug-ins.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System;

    /// <summary>
    /// Defines methods used for logging from the SRE and from within plug-ins.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Adds the specified message to the log.
        /// </summary>
        /// <param name="message">The level to log.</param>
        void Log(string message);

        /// <summary>
        /// Adds the specified exception to the log.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        void Log(Exception exception);
    }
}
