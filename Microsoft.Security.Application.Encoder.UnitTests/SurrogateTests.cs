// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SurrogateTests.cs" company="Microsoft Corporation">
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
//   Tests handling of Unicode Surrogates
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests handling of Unicode Surrogates
    /// </summary>
    [TestClass]
    public class SurrogateTests
    {
        /// <summary>
        /// Tests the lowest valid surrogate character pair for proper markup encoding.
        /// </summary>
        [TestMethod]
        public void TestHtmlLowerBoundarySurrogateEncoding()
        {
            const string target = "\uD800\uDC00";
            const string expected = "&#65536;";

            string result = Encoder.HtmlEncode(target);

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Tests the lowest valid surrogate character pair for proper markup encoding.
        /// </summary>
        [TestMethod]
        public void TestHtmlUpperBoundarySurrogateEncoding()
        {
            const string target = "\uDBFF\uDFFF";
            const string expected = "&#1114111;";

            string result = Encoder.HtmlEncode(target);

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Test that a high surrogate character which is not followed by a low surrogate character 
        /// returns the substitution character.
        /// </summary>
        [TestMethod]
        public void TestHtmlHighSurrogateWithoutLowSurrogate()
        {
            const string target = "\uD800";
            const string expected = "&#65533;"; // Substitution character.

            string result = Encoder.HtmlEncode(target);

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Test that a low surrogate character which was not preceded by a high surrogate character
        /// returns the substitution character.
        /// </summary>
        [TestMethod]
        public void TestHtmlLowSurrogateWithoutHighSurrogate()
        {
            const string target = "\uDC00";
            const string expected = "&#65533;"; // Substitution character.

            string result = Encoder.HtmlEncode(target);

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Tests the lowest valid surrogate character pair for proper CSS encoding.
        /// </summary>
        [TestMethod]
        public void TestCssLowerBoundarySurrogateEncoding()
        {
            const string target = "\uD800\uDC00";
            const string expected = @"\010000";

            string result = Encoder.CssEncode(target);

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Tests the lowest valid surrogate character pair for proper CSS encoding.
        /// </summary>
        [TestMethod]
        public void TestCssUpperBoundarySurrogateEncoding()
        {
            const string target = "\uDBFF\uDFFF";
            const string expected = @"\10FFFF";

            string result = Encoder.CssEncode(target);

            Assert.AreEqual(expected, result);
        }
    }
}
