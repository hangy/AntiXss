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

    /// <summary>
    /// The exception thrown when a character set which is not installed is used.
    /// </summary>
    [Serializable]
    internal class CharsetNotInstalledException : InvalidCharsetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharsetNotInstalledException"/> class.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        /// <param name="message">The message.</param>
        public CharsetNotInstalledException(int codePage, string message) :
            base(codePage, message)
        {
        }
    }
}