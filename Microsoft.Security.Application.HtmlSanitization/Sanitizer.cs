// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sanitizer.cs" company="Microsoft Corporation">
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
//   Sanitizes input HTML to make it safe to be displayed on a
//   browser by removing potentially dangerous tags.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application
{
    using System.IO;

    using Exchange.Data.TextConverters;

    /// <summary>
    ///  Sanitizes input HTML to make it safe to be displayed on a
    ///  browser by removing potentially dangerous tags.
    /// </summary>
    /// <remarks>
    ///  This santization library uses the Principle of Inclusions,
    ///  sometimes referred to as "safe-listing" to provide protection
    ///  against injection attacks.  With safe-listing protection,
    ///  algorithms look for valid inputs and automatically treat
    ///  everything outside that set as a potential attack.  This library
    ///  can be used as a defense in depth approach with other mitigation
    ///  techniques.
    /// </remarks>
    public static class Sanitizer
    {
        /// <summary>
        /// Sanitizes input HTML document for safe display on browser.
        /// </summary>
        /// <param name="input">Malicious HTML Document</param>
        /// <returns>A santizied HTML document</returns>
        /// <remarks>
        /// The method transforms and filters HTML of executable scripts.
        /// A safe list of tags and attributes are used to strip dangerous
        /// scripts from the HTML. HTML is also normalized where tags are
        /// properly closed and attributes are properly formatted.
        /// </remarks>
        public static string GetSafeHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            using TextReader stringReader = new StringReader(input);
            using TextWriter stringWriter = new StringWriter();
            HtmlToHtml htmlObject = new()
            {
                FilterHtml = true,
                OutputHtmlFragment = false,
                NormalizeHtml = true
            };

            htmlObject.Convert(stringReader, stringWriter);

            return stringWriter.ToString().Length != 0 ? stringWriter.ToString() : string.Empty;
        }

        /// <summary>
        /// Sanitizes input HTML fragment for safe display on browser.
        /// </summary>
        /// <param name="input">Malicious HTML fragment</param>
        /// <returns>Safe HTML fragment</returns>
        /// <remarks>
        /// The method transforms and filters HTML of executable scripts.
        /// A safe list of tags and attributes are used to strip dangerous
        /// scripts from the HTML. HTML is also normalized where tags are
        /// properly closed and attributes are properly formatted.
        /// </remarks>
        public static string GetSafeHtmlFragment(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            using TextReader stringReader = new StringReader(input);
            using TextWriter stringWriter = new StringWriter();
            HtmlToHtml htmlObject = new()
            {
                FilterHtml = true,
                OutputHtmlFragment = true,
                NormalizeHtml = true
            };

            htmlObject.Convert(stringReader, stringWriter);

            if (stringWriter.ToString().Length == 0)
            {
                return string.Empty;
            }

            // stripping <div> tags
            string output = stringWriter.ToString();
            if (string.Equals(output.Substring(0, 5), "<div>", System.StringComparison.OrdinalIgnoreCase))
            {
                output = output.Substring(5);
                output = output.Substring(0, output.Length - 8);
            }

            return output;
        }

        /// <summary>
        /// Sanitizes input HTML document for safe display on browser.
        /// </summary>
        /// <param name="sourceReader">Source text reader with malicious HTML</param>
        /// <param name="destinationWriter">Text Writer to write safe HTML</param>
        /// <remarks>
        /// The method transforms and filters HTML of executable scripts.
        /// A safe list of tags and attributes are used to strip dangerous
        /// scripts from the HTML. HTML is also normalized where tags are
        /// properly closed and attributes are properly formatted.
        /// </remarks>
        public static void GetSafeHtml(TextReader sourceReader, TextWriter destinationWriter)
        {
            HtmlToHtml htmlObject = new()
            {
                                            FilterHtml = true,
                                            OutputHtmlFragment = false,
                                            NormalizeHtml = true
                                        };

            htmlObject.Convert(sourceReader, destinationWriter);
        }

        /// <summary>
        /// Sanitizes input HTML document for safe display on browser.
        /// </summary>
        /// <param name="sourceReader">Source text reader with malicious HTML</param>
        /// <param name="destinationStream">Stream to write safe HTML</param>
        /// <remarks>
        /// The method transforms and filters HTML of executable scripts.
        /// A safe list of tags and attributes are used to strip dangerous
        /// scripts from the HTML. HTML is also normalized where tags are
        /// properly closed and attributes are properly formatted.
        /// </remarks>
        public static void GetSafeHtml(TextReader sourceReader, Stream destinationStream)
        {
            HtmlToHtml htmlObject = new()
            {
                                            FilterHtml = true,
                                            OutputHtmlFragment = false,
                                            NormalizeHtml = true
                                        };

            htmlObject.Convert(sourceReader, destinationStream);
        }

        /// <summary>
        /// Sanitizes input HTML fragment for safe display on browser.
        /// </summary>
        /// <param name="sourceReader">Source text reader with malicious HTML</param>
        /// <param name="destinationWriter">Stream to write safe HTML</param>
        /// <remarks>
        /// The method transforms and filters HTML of executable scripts.
        /// A safe list of tags and attributes are used to strip dangerous
        /// scripts from the HTML. HTML is also normalized where tags are
        /// properly closed and attributes are properly formatted.
        /// </remarks>
        public static void GetSafeHtmlFragment(TextReader sourceReader, TextWriter destinationWriter)
        {
            HtmlToHtml htmlObject = new()
            {
                                            FilterHtml = true,
                                            OutputHtmlFragment = true,
                                            NormalizeHtml = true
                                        };

            htmlObject.Convert(sourceReader, destinationWriter);
        }

        /// <summary>
        /// Sanitizes input HTML fragment for safe display on browser.
        /// </summary>
        /// <param name="sourceReader">Source text reader with malicious HTML</param>
        /// <param name="destinationStream">Stream to write safe HTML</param>
        /// <remarks>
        /// The method transforms and filters HTML of executable scripts.
        /// A safe list of tags and attributes are used to strip dangerous
        /// scripts from the HTML. HTML is also normalized where tags are
        /// properly closed and attributes are properly formatted.
        /// </remarks>
        public static void GetSafeHtmlFragment(TextReader sourceReader, Stream destinationStream)
        {
            HtmlToHtml htmlObject = new()
            {
                                            FilterHtml = true,
                                            OutputHtmlFragment = true,
                                            NormalizeHtml = true
                                        };

            htmlObject.Convert(sourceReader, destinationStream);
        }
    }
}
