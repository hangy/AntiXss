// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncoderUtilTest.cs" company="Microsoft Corporation">
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
//   Tests the Encoder utility class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.Tests
{
    using System;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the Encoder utility class
    /// </summary>
    [TestClass]
    public class EncoderUtilTest
    {
        /// <summary>
        /// Tests the output string builder.
        /// </summary>
        [TestMethod]
        public void GetOutputStringBuilder()
        {
            RunGetOutputStringBuilderTest(300, 30, 9000, "I forgot how to multiply.");
            RunGetOutputStringBuilderTest(300, 100, 16 * 1024, "Default capacity should never exceed 16k chars if input length is small.");
            RunGetOutputStringBuilderTest(30000, 2, 30000, "Default capacity can exceed 16k chars if input length is large.");
            RunGetOutputStringBuilderTest(1024, Int32.MaxValue, 16 * 1024, "Overflow guard failed.");
        }

        /// <summary>
        /// Runs a test based on the input parameters.
        /// </summary>
        /// <param name="inputLength">The length of the input.</param>
        /// <param name="worstCaseOutputCharsPerInputChar">The worst case scenario.</param>
        /// <param name="expectedCapacity">The expected capacity of the string builder created.</param>
        /// <param name="failureMessage">The message to use if the test fails.</param>
        private static void RunGetOutputStringBuilderTest(int inputLength, int worstCaseOutputCharsPerInputChar, int expectedCapacity, string failureMessage)
        {
            StringBuilder builder = EncoderUtil.GetOutputStringBuilder(inputLength, worstCaseOutputCharsPerInputChar);
            Assert.AreEqual(expectedCapacity, builder.Capacity, failureMessage);
        }
    }
}
