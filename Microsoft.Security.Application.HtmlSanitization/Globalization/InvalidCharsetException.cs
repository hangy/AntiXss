// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidCharsetException.cs" company="Microsoft Corporation">
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
//   Exception thrown when an invalid character set is used.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Globalization
{
    using System;
    using System.Runtime.Serialization;
    using GlobalizationStrings = CtsResources.GlobalizationStrings;

    /// <summary>
    /// Exception thrown when an invalid character set is used.
    /// </summary>
    [Serializable]
    internal class InvalidCharsetException : ExchangeDataException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCharsetException"/> class.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        public InvalidCharsetException(int codePage) :
            base(GlobalizationStrings.InvalidCodePage(codePage))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCharsetException"/> class.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        /// <param name="message">The exception message.</param>
        public InvalidCharsetException(int codePage, string message) :
            base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCharsetException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected InvalidCharsetException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}