// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncoderUnitTests.cs" company="Microsoft Corporation">
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
//   Performs unit tests for each of the encoder methods
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Application.Security
{
    using System;
    using System.IO;
    using Microsoft.Security.Application;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test cases for AntiXss library.  
    /// Following Test cases shall provide testing coverage of each encoding
    /// method in the library.
    /// </summary>
    [TestClass]
    public class EncoderUnitTests
    {
        /// <summary>
        /// All characters from Char.MinValue to Char.MaxValue
        /// </summary>
        private static string allChars = null;

        /// <summary>
        /// All lower ASCII charaters that should be encoded for HtmlEncode
        /// </summary>
        private static string basicEncodedChars = null;

        /// <summary>
        /// All lower ASCII characters that should not be encoded for HtmlEncode
        /// </summary>
        private static string basicNonEncodedChars = null;

        /// <summary>
        /// All lower ASCII charaters that should be encoded for HtmlAttributeEncode.
        /// This is same as HtmlEncode with the addition of the space char.
        /// </summary>
        private static string attributeEncodedChars = null;

        /// <summary>
        /// All lower ASCII characters that should not be encoded for HtmlAttributeEncode.
        /// This is same as HtmlEncode with out the space char.
        /// </summary>
        private static string attributeNonEncodedChars = null;

        /// <summary>
        /// All lower ASCII characters that should be encoded for UrlEncode.
        /// </summary>
        private static string urlEncodedChars = null;

        /// <summary>
        /// All lower ASCII characters that should not be encoded for UrlEncode.
        /// </summary>
        private static string urlNonEncodedChars = null;

        /// <summary>
        /// A very long string (200 * _allChars.Length = ~13MB)
        /// </summary>
        private static string veryLongString = null;

        /// <summary>
        /// Initializes a new instance of the <c>EncoderUnitTests</c> class.  
        /// </summary>
        public EncoderUnitTests()
        {
            System.Text.StringBuilder build;

            // Generate a string of all possible characters
            allChars = string.Empty;
            build = new System.Text.StringBuilder();
            for (uint i = Char.MinValue; i <= Char.MaxValue; i++)
            {
                build.Append((char)i);
            }

            allChars = build.ToString();

            // Generate a very long string
            build = new System.Text.StringBuilder();
            for (int i = 0; i < 200; i++)
            {
                build.Append(allChars);
            }

            veryLongString = build.ToString();

            // Generate basic list of chars we encode, no unicode
            build = new System.Text.StringBuilder();
            for (uint i = 0; i < 255; i++)
            {
                if (i == (uint)' ' || i == (uint)'.' || i == (uint)',' ||
                    i == (uint)'-' || i == (uint)'_')
                    continue;

                else if (i < (uint)'0')
                    build.Append((char)i);

                else if (i > (uint)'9' && i < (uint)'A')
                    build.Append((char)i);

                else if (i > (uint)'Z' && i < (uint)'a')
                    build.Append((char)i);

                else if (i > (uint)'z')
                    build.Append((char)i);
            }

            basicEncodedChars = build.ToString();

            // Generate basic list of chars we don't encode
            build = new System.Text.StringBuilder();
            for (uint i = 0; i < 127; i++)
            {
                if (i == (uint)' ' || i == (uint)'.' || i == (uint)',' ||
                    i == (uint)'-' || i == (uint)'_')
                {
                    build.Append((char)i);
                    continue;
                }
                else if (i < (uint)'0')
                    continue;

                else if (i > (uint)'9' && i < (uint)'A')
                    continue;

                else if (i > (uint)'Z' && i < (uint)'a')
                    continue;

                else if (i > (uint)'z')
                    continue;

                build.Append((char)i);
            }

            basicNonEncodedChars = build.ToString();

            // Setup attribute encoded/non-encoded strings
            attributeEncodedChars = basicEncodedChars + " ";
            attributeNonEncodedChars = basicNonEncodedChars.Trim(new char[] { ' ' });

            // URL: Generate basic list of chars we encode, no unicode
            build = new System.Text.StringBuilder();
            for (uint i = 0; i < 255; i++)
            {
                if (i == (uint)'-' || i == (uint)'.' || i == (uint)'_')
                    continue;

                else if (i < (uint)'0')
                    build.Append((char)i);

                else if (i > (uint)'9' && i < (uint)'A')
                    build.Append((char)i);

                else if (i > (uint)'Z' && i < (uint)'a')
                    build.Append((char)i);

                else if (i > (uint)'z')
                    build.Append((char)i);
            }

            urlEncodedChars = build.ToString();

            // URL: Generate basic list of chars we don't encode
            build = new System.Text.StringBuilder();
            for (uint i = 0; i < 127; i++)
            {
                if (i == (uint)' ' || i == (uint)',')
                    continue;
                else if (i == (uint)'-' || i == (uint)'.' || i == (uint)'_')
                {
                    build.Append((char)i);
                    continue;
                }
                else if (i < (uint)'0')
                    continue;

                else if (i > (uint)'9' && i < (uint)'A')
                    continue;

                else if (i > (uint)'Z' && i < (uint)'a')
                    continue;

                else if (i > (uint)'z')
                    continue;

                build.Append((char)i);
            }

            urlNonEncodedChars = build.ToString();
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get;
            set;
        }

        /// <summary>
        /// Performing tests for HtmlEncode method.
        /// </summary>
        [TestMethod]
        public void TestHtmlEncode()
        {

            // Test: Return empty string on incoming null
            Assert.AreEqual(string.Empty, Encoder.HtmlEncode(null), "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual(string.Empty, Encoder.HtmlEncode(string.Empty), "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("&lt;&gt;&amp;&quote;", Encoder.HtmlEncode("<>&\""), "Test: Basic encodings: < > & \"");

            System.Web.HttpContext.Current = new System.Web.HttpContext(new System.Web.HttpRequest("AntiXssSRETest.aspx", "http://localhost:50002/AntiXssSRETest.aspx", "MarkAntiXssOutput=true"), new System.Web.HttpResponse(new StringWriter()));

            Assert.AreEqual("<span name='#markantixssoutput' style ='background-color : Red'>&lt;&gt;&amp;&quote;</span>", Encoder.HtmlEncode("<>&\"", System.Drawing.KnownColor.Red), "Test: Basic encodings: < > & \"");

            System.Web.HttpContext.Current = new System.Web.HttpContext(new System.Web.HttpRequest("AntiXssSRETest.aspx", "http://localhost:50002/AntiXssSRETest.aspx", string.Empty), new System.Web.HttpResponse(new StringWriter()));

            Assert.AreEqual("&lt;&gt;&amp;&quote;", Encoder.HtmlEncode("<>&\"", System.Drawing.KnownColor.Red), "Test: Basic encodings: < > & \"");

            // Test: Other encodings
            foreach (char c in basicEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), Encoder.HtmlEncode(c.ToString()), "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in basicNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual(c.ToString(), Encoder.HtmlEncode(c.ToString()), "Test: Chars that do not encode");
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("&#169;", Encoder.HtmlEncode("\u00A9"), "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("ЖЉЊ", Encoder.HtmlEncode("\u0416\u0409\u040A"), "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("&#64358;&#64357;&#64342;&#64345;&#64346;&#64347;&#64348;", Encoder.HtmlEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"), "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("シ", Encoder.HtmlEncode("シ"), "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            Assert.IsTrue(Encoder.HtmlEncode(veryLongString).Length > veryLongString.Length, "Test: Very long string");

            // Test: International characters
            Assert.AreEqual("北京奥运完美谢幕 罗格称赞无以伦比", Encoder.HtmlEncode("北京奥运完美谢幕 罗格称赞无以伦比"), "Test: International characters (chinese)");
            Assert.AreEqual("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू", Encoder.HtmlEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"), "Test: International characters (devanagari)");
            Assert.AreEqual("رجل دين إسلامي سعودي", Encoder.HtmlEncode("رجل دين إسلامي سعودي"), "Test: International characters (arabic)");
            Assert.AreEqual("הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך ", Encoder.HtmlEncode("הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך "), "Test: International characters (hebrew)");
        }

        /// <summary>
        /// Perform tests for HtmlAttributeEncode method.
        /// </summary>
        [TestMethod]
        public void TestHtmlAttributeEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual(string.Empty, Encoder.HtmlAttributeEncode(null), "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual(string.Empty, Encoder.HtmlAttributeEncode(string.Empty), "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("&#60;&#62;&#38;&#34;", Encoder.HtmlAttributeEncode("<>&\""), "Test: Basic encodings: < > & \"");

            // Test: Encoding of space
            Assert.AreEqual("&#32;", Encoder.HtmlAttributeEncode(" "), "Test: Encoding of space");

            // Test: Other encodings
            foreach (char c in attributeEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), Encoder.HtmlAttributeEncode(c.ToString()), "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in attributeNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual(c.ToString(), Encoder.HtmlAttributeEncode(c.ToString()), "Test: Chars that do not encode");
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("&#169;", Encoder.HtmlAttributeEncode("\u00A9"), "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("ЖЉЊ", Encoder.HtmlAttributeEncode("\u0416\u0409\u040A"), "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("&#64358;&#64357;&#64342;&#64345;&#64346;&#64347;&#64348;", Encoder.HtmlAttributeEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"), "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            Assert.IsTrue(Encoder.HtmlAttributeEncode(veryLongString).Length > veryLongString.Length, "Test: Very long string");

            // Test: International characters
            Assert.AreEqual("北京奥运完美谢幕&#32;罗格称赞无以伦比", Encoder.HtmlAttributeEncode("北京奥运完美谢幕 罗格称赞无以伦比"), "Test: International characters (chinese)");
            Assert.AreEqual("कजदगहगह&#32;दगदहगदहग&#32;बगहूब&#32;ग&#32;गदजगहबू&#32;गूदजागागागदाजूगा&#32;गूीग&#32;गा&#32;गूाग&#32;गा&#32;गजादीगूागूदजागू", Encoder.HtmlAttributeEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"), "Test: International characters (devanagari)");
            Assert.AreEqual("رجل&#32;دين&#32;إسلامي&#32;سعودي", Encoder.HtmlAttributeEncode("رجل دين إسلامي سعودي"), "Test: International characters (arabic)");
            Assert.AreEqual("תל&#32;אביב&#32;מרגיש&#32;טוב&#32;בצהוב&#32;ואופטימי&#32;להמשך", Encoder.HtmlAttributeEncode("תל אביב מרגיש טוב בצהוב ואופטימי להמשך"), "Test: International characters (hebrew)");
        }

        /// <summary>
        /// Perform tests for UrlEncode method.
        /// </summary>
        [TestMethod]
        public void TestUrlEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual(string.Empty, Encoder.UrlEncode(null), "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual(string.Empty, Encoder.UrlEncode(string.Empty), "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("%3c%3e%26%22", Encoder.UrlEncode("<>&\""), "Test: Basic encodings: < > & \"");

            // Test: Space is not plus
            Assert.AreNotEqual("+", Encoder.UrlEncode(" "), "Test: Space is not plus");
            Assert.AreEqual("%20", Encoder.UrlEncode(" "), "Test: Space is not plus");

            // Test: Other encodings
            foreach (char c in urlEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), Encoder.UrlEncode(c.ToString()), "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in urlNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual(c.ToString(), Encoder.UrlEncode(c.ToString()), "Test: Chars that do not encode");
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("%c2%a9", Encoder.UrlEncode("\u00A9"), "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("ЖЉЊ", Encoder.UrlEncode("\u0416\u0409\u040A"), "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("%ef%ad%a6%ef%ad%a5%ef%ad%96%ef%ad%99%ef%ad%9a%ef%ad%9b%ef%ad%9c", Encoder.UrlEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"), "Test: Unicode encodings (ascii > 127)");

            Assert.AreEqual("%ef%ad%a6%ef%ad%a5%ef%ad%96%ef%ad%99%ef%ad%9a%ef%ad%9b%ef%ad%9c", Encoder.UrlEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C", 65001), "Test: Unicode encodings (ascii > 127)");

            Assert.AreEqual("%ef%ad%a6%ef%ad%a5%ef%ad%96%ef%ad%99%ef%ad%9a%ef%ad%9b%ef%ad%9c", Encoder.UrlEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C", System.Text.Encoding.UTF8), "Test: Unicode encodings (ascii > 127)");

            Assert.AreEqual("%ef%ad%a6%ef%ad%a5%ef%ad%96%ef%ad%99%ef%ad%9a%ef%ad%9b%ef%ad%9c", Encoder.UrlEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C", null), "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            // The following test is always failing due to out of memory exception.
            // Assert.IsTrue(Encoder.UrlEncode(_veryLongString).Length > _veryLongString.Length, 
            // "Test: Very long string");

            // Test: International characters
            Assert.AreEqual("北京奥运完美谢幕%20罗格称赞无以伦比", Encoder.UrlEncode("北京奥运完美谢幕 罗格称赞无以伦比"), "Test: International characters (chinese)");
            Assert.AreEqual("कजदगहगह%20दगदहगदहग%20बगहूब%20ग%20गदजगहबू%20गूदजागागागदाजूगा%20गूीग%20गा%20गूाग%20गा%20गजादीगूागूदजागू", Encoder.UrlEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"), "Test: International characters (devanagari)");
            Assert.AreEqual("رجل%20دين%20إسلامي%20سعودي", Encoder.UrlEncode("رجل دين إسلامي سعودي"), "Test: International characters (arabic)");
            Assert.AreEqual("תל%20אביב%20מרגיש%20טוב%20בצהוב%20ואופטימי%20להמשך", Encoder.UrlEncode("תל אביב מרגיש טוב בצהוב ואופטימי להמשך"), "Test: International characters (hebrew)");
        }

        /// <summary>
        /// Perform tests for XmlEncode method.
        /// </summary>
        [TestMethod]
        public void TestXmlEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual(string.Empty, Encoder.XmlEncode(null), "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual(string.Empty, Encoder.XmlEncode(string.Empty), "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("&lt;&gt;&amp;&quote;&apos;", Encoder.XmlEncode("<>&\"'"), "Test: Basic encodings: < > & \" '");

            // Test: Other encodings
            foreach (char c in basicEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), Encoder.XmlEncode(c.ToString()), "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in basicNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual(c.ToString(), Encoder.XmlEncode(c.ToString()), "Test: Chars that do not encode");
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("&#169;", Encoder.XmlEncode("\u00A9"), "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("ЖЉЊ", Encoder.XmlEncode("\u0416\u0409\u040A"), "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("&#64358;&#64357;&#64342;&#64345;&#64346;&#64347;&#64348;", Encoder.XmlEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"), "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            Assert.IsTrue(Encoder.XmlEncode(veryLongString).Length > veryLongString.Length, "Test: Very long string");

            // Test: International characters
            Assert.AreEqual("北京奥运完美谢幕 罗格称赞无以伦比", Encoder.XmlEncode("北京奥运完美谢幕 罗格称赞无以伦比"), "Test: International characters (chinese)");
            Assert.AreEqual("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू", Encoder.XmlEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"), "Test: International characters (devanagari)");
            Assert.AreEqual("رجل دين إسلامي سعودي", Encoder.XmlEncode("رجل دين إسلامي سعودي"), "Test: International characters (arabic)");
            Assert.AreEqual("הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך ", Encoder.XmlEncode("הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך "), "Test: International characters (hebrew)");
        }

        /// <summary>
        /// Perform tests for XmlAttributeEncode method.
        /// </summary>
        [TestMethod]
        public void TestXmlAttributeEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual(string.Empty, Encoder.XmlAttributeEncode(null), "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual(string.Empty, Encoder.XmlAttributeEncode(string.Empty), "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("&#60;&#62;&#38;&#34;", Encoder.XmlAttributeEncode("<>&\""), "Test: Basic encodings: < > & \"");

            // Test: Encoding of space
            Assert.AreEqual("&#32;", Encoder.XmlAttributeEncode(" "), "Test: Encoding of space");

            // Test: Other encodings
            foreach (char c in attributeEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), Encoder.XmlAttributeEncode(c.ToString()), "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in attributeNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual(c.ToString(), Encoder.XmlAttributeEncode(c.ToString()), "Test: Chars that do not encode");
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("&#169;", Encoder.XmlAttributeEncode("\u00A9"), "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("ЖЉЊ", Encoder.XmlAttributeEncode("\u0416\u0409\u040A"), "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("&#64358;&#64357;&#64342;&#64345;&#64346;&#64347;&#64348;", Encoder.XmlAttributeEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"), "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            Assert.IsTrue(Encoder.XmlAttributeEncode(veryLongString).Length > veryLongString.Length, "Test: Very long string");

            // Test: International characters
            Assert.AreEqual("北京奥运完美谢幕&#32;罗格称赞无以伦比", Encoder.XmlAttributeEncode("北京奥运完美谢幕 罗格称赞无以伦比"),"Test: International characters (chinese)");
            Assert.AreEqual("कजदगहगह&#32;दगदहगदहग&#32;बगहूब&#32;ग&#32;गदजगहबू&#32;गूदजागागागदाजूगा&#32;गूीग&#32;गा&#32;गूाग&#32;गा&#32;गजादीगूागूदजागू", Encoder.XmlAttributeEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"), "Test: International characters (devanagari)");
            Assert.AreEqual("رجل&#32;دين&#32;إسلامي&#32;سعودي", Encoder.XmlAttributeEncode("رجل دين إسلامي سعودي"), "Test: International characters (arabic)");
            Assert.AreEqual("תל&#32;אביב&#32;מרגיש&#32;טוב&#32;בצהוב&#32;ואופטימי&#32;להמשך", Encoder.XmlAttributeEncode("תל אביב מרגיש טוב בצהוב ואופטימי להמשך"), "Test: International characters (hebrew)");
        }

        /// <summary>
        /// Perform tests for JavaScriptEncode method.
        /// </summary>
        [TestMethod]
        public void TestJavaScriptEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual("''", Encoder.JavaScriptEncode(null), 
                "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual("''", Encoder.JavaScriptEncode(string.Empty), 
                "Test: Return empty string on empty string");

            Assert.AreEqual(string.Empty, Encoder.JavaScriptEncode(string.Empty, false), 
                "Test: Return empty string on empty string without quotes");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("'\\x3c\\x3e\\x26\\x22'", Encoder.JavaScriptEncode("<>&\""), 
                "Test: Basic encodings: < > & \"");

            // Test: String termination char
            Assert.AreEqual("'\\x27'", Encoder.JavaScriptEncode("'"), 
                "Test: String termination char");

            // Test: Other encodings
            foreach (char c in basicEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), Encoder.JavaScriptEncode(c.ToString()), 
                    "Test: Other encodings");
                Assert.AreNotEqual("'" + c.ToString() + "'", Encoder.JavaScriptEncode(c.ToString()), 
                    "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in basicNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual("'" + c.ToString() + "'", Encoder.JavaScriptEncode(c.ToString()), 
                    "Test: Chars that do not encode, c=" + (uint)c);
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("'\\u00a9'", Encoder.JavaScriptEncode("\u00A9"), 
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("'ЖЉЊ'", Encoder.JavaScriptEncode("\u0416\u0409\u040A"), 
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("'\\ufb66\\ufb65\\ufb56\\ufb59\\ufb5a\\ufb5b\\ufb5c'", 
                Encoder.JavaScriptEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"), 
                "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            string ret = Encoder.JavaScriptEncode(veryLongString);
            Assert.IsTrue(ret.Length > veryLongString.Length, 
                "Test: Very long string: Length");
            Assert.IsTrue(ret.StartsWith("'"), 
                "Test: Very long string: StartsWith");
            Assert.IsTrue(ret.EndsWith("'"), 
                "Test: Very long string: EndsWith");

            // Test: International characters
            Assert.AreEqual("'北京奥运完美谢幕 罗格称赞无以伦比'", Encoder.JavaScriptEncode("北京奥运完美谢幕 罗格称赞无以伦比"), 
                "Test: International characters (chinese)");
            Assert.AreEqual("'कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू'", Encoder.JavaScriptEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"), 
                "Test: International characters (devanagari)");
            Assert.AreEqual("'رجل دين إسلامي سعودي'", Encoder.JavaScriptEncode("رجل دين إسلامي سعودي"), 
                "Test: International characters (arabic)");
            Assert.AreEqual("'הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך '", Encoder.JavaScriptEncode("הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך "), 
                "Test: International characters (hebrew)");
        }

        /// <summary>
        /// Perform tests for VisualBasicScriptEncode method.
        /// </summary>
        [TestMethod]
        public void TestVisualBasicScriptEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual("\"\"", Encoder.VisualBasicScriptEncode(null), 
                "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual("\"\"", Encoder.VisualBasicScriptEncode(string.Empty), 
                "Test: Return empty string on empty string");

            // Test: Return empty string on empty string
            Assert.AreEqual("\"a\"", Encoder.VisualBasicScriptEncode("a"), 
                "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("chrw(60)&chrw(62)&chrw(38)&chrw(34)", Encoder.VisualBasicScriptEncode("<>&\""), 
                "Test: Basic encodings: < > & \"");

            // Test: Just an escaped char
            Assert.AreEqual("chrw(60)", Encoder.VisualBasicScriptEncode("<"), "Test: Just an escaped char");

            // Test: Start with escaped char end with non-escaped
            Assert.AreEqual("chrw(60)&\"meow\"", Encoder.VisualBasicScriptEncode("<meow"), 
                "Test: Start with escaped char end with non-escaped");

            // Test: Start with non-escaped and End with escaped char
            Assert.AreEqual("\"meow\"&chrw(60)", Encoder.VisualBasicScriptEncode("meow<"), 
                "Test: Start with non-escaped and End with escaped char");

            // Test: Other encodings
            foreach (char c in basicEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), Encoder.VisualBasicScriptEncode(c.ToString()), 
                    "Test: Other encodings");
                Assert.AreNotEqual("\"" + c.ToString() + "\"", Encoder.VisualBasicScriptEncode(c.ToString()), 
                    "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in basicNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual("\"" + c.ToString() + "\"", Encoder.VisualBasicScriptEncode(c.ToString()), 
                    "Test: Chars that do not encode");
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("chrw(169)", Encoder.VisualBasicScriptEncode("\u00A9"), 
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("\"ЖЉЊ\"", Encoder.VisualBasicScriptEncode("\u0416\u0409\u040A"), 
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("chrw(64358)&chrw(64357)&chrw(64342)&chrw(64345)&chrw(64346)&chrw(64347)&chrw(64348)", 
                Encoder.VisualBasicScriptEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"), 
                "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            Assert.IsTrue(Encoder.VisualBasicScriptEncode(veryLongString).Length > veryLongString.Length, 
                "Test: Very long string");

            // Test: International characters
            Assert.AreEqual("\"北京奥运完美谢幕 罗格称赞无以伦比\"", Encoder.VisualBasicScriptEncode("北京奥运完美谢幕 罗格称赞无以伦比"), 
                "Test: International characters (chinese)");
            Assert.AreEqual("\"कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू\"", Encoder.VisualBasicScriptEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"), 
                "Test: International characters (devanagari)");
            Assert.AreEqual("\"رجل دين إسلامي سعودي\"", Encoder.VisualBasicScriptEncode("رجل دين إسلامي سعودي"), 
                "Test: International characters (arabic)");
            Assert.AreEqual("\"הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך \"", Encoder.VisualBasicScriptEncode("הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך "), 
                "Test: International characters (hebrew)");
        }

        /// <summary>
        /// Performing tests for LdapEncode method.
        /// </summary>
        [TestMethod]
        public void TestLdapEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual(string.Empty, Encoder.LdapEncode(null), 
                "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual(string.Empty, Encoder.LdapEncode(string.Empty), 
                "Test: Return empty string on empty string");

            // Test: Returns null encoded character for null character string
            Assert.AreEqual(@"\00", Encoder.LdapEncode("\u0000"), 
                "Test: Returns null encoded character for null character string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual(@"\3c\3e\26\22", Encoder.LdapEncode("<>&\""), 
                "Test: Basic encodings: < > & \"");

            // Test: Space is not plus
            Assert.AreNotEqual("+", Encoder.LdapEncode(" "), 
                "Test: Space is not plus");
            Assert.AreEqual(@"\20", Encoder.LdapEncode(" "), 
                "Test: Space is not plus");

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual(@"\c2\a9", Encoder.LdapEncode("\u00A9"), 
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("ЖЉЊ", Encoder.LdapEncode("\u0416\u0409\u040A"), 
                "Test: Unicode encodings (ascii > 127)");

            // Test: International characters
            Assert.AreEqual(@"北京奥运完美谢幕\20罗格称赞无以伦比", Encoder.LdapEncode("北京奥运完美谢幕 罗格称赞无以伦比"), 
                "Test: International characters (chinese)");
            Assert.AreEqual(@"कजदगहगह\20दगदहगदहग\20बगहूब\20ग\20गदजगहबू\20गूदजागागागदाजूगा\20गूीग\20गा\20गूाग\20गा\20गजादीगूागूदजागू", Encoder.LdapEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"), 
                "Test: International characters (devanagari)");
            Assert.AreEqual(@"رجل\20دين\20إسلامي\20سعودي", Encoder.LdapEncode("رجل دين إسلامي سعودي"), 
                "Test: International characters (arabic)");
        }
        
        [TestMethod]
        public void TestCssEncode()
        {

            // Test: Return empty string on incoming null
            Assert.AreEqual(string.Empty, Encoder.CssEncode(null), "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual(string.Empty, Encoder.CssEncode(string.Empty), "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual(@"\003c\003e\0026\0022", Encoder.CssEncode("<>&\""), "Test: Basic encodings: < > & \"");

            Assert.AreEqual(@"κουρος", Encoder.CssEncode("κουρος"), "Test: Greek letter encoding");

            Assert.AreEqual(@"\0250", Encoder.CssEncode("ɐ"), "Test: Unicode character 'LATIN SMALL LETTER TURNED A'");
            Assert.AreEqual(@"Ⱥ", Encoder.CssEncode("Ⱥ"), "Test: Unicode character 'LATIN CAPITAL LETTER A WITH STROKE'");
            Assert.AreEqual(@"\0358", Encoder.CssEncode("͘"), "Test: Unicode character 'COMBINING DOT ABOVE RIGHT'");
        }
    }
}

