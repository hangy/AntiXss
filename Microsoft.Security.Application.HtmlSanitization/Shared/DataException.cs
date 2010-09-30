// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataException.cs" company="Microsoft Corporation">
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
//   Thrown when a data exception occurs.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when a data exception occurs.
    /// </summary>
    [Serializable()]
#if EXCHANGECOMMONEXCEPTIONS
    internal class ExchangeDataException : Microsoft.Exchange.Data.Common.LocalizedException
#else
    internal class ExchangeDataException : Exception
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeDataException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ExchangeDataException(string message) :
#if EXCHANGECOMMONEXCEPTIONS
            base(new Microsoft.Exchange.Data.Common.LocalizedString(message))
#else
            base(message)
#endif
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeDataException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ExchangeDataException(string message, Exception innerException) :
#if EXCHANGECOMMONEXCEPTIONS
            base(new Microsoft.Exchange.Data.Common.LocalizedString(message), innerException)
#else
            base(message, innerException)
#endif
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeDataException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected ExchangeDataException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}

