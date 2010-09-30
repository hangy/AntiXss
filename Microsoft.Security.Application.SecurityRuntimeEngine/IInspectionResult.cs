// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInspectionResult.cs" company="Microsoft Corporation">
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
//   Defines properties that must be implemented for an inspection result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    /// <summary>
    /// Defines properties that must be implemented for an inspection result.
    /// </summary>
    public interface IInspectionResult
    {
        /// <summary>
        /// Gets the severity of the inspection results.
        /// </summary>
        /// <value>
        /// The severity of the inspection results.
        /// </value>
        InspectionResultSeverity Severity
        {
            get;
        }

        /// <summary>
        /// Gets the reason, if any, processing should stop.
        /// </summary>
        /// <value>
        /// The reason, if any, processing should stop.
        /// </value>
        string StopReason
        {
            get;
        }
    }
}
