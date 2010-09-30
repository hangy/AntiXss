// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLogger.cs" company="Microsoft Corporation">
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
//   A basic logger implementation using System.Diagnostics.Debug.WriteLine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.SampleLogger
{
    using System;
    using System.Diagnostics;

    using PlugIns;

    /// <summary>
    /// A basic logger implementation using System.Diagnostics.Debug.WriteLine.
    /// </summary>
    public class DebugLogger : ILogger
    {
        /// <summary>
        /// Adds the specified message to the log.
        /// </summary>
        /// <param name="message">The level to log.</param>
        public void Log(string message)
        {
            Debug.WriteLine(message);
        }

        /// <summary>
        /// Adds the specified exception to the log.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public void Log(Exception exception)
        {
            Debug.WriteLine("** Exception **");
            this.DumpException(exception, 0);
        }

        /// <summary>
        /// Dumps the exception and any inner exceptions.
        /// </summary>
        /// <param name="exception">The exception to dump.</param>
        /// <param name="indent">The indent to use to nest inner exceptions.</param>
        private void DumpException(Exception exception, int indent)
        {
            string padding = new string(' ', indent);

            Debug.WriteLine(padding + exception.Message);
            if (exception.InnerException != null)
            {
                this.DumpException(exception, indent + 2);
            }

            Debug.WriteLine(padding + exception.StackTrace);
        }
    }
}
