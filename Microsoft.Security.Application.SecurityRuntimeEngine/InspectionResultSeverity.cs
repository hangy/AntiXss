// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InspectionResultSeverity.cs" company="Microsoft Corporation">
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
//   The severity of an inspection result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    /// <summary>
    /// The severity of an inspection result.
    /// </summary>
    public enum InspectionResultSeverity
    {
        /// <summary>
        /// All further processing must stop.
        /// </summary>
        Halt = 0,

        /// <summary>
        /// Further processing may continue.
        /// </summary>
        Continue = 1,

        /// <summary>
        /// The inspection has discovered something suspect in its inspection, but processing may
        /// continue until the number of suspect results is greater than the configured
        /// <see cref="SecurityRuntimeSettings.AllowedSuspectResults"/>
        /// </summary>
        Suspect = 2
    }
}
