// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreditCardInspectorTests.cs" company="Microsoft Corporation">
//   Copyright (c) 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   Tests the Credit Card Inspector
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns.UnitTests
{
    using System.Linq;
    using System.Text;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the Credit Card Inspector
    /// </summary>
    [TestClass]
    public class CreditCardInspectorTests
    {
        /// <summary>
        /// Encoding for string to byte array conversion.
        /// </summary>
        private readonly UTF8Encoding encoding = new UTF8Encoding();

        /// <summary>
        /// Tests setting the settings on the plug-in.
        /// </summary>
        [TestMethod]
        public void TestSettingSettings()
        {
            CreditCardResponseInspector target = new CreditCardResponseInspector();
            CreditCardInspectorSettings settings = new CreditCardInspectorSettings();

            target.Settings = settings;

            Assert.AreSame(settings, target.Settings);
        }

        /// <summary>
        /// Checks the inspector returns a configuration section name.
        /// </summary>
        [TestMethod]
        public void CheckAConfigurationSectionNameIsReturned()
        {
            CreditCardResponseInspector target = new CreditCardResponseInspector();

            Assert.IsFalse(string.IsNullOrEmpty(target.ConfigurationSectionName));
        }

        /// <summary>
        /// Checks the default values when the inspector is created.
        /// </summary>
        [TestMethod]
        public void TestDefaultValues()
        {
            CreditCardResponseInspector target = new CreditCardResponseInspector();

            Assert.IsTrue(target.ExcludedPaths.Count == 0);
        }        

        /// <summary>
        /// Tests that an empty string will not trigger detection.
        /// </summary>
        [TestMethod]
        public void TestEmptyStringWillNotGetDetectedOrChanged()
        {
            string testString = string.Empty;
            byte[] testData = this.encoding.GetBytes(testString);
            CreditCardResponseInspector target = new CreditCardResponseInspector();
            IInspectionResult result = target.Inspect(null, "text/html", ref testData);

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
            Assert.IsTrue(this.encoding.GetBytes(testString).SequenceEqual(testData));
        }

        /// <summary>
        /// Tests that basic page will not trigger detection.
        /// </summary>
        [TestMethod]
        public void TestBasicPageWithNoCreditCardNumbersWillNotGetDetectedOrChanged()
        {
            const string TestString = "<html><body></body></html>";
            byte[] testData = this.encoding.GetBytes(TestString);
            CreditCardResponseInspector target = new CreditCardResponseInspector();
            IInspectionResult result = target.Inspect(null, "text/html", ref testData);

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
            Assert.IsTrue(this.encoding.GetBytes(TestString).SequenceEqual(testData));
        }

        /// <summary>
        /// Tests that page with a simple CC number will trigger detection.
        /// </summary>
        [TestMethod]
        public void TestPageWithCreditCardNumbersWillBeDetected()
        {
            const string TestString = "<html><body>4111111111111111</body></html>";
            byte[] testData = this.encoding.GetBytes(TestString);
            CreditCardResponseInspector target = new CreditCardResponseInspector();
            IInspectionResult result = target.Inspect(null, "text/html", ref testData);

            Assert.AreEqual(InspectionResultSeverity.Halt, result.Severity);
            Assert.IsTrue(this.encoding.GetBytes(TestString).SequenceEqual(testData));
        }

        /// <summary>
        /// Tests that page with a hyphen separated CC number will trigger detection.
        /// </summary>
        [TestMethod]
        public void TestPageWithHypenCreditCardNumbersWillBeDetected()
        {
            const string TestString = "<html><body>4111-1111-1111-1111</body></html>";
            byte[] testData = this.encoding.GetBytes(TestString);
            CreditCardResponseInspector target = new CreditCardResponseInspector();
            IInspectionResult result = target.Inspect(null, "text/html", ref testData);

            Assert.AreEqual(InspectionResultSeverity.Halt, result.Severity);
            Assert.IsTrue(this.encoding.GetBytes(TestString).SequenceEqual(testData));
        }

        /// <summary>
        /// Tests that page with a space separated CC number will trigger detection.
        /// </summary>
        [TestMethod]
        public void TestPageWithSpaceCreditCardNumbersWillBeDetected()
        {
            const string TestString = "<html><body>4111 1111 1111 1111</body></html>";
            byte[] testData = this.encoding.GetBytes(TestString);
            CreditCardResponseInspector target = new CreditCardResponseInspector();
            IInspectionResult result = target.Inspect(null, "text/html", ref testData);

            Assert.AreEqual(InspectionResultSeverity.Halt, result.Severity);
            Assert.IsTrue(this.encoding.GetBytes(TestString).SequenceEqual(testData));
        }

        /// <summary>
        /// Tests that page with a near simple CC number will not trigger detection.
        /// </summary>
        [TestMethod]
        public void TestPageWithNearCreditCardNumbersWillBeNotDetected()
        {
            const string TestString = "<html><body>411111111111111A</body></html>";
            byte[] testData = this.encoding.GetBytes(TestString);
            CreditCardResponseInspector target = new CreditCardResponseInspector();
            IInspectionResult result = target.Inspect(null, "text/html", ref testData);

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
            Assert.IsTrue(this.encoding.GetBytes(TestString).SequenceEqual(testData));
        }

        /// <summary>
        /// Tests that page with a hyphen separated near CC number will not trigger detection.
        /// </summary>
        [TestMethod]
        public void TestPageWithHypenNearCreditCardNumbersWillBeDetected()
        {
            const string TestString = "<html><body>4111-1111-1111-111As</body></html>";
            byte[] testData = this.encoding.GetBytes(TestString);
            CreditCardResponseInspector target = new CreditCardResponseInspector();
            IInspectionResult result = target.Inspect(null, "text/html", ref testData);

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
            Assert.IsTrue(this.encoding.GetBytes(TestString).SequenceEqual(testData));
        }

        /// <summary>
        /// Tests that page with a space separated CC number will trigger detection.
        /// </summary>
        [TestMethod]
        public void TestPageWithSpaceNearCreditCardNumbersWillBeDetected()
        {
            const string TestString = "<html><body>4111 1111 1111 111A</body></html>";
            byte[] testData = this.encoding.GetBytes(TestString);
            CreditCardResponseInspector target = new CreditCardResponseInspector();
            IInspectionResult result = target.Inspect(null, "text/html", ref testData);

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
            Assert.IsTrue(this.encoding.GetBytes(TestString).SequenceEqual(testData));
        }

        /// <summary>
        /// Tests that an empty page doesn't halt execution.
        /// </summary>
        [TestMethod]
        public void TestNullPageInspection()
        {
            byte[] testData = null;
            CreditCardResponseInspector target = new CreditCardResponseInspector();
            IInspectionResult result = target.Inspect(null, "text/html", ref testData);

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
            Assert.IsNull(testData);
        }

        /// <summary>
        /// Tests that page with a space separated CC number will trigger detection.
        /// </summary>
        [TestMethod]
        public void TestPageImageMimeTypeIsNotInspected()
        {
            const string TestString = "4111 1111 1111 1111";
            byte[] testData = this.encoding.GetBytes(TestString);
            CreditCardResponseInspector target = new CreditCardResponseInspector();
            IInspectionResult result = target.Inspect(null, "image/jpeg", ref testData);

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
            Assert.IsTrue(this.encoding.GetBytes(TestString).SequenceEqual(testData));
        }
    }
}
