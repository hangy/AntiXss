using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Microsoft.Security.Application;
#region Namespace - Microsoft.InformationSecurity.CISF

namespace Microsoft.InformationSecurity.CISF
{
    /// <summary>
    /// Test cases for AntiXss library.  
    /// Following Test cases shall provide testing coverage of each encoding
    /// method in the library.
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        #region MEMBERS

        /// <summary>
        /// All characters from Char.MinValue to Char.MaxValue
        /// </summary>
        private static string _allChars = null;
        /// <summary>
        /// All lower ASCII charaters that should be encoded for HtmlEncode
        /// </summary>
        private static string _basicEncodedChars = null;
        /// <summary>
        /// All lower ASCII characters that should not be encoded for HtmlEncode
        /// </summary>
        private static string _basicNonEncodedChars = null;
        /// <summary>
        /// All lower ASCII charaters that should be encoded for HtmlAttributeEncode.
        /// This is same as HtmlEncode with the addition of the space char.
        /// </summary>
        private static string _attributeEncodedChars = null;
        /// <summary>
        /// All lower ASCII characters that should not be encoded for HtmlAttributeEncode.
        /// This is same as HtmlEncode with out the space char.
        /// </summary>
        private static string _attributeNonEncodedChars = null;
        /// <summary>
        /// All lower ASCII characters that should be encoded for UrlEncode.
        /// </summary>
        private static string _urlEncodedChars = null;
        /// <summary>
        /// All lower ASCII characters that should not be encoded for UrlEncode.
        /// </summary>
        private static string _urlNonEncodedChars = null;
        /// <summary>
        /// A very long string (200 * _allChars.Length = ~13MB)
        /// </summary>
        private static string _veryLongString = null;


        #endregion


        #region Construct_Data_For_Tests

        /// <summary>
        /// Construct some usefull data for use in tests.  This is a 
        /// static constructor as each test method is called from a new
        /// class instance.  This will save us a few cpu cycles.
        /// </summary>

        public UnitTests()
        {
            //
            StringBuilder build;

            // Generate a string of all possible characters
            _allChars = "";
            build = new StringBuilder();
            for (uint i = Char.MinValue; i <= Char.MaxValue; i++)
            {
                build.Append((char)i);
            }
            _allChars = build.ToString();

            // Generate a very long string
            build = new StringBuilder();
            for (int i = 0; i < 200; i++)
            {
                build.Append(_allChars);
            }
            _veryLongString = build.ToString();

            // Generate basic list of chars we encode, no unicode
            build = new StringBuilder();
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
            _basicEncodedChars = build.ToString();

            // Generate basic list of chars we don't encode
            build = new StringBuilder();
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
            _basicNonEncodedChars = build.ToString();

            // Setup attribute encoded/non-encoded strings
            _attributeEncodedChars = _basicEncodedChars + " ";
            _attributeNonEncodedChars = _basicNonEncodedChars.Trim(new char[] { ' ' });

            // URL: Generate basic list of chars we encode, no unicode
            build = new StringBuilder();
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
            _urlEncodedChars = build.ToString();

            // URL: Generate basic list of chars we don't encode
            build = new StringBuilder();
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
            _urlNonEncodedChars = build.ToString();

        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #endregion


        #region Execute_Tests

        #region HTMLEncode_Test
        /// <summary>
        /// Performing tests for HtmlEncode method.
        /// </summary>
        [TestMethod]
        public void TestHtmlEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual("", AntiXss.HtmlEncode(null),
                "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual("", AntiXss.HtmlEncode(""),
                "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("&#60;&#62;&#38;&#34;", AntiXss.HtmlEncode("<>&\""),
                "Test: Basic encodings: < > & \"");

            // Test: Other encodings
            foreach (char c in _basicEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), AntiXss.HtmlEncode(c.ToString()),
                    "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in _basicNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual(c.ToString(), AntiXss.HtmlEncode(c.ToString()),
                    "Test: Chars that do not encode");
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("&#169;", AntiXss.HtmlEncode("\u00A9"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("ЖЉЊ", AntiXss.HtmlEncode("\u0416\u0409\u040A"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("&#64358;&#64357;&#64342;&#64345;&#64346;&#64347;&#64348;",
                AntiXss.HtmlEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("シ", AntiXss.HtmlEncode("シ"),
                "Test: Unicode encodings (ascii > 127)");


            // Test: Very long string
            Assert.IsTrue(AntiXss.HtmlEncode(_veryLongString).Length > _veryLongString.Length,
                "Test: Very long string");

            //Test: International characters
            Assert.AreEqual("北京奥运完美谢幕 罗格称赞无以伦比", AntiXss.HtmlEncode("北京奥运完美谢幕 罗格称赞无以伦比"),
                "Test: International characters (chinese)");
            Assert.AreEqual("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू", AntiXss.HtmlEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"),
                "Test: International characters (devanagari)");
            Assert.AreEqual("رجل دين إسلامي سعودي", AntiXss.HtmlEncode("رجل دين إسلامي سعودي"),
                "Test: International characters (arabic)");
            Assert.AreEqual("הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך ", AntiXss.HtmlEncode("הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך "),
                "Test: International characters (hebrew)");
        }

        #endregion

        #region HTMLAttributeEncode_Test
        /// <summary>
        /// Perform tests for HtmlAttributeEncode method.
        /// </summary>
        [TestMethod]
        public void TestHtmlAttributeEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual("", AntiXss.HtmlAttributeEncode(null),
                "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual("", AntiXss.HtmlAttributeEncode(""),
                "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("&#60;&#62;&#38;&#34;", AntiXss.HtmlAttributeEncode("<>&\""),
                "Test: Basic encodings: < > & \"");

            // Test: Encoding of space
            Assert.AreEqual("&#32;", AntiXss.HtmlAttributeEncode(" "),
                "Test: Encoding of space");

            // Test: Other encodings
            foreach (char c in _attributeEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), AntiXss.HtmlAttributeEncode(c.ToString()),
                    "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in _attributeNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual(c.ToString(), AntiXss.HtmlAttributeEncode(c.ToString()),
                    "Test: Chars that do not encode");
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("&#169;", AntiXss.HtmlAttributeEncode("\u00A9"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("ЖЉЊ", AntiXss.HtmlAttributeEncode("\u0416\u0409\u040A"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("&#64358;&#64357;&#64342;&#64345;&#64346;&#64347;&#64348;",
                AntiXss.HtmlAttributeEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"),
                "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            Assert.IsTrue(AntiXss.HtmlAttributeEncode(_veryLongString).Length > _veryLongString.Length,
                "Test: Very long string");

            //Test: International characters
            Assert.AreEqual("北京奥运完美谢幕&#32;罗格称赞无以伦比", AntiXss.HtmlAttributeEncode("北京奥运完美谢幕 罗格称赞无以伦比"),
                "Test: International characters (chinese)");
            Assert.AreEqual("कजदगहगह&#32;दगदहगदहग&#32;बगहूब&#32;ग&#32;गदजगहबू&#32;गूदजागागागदाजूगा&#32;गूीग&#32;गा&#32;गूाग&#32;गा&#32;गजादीगूागूदजागू", AntiXss.HtmlAttributeEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"),
                "Test: International characters (devanagari)");
            Assert.AreEqual("رجل&#32;دين&#32;إسلامي&#32;سعودي", AntiXss.HtmlAttributeEncode("رجل دين إسلامي سعودي"),
                "Test: International characters (arabic)");
            Assert.AreEqual("תל&#32;אביב&#32;מרגיש&#32;טוב&#32;בצהוב&#32;ואופטימי&#32;להמשך", AntiXss.HtmlAttributeEncode("תל אביב מרגיש טוב בצהוב ואופטימי להמשך"),
                "Test: International characters (hebrew)");

        }
        #endregion

        #region URLEncode_Test

        /// <summary>
        /// Perform tests for UrlEncode method.
        /// </summary>
        [TestMethod]
        public void TestUrlEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual("", AntiXss.UrlEncode(null),
                "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual("", AntiXss.UrlEncode(""),
                "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("%3c%3e%26%22", AntiXss.UrlEncode("<>&\""),
                "Test: Basic encodings: < > & \"");

            // Test: Space is not plus
            Assert.AreNotEqual("+", AntiXss.UrlEncode(" "),
                "Test: Space is not plus");
            Assert.AreEqual("%20", AntiXss.UrlEncode(" "),
                "Test: Space is not plus");

            // Test: Other encodings
            foreach (char c in _urlEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), AntiXss.UrlEncode(c.ToString()),
                    "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in _urlNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual(c.ToString(), AntiXss.UrlEncode(c.ToString()),
                    "Test: Chars that do not encode");
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("%u00a9", AntiXss.UrlEncode("\u00A9"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("ЖЉЊ", AntiXss.UrlEncode("\u0416\u0409\u040A"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("%ufb66%ufb65%ufb56%ufb59%ufb5a%ufb5b%ufb5c",
                AntiXss.UrlEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"),
                "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            Assert.IsTrue(AntiXss.UrlEncode(_veryLongString).Length > _veryLongString.Length,
                "Test: Very long string");

            //Test: International characters
            Assert.AreEqual("北京奥运完美谢幕%20罗格称赞无以伦比", AntiXss.UrlEncode("北京奥运完美谢幕 罗格称赞无以伦比"),
                "Test: International characters (chinese)");
            Assert.AreEqual("कजदगहगह%20दगदहगदहग%20बगहूब%20ग%20गदजगहबू%20गूदजागागागदाजूगा%20गूीग%20गा%20गूाग%20गा%20गजादीगूागूदजागू", AntiXss.UrlEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"),
                "Test: International characters (devanagari)");
            Assert.AreEqual("رجل%20دين%20إسلامي%20سعودي", AntiXss.UrlEncode("رجل دين إسلامي سعودي"),
                "Test: International characters (arabic)");
            Assert.AreEqual("תל%20אביב%20מרגיש%20טוב%20בצהוב%20ואופטימי%20להמשך", AntiXss.UrlEncode("תל אביב מרגיש טוב בצהוב ואופטימי להמשך"),
                "Test: International characters (hebrew)");
        }

        #endregion

        #region XMLEncode_Test

        /// <summary>
        /// Perform tests for XmlEncode method.
        /// </summary>
        [TestMethod]
        public void TestXmlEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual("", AntiXss.XmlEncode(null),
                "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual("", AntiXss.XmlEncode(""),
                "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("&#60;&#62;&#38;&#34;", AntiXss.XmlEncode("<>&\""),
                "Test: Basic encodings: < > & \"");

            // Test: Other encodings
            foreach (char c in _basicEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), AntiXss.XmlEncode(c.ToString()),
                    "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in _basicNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual(c.ToString(), AntiXss.XmlEncode(c.ToString()),
                    "Test: Chars that do not encode");
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("&#169;", AntiXss.XmlEncode("\u00A9"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("ЖЉЊ", AntiXss.XmlEncode("\u0416\u0409\u040A"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("&#64358;&#64357;&#64342;&#64345;&#64346;&#64347;&#64348;",
                AntiXss.XmlEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"),
                "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            Assert.IsTrue(AntiXss.XmlEncode(_veryLongString).Length > _veryLongString.Length,
                "Test: Very long string");

            //Test: International characters
            Assert.AreEqual("北京奥运完美谢幕 罗格称赞无以伦比", AntiXss.XmlEncode("北京奥运完美谢幕 罗格称赞无以伦比"),
                "Test: International characters (chinese)");
            Assert.AreEqual("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू", AntiXss.XmlEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"),
                "Test: International characters (devanagari)");
            Assert.AreEqual("رجل دين إسلامي سعودي", AntiXss.XmlEncode("رجل دين إسلامي سعودي"),
                "Test: International characters (arabic)");
            Assert.AreEqual("הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך ", AntiXss.XmlEncode("הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך "),
                "Test: International characters (hebrew)");

        }

        #endregion

        #region XMLAttributeEncode_Test

        /// <summary>
        /// Perform tests for XmlAttributeEncode method.
        /// </summary>
        [TestMethod]
        public void TestXmlAttributeEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual("", AntiXss.XmlAttributeEncode(null),
                "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual("", AntiXss.XmlAttributeEncode(""),
                "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("&#60;&#62;&#38;&#34;", AntiXss.XmlAttributeEncode("<>&\""),
                "Test: Basic encodings: < > & \"");

            // Test: Encoding of space
            Assert.AreEqual("&#32;", AntiXss.XmlAttributeEncode(" "),
                "Test: Encoding of space");

            // Test: Other encodings
            foreach (char c in _attributeEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), AntiXss.XmlAttributeEncode(c.ToString()),
                    "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in _attributeNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual(c.ToString(), AntiXss.XmlAttributeEncode(c.ToString()),
                    "Test: Chars that do not encode");
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("&#169;", AntiXss.XmlAttributeEncode("\u00A9"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("ЖЉЊ", AntiXss.XmlAttributeEncode("\u0416\u0409\u040A"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("&#64358;&#64357;&#64342;&#64345;&#64346;&#64347;&#64348;",
                AntiXss.XmlAttributeEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"),
                "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            Assert.IsTrue(AntiXss.XmlAttributeEncode(_veryLongString).Length > _veryLongString.Length,
                "Test: Very long string");

            //Test: International characters
            Assert.AreEqual("北京奥运完美谢幕&#32;罗格称赞无以伦比", AntiXss.XmlAttributeEncode("北京奥运完美谢幕 罗格称赞无以伦比"),
                "Test: International characters (chinese)");
            Assert.AreEqual("कजदगहगह&#32;दगदहगदहग&#32;बगहूब&#32;ग&#32;गदजगहबू&#32;गूदजागागागदाजूगा&#32;गूीग&#32;गा&#32;गूाग&#32;गा&#32;गजादीगूागूदजागू", AntiXss.XmlAttributeEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"),
                "Test: International characters (devanagari)");
            Assert.AreEqual("رجل&#32;دين&#32;إسلامي&#32;سعودي", AntiXss.XmlAttributeEncode("رجل دين إسلامي سعودي"),
                "Test: International characters (arabic)");
            Assert.AreEqual("תל&#32;אביב&#32;מרגיש&#32;טוב&#32;בצהוב&#32;ואופטימי&#32;להמשך", AntiXss.XmlAttributeEncode("תל אביב מרגיש טוב בצהוב ואופטימי להמשך"),
                "Test: International characters (hebrew)");

        }

        #endregion

        #region JavaScriptEncode_Test

        /// <summary>
        /// Perform tests for JavaScriptEncode method.
        /// </summary>
        [TestMethod]
        public void TestJavaScriptEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual("''", AntiXss.JavaScriptEncode(null),
                "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual("''", AntiXss.JavaScriptEncode(""),
                "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("'\\x3c\\x3e\\x26\\x22'", AntiXss.JavaScriptEncode("<>&\""),
                "Test: Basic encodings: < > & \"");

            // Test: String termination char
            Assert.AreEqual("'\\x27'", AntiXss.JavaScriptEncode("'"),
                "Test: String termination char");

            // Test: Other encodings
            foreach (char c in _basicEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), AntiXss.JavaScriptEncode(c.ToString()),
                    "Test: Other encodings");
                Assert.AreNotEqual("'" + c.ToString() + "'", AntiXss.JavaScriptEncode(c.ToString()),
                    "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in _basicNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual("'" + c.ToString() + "'", AntiXss.JavaScriptEncode(c.ToString()),
                    "Test: Chars that do not encode, c=" + (uint)c);
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("'\\u00a9'", AntiXss.JavaScriptEncode("\u00A9"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("'ЖЉЊ'", AntiXss.JavaScriptEncode("\u0416\u0409\u040A"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("'\\ufb66\\ufb65\\ufb56\\ufb59\\ufb5a\\ufb5b\\ufb5c'",
                AntiXss.JavaScriptEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"),
                "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            string ret = AntiXss.JavaScriptEncode(_veryLongString);
            Assert.IsTrue(ret.Length > _veryLongString.Length,
                "Test: Very long string: Length");
            Assert.IsTrue(ret.StartsWith("'"),
                "Test: Very long string: StartsWith");
            Assert.IsTrue(ret.EndsWith("'"),
                "Test: Very long string: EndsWith");

            //Test: International characters
            Assert.AreEqual("'北京奥运完美谢幕 罗格称赞无以伦比'", AntiXss.JavaScriptEncode("北京奥运完美谢幕 罗格称赞无以伦比"),
                "Test: International characters (chinese)");
            Assert.AreEqual("'कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू'", AntiXss.JavaScriptEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"),
                "Test: International characters (devanagari)");
            Assert.AreEqual("'رجل دين إسلامي سعودي'", AntiXss.JavaScriptEncode("رجل دين إسلامي سعودي"),
                "Test: International characters (arabic)");
            Assert.AreEqual("'הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך '", AntiXss.JavaScriptEncode("הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך "),
                "Test: International characters (hebrew)");
        }

        #endregion

        #region VisualBasicScriptEncode_Test

        /// <summary>
        /// Perform tests for VisualBasicScriptEncode method.
        /// </summary>
        [TestMethod]
        public void TestVisualBasicScriptEncode()
        {
            // Test: Return empty string on incoming null
            Assert.AreEqual("\"\"", AntiXss.VisualBasicScriptEncode(null),
                "Test: Return empty string on incoming null");

            // Test: Return empty string on empty string
            Assert.AreEqual("\"\"", AntiXss.VisualBasicScriptEncode(""),
                "Test: Return empty string on empty string");

            // Test: Basic encodings: < > & "
            Assert.AreEqual("chrw(60)&chrw(62)&chrw(38)&chrw(34)", AntiXss.VisualBasicScriptEncode("<>&\""),
                "Test: Basic encodings: < > & \"");

            // Test: Just an escaped char
            Assert.AreEqual("chrw(60)", AntiXss.VisualBasicScriptEncode("<"), "Test: Just an escaped char");

            // Test: Start with escaped char end with non-escaped
            Assert.AreEqual("chrw(60)&\"meow\"", AntiXss.VisualBasicScriptEncode("<meow"),
                "Test: Start with escaped char end with non-escaped");

            // Test: Start with non-escaped and End with escaped char
            Assert.AreEqual("\"meow\"&chrw(60)", AntiXss.VisualBasicScriptEncode("meow<"),
                "Test: Start with non-escaped and End with escaped char");

            // Test: Other encodings
            foreach (char c in _basicEncodedChars.ToCharArray())
            {
                Assert.AreNotEqual(c.ToString(), AntiXss.VisualBasicScriptEncode(c.ToString()),
                    "Test: Other encodings");
                Assert.AreNotEqual("\"" + c.ToString() + "\"", AntiXss.VisualBasicScriptEncode(c.ToString()),
                    "Test: Other encodings");
            }

            // Test: Chars that do not encode
            foreach (char c in _basicNonEncodedChars.ToCharArray())
            {
                Assert.AreEqual("\"" + c.ToString() + "\"", AntiXss.VisualBasicScriptEncode(c.ToString()),
                    "Test: Chars that do not encode");
            }

            // Test: Unicode encodings (ascii > 127)
            Assert.AreEqual("chrw(169)", AntiXss.VisualBasicScriptEncode("\u00A9"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("\"ЖЉЊ\"", AntiXss.VisualBasicScriptEncode("\u0416\u0409\u040A"),
                "Test: Unicode encodings (ascii > 127)");
            Assert.AreEqual("chrw(64358)&chrw(64357)&chrw(64342)&chrw(64345)&chrw(64346)&chrw(64347)&chrw(64348)",
                AntiXss.VisualBasicScriptEncode("\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C"),
                "Test: Unicode encodings (ascii > 127)");

            // Test: Very long string
            Assert.IsTrue(AntiXss.VisualBasicScriptEncode(_veryLongString).Length > _veryLongString.Length,
                "Test: Very long string");

            //Test: International characters
            Assert.AreEqual("\"北京奥运完美谢幕 罗格称赞无以伦比\"", AntiXss.VisualBasicScriptEncode("北京奥运完美谢幕 罗格称赞无以伦比"),
                "Test: International characters (chinese)");
            Assert.AreEqual("\"कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू\"", AntiXss.VisualBasicScriptEncode("कजदगहगह दगदहगदहग बगहूब ग गदजगहबू गूदजागागागदाजूगा गूीग गा गूाग गा गजादीगूागूदजागू"),
                "Test: International characters (devanagari)");
            Assert.AreEqual("\"رجل دين إسلامي سعودي\"", AntiXss.VisualBasicScriptEncode("رجل دين إسلامي سعودي"),
                "Test: International characters (arabic)");
            Assert.AreEqual("\"הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך \"", AntiXss.VisualBasicScriptEncode("הרכז הפורטוריקני המצוין של מכבי תל אביב מרגיש טוב בצהוב ואופטימי להמשך "),
                "Test: International characters (hebrew)");
        }

        #endregion



        #endregion

    }
}
#endregion

