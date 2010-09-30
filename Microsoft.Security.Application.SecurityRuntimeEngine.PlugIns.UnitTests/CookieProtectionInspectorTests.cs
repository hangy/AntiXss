// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieProtectionInspectorTests.cs" company="Microsoft Corporation">
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
//   Tests for the CookieProtectionInspector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns.UnitTests
{
    using Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the CookieProtectionInspector
    /// </summary>
    [TestClass]
    public class CookieProtectionInspectorTests
    {
        /// <summary>
        /// Checks the default values for Excluded Paths when the inspector is created.
        /// </summary>
        [TestMethod]
        public void TestCookieDefaultValues()
        {
            CookieProtectionInspector target = new CookieProtectionInspector();

            Assert.IsTrue(target.ExcludedPaths.Count == 0);
        }

        /// <summary>
        /// Checks the inspector returns a configuration section name.
        /// </summary>
        [TestMethod]
        public void TestCookieConfigSectionNameIsReturned()
        {
            CookieProtectionInspector target = new CookieProtectionInspector();

            Assert.IsFalse(string.IsNullOrEmpty(target.ConfigurationSectionName));
        }

        /// <summary>
        /// Tests setting the settings on the plugin.
        /// </summary>
        [TestMethod]
        public void TestCookieSettingSettings()
        {
            CookieProtectionInspector target = new CookieProtectionInspector();
            CookieProtectionInspectorSettings settings = new CookieProtectionInspectorSettings();

            target.Settings = settings;

            Assert.AreSame(settings, target.Settings);
        }

        /// <summary>
        /// Tests getting the ExcludedPaths for the plugin.
        /// </summary>
        [TestMethod]
        public void TestCookieGetExcludedPaths()
        {
            CookieProtectionInspector target = new CookieProtectionInspector();
            CookieProtectionInspectorSettings settings = new CookieProtectionInspectorSettings();

            target.Settings = settings;

            Assert.AreSame(settings.ExcludedPaths, target.Settings.ExcludedPaths);
        }

        /// <summary>
        /// Tests getting the ExcludedPaths for the plugin.
        /// </summary>
        [TestMethod]
        public void TestCookieNameConfigurationElementCollectionOverrideMethods()
        {
            CookieProtectionInspectorSettings settings = new CookieProtectionInspectorSettings();
            int hashCode = settings.GetHashCode();
            Assert.IsTrue(hashCode == 0);

            NameConfigurationElement excludedCookie = new NameConfigurationElement("testCookie1");
            settings.ExcludedCookies.Add(excludedCookie);

            hashCode = settings.GetHashCode();
            Assert.IsTrue(hashCode != 0);

            excludedCookie = new NameConfigurationElement("testCookie2");
            settings.ExcludedCookies[0] = excludedCookie;
            Assert.IsTrue(settings.ExcludedCookies[0].Name == "testCookie2");

            Assert.IsNotNull(settings.ExcludedCookies.CreateChild());

            Assert.IsTrue(settings.ExcludedCookies.IndexOf(string.Empty) == -1);

            settings.ExcludedCookies.Remove(NameConfigurationElementCollection.GetKey(settings.ExcludedCookies[0]));
            Assert.IsTrue(settings.ExcludedCookies.Count == 0);
        }

        /// <summary>
        /// Tests the cookie HTTP only is set to true.
        /// </summary>
        [TestMethod]
        public void TestCookieHttpOnlyTrue()
        {
            CookieProtectionInspectorSettings cookieProtectionInspectorSettings = new CookieProtectionInspectorSettings();
            NameConfigurationElement excludedCookie = new NameConfigurationElement("testCookie1");
            cookieProtectionInspectorSettings.ExcludedCookies.Add(excludedCookie);

            MockHttpResponse httpResponse = new MockHttpResponse();
            httpResponse.AppendCookie("testCookie2", "Test Cookie");

            Assert.IsFalse(httpResponse.Cookies[0].HttpOnly);
            CookieProtectionInspector.Inspect(httpResponse.Cookies[0], cookieProtectionInspectorSettings);
            Assert.IsTrue(httpResponse.Cookies[0].HttpOnly);
        }

        /// <summary>
        /// Tests the cookie HTTP only remains true when previously set and cookie is an exception.
        /// </summary>
        [TestMethod]
        public void TestCookieExcludedHttpOnlyRemainsTrue()
        {
            CookieProtectionInspectorSettings cookieProtectionInspectorSettings = new CookieProtectionInspectorSettings();
            NameConfigurationElement excludedCookie = new NameConfigurationElement("testCookie1");
            cookieProtectionInspectorSettings.ExcludedCookies.Add(excludedCookie);

            MockHttpResponse httpResponse = new MockHttpResponse();
            httpResponse.AppendCookie("testCookie2", "Test Cookie");
            httpResponse.Cookies[0].HttpOnly = true;

            Assert.IsTrue(httpResponse.Cookies[0].HttpOnly);
            CookieProtectionInspector.Inspect(httpResponse.Cookies[0], cookieProtectionInspectorSettings);
            Assert.IsTrue(httpResponse.Cookies[0].HttpOnly);
        }

        /// <summary>
        /// Tests the cookie HTTP only remains false when cookie is an exception.
        /// </summary>
        [TestMethod]
        public void TestCookieExcludedHttpOnlyRemainsFalse()
        {
            CookieProtectionInspectorSettings cookieProtectionInspectorSettings = new CookieProtectionInspectorSettings();

            NameConfigurationElement excludedCookie = new NameConfigurationElement("testCookie");
            cookieProtectionInspectorSettings.ExcludedCookies.Add(excludedCookie);

            MockHttpResponse httpResponse = new MockHttpResponse();
            httpResponse.AppendCookie("testCookie", "Test Cookie");

            Assert.IsFalse(httpResponse.Cookies[0].HttpOnly);
            CookieProtectionInspector.Inspect(httpResponse.Cookies[0], cookieProtectionInspectorSettings);
            Assert.IsFalse(httpResponse.Cookies[0].HttpOnly);
        }

        /// <summary>
        /// Tests the Inspect method when cookie parameter is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void TestCookieWhenCookieIsNull()
        {
            CookieProtectionInspectorSettings cookieProtectionInspectorSettings = new CookieProtectionInspectorSettings();

            NameConfigurationElement excludedCookie = new NameConfigurationElement("testCookie");
            cookieProtectionInspectorSettings.ExcludedCookies.Add(excludedCookie);

            MockHttpResponse httpResponse = new MockHttpResponse();
            httpResponse.AppendCookie("testCookie", "Test Cookie");

            Assert.IsFalse(httpResponse.Cookies[0].HttpOnly);
            CookieProtectionInspector.Inspect(null, cookieProtectionInspectorSettings);
            Assert.IsFalse(httpResponse.Cookies[0].HttpOnly);
        }

        /// <summary>
        /// Tests the Inspect method when NameConfigurationElement parameter is null.
        /// </summary>
        [TestMethod]
        public void TestCookieWhenNameConfigurationElementIsNull()
        {
            CookieProtectionInspectorSettings cookieProtectionInspectorSettings = new CookieProtectionInspectorSettings();

            NameConfigurationElement excludedCookie = new NameConfigurationElement("testCookie");
            cookieProtectionInspectorSettings.ExcludedCookies.Add(excludedCookie);

            MockHttpResponse httpResponse = new MockHttpResponse();
            httpResponse.AppendCookie("testCookie", "Test Cookie");

            Assert.IsFalse(httpResponse.Cookies[0].HttpOnly);
            CookieProtectionInspector.Inspect(httpResponse.Cookies[0], null);
            Assert.IsTrue(httpResponse.Cookies[0].HttpOnly);
        }
    }
}
