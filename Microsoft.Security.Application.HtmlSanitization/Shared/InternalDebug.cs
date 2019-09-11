// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternalDebug.cs" company="Microsoft Corporation">
//   Copyright (c) 2008, 2009, 2010 All Rights Reserved, Microsoft Corporation
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
//   A class to provides internal debugging services
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Internal
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;

    /// <summary>
    /// A class to provides internal debugging services.
    /// </summary>
    internal static class InternalDebug
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use system diagnostics debug and tracing..
        /// </summary>
        /// <value>
        /// <c>true</c> if [use system diagnostics]; otherwise, <c>false</c>.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Valid when precompiler DEBUG is true.")]
        internal static bool UseSystemDiagnostics
        {
            get;
            set;
        }

        /// <summary>
        /// Writes information about the trace to the trace listeners.
        /// </summary>
        /// <param name="traceType">Type of the trace.</param>
        /// <param name="format">The format of the trace.</param>
        /// <param name="traceObjects">The trace objects.</param>
        [Conditional("DEBUG")]
        public static void Trace(long traceType, string format, params object[] traceObjects)
        {
#if DEBUG
            if (UseSystemDiagnostics)
            {
            }
#endif
        }

        /// <summary>
        /// Evaluates an expression and, when the result is false, prints a diagnostic message and aborts the program.
        /// </summary>
        /// <param name="condition">Expression (including pointers) that evaluates to nonzero or 0.</param>
        /// <param name="formatString">The format string to throw if the assert fails.</param>
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string formatString)
        {
#if DEBUG
            if (!UseSystemDiagnostics)
            {
                if (!condition)
                {
                    throw new DebugAssertionViolationException(formatString);
                }
            }
            else
            {
                Debug.Assert(condition, formatString);
            }
#endif
        }

        /// <summary>
        /// Evaluates an expression and, when the result is false, prints a diagnostic message and aborts the program.
        /// </summary>
        /// <param name="condition">Expression (including pointers) that evaluates to nonzero or 0.</param>
        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
#if DEBUG
            if (!UseSystemDiagnostics)
            {
                if (!condition)
                {
                    throw new DebugAssertionViolationException("Assertion failed");
                }
            }
            else
            {
               Debug.Assert(condition, string.Empty);
            }
#endif
        }

        /// <summary>
        /// An exception thrown when a debug assertion fails.
        /// </summary>
        internal class DebugAssertionViolationException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DebugAssertionViolationException"/> class.
            /// </summary>
            public DebugAssertionViolationException()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DebugAssertionViolationException"/> class.
            /// </summary>
            /// <param name="message">The exception message.</param>
            public DebugAssertionViolationException(string message) : base(message)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DebugAssertionViolationException"/> class.
            /// </summary>
            /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
            /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
            /// <exception cref="System.ArgumentNullException">
            /// The <paramref name="info"/> parameter is null.
            /// </exception>
            /// <exception cref="System.Runtime.Serialization.SerializationException">
            /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
            /// </exception>
            protected DebugAssertionViolationException(SerializationInfo info, 
                 StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
