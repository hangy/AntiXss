// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooManySuspectInspectionsResult.cs" company="Microsoft Corporation">
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
//   Inspection Result used when the suspect count has gone past the configured limits.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    /// <summary>
    /// Inspection Result used when the suspect count has gone past the configured limits.
    /// </summary>
    internal sealed class TooManySuspectInspectionsResult : IInspectionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TooManySuspectInspectionsResult"/> class.
        /// </summary>
        /// <param name="reason">The reason for the halt.</param>
        public TooManySuspectInspectionsResult(string reason)
        {
            this.StopReason = reason;
        }

        /// <summary>
        /// Gets the severity of the inspection results.
        /// </summary>
        /// <value>The severity of the inspection results.</value>
        public InspectionResultSeverity Severity
        {
            get
            {
                return InspectionResultSeverity.Halt;
            }
        }

        /// <summary>
        /// Gets the reason, if any, processing should stop.
        /// </summary>
        /// <value>The reason, if any, processing should stop.</value>
        public string StopReason
        {
            get;
            private set;
        }
    }
}
