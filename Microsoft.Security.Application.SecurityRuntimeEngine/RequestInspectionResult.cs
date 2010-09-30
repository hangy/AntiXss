// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestInspectionResult.cs" company="Microsoft Corporation">
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
//   The results of a request inspection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The results of a request inspection.
    /// </summary>
    public class RequestInspectionResult : IInspectionResult
    {
        /// <summary>
        /// A list of problem parameters.
        /// </summary>
        private readonly Collection<RequestProblemParameter> resultProblemParameters = new Collection<RequestProblemParameter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestInspectionResult"/> class with a default severity of 
        /// <see cref="InspectionResultSeverity.Halt"/>.
        /// </summary>
        public RequestInspectionResult()
            : this(InspectionResultSeverity.Halt)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestInspectionResult"/> class.
        /// </summary>
        /// <param name="severity">The severity of the results.</param>
        public RequestInspectionResult(InspectionResultSeverity severity)
            : this(severity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestInspectionResult"/> class.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="problemParameters">The problem parameters.</param>
        public RequestInspectionResult(InspectionResultSeverity severity, IEnumerable<RequestProblemParameter> problemParameters)
        {
            this.Severity = severity;

            if (problemParameters == null)
            {
                return;
            }

            foreach (RequestProblemParameter problemParameter in problemParameters)
            {
                this.resultProblemParameters.Add(problemParameter);
            }
        }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>The severity.</value>
        public InspectionResultSeverity Severity
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the problem parameters.
        /// </summary>
        /// <value>The problem parameters.</value>
        public IEnumerable<RequestProblemParameter> ProblemParameters
        {
            get
            {
                return this.resultProblemParameters;
            }
        }

        /// <summary>
        /// Gets the stop reason for these results.
        /// </summary>
        /// <value>The stop reason.</value>
        public virtual string StopReason
        {
            get
            {
                string stopReason = "Halted during request inspection.";

                if (this.resultProblemParameters != null && this.resultProblemParameters.Count > 0)
                {
                    stopReason += Environment.NewLine;
                    foreach (RequestProblemParameter problemParameter in this.resultProblemParameters)
                    {
                        stopReason += problemParameter;
                        stopReason += Environment.NewLine;
                    }
                }

                return stopReason;
            }
        }

        /// <summary>
        /// Gets the problem parameter count.
        /// </summary>
        /// <returns>The number of problem parameters.</returns>
        protected int ProblemParameterCount
        {
            get
            {
                return this.resultProblemParameters.Count;
            }
        }

        /// <summary>
        /// Adds a parameter to the list of problem parameters.
        /// </summary>
        /// <param name="parameter">The parameter to add.</param>
        protected void AddProblemParameter(RequestProblemParameter parameter)
        {
            this.resultProblemParameters.Add(parameter);
        }
    }
}
