// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpperUnicode.cs" company="Microsoft Corporation">
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
//   Performs a test on character values beyond the base plane.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Performs tests on character values beyond the base plane.
    /// </summary>
    [TestClass]
    public class UpperUnicode
    {
        /// <summary>
        /// Validates that characters beyond the base plane get encoding, using AncientGreek as the example.
        /// </summary>
        /// <remarks>All characters beyond the base plane should be encoded as their surrogate pair values.</remarks>
        [TestMethod]
        public void UpperUnicodeAncientGreekMusicalNotation()
        {
            const long CodePageStart = 0x1D200;
            const long CodePageEnd = 0x1D24F;

            UnicodeCharacterEncoder.MarkAsSafe(LowerCodeCharts.Default, LowerMidCodeCharts.None, MidCodeCharts.None, UpperMidCodeCharts.None, UpperCodeCharts.None);

            for (long i = CodePageStart; i < CodePageEnd; i++)
            {
                long h = ((i - 0x10000) / 0x400) + 0xD800;
                long l = ((i - 0x10000) % 0x400) + 0xDC00;

                string target = Convert.ToString((char)h) + Convert.ToString((char)l);
                string expected = "&#" + int.Parse(Convert.ToString(i, 16), System.Globalization.NumberStyles.HexNumber) + ";";
                string actual = Encoder.HtmlEncode(target);

                string testmessage = "0x" + i.ToString("x").PadLeft(5, '0') + " (gap value) ";

                Assert.AreEqual(expected, actual, "Encoder.HtmlEncode " + testmessage + " beyond base plane.");
            }
        }
    }
}
