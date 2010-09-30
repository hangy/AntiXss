// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogLevel.cs" company="Microsoft Corporation">
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
// </copyright>
// <summary>
//   The level of a message to be logged.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    /// <summary>
    /// The level of a message to be logged.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// An informational message
        /// </summary>
        Informational,

        /// <summary>
        /// A warning message
        /// </summary>
        Warning,

        /// <summary>
        /// An error message
        /// </summary>
        Error,

        /// <summary>
        /// A fatal error message after which all processing stops.
        /// </summary>
        Fatal
    }
}
