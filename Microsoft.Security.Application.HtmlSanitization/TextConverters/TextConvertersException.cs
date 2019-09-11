// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextConvertersException.cs" company="Microsoft Corporation">
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
//
// </copyright>
// <summary>
//    
// </summary>

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.Runtime.Serialization;
    using Strings = CtsResources.TextConvertersStrings;
    
    internal enum HeaderFooterFormat
    {
        Text,        
        Html,
    }
    
    [Serializable]
    internal class TextConvertersException : ExchangeDataException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextConvertersException"/> class.
        /// </summary>
        internal TextConvertersException() :                 
            base("internal text conversion error (document too complex)")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextConvertersException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        internal TextConvertersException(string message) :
            base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextConvertersException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        internal TextConvertersException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
