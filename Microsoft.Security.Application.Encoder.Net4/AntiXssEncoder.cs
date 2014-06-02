// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AntiXssEncoder.cs" company="Microsoft Corporation">
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
//   Provides swap-out for .NET 4.0 encoding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Provides swap-out for .NET 4.0 encoding.
    /// </summary>
    public class AntiXssEncoder : System.Web.Util.HttpEncoder
    {
        /// <summary>
        /// Encodes a string into an HTML-encoded string.
        /// </summary>
        /// <param name="value">The string to encode.</param>
        /// <param name="output">The text writer to write the encoded value to.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="output"/> is null. </exception>
        protected override void HtmlEncode(string value, TextWriter output)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            output.Write(Encoder.HtmlEncode(value));
        }

        /// <summary>
        /// Encodes an incoming value into a string that can be inserted into an HTML attribute that is delimited by using double quotation marks.
        /// </summary>
        /// <param name="value">The string to encode.</param>
        /// <param name="output">The text writer to write the encoded value to.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="output"/> is null. </exception>
        protected override void HtmlAttributeEncode(string value, TextWriter output)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            output.Write(Encoder.HtmlAttributeEncode(value));
        }

        /// <summary>
        /// Encodes an array of characters that are not allowed in a URL into a hexadecimal character-entity equivalent.
        /// </summary>
        /// <param name="bytes">An array of bytes to encode.</param>
        /// <param name="offset">The position in the <paramref name="bytes"/> array at which to begin encoding.</param>
        /// <param name="count">The number of items in the <paramref name="bytes"/> array to encode.</param>
        /// <returns>An array of encoded characters.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset"/> is less than zero or greater than the length of the <paramref name="bytes"/> array. -or-<paramref name="count"/> is less than zero or <paramref name="count"/> plus <paramref name="offset"/> is greater than the length of the <paramref name="bytes"/> array.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="bytes"/> is null or of zero length. </exception>
        protected override byte[] UrlEncode(byte[] bytes, int offset, int count)
        {
            if (count == 0)
            {
                return null;
            }

            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentNullException("bytes");
            }

            if ((offset < 0) || (offset > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if ((count < 0) || ((offset + count) > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("count");
            }

            string utf8String = Encoding.UTF8.GetString(bytes, offset, count);
            string result = Encoder.UrlEncode(utf8String, Encoding.UTF8);
            return Encoding.UTF8.GetBytes(result);
        }

        /// <summary>
        /// URL-encodes the path section of a URL string and returns the encoded string.
        /// </summary>
        /// <param name="value">The text to URL-encode.</param>
        /// <returns>The URL encoded text.</returns>
        protected override string UrlPathEncode(string value)
        {
            return Encoder.UrlPathEncode(value);
        }
    }
}
