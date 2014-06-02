// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utf16StringReaderTest.cs" company="Microsoft Corporation">
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
//   Tests the Utf16StringReader
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Security.Application;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the Utf16StringReader
    /// </summary>
    [TestClass]
    public class Utf16StringReaderTest 
    {
        /// <summary>
        /// Unicode replacement character.
        /// </summary>
        private const int UnicodeReplacementCharacterCodePoint = '\uFFFD';

        /// <summary>
        /// Validates invalid Unicode points are substituted correctly.
        /// </summary>
        [TestMethod]
        public void CanHandleAllInvalidUnicodeCodePoints()
        {
            // Arrange
            int[] allInvalidUnicodeCodePoints =
                CreateCharacterRange(0xDC00, 0xDFFF) // low surrogates before high surrogates so that we don't accidentally make a valid code point in this test
                .Concat(CreateCharacterRange(0xD800, 0xDBFF))
                .ToArray();
            int[] expectedResult = Enumerable.Repeat(UnicodeReplacementCharacterCodePoint, 2048 /* num surrogates */).ToArray();

            string stringContainingAllInvalidUnicodeCodePoints = string.Concat(allInvalidUnicodeCodePoints.Select(ConvertFromCodePoint));

            // Act
            int[] roundTrippedCodePoints = ReadAllScalarValues(stringContainingAllInvalidUnicodeCodePoints);

            // Assert
            CollectionAssert.AreEqual(expectedResult, roundTrippedCodePoints);
        }

        /// <summary>
        /// Validates that valid Unicode points are left along.
        /// </summary>
        [TestMethod]
        public void CanHandleAllValidUnicodeCodePoints() 
        {
            // Arrange
            int[] allValidUnicodeCodePoints =
                CreateCharacterRange(0x0000, 0xD7FF)
                .Concat(CreateCharacterRange(0xE000, 0x10FFFF))
                .ToArray();
            int[] expectedResult = allValidUnicodeCodePoints;

            string stringContainingAllValidUnicodeCodePoints = string.Concat(allValidUnicodeCodePoints.Select(ConvertFromCodePoint));

            // Act
            int[] roundTrippedCodePoints = ReadAllScalarValues(stringContainingAllValidUnicodeCodePoints);

            // Assert
            CollectionAssert.AreEqual(expectedResult, roundTrippedCodePoints);
        }

        /// <summary>
        /// Validates that a string with weird surrogates in the midst of the string gets substituted correctly.
        /// </summary>
        [TestMethod]
        public void InvalidStringWithSurrogates() 
        {
            // Arrange
            string inputString = "X-\uD800-\uDFFF-Z";
            int[] expectedResult = new int[] 
            {
                (int)'X',
                (int)'-',
                UnicodeReplacementCharacterCodePoint,
                (int)'-',
                UnicodeReplacementCharacterCodePoint,
                (int)'-',
                (int)'Z' 
            };

            // Act
            int[] roundTrippedCodePoints = ReadAllScalarValues(inputString);

            // Assert
            CollectionAssert.AreEqual(expectedResult, roundTrippedCodePoints);
        }

        /// <summary>
        /// Validates that a string with a high surrogate at the end of the string gets substituted correctly.
        /// </summary>
        [TestMethod]
        public void InvalidStringWithSurrogatesHighSurrogateAtEndOfString() 
        {
            // Arrange
            string inputString = "X-\uD800";
            int[] expectedResult = new int[] 
            {
                (int)'X',
                (int)'-',
                UnicodeReplacementCharacterCodePoint 
            };

            // Act
            int[] roundTrippedCodePoints = ReadAllScalarValues(inputString);

            // Assert
            CollectionAssert.AreEqual(expectedResult, roundTrippedCodePoints);
        }

        /// <summary>
        /// Validates that a string with a two high surrogates at the end of the string gets substituted correctly.
        /// </summary>
        [TestMethod]
        public void InvalidStringWithSurrogatesTwoHighSurrogates() 
        {
            // This test makes sure that we don't accidentally consume the next
            // character after encountering an unmatched high surrogate.

            // Arrange
            string inputString = "X-\uD800\uD800\uDD00-Z";
            int[] expectedResult = new int[] 
            {
                (int)'X',
                (int)'-',
                UnicodeReplacementCharacterCodePoint,
                0x10100,
                (int)'-',
                (int)'Z' 
            };

            // Act
            int[] roundTrippedCodePoints = ReadAllScalarValues(inputString);

            // Assert
            CollectionAssert.AreEqual(expectedResult, roundTrippedCodePoints);
        }

        /// <summary>
        /// Validates that a string with a valid surrogates  at the end of the string does not get substituted.
        /// </summary>
        [TestMethod]
        public void ValidStringWithSurrogates() 
        {
            // Arrange
            string inputString = "X-\U00010000-\uABCD-\U0010ABCD-Z";
            int[] expectedResult = new int[] 
            {
                (int)'X',
                (int)'-',
                0x10000,
                (int)'-',
                0xABCD,
                (int)'-',
                0x10ABCD,
                (int)'-',
                (int)'Z' 
            };

            // Act
            int[] roundTrippedCodePoints = ReadAllScalarValues(inputString);

            // Assert
            CollectionAssert.AreEqual(expectedResult, roundTrippedCodePoints);
        }

        /// <summary>
        /// Manually converts a integer to its code point.
        /// </summary>
        /// <param name="codePoint">The code point to convert.</param>
        /// <returns>A string representing the code point.</returns>
        private static string ConvertFromCodePoint(int codePoint) 
        {
            // Do this manually since ConvertFromUtf32 has its own invalid character checking,
            // and we might want to generate invalid strings.
            if (codePoint <= char.MaxValue) 
            {
                return new string(new char[] { (char)codePoint });
            }

            codePoint -= 0x10000;
            return new string(new char[] { (char)((codePoint / 1024) + 0xD800), (char)((codePoint % 1024) + 0xDC00) });
        }

        /// <summary>
        /// Creates a range of integers of characters from <paramref name="firstCodePointInclusive"/> to <paramref name="lastCodePointInclusive"/>.
        /// </summary>
        /// <param name="firstCodePointInclusive">The number to start at.</param>
        /// <param name="lastCodePointInclusive">The number to finish at.</param>
        /// <returns>A range of integers of characters from <paramref name="firstCodePointInclusive"/> to <paramref name="lastCodePointInclusive"/>.</returns>
        private static IEnumerable<int> CreateCharacterRange(int firstCodePointInclusive, int lastCodePointInclusive) 
        {
            return Enumerable.Range(firstCodePointInclusive, lastCodePointInclusive - firstCodePointInclusive + 1);
        }

        /// <summary>
        /// Reads all scalar values from the specified input string.
        /// </summary>
        /// <param name="inputString">The string to read values from.</param>
        /// <returns>An array of scalar values contained in the string.</returns>
        private static int[] ReadAllScalarValues(string inputString) 
        {
            Utf16StringReader stringReader = new Utf16StringReader(inputString);
            List<int> retVal = new List<int>();
            while (true) 
            {
                int nextValue = stringReader.ReadNextScalarValue();
                if (nextValue < 0) 
                {
                    return retVal.ToArray();
                }

                retVal.Add(nextValue);
            }
        }
    }
}
