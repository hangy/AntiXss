// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClickJackInspectorTests.cs" company="Microsoft Corporation">
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
//   Tests for the ClickJackResponseHeaderInspector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns.UnitTests
{
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the ClickJackResponseHeaderInspector
    /// </summary>
    [TestClass]
    public class ClickJackInspectorTests
    {
        /// <summary>
        /// Checks the inspector returns a configuration section name.
        /// </summary>
        [TestMethod]
        public void CheckAConfigurationSectionNameIsReturned()
        {
            ClickJackResponseHeaderInspector target = new ClickJackResponseHeaderInspector();

            Assert.IsFalse(string.IsNullOrEmpty(target.ConfigurationSectionName));
        }

        /// <summary>
        /// Checks the default values when the inspector is created.
        /// </summary>
        [TestMethod]
        public void TestDefaultValues()
        {
            ClickJackResponseHeaderInspector target = new ClickJackResponseHeaderInspector();

            Assert.IsTrue(target.ExcludedPaths.Count == 0);
        }

        /// <summary>
        /// Checks the default header value is added to a request when no configuration has taken place.
        /// </summary>
        [TestMethod]
        public void TestDefaultHeaderIsAdded()
        {
            ClickJackResponseHeaderInspector target = new ClickJackResponseHeaderInspector();
            MockHttpResponse httpResponse = new MockHttpResponse();

            target.Inspect(null, httpResponse);

            Assert.IsTrue(httpResponse.Headers.HasKeys());
            Assert.IsNotNull(httpResponse.Headers["X-FRAME-OPTIONS"]);
            Assert.AreEqual("DENY", httpResponse.Headers["X-FRAME-OPTIONS"]);
        }

        /// <summary>
        /// Checks that if the inspector is configured to send DENY it does so.
        /// </summary>
        [TestMethod]
        public void CheckHeaderIsAddedWithConfigurationSourcedDenyValue()
        {
            ClickJackResponseHeaderInspector target = new ClickJackResponseHeaderInspector();
            ClickJackInspectorSettings settings = new ClickJackInspectorSettings
                                                      {
                                                          HeaderValue = ClickJackHeaderValue.Deny
                                                      };
            target.Settings = settings;
            MockHttpResponse httpResponse = new MockHttpResponse();

            target.Inspect(null, httpResponse);

            Assert.IsTrue(httpResponse.Headers.HasKeys());
            Assert.IsNotNull(httpResponse.Headers["X-FRAME-OPTIONS"]);
            Assert.AreEqual("DENY", httpResponse.Headers["X-FRAME-OPTIONS"]);
        }

        /// <summary>
        /// Checks that if the inspector is configured to send SAMEORIGIN it does so.
        /// </summary>
        [TestMethod]
        public void CheckHeaderIsAddedWithConfigurationSourcedSameOriginValue()
        {
            ClickJackResponseHeaderInspector target = new ClickJackResponseHeaderInspector();
            ClickJackInspectorSettings settings = new ClickJackInspectorSettings
                                                      {
                                                          HeaderValue = ClickJackHeaderValue.SameOrigin
                                                      };
            target.Settings = settings;
            MockHttpResponse httpResponse = new MockHttpResponse();

            target.Inspect(null, httpResponse);

            Assert.IsTrue(httpResponse.Headers.HasKeys());
            Assert.IsNotNull(httpResponse.Headers["X-FRAME-OPTIONS"]);
            Assert.AreEqual("SAMEORIGIN", httpResponse.Headers["X-FRAME-OPTIONS"]);
        }

        /// <summary>
        /// Checks the continue result is returned when the header is set to DENY.
        /// </summary>
        [TestMethod]
        public void CheckReturnValueWillNotHaltExecutionWithDenyHeaderSet()
        {
            ClickJackResponseHeaderInspector target = new ClickJackResponseHeaderInspector();
            ClickJackInspectorSettings settings = new ClickJackInspectorSettings
            {
                HeaderValue = ClickJackHeaderValue.Deny
            };
            target.Settings = settings;

            IInspectionResult result = target.Inspect(null, new MockHttpResponse());

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
        }

        /// <summary>
        /// Checks the continue result is returned when the header is set to SAMEORIGIN.
        /// </summary>
        [TestMethod]
        public void CheckReturnValueWillNotHaltExecutionWithSameOriginHeaderSet()
        {
            ClickJackResponseHeaderInspector target = new ClickJackResponseHeaderInspector();
            ClickJackInspectorSettings settings = new ClickJackInspectorSettings
            {
                HeaderValue = ClickJackHeaderValue.SameOrigin
            };
            target.Settings = settings;

            IInspectionResult result = target.Inspect(null, new MockHttpResponse());

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
        }

        /// <summary>
        /// Tests setting the settings on the plugin.
        /// </summary>
        [TestMethod]
        public void TestSettingSettings()
        {
            ClickJackResponseHeaderInspector target = new ClickJackResponseHeaderInspector();
            ClickJackInspectorSettings settings = new ClickJackInspectorSettings();

            target.Settings = settings;

            Assert.AreSame(settings, target.Settings);
        }

        /// <summary>
        /// Tests that the default settings are for DENY.
        /// </summary>
        [TestMethod]
        public void TestDefaultSettings()
        {
            ClickJackInspectorSettings target = new ClickJackInspectorSettings();
            Assert.AreEqual(ClickJackHeaderValue.Deny, target.HeaderValue);
        }

        /// <summary>
        /// Tests that setting the HeaderValue property in settings works as expected.
        /// </summary>
        [TestMethod]
        public void TestSettingsHeaderValueProperty()
        {
            ClickJackInspectorSettings target = new ClickJackInspectorSettings();
            const ClickJackHeaderValue Expected = ClickJackHeaderValue.SameOrigin;

            target.HeaderValue = ClickJackHeaderValue.SameOrigin;

            Assert.AreEqual(Expected, target.HeaderValue);
        }
    }
}
