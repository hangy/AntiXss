// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidSurrogatePairException.cs" company="Microsoft Corporation">
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
//   Thrown when a bad surrogate pair is encountered.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when a bad surrogate pair is encountered.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Usage",
        "CA2237:MarkISerializableTypesWithSerializable",
        Justification = "The exception does not leave the application domain and serialization breaks medium trust.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1032:ImplementStandardExceptionConstructors",
        Justification = "The exception does not leave the application domain and serialization breaks medium trust.")]
    public partial class InvalidSurrogatePairException : Exception
    {
        /// <summary>
        /// The exception state
        /// </summary>
        [NonSerialized]
        private ExceptionState exceptionState;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSurrogatePairException"/> class.
        /// </summary>
        public InvalidSurrogatePairException()
        {
            this.HookSerializationEvents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSurrogatePairException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidSurrogatePairException(string message)
            : base(message)
        {
            this.HookSerializationEvents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSurrogatePairException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public InvalidSurrogatePairException(string message, Exception inner)
            : base(message, inner)
        {
            this.HookSerializationEvents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSurrogatePairException"/> class.
        /// </summary>
        /// <param name="highSurrogate">The high surrogate value which caused the error.</param>
        /// <param name="lowSurrogate">The low surrogate value which caused the error.</param>
        public InvalidSurrogatePairException(char highSurrogate, char lowSurrogate)
        {
            this.HighSurrogate = highSurrogate;
            this.LowSurrogate = lowSurrogate;

            this.HookSerializationEvents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSurrogatePairException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="highSurrogate">The high surrogate value which caused the error.</param>
        /// <param name="lowSurrogate">The low surrogate value which caused the error.</param>
        public InvalidSurrogatePairException(string message, char highSurrogate, char lowSurrogate)
            : base(message)
        {
            this.HighSurrogate = highSurrogate;
            this.LowSurrogate = lowSurrogate;

            this.HookSerializationEvents();
        }

        /// <summary>
        /// Gets or sets the high surrogate value.
        /// </summary>
        /// <value>The high surrogate.</value>
        public char HighSurrogate
        {
            get
            {
                return this.exceptionState.HighSurrogate;
            }

            protected set
            {
                this.exceptionState.HighSurrogate = value;
            }
        }

        /// <summary>
        /// Gets or sets the low surrogate value.
        /// </summary>
        /// <value>The low surrogate.</value>
        public char LowSurrogate
        {
            get
            {
                return this.exceptionState.LowSurrogate;
            }

            protected set
            {
                this.exceptionState.LowSurrogate = value;
            }
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
        public override string Message
        {
            get
            {
                if (this.HighSurrogate == 0 && this.LowSurrogate == 0)
                {
                    return base.Message;
                }

                string surrogatePair = string.Format(
                    CultureInfo.CurrentUICulture,
                    "Surrogate Pair = 	{0:x4}:{1:x4}",
                    Convert.ToInt32(this.HighSurrogate),
                    Convert.ToInt32(this.LowSurrogate));

                return surrogatePair + Environment.NewLine + "Message: " + base.Message;
            }
        }

        /// <summary>
        /// Hooks the necessary events to support serialization in v4.0
        /// </summary>
        partial void HookSerializationEvents();

        /// <summary>
        /// This type holds state information that allows the exception type to be serialized but remain transparent.
        /// </summary>
        private partial struct ExceptionState
        {
            /// <summary>
            /// The high surrogate character.
            /// </summary>
            public char HighSurrogate;

            /// <summary>
            /// The low surrogate character.
            /// </summary>
            public char LowSurrogate;
        }
    }

#if SAFESERIALIZATIONMANAGER
    /// <summary>
    /// .NET 4.0 version of the exception thrown when a bad surrogate pair is encountered.
    /// </summary>
    [Serializable]
    public partial class InvalidSurrogatePairException 
    {
        /// <summary>
        /// Hooks the necessary events to support serialization in v4.0
        /// </summary>        
        partial void HookSerializationEvents() 
        {
            this.SerializeObjectState += (sender, e) => 
            {
                e.AddSerializedState(this.exceptionState);
            };
        }

        /// <summary>
        /// This type holds state information that allows the exception type to be serialized but remain transparent.
        /// </summary>
        [Serializable]
        private partial struct ExceptionState : ISafeSerializationData 
        {
            /// <summary>
            /// This method is called to complete hydration of the serialized exception object.
            /// </summary>
            /// <param name="deserialized">
            /// The deserialized object to rehydrate from.
            /// </param>
            void ISafeSerializationData.CompleteDeserialization(object deserialized)
            {
                InvalidSurrogatePairException exception = (InvalidSurrogatePairException)deserialized;
                exception.exceptionState = this;
            }
        }
    }
#endif
}
