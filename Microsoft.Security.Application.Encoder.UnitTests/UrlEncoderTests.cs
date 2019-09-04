// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlEncoderTests.cs" company="Microsoft Corporation">
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
//   Performs tests on the UrlEncoder
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Performs tests on the UrlEncoder
    /// </summary>
    [TestClass]
    public class UrlEncoderTests
    {
        /// <summary>
        /// Tests that passing a null to UrlPathEncode should return a null.
        /// </summary>
        [TestMethod]
        public void PassingANullToUrlPathEncodeShouldReturnANull()
        {
            const string Target = null;
            const string Expected = null;

            string actual = Encoder.UrlPathEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that passing an empty string to UrlPathEncode should return an empty string.
        /// </summary>
        [TestMethod]
        public void PassingAnEmptyStringToUrlPathEncodeEncodeShouldReturnAnEmptyString()
        {
            string target = string.Empty;
            string expected = string.Empty;

            string actual = Encoder.UrlPathEncode(target);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests that a simple path is left alone.
        /// </summary>
        [TestMethod]
        public void SimpleStringWithNoReservedCharactersShouldNotBeEncoded()
        {
            const string Target = "/abc/def/ghi";
            const string Expected = Target;

            string actual = Encoder.UrlPathEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that a simple path with a query string is left alone.
        /// </summary>
        [TestMethod]
        public void SimpleStringWithNoReservedCharactersAndAQueryStringShouldNotBeEncoded()
        {
            const string Target = "/abc/def/ghi?hello = i'm a query string";
            const string Expected = Target;

            string actual = Encoder.UrlPathEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that a simple path with a space in has the space encoded
        /// </summary>
        [TestMethod]
        public void SimpleStringWithASpaceShouldHaveTheSpaceEncoded()
        {
            const string Target = "/abc/d f/ghi";
            const string Expected = "/abc/d%20f/ghi";

            string actual = Encoder.UrlPathEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that a simple path with an encoded space should have the existing coding left alone
        /// </summary>
        [TestMethod]
        public void SimpleStringWithAnEncodedSpaceShouldNotBeEncoded()
        {
            const string Target = "/abc/d%20f/ghi";
            const string Expected = Target;

            string actual = Encoder.UrlPathEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that alphanumeric characters are left alone
        /// </summary>
        [TestMethod]
        public void AlphanumericStringsShouldNotBeEncoded()
        {
            const string Target = "01234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string Expected = Target;

            string actual = Encoder.UrlPathEncode(Target);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that double encoding of a string will not change it the second time around.
        /// </summary>
        [TestMethod]
        public void DoubleEncodingShouldNotChangeAnAlreadyEncodedString()
        {
            const string Target = "/abc/d f/ghi?hello = i'm a query string";
            const string Expected = "/abc/d%20f/ghi?hello = i'm a query string";

            string intermediate = Encoder.UrlPathEncode(Target);
            string actual = Encoder.UrlPathEncode(intermediate);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that URL decoding an encoded path should end up with the original path
        /// </summary>
        [TestMethod]
        public void DecodingAPathWithUrlDecodeShouldRevertToTheOriginalString()
        {
            const string Target = "/abc/d f/ghi?hello = i'm a query string";
            const string Expected = Target;

            string intermediate = Encoder.UrlPathEncode(Target);
            string actual = HttpUtility.UrlDecode(intermediate);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Tests that all valid hex characters do not get encoded.
        /// </summary>
        [TestMethod]
        public void ValidHexCharactersShouldNotBeEncoded()
        {
            for (int i = 0; i < 256; i++)
            {
                string targetLower = "%" + i.ToString("x2");
                string targetUpper = "%" + i.ToString("X2");
                string expectedLower = targetLower;
                string expectedUpper = targetUpper;

                string actualLower = Encoder.UrlPathEncode(targetLower);
                string actualUpper = Encoder.UrlPathEncode(targetUpper);

                Assert.AreEqual(expectedLower, actualLower);
                Assert.AreEqual(expectedUpper, actualUpper);
            }
        }

        /// <summary>
        /// Validates that characters on the safe list are not encoded, and ones which are not are encoded.
        /// </summary>
        [TestMethod]
        public void IndividualCharactersNotOnTheSafeListShouldBeEncoded()
        {
            List<int> safeList = new List<int>
            {
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, // Digits
                0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, // A-Z
                0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, // a-z
                0x23, 0x25, 0x28, 0x29, 0x2d, 0x2e, 0x2f, 0x5c, 0x5f, 0x7e, // Safe symbols
                0x3F // Special case - question mark, which we split on.
            };

            for (int i = 0; i <= 127; i++)
            {
                string target = Convert.ToString((char)i);

                string expected;

                if (!safeList.Contains(i))
                {
                    expected = "%" + i.ToString("x2");
                }
                else
                {
                    expected = target;
                }

                string actual = Encoder.UrlPathEncode(target);

                Assert.AreEqual(expected, actual, "UrlPathEncode(0x" + i.ToString("x2") + ")");
            }
        }
    }
}
