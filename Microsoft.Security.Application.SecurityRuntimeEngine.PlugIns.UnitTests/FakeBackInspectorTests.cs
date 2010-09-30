// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeBackInspectorTests.cs" company="Microsoft Corporation">
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
//   Tests for the FakeBackPageInspector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns.UnitTests
{
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the FakeBackPageInspector.
    /// </summary>
    [TestClass]
    public class FakeBackInspectorTests
    {
        /// <summary>
        /// Tests that a POST request is acceptable for an IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAHaltIsNotReturnedWhenAPostBackIsGeneratedFromAPostRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(true, "POST");

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
        }

        /// <summary>
        /// Tests that a POST request is acceptable for a non-post-back request.
        /// </summary>
        [TestMethod]
        public void TestThatAHaltIsNotReturnedWhenANonPostBackPageIsGeneratedFromAPostRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(false, "POST");

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
        }

        /// <summary>
        /// Tests that a GET request is not acceptable for an IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAHaltIsReturnedWhenAPostBackIsGeneratedFromAGetRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(true, "GET");

            Assert.AreEqual(InspectionResultSeverity.Halt, result.Severity);
        }

        /// <summary>
        /// Tests that a GET request is acceptable for not IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAContinueIsReturnedWhenANoPostBackIsGeneratedFromAGetRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(false, "GET");

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
        }

        /// <summary>
        /// Tests that a OPTIONS request is not acceptable for an IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAHaltIsReturnedWhenAPostBackIsGeneratedFromAnOptionsRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(true, "OPTIONS");

            Assert.AreEqual(InspectionResultSeverity.Halt, result.Severity);
        }

        /// <summary>
        /// Tests that a GET request is acceptable for not IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAContinueIsReturnedWhenANoPostBackIsGeneratedFromAnOptionsRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(false, "OPTIONS");

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
        }

        /// <summary>
        /// Tests that a HEAD request is not acceptable for an IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAHaltIsReturnedWhenAPostBackIsGeneratedFromAnHeadRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(true, "HEAD");

            Assert.AreEqual(InspectionResultSeverity.Halt, result.Severity);
        }

        /// <summary>
        /// Tests that a HEAD request is acceptable for not IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAContinueIsReturnedWhenANoPostBackIsGeneratedFromAnHeadRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(false, "HEAD");

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
        }

        /// <summary>
        /// Tests that a PUT request is not acceptable for an IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAHaltIsReturnedWhenAPostBackIsGeneratedFromAPutRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(true, "PUT");

            Assert.AreEqual(InspectionResultSeverity.Halt, result.Severity);
        }

        /// <summary>
        /// Tests that a PUT request is acceptable for not IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAContinueIsReturnedWhenANoPostBackIsGeneratedFromAnPutRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(false, "PUT");

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
        }

        /// <summary>
        /// Tests that a DELETE request is not acceptable for an IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAHaltIsReturnedWhenAPostBackIsGeneratedFromADeleteRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(true, "DELETE");

            Assert.AreEqual(InspectionResultSeverity.Halt, result.Severity);
        }

        /// <summary>
        /// Tests that a DELETE request is acceptable for not IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAContinueIsReturnedWhenANoPostBackIsGeneratedFromAnDeleteRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(false, "DELETE");

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
        }

        /// <summary>
        /// Tests that a TRACE request is not acceptable for an IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAHaltIsReturnedWhenAPostBackIsGeneratedFromATraceRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(true, "TRACE");

            Assert.AreEqual(InspectionResultSeverity.Halt, result.Severity);
        }

        /// <summary>
        /// Tests that a TRACE request is acceptable for not IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAContinueIsReturnedWhenANoPostBackIsGeneratedFromATraceRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(false, "TRACE");

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
        }

        /// <summary>
        /// Tests that a CONNECT request is not acceptable for an IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAHaltIsReturnedWhenAPostBackIsGeneratedFromAConnectRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(true, "CONNECT");

            Assert.AreEqual(InspectionResultSeverity.Halt, result.Severity);
        }

        /// <summary>
        /// Tests that a TRACE request is acceptable for not IsPostBack.
        /// </summary>
        [TestMethod]
        public void TestThatAContinueIsReturnedWhenANoPostBackIsGeneratedFromAConnectRequest()
        {
            IInspectionResult result = FakeBackPageInspector.Inspect(false, "CONNECT");

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
        }

        /// <summary>
        /// Tests setting the settings on the plugin.
        /// </summary>
        [TestMethod]
        public void TestSettingSettings()
        {
            FakeBackPageInspector target = new FakeBackPageInspector();
            FakeBackInspectorSettings settings = new FakeBackInspectorSettings();

            target.Settings = settings;

            Assert.AreSame(settings, target.Settings);
        }

        /// <summary>
        /// Checks the inspector returns a configuration section name.
        /// </summary>
        [TestMethod]
        public void CheckAConfigurationSectionNameIsReturned()
        {
            FakeBackPageInspector target = new FakeBackPageInspector();

            Assert.IsFalse(string.IsNullOrEmpty(target.ConfigurationSectionName));
        }

        /// <summary>
        /// Checks the default values when the inspector is created.
        /// </summary>
        [TestMethod]
        public void TestDefaultValues()
        {
            FakeBackPageInspector target = new FakeBackPageInspector();

            Assert.IsTrue(target.ExcludedPaths.Count == 0);
        }        
    }
}
