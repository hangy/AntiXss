// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITextSinkEx.cs" company="Microsoft Corporation">
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
//    Interface declaration for classes needing to write.
// </summary>

namespace Microsoft.Exchange.Data.TextConverters
{
    /// <summary>
    /// Interface declaration for classes needing to write.
    /// </summary>
    internal interface ITextSinkEx : ITextSink
    {
        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        void Write(string value);

        /// <summary>
        /// Writes the new line.
        /// </summary>
        void WriteNewLine();
    }
}