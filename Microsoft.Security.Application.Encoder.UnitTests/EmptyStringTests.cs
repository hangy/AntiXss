// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyStringTests.cs" company="Microsoft Corporation">
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
//   Test for Empty String
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests input Empty String output Empty String
    /// </summary>
    [TestClass]
    public class EmptyStringTests
    {
        /// <summary>
        /// Tests that passing a empty string to html encode should return a empty string.
        /// </summary>
        [TestMethod]
        public void PassingEmptyStringToHtmlEncodeReturnsEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;
            string actual = Encoder.HtmlEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a empty string to url encode should return a empty string.
        /// </summary>
        [TestMethod]
        public void PassingEmptyStringToUrlEncodeReturnsEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;
            string actual = Encoder.UrlEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a empty string to html from url enocde should return a empty string.
        /// </summary>
        [TestMethod]
        public void PassingEmptyStringToHtmlFromUrlEncodeReturnsEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;
            string actual = Encoder.HtmlFormUrlEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a empty string to xml encode should return a empty string.
        /// </summary>
        [TestMethod]
        public void PassingEmptyStringToXmlEncodeReturnsEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;
            string actual = Encoder.XmlEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a empty string to xml attribute encode should return a empty string.
        /// </summary>
        [TestMethod]
        public void PassingEmptyStringToXmlAttributeEncodeReturnsEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;
            string actual = Encoder.XmlAttributeEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a empty string to Ldap DN Encode should return a empty string.
        /// </summary>
        [TestMethod]
        public void PassingEmptyStringToLdapDistinguishedNameEncodeReturnsEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;
            string actual = Encoder.LdapDistinguishedNameEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a empty string to html attribute encode should return a empty string.
        /// </summary>
        [TestMethod]
        public void PassingEmptyStringToHtmlAttributeEncodeReturnsEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;
            string actual = Encoder.HtmlAttributeEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a empty string to Ldap Filter Encode should return a empty string.
        /// </summary>
        [TestMethod]
        public void PassingEmptyStringToLdapFilterEncodeReturnsEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;
            string actual = Encoder.LdapFilterEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a empty string to Css Encode should return a empty string.
        /// </summary>
        [TestMethod]
        public void PassingEmptyStringToCssEncodeReturnsEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;
            string actual = Encoder.CssEncode(target);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing a empty string to URL Path Encode should return a empty string.
        /// </summary>
        [TestMethod]
        public void PassingEmptyStringToUrlPathEncodeReturnsEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;
            string actual = Encoder.UrlPathEncode(target);
            Assert.AreEqual(expected, actual);
        }
    }
}
