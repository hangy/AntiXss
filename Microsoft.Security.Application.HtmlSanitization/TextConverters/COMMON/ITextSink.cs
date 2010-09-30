// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITextSink.cs" company="Microsoft Corporation">
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
//    Interface declaration for classes with Test Sink.
// </summary>

namespace Microsoft.Exchange.Data.TextConverters
{
    /// <summary>
    /// Interface declaration for classes with Test Sink.
    /// </summary>
    internal interface ITextSink
    {
        /// <summary>
        /// Gets a value indicating whether this instance is enough.
        /// </summary>
        /// <value><c>true</c> if this instance is enough; otherwise, <c>false</c>.</value>
        bool IsEnough { get; }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        void Write(char[] buffer, int offset, int count);

        /// <summary>
        /// Writes the specified ucs32 char.
        /// </summary>
        /// <param name="ucs32Char">The ucs32 char.</param>
        void Write(int ucs32Char);
    }
}