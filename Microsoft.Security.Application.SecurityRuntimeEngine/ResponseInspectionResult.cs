// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseInspectionResult.cs" company="Microsoft Corporation">
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
//   The results of a response inspection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;

    /// <summary>
    /// The results of a response inspection.
    /// </summary>
    public class ResponseInspectionResult : RequestInspectionResult
    {
        /// <summary>
        /// A collection of problem headers and values from the response.
        /// </summary>
        private readonly NameValueCollection responseProblemHeaders = new NameValueCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseInspectionResult"/> class with a default severity of 
        /// <see cref="InspectionResultSeverity.Halt"/>.
        /// </summary>
        public ResponseInspectionResult()
            : this(InspectionResultSeverity.Halt)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseInspectionResult"/> class.
        /// </summary>
        /// <param name="severity">The severity of the results.</param>
        public ResponseInspectionResult(InspectionResultSeverity severity)
            : this(severity, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseInspectionResult"/> class.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="problemRequestParameters">The problem parameters.</param>
        /// <param name="problemResponseHeaders">A <see cref="NameValueCollection"/> of suspect response header names and values.</param>
        public ResponseInspectionResult(
            InspectionResultSeverity severity,
            IEnumerable<RequestProblemParameter> problemRequestParameters,
            NameValueCollection problemResponseHeaders)
        {
            this.Severity = severity;

            if (problemRequestParameters != null)
            {
                foreach (RequestProblemParameter problemParameter in problemRequestParameters)
                {
                    this.AddProblemParameter(problemParameter);
                }
            }

            if (problemResponseHeaders != null)
            {
                this.responseProblemHeaders.Add(problemResponseHeaders);
            }
        }

        /// <summary>
        /// Gets the stop reason for these results.
        /// </summary>
        /// <value>The stop reason.</value>
        public override string StopReason
        {
            get
            {
                string stopReason = "Halted during response inspection.";

                if (this.ProblemParameters != null && this.ProblemParameterCount > 0)
                {
                    stopReason += Environment.NewLine;
                    stopReason += "Suspect Request Parameters:";
                    stopReason += Environment.NewLine;
                    foreach (RequestProblemParameter problemParameter in this.ProblemParameters)
                    {
                        stopReason += problemParameter;
                        stopReason += Environment.NewLine;
                    }
                }

                if (this.responseProblemHeaders != null && this.responseProblemHeaders.HasKeys())
                {
                    stopReason += Environment.NewLine;
                    stopReason += "Suspect Response Headers:";
                    stopReason += Environment.NewLine;
                    foreach (string key in this.responseProblemHeaders)
                    {
                        stopReason += string.Format(CultureInfo.CurrentCulture, "{0}:{1}\n", key, this.responseProblemHeaders[key]);
                    }
                }

                return stopReason;
            }
        }
    }
}
