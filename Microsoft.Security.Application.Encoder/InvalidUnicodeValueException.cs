// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidUnicodeValueException.cs" company="Microsoft Corporation">
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
//   Thrown when a invalid Unicode valid is encountered.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when a invalid Unicode valid is encountered.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Usage",
        "CA2237:MarkISerializableTypesWithSerializable",
        Justification = "The exception does not leave the application domain and serialization breaks medium trust.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1032:ImplementStandardExceptionConstructors",
        Justification = "The exception does not leave the application domain and serialization breaks medium trust.")]
    public partial class InvalidUnicodeValueException : Exception
    {
        /// <summary>
        /// The exception state
        /// </summary>
        [NonSerialized]
        private ExceptionState exceptionState;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUnicodeValueException"/> class.
        /// </summary>
        public InvalidUnicodeValueException()
        {
            this.HookSerializationEvents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUnicodeValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidUnicodeValueException(string message)
            : base(message)
        {
            this.HookSerializationEvents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUnicodeValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public InvalidUnicodeValueException(string message, Exception inner)
            : base(message, inner)
        {
            this.HookSerializationEvents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUnicodeValueException"/> class.
        /// </summary>
        /// <param name="value">The invalid value.</param>
        public InvalidUnicodeValueException(int value)
        {
            this.Value = value;

            this.HookSerializationEvents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUnicodeValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="value">The invalid value.</param>
        public InvalidUnicodeValueException(string message, int value)
            : base(message)
        {
            this.Value = value;

            this.HookSerializationEvents();
        }

        /// <summary>
        /// Gets or sets the the invalid value.
        /// </summary>
        /// <value>The invalid value.</value>
        public int Value
        {
            get
            {
                return this.exceptionState.Value;
            }
            
            protected set
            {
                this.exceptionState.Value = value;
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
                if (this.Value == 0)
                {
                    return base.Message;
                }

                return string.Format(CultureInfo.CurrentUICulture, "Value : {0:x4}", this.Value) + Environment.NewLine + "Message: " + base.Message;
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
            /// The invalid Unicode value.
            /// </summary>
            public int Value;
        }
    }

#if SAFESERIALIZATIONMANAGER
    /// <summary>
    /// .NET 4.0 version of the exception thrown when a invalid Unicode valid is encountered.
    /// </summary>
    [Serializable]    
    public partial class InvalidUnicodeValueException
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
                InvalidUnicodeValueException exception = (InvalidUnicodeValueException)deserialized;
                exception.exceptionState = this;
            }
        }
    }
#endif
}
