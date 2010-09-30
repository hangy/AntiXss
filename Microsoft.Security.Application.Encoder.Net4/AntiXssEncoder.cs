// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AntiXssEncoder.cs" company="Microsoft Corporation">
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
//   Provides an .NET 4.0 HttpEncoder implementation which uses AntiXSS.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Security.Application.Encoded.Net4
{
    using System;
    using System.IO;
    using System.Web.Util;

    /// <summary>
    /// Provides an .NET 4.0 HttpEncoder implementation which uses AntiXSS.
    /// </summary>
    /// <remarks>
    /// To replace the default encoder with one which uses AntiXSS you must register the type in web.config.
    /// To do so make the following changes:
    /// &lt;system.web&lt;
    ///   &lt;httpRuntime encoderType="Microsoft.Security.Application.Encoded.Net4.AntiXssEncoder, Microsoft.Security.Application.Encoded.Net4" /&gt;
    ///   ....
    /// &lt;/system.web&lt;
    /// </remarks>
    public sealed class AntiXssEncoder : HttpEncoder
    {
        /// <summary>
        /// Encodes a string into an HTML-encoded string.
        /// </summary>
        /// <param name="value">The string to encode.</param>
        /// <param name="output">The text writer to write the encoded value to.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="output"/> is null.</exception>
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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="output"/> is null.</exception>
        protected override void HtmlAttributeEncode(string value, TextWriter output)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }
            
            output.Write(Encoder.HtmlAttributeEncode(value));
        }
    }
}
