// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullInputTest.cs" company="Microsoft Corporation">
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
//   Test for null
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests input null output null
    /// </summary>
    [TestClass]
    public class NullInputTest
    {
        /// <summary>
        /// Tests that passing a null to html encode should return a null.
        /// </summary>
        [TestMethod]
        public void PassingNullToHtmlEncodeReturnsNull()
        {
            const string target = null;
            const string expected = null;
            string actual = Encoder.HtmlEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a null to url encode should return a null.
        /// </summary>
        [TestMethod]
        public void PassingNullToUrlEncodeReturnsNull()
        {
            const string target = null;
            const string expected = null;
            string actual = Encoder.UrlEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a null to html from url enocde should return a null.
        /// </summary>
        [TestMethod]
        public void PassingNullToHtmlFromUrlEncodeReturnsNull()
        {
            const string target = null;
            const string expected = null;
            string actual = Encoder.HtmlFormUrlEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a null to xml encode should return a null.
        /// </summary>
        [TestMethod]
        public void PassingNullToXmlEncodeReturnsNull()
        {
            const string target = null;
            const string expected = null;
            string actual = Encoder.XmlEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a null to xml attribute encode should return a null.
        /// </summary>
        [TestMethod]
        public void PassingNullToXmlAttributeEncodeReturnsNull()
        {
            const string target = null;
            const string expected = null;
            string actual = Encoder.XmlAttributeEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a null to Ldap DN Encode should return a null.
        /// </summary>
        [TestMethod]
        public void PassingNullToLdapDistinguishedNameEncodeReturnsNull()
        {
            const string target = null;
            const string expected = null;
            string actual = Encoder.LdapDistinguishedNameEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a null to html attribute encode should return a null.
        /// </summary>
        [TestMethod]
        public void PassingNullToHtmlAttributeEncodeReturnsNull()
        {
            const string target = null;
            const string expected = null;
            string actual = Encoder.HtmlAttributeEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a null to Ldap Filter Encode should return a null.
        /// </summary>
        [TestMethod]
        public void PassingNullToLdapFilterEncodeReturnsNull()
        {
            const string target = null;
            const string expected = null;
            string actual = Encoder.LdapFilterEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a null to Css Encode should return a null.
        /// </summary>
        [TestMethod]
        public void PassingNullToCssEncodeReturnsNull()
        {
            const string target = null;
            const string expected = null;
            string actual = Encoder.CssEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a null to Url Path Encode should return a null.
        /// </summary>
        [TestMethod]
        public void PassingNullToUrlPathEncodeReturnsNull()
        {
            const string target = null;
            const string expected = null;
            string actual = Encoder.UrlPathEncode(target);
            Assert.AreEqual(expected, actual);
        }
    }
}
