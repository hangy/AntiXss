// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageInspectionResult.cs" company="Microsoft Corporation">
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
//   The results of a page inspection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The results of a page inspection.
    /// </summary>
    public class PageInspectionResult : IInspectionResult
    {
        /// <summary>
        /// A list of problem controls.
        /// </summary>
        private readonly Collection<ProblemControl> resultProblemControls = new Collection<ProblemControl>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PageInspectionResult"/> class with a default severity of 
        /// <see cref="InspectionResultSeverity.Halt"/>.
        /// </summary>
        public PageInspectionResult()
            : this(InspectionResultSeverity.Halt)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageInspectionResult"/> class.
        /// </summary>
        /// <param name="severity">The severity of the results.</param>
        public PageInspectionResult(InspectionResultSeverity severity)
            : this(severity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageInspectionResult"/> class.
        /// </summary>
        /// <param name="severity">The severity of the results.</param>
        /// <param name="problemControls">The controls on the page that have caused a problem.</param>
        public PageInspectionResult(InspectionResultSeverity severity, IEnumerable<ProblemControl> problemControls)
        {
            this.Severity = severity;

            if (problemControls == null)
            {
                return;
            }

            foreach (ProblemControl problemControl in problemControls)
            {
                this.resultProblemControls.Add(problemControl);
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
        /// Gets the problem controls.
        /// </summary>
        /// <value>The problem controls.</value>
        public IEnumerable<ProblemControl> ProblemControls
        {
            get
            {
                return this.resultProblemControls;
            }
        }

        /// <summary>
        /// Gets the reason, if any, processing should stop.
        /// </summary>
        /// <value>The reason, if any, processing should stop.</value>
        public string StopReason
        {
            get
            {
                string stopReason = "Halted during page inspection.";

                if (this.resultProblemControls != null && this.resultProblemControls.Count > 0)
                {
                    stopReason += Environment.NewLine;
                    foreach (ProblemControl problemControl in this.resultProblemControls)
                    {
                        stopReason += problemControl;
                        stopReason += Environment.NewLine;
                    }
                }

                return stopReason;
            }
        }
    }
}
