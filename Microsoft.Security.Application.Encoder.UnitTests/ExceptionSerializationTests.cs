// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionSerializationTests.cs" company="Microsoft Corporation">
//   Copyright (c) 2011 All Rights Reserved, Microsoft Corporation
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
//   Tests serialization of Anti-XSS exceptions
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.Tests
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Microsoft.Security.Application;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests handling of Unicode Surrogates
    /// </summary>
    [TestClass]
    public class ExceptionSerializationTests
    {
        /// <summary>
        /// Validates the 4.0 serialization works correctly for the InvalidUnicodeValueException.
        /// </summary>
        [TestMethod]
        public void InvalidUnicodeValueExceptionShouldSerializeCorrectly()
        {
            // Arrange
            InvalidUnicodeValueException ex = new InvalidUnicodeValueException("the-message", 42);

            // Act
            InvalidUnicodeValueException cloned = SerializeAndDeserialize(ex);

            // Assert
            Assert.AreEqual("Value : 002a" + Environment.NewLine + "Message: the-message", cloned.Message);
            Assert.AreEqual(42, cloned.Value);
        }

        /// <summary>
        /// Validates the 4.0 serialization works correctly for the InvalidSurrogatPairException.
        /// </summary>
        [TestMethod]
        public void InvalidSurrogatePairExceptionShouldSerializeCorrectly()
        {
            // Arrange
            InvalidSurrogatePairException ex = new InvalidSurrogatePairException("the-message", '\u1234', '\u5678');

            // Act
            InvalidSurrogatePairException cloned = SerializeAndDeserialize(ex);

            // Assert
            Assert.AreEqual("Surrogate Pair = 	1234:5678" + Environment.NewLine + "Message: the-message", cloned.Message);
            Assert.AreEqual('\u1234', cloned.HighSurrogate);
            Assert.AreEqual('\u5678', cloned.LowSurrogate);
        }

        /// <summary>
        /// Serializes and deserializes for test.
        /// </summary>
        /// <typeparam name="T">The type of use.</typeparam>
        /// <param name="instance">The instance of the type</param>
        /// <returns>A deserialized instance of <paramref name="instance"/>.</returns>
        private static T SerializeAndDeserialize<T>(T instance)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, instance);
            ms.Position = 0;
            return (T)formatter.Deserialize(ms);
        }
    }
}
