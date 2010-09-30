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

namespace Microsoft.Application.Security
{
    using VisualStudio.TestTools.UnitTesting;

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
            const string Target = "\uD800\uDC00";
            const string Expected = "&#65536;";

            string result = Microsoft.Security.Application.Encoder.HtmlEncode(Target);

            Assert.AreEqual(Expected, result);
        }

        /// <summary>
        /// Tests the lowest valid surrogate character pair for proper markup encoding.
        /// </summary>
        [TestMethod]
        public void TestHtmlUpperBoundarySurrogateEncoding()
        {
            const string Target = "\uDBFF\uDFFF";
            const string Expected = "&#1114111;";

            string result = Microsoft.Security.Application.Encoder.HtmlEncode(Target);

            Assert.AreEqual(Expected, result);
        }

        /// <summary>
        /// Test that a high surrogate character which is not followed by a low surrogate character throws
        /// an InvalidSurrogatePair exception when markup encoding.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Microsoft.Security.Application.InvalidSurrogatePairException))]
        public void TestHtmlHighSurrogateWithoutLowSurrogate()
        {
            Microsoft.Security.Application.Encoder.HtmlEncode("\uD800");
        }

        /// <summary>
        /// Test that a low surrogate character which was not preceded by a high surrogate character throws
        /// an InvalidSurrogatePair exception when markup encoding.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Microsoft.Security.Application.InvalidSurrogatePairException))]
        public void TestHtmlLowSurrogateWithoutHighSurrogate()
        {
            Microsoft.Security.Application.Encoder.HtmlEncode("\uDC00");
        }

        /// <summary>
        /// Tests the lowest valid surrogate character pair for proper CSS encoding.
        /// </summary>
        [TestMethod]
        public void TestCssLowerBoundarySurrogateEncoding()
        {
            const string Target = "\uD800\uDC00";
            const string Expected = @"\010000";

            string result = Microsoft.Security.Application.Encoder.CssEncode(Target);

            Assert.AreEqual(Expected, result);
        }

        /// <summary>
        /// Tests the lowest valid surrogate character pair for proper CSS encoding.
        /// </summary>
        [TestMethod]
        public void TestCssUpperBoundarySurrogateEncoding()
        {
            const string Target = "\uDBFF\uDFFF";
            const string Expected = @"\10FFFF";

            string result = Microsoft.Security.Application.Encoder.CssEncode(Target);

            Assert.AreEqual(Expected, result);
        }

        /// <summary>
        /// Test that a high surrogate character which is not followed by a low surrogate character throws
        /// an InvalidSurrogatePair exception when CSS encoding.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Microsoft.Security.Application.InvalidSurrogatePairException))]
        public void TestCssHighSurrogateWithoutLowSurrogate()
        {
            Microsoft.Security.Application.Encoder.CssEncode("\uD800");
        }

        /// <summary>
        /// Test that a low surrogate character which was not preceded by a high surrogate character throws
        /// an InvalidSurrogatePair exception when CSS encoding.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Microsoft.Security.Application.InvalidSurrogatePairException))]
        public void TestCssLowSurrogateWithoutHighSurrogate()
        {
            Microsoft.Security.Application.Encoder.HtmlEncode("\uDC00");
        }    
    }
}
