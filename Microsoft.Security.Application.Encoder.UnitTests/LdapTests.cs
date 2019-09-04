// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LdapTests.cs" company="Microsoft Corporation">
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
//   Tests LDAP encoding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests LDAP encoding.
    /// </summary>
    [TestClass]
    public class LdapTests
    {
        /// <summary>
        /// Tests that passing a null to filter encoding should return a null.
        /// </summary>
        [TestMethod]
        public void PassingANullToFilterEncodeShouldReturnANull()
        {
            const string Target = null;
            const string Expected = null;

            string actual = Encoder.LdapFilterEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that passing an empty string to filter encoding should return an empty string.
        /// </summary>
        [TestMethod]
        public void PassingAnEmptyStringToFilterEncodeShouldReturnAnEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;

            string actual = Encoder.LdapFilterEncode(target);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests the encoding of RFC4515 parenthesis example.
        /// </summary>
        [TestMethod]
        public void FilterEncodingShouldEncodeParenthesis()
        {
            const string Target = "Parens R Us (for all your parenthetical needs)";
            const string Expected = @"Parens R Us \28for all your parenthetical needs\29";

            string actual = Encoder.LdapFilterEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests the encoding of RFC4515 asterisk example.
        /// </summary>
        [TestMethod]
        public void FilterEncodingShouldEncodeAsterisks()
        {
            const string Target = "*";
            const string Expected = @"\2a";

            string actual = Encoder.LdapFilterEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests the encoding of RFC4515 backslash example.
        /// </summary>
        [TestMethod]
        public void FilterEncodingShouldEncodeBackslashes()
        {
            const string Target = @"C:\MyFile";
            const string Expected = @"C:\5cMyFile";

            string actual = Encoder.LdapFilterEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests the encoding of RFC4515 binary example.
        /// </summary>
        [TestMethod]
        public void FilterEncodingShouldEncodeBinary()
        {
            char[] binaryData = new[] { '\u0000', '\u0000', '\u0000', '\u0004' };
            string target = new string(binaryData);
            const string Expected = @"\00\00\00\04";

            string actual = Encoder.LdapFilterEncode(target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests the encoding of RFC4515 accented example.
        /// </summary>
        [TestMethod]
        public void FilterEncodingShouldEncodeAccentedCharacters()
        {
            const string Target = "Lučić";
            const string Expected = @"Lu\c4\8di\c4\87";

            string actual = Encoder.LdapFilterEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests the null non-printable character is encoded correctly in filter encoding.
        /// </summary>
        [TestMethod]
        public void FilterNullCharactersShouldBeHashThenHexEncoded()
        {
            const string Target = "\u0000";
            const string Expected = @"\00";

            string actual = Encoder.LdapFilterEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests the null non-printable character is encoded correctly in DN encoding.
        /// </summary>
        [TestMethod]
        public void FilterDelCharactersShouldBeHashThenHexEncoded()
        {
            const string Target = "\u007f";
            const string Expected = @"\7f";

            string actual = Encoder.LdapFilterEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Checks the characters on http://projects.webappsec.org/LDAP-Injection are all escaped during filter encoding.
        /// </summary>
        [TestMethod]
        public void PassingAnUnsafeCharacterToFilterEncodeShouldNotReturnTheCharacter()
        {
            string[] targetArray = new[] { "\u0000", "(", ")", "\\", "*", "/" };

            foreach (string target in targetArray)
            {
                string notExpected = target;
                string actual = Encoder.LdapFilterEncode(target);
                Assert.AreNotEqual(notExpected, actual);
            }
        }

        /// <summary>
        /// Tests that passing a null to DN encoding should return a null.
        /// </summary>
        [TestMethod]
        public void PassingANullToDistinguisedNameEncodeShouldReturnANull()
        {
            const string Target = null;
            const string Expected = null;

            string actual = Encoder.LdapDistinguishedNameEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that passing an empty string to DN encoding should return an empty string.
        /// </summary>
        [TestMethod]
        public void PassingAnEmptyStringToDistinguisedNameEncodeShouldReturnAnEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;

            string actual = Encoder.LdapDistinguishedNameEncode(target);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that passing the always encoded values to DN encoding returns their slash escaped value.
        /// </summary>
        [TestMethod]
        public void PassingAnAlwaysEncodedValueToDistinguishedNameEncodeShouldReturnTheirSlashEscapedValue()
        {
            const string Target = ",+\"\\<>;";
            const string Expected = "\\,\\+\\\"\\\\\\<\\>\\;";

            string actual = Encoder.LdapDistinguishedNameEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Checks the characters on http://projects.webappsec.org/LDAP-Injection are all escaped during DN encoding.
        /// </summary>
        [TestMethod]
        public void PassingAnUnsafeCharacterToDistinguishedNameEncodeShouldNotReturnTheCharacter()
        {
            string[] targetArray = new[] { "&", "!", "|", "=", "<", ">", ",", "+", "-", "\"", "'", ";" };

            foreach (string target in targetArray)
            {
                string notExpected = target;
                string actual = Encoder.LdapDistinguishedNameEncode(target);
                Assert.AreNotEqual(notExpected, actual);
            }
        }

        /// <summary>
        /// Test that spaces at the start of a string get escaped properly.
        /// </summary>
        [TestMethod]
        public void PassingASpaceAtTheBeginningOfAStringToDistinguishedNameEncodeMustEscapeTheSpace()
        {
            const string Target = "  abcdef";
            const string Expected = "\\  abcdef";

            string actual = Encoder.LdapDistinguishedNameEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Test that spaces at the end of a string get escaped properly.
        /// </summary>
        [TestMethod]
        public void PassingASpaceAtTheEndOfAStringToDistinguishedNameEncodeMustEscapeTheSpace()
        {
            const string Target = "abcdef  ";
            const string Expected = "abcdef \\ ";

            string actual = Encoder.LdapDistinguishedNameEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Test that a single space input does not trigger both start and finish escaping, but returns a single escaped space.
        /// </summary>
        [TestMethod]
        public void PassingASingleSpaceStringShouldReturnASingleEscapedString()
        {
            const string Target = " ";
            const string Expected = "\\ ";

            string actual = Encoder.LdapDistinguishedNameEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Test that hashes at the start of a string get escaped properly.
        /// </summary>
        [TestMethod]
        public void PassingAHashAtTheBeginningOfAStringToDistinguishedNameEncodeMustEscapeTheHash()
        {
            const string Target = "##abcdef";
            const string Expected = "\\##abcdef";

            string actual = Encoder.LdapDistinguishedNameEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that the override of initial character handling works with hashes.
        /// </summary>
        [TestMethod]

        public void PassingAHashAtTheBeginningOfAStringToDistinguishedNameEncodeButOverridingTheInitialRulesDoesNotEncodeTheHash()
        {
            const string Target = "##abcdef";
            const string Expected = "##abcdef";

            string actual = Encoder.LdapDistinguishedNameEncode(Target, false, true);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that the override of initial character handling works with space.
        /// </summary>
        [TestMethod]
        public void PassingASpaceAtTheBeginningOfAStringToDistinguishedNameEncodeButOverridingTheInitialRulesDoesNotEncodeTheSpace()
        {
            const string Target = "  abcdef";
            const string Expected = "  abcdef";

            string actual = Encoder.LdapDistinguishedNameEncode(Target, false, true);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that the override of final character handling works with hashes.
        /// </summary>
        [TestMethod]
        public void PassingASpaceAtTheEndOfAStringToDistinguishedNameEncodeButOverridingTheFinalRuleDoesNotEncodeTheSpace()
        {
            const string Target = "abcdef# ";
            const string Expected = "abcdef# ";

            string actual = Encoder.LdapDistinguishedNameEncode(Target, true, false);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests the null non-printable character is encoded correctly in DN encoding.
        /// </summary>
        [TestMethod]
        public void DistinguishedNameNullCharactersShouldBeHashThenHexEncoded()
        {
            const string Target = "\u0000";
            const string Expected = "#00";

            string actual = Encoder.LdapDistinguishedNameEncode(Target, true, false);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests the null non-printable character is encoded correctly in DN encoding.
        /// </summary>
        [TestMethod]
        public void DistinguishedNameDelCharactersShouldBeHashThenHexEncoded()
        {
            const string Target = "\u007f";
            const string Expected = "#7F";

            string actual = Encoder.LdapDistinguishedNameEncode(Target, true, false);

            Assert.AreEqual(Expected, actual);
        }
    }
}
