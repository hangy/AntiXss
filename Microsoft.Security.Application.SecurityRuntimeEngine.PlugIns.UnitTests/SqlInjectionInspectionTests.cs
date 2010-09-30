// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlInjectionInspectionTests.cs" company="Microsoft Corporation">
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
//   Tests for the SqlInjectionInspector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns.UnitTests
{
    using System.Web;

    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test methods for the SQL Injection Inspector
    /// </summary>
    [TestClass]
    public class SqlInjectionInspectionTests
    {
        /// <summary>
        /// Checks the inspector returns a configuration section name.
        /// </summary>
        [TestMethod]
        public void CheckAConfigurationSectionNameIsReturned()
        {
            SqlInjectionRequestInspector target = new SqlInjectionRequestInspector();

            Assert.IsFalse(string.IsNullOrEmpty(target.ConfigurationSectionName));
        }

        /// <summary>
        /// Checks the default values when the inspector is created.
        /// </summary>
        [TestMethod]
        public void TestDefaultValues()
        {
            SqlInjectionRequestInspector target = new SqlInjectionRequestInspector();

            Assert.IsTrue(target.ExcludedPaths.Count == 0);
        }

        /// <summary>
        /// Tests setting the settings on the plugin.
        /// </summary>
        [TestMethod]
        public void TestSettingSettings()
        {
            SqlInjectionRequestInspector target = new SqlInjectionRequestInspector();
            SqlInjectionInspectorSettings settings = new SqlInjectionInspectorSettings();

            target.Settings = settings;

            Assert.AreSame(settings, target.Settings);
        }

        /// <summary>
        /// Tests excecution would not be halted if there are no parameters.
        /// (Although this would never happen in 'real life'.)
        /// </summary>
        [TestMethod]
        public void TestThatExecutionIsNotHaltedIfThereAreNoParametersAtAll()
        {
            SqlInjectionRequestInspector target = new SqlInjectionRequestInspector();

            IInspectionResult result = target.Inspect(new MockHttpRequest());

            Assert.AreNotEqual(InspectionResultSeverity.Halt, result.Severity);
        }

        /// <summary>
        /// Tests that a normal test string does not halt processing.
        /// </summary>
        [TestMethod]
        public void TestHelloDoesNotHalt()
        {
            const string AttackVector = "Hello";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Continue;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Tests that a short string does not halt processing.
        /// </summary>
        [TestMethod]
        public void TestShortStringDoesNotHalt()
        {
            const string AttackVector = "Hi";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Continue;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Tests that or 1=1--; is not detected as a hack attempt.
        /// </summary>
        [TestMethod]
        public void TestOr1WithoutTerminator()
        {
            const string AttackVector = "or 1=1--";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Continue;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Tests an injection with an AND, not an OR
        /// </summary>
        [TestMethod]
        public void TestLogicalAnd()
        {
            const string AttackVector = "x' AND something IS NULL; --";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Halt;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Tests a table sniff attempt is detected.
        /// </summary>
        [TestMethod]
        public void TestTableSniff()
        {
            const string AttackVector = "x' AND 1=(SELECT COUNT(*) FROM superSecretTable); --";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Halt;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Tests a table drop attempt is detected.
        /// </summary>
        [TestMethod]
        public void TestTableDrop()
        {
            const string AttackVector = "x'; DROP TABLE byebye; --";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Halt;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Tests a insert attempt is detected.
        /// </summary>
        [TestMethod]
        public void TestInsertDetection()
        {
            const string AttackVector = "x'; INSERT INTO users ('email','password','username','fullname') VALUES ('steveb@contoso.com','developersdevelopersdevelopers','steveb','Steve B');--";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Halt;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Tests a update attempt is detected.
        /// </summary>
        [TestMethod]
        public void TestUpdateDetection()
        {
            const string AttackVector = "x'; UPDATE users SET email = 'billg@fabrikam.com' WHERE email = 'steveb@contoso.com';--";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Halt;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Tests that a statement without terminating comments, but an OR instead is detected.
        /// </summary>
        [TestMethod]
        public void TestTerminatorWithNoCommentsIsDetected()
        {
            const string AttackVector = "anything' OR 'x'='x";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Halt;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Tests a whole command string is detected.
        /// </summary>
        [TestMethod]
        public void TestWholeCommandNoShortCircuitIsDetected()
        {
            const string AttackVector = "select * from thingy;";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Halt;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Tests almost a whole command is not a false positive.
        /// </summary>
        [TestMethod]
        public void TestAlmostWholeCommandNoShortCircuitIsDetected()
        {
            const string AttackVector = "selec * from thingy;";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Continue;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Tests a bunch of keywords is not a false positive.
        /// </summary>
        [TestMethod]
        public void TestKeywordsButNoCommand()
        {
            const string AttackVector = "SELECT INSERT FROM UNION;";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Continue;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Tests for xp_cmdshell calls.
        /// </summary>
        [TestMethod]
        public void TestXpCmdShellIsDetected()
        {
            const string AttackVector = "'; exec xp_cmdshell 'fdisk';";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Halt;

            RequestInspectionResult queryStringResult = InspectWithQueryStringAttack(AttackVector);
            RequestInspectionResult formResult = InspectWithFormAttack(AttackVector);
            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector);
            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringResult.Severity, "QueryString sourced attack");
            Assert.AreEqual(Expected, formResult.Severity, "Form sourced attack");
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult queryStringNameResult = InspectWithQueryStringParamAttack(AttackVector);
            RequestInspectionResult formNameResult = InspectWithFormParamAttack(AttackVector);
            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector);
            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector);

            Assert.AreEqual(Expected, queryStringNameResult.Severity, "QueryString parameter name sourced attack");
            Assert.AreEqual(Expected, formNameResult.Severity, "Form variable name sourced attack");
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Test that form field exclusions work.
        /// </summary>
        [TestMethod]
        public void TestThatTheDefaultFormFieldExclusionsWork()
        {
            const string AttackVector = "'; exec xp_cmdshell 'fdisk';";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Continue;
            SqlInjectionInspectorSettings settings = new SqlInjectionInspectorSettings();

            foreach (NameConfigurationElement ignoredFormParameterName in settings.IgnoredFormParameterNames)
            {
                RequestInspectionResult result = InspectWithFormAttack(AttackVector, ignoredFormParameterName.Name, settings);
                Assert.AreEqual(Expected, result.Severity);
            }
        }

        /// <summary>
        /// Test that setting custom form field exclusions work as expected.
        /// </summary>
        [TestMethod]
        public void TestThatCustomDefaultFieldExclusionWorks()
        {
            const string AttackVector = "'; exec xp_cmdshell 'fdisk';";
            const string ParameterName = "hello";            
            NameConfigurationElement excludedField = new NameConfigurationElement(ParameterName);
            SqlInjectionInspectorSettings settings = new SqlInjectionInspectorSettings();
            settings.IgnoredFormParameterNames.Clear();
            settings.IgnoredFormParameterNames.Add(excludedField);

            RequestInspectionResult resultWhenUsingIgnoredParameter = InspectWithFormAttack(AttackVector, ParameterName, settings);
            Assert.AreEqual(InspectionResultSeverity.Continue, resultWhenUsingIgnoredParameter.Severity);

            RequestInspectionResult resultWhenUsingNotIgnoredParameter = InspectWithFormAttack(AttackVector, ParameterName + "__", settings);
            Assert.AreEqual(InspectionResultSeverity.Halt, resultWhenUsingNotIgnoredParameter.Severity);
        }

        /// <summary>
        /// Check that turning off cookie inspection via settings results in an attack in a cookie being missed.
        /// </summary>
        [TestMethod]
        public void CheckTurningOffCookieInspectionWorks()
        {
            const string AttackVector = "'or 1=1--";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Continue;
            SqlInjectionInspectorSettings settings = new SqlInjectionInspectorSettings
                                                         {
                                                             InspectCookies = false
                                                         };            

            RequestInspectionResult cookieResult = InspectWithCookieAttack(AttackVector, settings);
            Assert.AreEqual(Expected, cookieResult.Severity, "Cookie sourced attack");

            RequestInspectionResult cookieNameResult = InspectWithCookieNameAttack(AttackVector, settings);
            Assert.AreEqual(Expected, cookieNameResult.Severity, "Cookie name sourced attack");
        }

        /// <summary>
        /// Test that setting custom cookie exclusions work as expected.
        /// </summary>
        [TestMethod]
        public void TestThatCustomCookieExclusionWorks()
        {
            const string AttackVector = "select * from secret";
            const string CookieName = "nomnomnom";
            NameConfigurationElement excludedField = new NameConfigurationElement(CookieName);
            SqlInjectionInspectorSettings settings = new SqlInjectionInspectorSettings();
            settings.IgnoredCookieNames.Clear();
            settings.IgnoredCookieNames.Add(excludedField);

            RequestInspectionResult resultWhenUsingIgnoredParameter = InspectWithCookieAttack(AttackVector, CookieName, settings);
            Assert.AreEqual(InspectionResultSeverity.Continue, resultWhenUsingIgnoredParameter.Severity);

            RequestInspectionResult resultWhenUsingNotIgnoredParameter = InspectWithCookieAttack(AttackVector, CookieName + "__", settings);
            Assert.AreEqual(InspectionResultSeverity.Halt, resultWhenUsingNotIgnoredParameter.Severity);
        }

        /// <summary>
        /// Check that turning off header inspection via settings results in an attack in a header being missed.
        /// </summary>
        [TestMethod]
        public void CheckTurningOffHeaderInspectionWorks()
        {
            const string AttackVector = "'or 1=1--";
            const InspectionResultSeverity Expected = InspectionResultSeverity.Continue;
            SqlInjectionInspectorSettings settings = new SqlInjectionInspectorSettings
            {
                InspectHeaders = false
            };

            RequestInspectionResult headerResult = InspectWithHeaderAttack(AttackVector, settings);
            Assert.AreEqual(Expected, headerResult.Severity, "Header sourced attack");

            RequestInspectionResult headerNameResult = InspectWithHeaderNameAttack(AttackVector, settings);
            Assert.AreEqual(Expected, headerNameResult.Severity, "Header name sourced attack");
        }

        /// <summary>
        /// Runs the SQL Injection inspector against the specified request.
        /// </summary>
        /// <param name="request">The request to inspect.</param>
        /// <param name="settings">The inspector setetings to use, if any.</param>
        /// <returns>The results of the inspection.</returns>
        private static RequestInspectionResult RunInspection(HttpRequestBase request, SqlInjectionInspectorSettings settings)
        {
            SqlInjectionRequestInspector target = new SqlInjectionRequestInspector();
            if (settings != null)
            {
                target.Settings = settings;
            }

            IInspectionResult result = target.Inspect(request);
            RequestInspectionResult typedResult = result as RequestInspectionResult;
            Assert.IsNotNull(typedResult);
            return typedResult;
        }
     
        /// <summary>
        /// Builds a request putting the specified attack vector into a query string parameter then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithQueryStringAttack(string attackVector)
        {            
            MockHttpRequest request = new MockHttpRequest();
            request.QueryString.Add("param1", attackVector);
            return RunInspection(request, null);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into a form parameter then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithFormAttack(string attackVector)
        {
            return InspectWithFormAttack(attackVector, null);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into a form parameter then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <param name="settings">The settings to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithFormAttack(string attackVector, SqlInjectionInspectorSettings settings)
        {
            return InspectWithFormAttack(attackVector, "param1", settings);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into a form parameter then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <param name="parameterName">The parameter name to use.</param>
        /// <param name="settings">The settings to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithFormAttack(string attackVector, string parameterName, SqlInjectionInspectorSettings settings)
        {
            MockHttpRequest request = new MockHttpRequest();
            request.Form.Add(parameterName, attackVector);
            return RunInspection(request, settings);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into a cookie then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithCookieAttack(string attackVector)
        {
            return InspectWithCookieAttack(attackVector, null);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into a cookie then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <param name="settings">The settings to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithCookieAttack(string attackVector, SqlInjectionInspectorSettings settings)
        {
            return InspectWithCookieAttack(attackVector, "cookie1", settings);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into a cookie then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <param name="cookieName">The cookie name to use.</param>
        /// <param name="settings">The settings to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithCookieAttack(string attackVector, string cookieName, SqlInjectionInspectorSettings settings)
        {
            MockHttpRequest request = new MockHttpRequest();
            HttpCookie attackCookie = new HttpCookie(cookieName, attackVector)
            {
                Domain = "contoso.com"
            };
            request.Cookies.Add(attackCookie);
            return RunInspection(request, settings);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into an http header then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithHeaderAttack(string attackVector)
        {
            return InspectWithHeaderAttack(attackVector, null);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into an http header then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <param name="settings">The settings to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithHeaderAttack(string attackVector, SqlInjectionInspectorSettings settings)
        {
            MockHttpRequest request = new MockHttpRequest();
            request.Headers.Add("httpParam1", attackVector);
            return RunInspection(request, settings);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into a query string parameter name then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithQueryStringParamAttack(string attackVector)
        {
            MockHttpRequest request = new MockHttpRequest();
            request.QueryString.Add(attackVector, "hi");
            return RunInspection(request, null);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into a form parameter name then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithFormParamAttack(string attackVector)
        {
            MockHttpRequest request = new MockHttpRequest();
            request.Form.Add(attackVector, "hi");
            return RunInspection(request, null);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into a cookie name then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithCookieNameAttack(string attackVector)
        {
            return InspectWithCookieNameAttack(attackVector, null);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into a cookie name then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <param name="settings">The settings to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithCookieNameAttack(string attackVector, SqlInjectionInspectorSettings settings)
        {
            MockHttpRequest request = new MockHttpRequest();
            HttpCookie attackCookie = new HttpCookie(attackVector, "hi")
            {
                Domain = "contoso.com"
            };
            request.Cookies.Add(attackCookie);
            return RunInspection(request, settings);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into an http header name then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithHeaderNameAttack(string attackVector)
        {
            return InspectWithHeaderNameAttack(attackVector, null);
        }

        /// <summary>
        /// Builds a request putting the specified attack vector into an http header name then inspects the request.
        /// </summary>
        /// <param name="attackVector">The potential attack vector to use.</param>
        /// <param name="settings">The settings to use.</param>
        /// <returns>The results of the page inspection.</returns>
        private static RequestInspectionResult InspectWithHeaderNameAttack(string attackVector, SqlInjectionInspectorSettings settings)
        {
            MockHttpRequest request = new MockHttpRequest();
            request.Headers.Add(attackVector, "hi");
            return RunInspection(request, settings);
        }      
    }
}
