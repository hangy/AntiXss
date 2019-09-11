// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharsetNotInstalledException.cs" company="Microsoft Corporation">
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
//   The exception thrown when a character set which is not installed is used.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Globalization
{
    using System;
    using System.Runtime.Serialization;
    using GlobalizationStrings = CtsResources.GlobalizationStrings;

    /// <summary>
    /// The exception thrown when a character set which is not installed is used.
    /// </summary>
    [Serializable]
    internal class CharsetNotInstalledException : InvalidCharsetException
    {
        // Orphaned WPL code.
#if false
        /// <summary>
        /// Initializes a new instance of the <see cref="CharsetNotInstalledException"/> class.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        public CharsetNotInstalledException(int codePage) :
            base(codePage, GlobalizationStrings.NotInstalledCodePage(codePage))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharsetNotInstalledException"/> class.
        /// </summary>
        /// <param name="charsetName">Name of the charset.</param>
        public CharsetNotInstalledException(string charsetName) :
            base(charsetName, GlobalizationStrings.NotInstalledCharset(charsetName ?? "<null>"))
        {
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="CharsetNotInstalledException"/> class.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        /// <param name="message">The message.</param>
        public CharsetNotInstalledException(int codePage, string message) :
            base(codePage, message)
        {
        }

#if false
        // Orphaned WPL code.

        /// <summary>
        /// Initializes a new instance of the <see cref="CharsetNotInstalledException"/> class.
        /// </summary>
        /// <param name="charsetName">Name of the charset.</param>
        /// <param name="message">The message.</param>
        public CharsetNotInstalledException(string charsetName, string message) :
            base(charsetName, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharsetNotInstalledException"/> class.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CharsetNotInstalledException(int codePage, string message, Exception innerException) :
            base(codePage, message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharsetNotInstalledException"/> class.
        /// </summary>
        /// <param name="charsetName">Name of the charset.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CharsetNotInstalledException(string charsetName, string message, Exception innerException) :
            base(charsetName, message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharsetNotInstalledException"/> class.
        /// </summary>
        /// <param name="charsetName">Name of the charset.</param>
        /// <param name="codePage">The code page.</param>
        internal CharsetNotInstalledException(string charsetName, int codePage) :
            base(charsetName, codePage, GlobalizationStrings.NotInstalledCharsetCodePage(codePage, charsetName ?? "<null>"))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharsetNotInstalledException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected CharsetNotInstalledException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
#endif
    }
}