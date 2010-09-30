// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestStoppedException.cs" company="Microsoft Corporation">
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
//   Thrown when a request has been stopped by a Security Runtime Engine plug-in.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System;

    /// <summary>
    /// Thrown when a request has been stopped by a Security Runtime Engine plug-in.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Usage",
        "CA2237:MarkISerializableTypesWithSerializable",
        Justification = "The exception does not leave the application domain and serialization breaks medium trust.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1032:ImplementStandardExceptionConstructors",
        Justification = "The exception does not leave the application domain and serialization breaks medium trust.")]
    public class RequestStoppedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestStoppedException"/> class.
        /// </summary>
        public RequestStoppedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestStoppedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RequestStoppedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestStoppedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public RequestStoppedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestStoppedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="stoppedBy">The name of the module which stopped the request.</param>
        public RequestStoppedException(string message, string stoppedBy)
            : base(message)
        {
            this.StoppedBy = stoppedBy;
        }

        /// <summary>
        /// Gets or sets the name of the module which stopped the request.
        /// </summary>
        /// <value>The name of the module which stopped the request.</value>
        public string StoppedBy
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(this.StoppedBy))
                {
                    return base.Message;
                }

                return "Stopping Module: " + this.StoppedBy + Environment.NewLine + "Message: " + base.Message;
            }
        }
    }
}
