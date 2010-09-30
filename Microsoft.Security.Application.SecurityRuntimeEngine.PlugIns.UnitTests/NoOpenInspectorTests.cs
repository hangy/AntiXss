// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoOpenInspectorTests.cs" company="Microsoft Corporation">
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
//   Tests for the NoOpenResponseHeaderInspector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns.UnitTests
{
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the NoOpenResponseHeaderInspector.
    /// </summary>
    [TestClass]
    public class NoOpenInspectorTests
    {
        /// <summary>
        /// Checks the inspector returns a configuration section name.
        /// </summary>
        [TestMethod]
        public void CheckAConfigurationSectionNameIsReturned()
        {
            NoOpenResponseHeaderInspector target = new NoOpenResponseHeaderInspector();

            Assert.IsFalse(string.IsNullOrEmpty(target.ConfigurationSectionName));
        }

        /// <summary>
        /// Checks the default values when the inspector is created.
        /// </summary>
        [TestMethod]
        public void TestDefaultValues()
        {
            NoOpenResponseHeaderInspector target = new NoOpenResponseHeaderInspector();

            Assert.IsTrue(target.ExcludedPaths.Count == 0);
        }

        /// <summary>
        /// Checks the default header value is added to a request when no configuration has taken place.
        /// </summary>
        [TestMethod]
        public void TestHeaderIsAdded()
        {
            NoOpenResponseHeaderInspector target = new NoOpenResponseHeaderInspector();
            MockHttpResponse httpResponse = new MockHttpResponse();

            target.Inspect(null, httpResponse);

            Assert.IsTrue(httpResponse.Headers.HasKeys());
            Assert.IsNotNull(httpResponse.Headers["X-Download-Options"]);
            Assert.AreEqual("noopen", httpResponse.Headers["X-Download-Options"]);
        }

        /// <summary>
        /// Tests setting the settings on the plugin.
        /// </summary>
        [TestMethod]
        public void TestSettingSettings()
        {
            NoOpenResponseHeaderInspector target = new NoOpenResponseHeaderInspector();
            NoOpenHeaderInspectorSettings settings = new NoOpenHeaderInspectorSettings();

            target.Settings = settings;

            Assert.AreSame(settings, target.Settings);
        }

        /// <summary>
        /// Checks the continue result is returned.
        /// </summary>
        [TestMethod]
        public void CheckReturnValueWillNotHaltExecution()
        {
            NoOpenResponseHeaderInspector target = new NoOpenResponseHeaderInspector();
            
            IInspectionResult result = target.Inspect(null, new MockHttpResponse());

            Assert.AreEqual(InspectionResultSeverity.Continue, result.Severity);
        }
    }
}
