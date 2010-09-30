// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlInjectionRequestInspector.cs" company="Microsoft Corporation">
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
//
// </copyright>
// <summary>
//   A request inspector to detect some SQL injection strings in request parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Web;

    using Data.Schema.ScriptDom;
    using Data.Schema.ScriptDom.Sql;

    /// <summary>
    /// A request inspector to detect some SQL injection strings in request parameters.
    /// </summary>
    [Export(typeof(IRequestInspector))]
    public class SqlInjectionRequestInspector : IRequestInspector, IConfigurablePlugIn
    {
        /// <summary>
        /// The configuration section name,
        /// </summary>
        private const string ConfigSectionName = "sreSqlInjectionSettings";

        /// <summary>
        /// Internal, strongly typed settings.
        /// </summary>
        private SqlInjectionInspectorSettings internalSettings = new SqlInjectionInspectorSettings();

        /// <summary>
        /// Gets the list of excluded paths for the plug-in.
        /// </summary>
        /// <value>The list of excluded paths for the plug-in.</value>
        public ExcludedPathCollection ExcludedPaths
        {
            get
            {
                return this.internalSettings == null ? null : this.internalSettings.ExcludedPaths;
            }
        }

        /// <summary>
        /// Gets the configuration section name for the plug-in.
        /// </summary>
        /// <value>The configuration section name for the plug-in.</value>
        public string ConfigurationSectionName
        {
            get
            {
                return ConfigSectionName;
            }
        }

        /// <summary>
        /// Gets or sets the settings for the plug-in.
        /// </summary>
        /// <value>The settings for the plug-in.</value>
        public BasePlugInConfiguration Settings
        {
            get
            {
                return this.internalSettings;
            }

            set
            {
                this.internalSettings = (SqlInjectionInspectorSettings)value;
            }
        }

        /// <summary>
        /// Inspects an HTTP request for potential problems.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestBase"/> to inspect.</param>
        /// <returns>
        /// An <see cref="IInspectionResult"/> containing the results of the inspection.
        /// </returns>
        public IInspectionResult Inspect(HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            List<RequestProblemParameter> problemParameters = new List<RequestProblemParameter>();
            NameValueCollection discoveredProblemNameValues;

            if (InspectNameValueCollectionForSqlInjection(request.QueryString, out discoveredProblemNameValues, null))
            {
                NameValueCollection values = discoveredProblemNameValues;
                problemParameters.AddRange(from string parameterName in discoveredProblemNameValues
                                           select new RequestProblemParameter(RequestParameterType.QueryString, parameterName, values[parameterName]));
            }

            if (InspectNameValueCollectionForSqlInjection(request.Form, out discoveredProblemNameValues, this.internalSettings.IgnoredFormParameterNames))
            {
                NameValueCollection values = discoveredProblemNameValues;
                problemParameters.AddRange(from string parameterName in discoveredProblemNameValues
                                           select new RequestProblemParameter(RequestParameterType.Form, parameterName, values[parameterName]));
            }

            if (this.internalSettings.InspectHeaders && InspectNameValueCollectionForSqlInjection(request.Headers, out discoveredProblemNameValues, null))
            {
                NameValueCollection values = discoveredProblemNameValues;
                problemParameters.AddRange(from string parameterName in discoveredProblemNameValues
                                           select new RequestProblemParameter(RequestParameterType.HttpHeader, parameterName, values[parameterName]));
            }

            if (this.internalSettings.InspectCookies && InspectCookieCollectionForSqlInjection(request.Cookies, out discoveredProblemNameValues, this.internalSettings.IgnoredCookieNames))
            {
                NameValueCollection values = discoveredProblemNameValues;
                problemParameters.AddRange(from string parameterName in discoveredProblemNameValues
                                           select new RequestProblemParameter(RequestParameterType.Cookie, parameterName, values[parameterName]));
            }      
  
            RequestInspectionResult result  = problemParameters.Count > 0 ? new RequestInspectionResult(InspectionResultSeverity.Halt, problemParameters) : new RequestInspectionResult(InspectionResultSeverity.Continue);
            return result;
        }

        /// <summary>
        /// Returns a value indicating if the specified input could be a SQL injection attempt.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>True if the input is a possible SQL injection attempt, otherwise false</returns>
        private static bool IsSqlInjectible(string input)
        {
            return DetectCompleteSql(input) || DetectPartialSql(input) || DetectLogicInjection(input);
        }

        /// <summary>
        /// Detects logic short circuit attempts.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>True if a logic short circuit has been detected, otherwise false.</returns>
        private static bool DetectLogicInjection(string input)
        {
            // First determine whether there are two or more instances of single quotes.
            // This could potentially be SQL that attempts to change logic in a SQL query.
            int firstSingleQuoteIndex = input.IndexOf("'", StringComparison.Ordinal);

            // If there is no single quote in the input, then logic injection attack is unlikely.
            if (firstSingleQuoteIndex == -1)
            {
                return false;
            }

            int lastSingleQuoteIndex = input.LastIndexOf("'", StringComparison.Ordinal);

            // If there is only one single quote in the input, then logic injection attack is unlikely.
            if (firstSingleQuoteIndex == lastSingleQuoteIndex)
            {
                return false;
            }

            /* At this point, there is more than one single quote in the input.
             * Strip out any quotes after the first instance and then pass it to the parser.
             * For example, consider the query: select * from users where username=’ ” + param + “ ‘ “.
             * 
             * Input attack vectors:
             * (a) someText' or username='alias
             * (b) a' or 'a'='a
             * 
             * This function is designed to catch these types of attacks. */

            /* split the input into two sections. First variables contains everything from the
             * start to the first occurrence (including) of the single quote.
             * The second half contains everything after the first occurrence of the single quote. */

            string firstHalf = input.Substring(0, firstSingleQuoteIndex + 1);
            string secondHalf = input.Substring(firstSingleQuoteIndex + 1);

            /* Strip out any single quotes */

            secondHalf = secondHalf.Replace("'", string.Empty);

            string finalScrubbedInput = firstHalf + secondHalf;

            TSql100Parser parser = new TSql100Parser(false);
            IList<ParseError> errors;

            /* Try input with permutation. If input completes an invalid and partial SQL,
             * then there is potential for an attack. This will capture cases where
             * attackers attempt to escape out of valid SQL or modify logic. (i.e. OR 1=1)
             */

            const string SqlCommandPrefix = "SELECT * FROM USERS WHERE ID='";
            string completeSql = SqlCommandPrefix + finalScrubbedInput;

            using (StringReader sr = new StringReader(completeSql))
            {
                parser.Parse(sr, out errors);
            }

            return errors == null || errors.Count <= 0;
        }

        /// <summary>
        /// Detects partial SQL command injection.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>True if a partial command has been detected, otherwise false.</returns>
        private static bool DetectPartialSql(string input)
        {
            // Only processing input greater than 2 characters
            if (input.Length < 3)
            {
                return false;
            }

            TSql100Parser parser = new TSql100Parser(false);
            IList<ParseError> errors;

            // Try input with permutation. If input completes an invalid and partial SQL,
            // then there is potential for an attack. This will capture cases where
            // attackers attempt to escape out of valid SQL or modify logic. (i.e. OR 1=1)
            const string StartSql = "SELECT * FROM USERS WHERE ID='";
            string completedSql = StartSql + input;

            using (StringReader sr = new StringReader(completedSql))
            {
                parser.Parse(sr, out errors);
            }

            return errors == null || errors.Count <= 0;
        }

        /// <summary>
        /// Detects a complete SQL command in the specified input.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>True if a logic short circuit has been detected, otherwise false.</returns>
        private static bool DetectCompleteSql(string input)
        {
            // Parser treats strings of length 1 or 2 as stored procedure calls. These will be skipped as they do not form complete SQL.
            input = input.Trim();
            string[] numberOfTokens = input.Split(' ');

            if (numberOfTokens.Length < 3)
            {
                return false;
            }

            // Try input in raw format. Detects complete SQL.
            using (StringReader sr = new StringReader(input))
            {
                TSql100Parser parser = new TSql100Parser(false);
                IList<ParseError> errors;

                parser.Parse(sr, out errors);
                return errors == null || errors.Count <= 0;
            }
        }

        /// <summary>
        /// Inspects the supplied name/value collection for SQL injection.
        /// </summary>
        /// <param name="nameValueCollection">The <see cref="NameValueCollection"/> to inspect.</param>
        /// <param name="problemParameters">A list of the names from <paramref name="nameValueCollection"/> which may create SQL injection attacks.</param>
        /// <param name="namesToSkip">A list of names to skip, if any.</param>
        /// <returns><c>true</c> if a potential SQL injection attack has been detected, otherwise <c>false</c>.</returns>
        private static bool InspectNameValueCollectionForSqlInjection(NameValueCollection nameValueCollection, out NameValueCollection problemParameters, NameConfigurationElementCollection namesToSkip)
        {
            NameValueCollection detectedInjectionParameters = new NameValueCollection();

            foreach (string parameterName in nameValueCollection)
            {
                if (namesToSkip != null && namesToSkip.Contains(parameterName))
                {
                    continue;
                }

                if (IsSqlInjectible(parameterName))
                {
                    detectedInjectionParameters.Add(parameterName, string.Empty);
                }                
                        
                if (IsSqlInjectible(nameValueCollection[parameterName]))
                {
                    detectedInjectionParameters.Add(parameterName, nameValueCollection[parameterName]);
                }
            }

            if (detectedInjectionParameters.Count == 0)
            {
                problemParameters = null;
                return false;                
            }

            problemParameters = detectedInjectionParameters;
            return true;
        }

        /// <summary>
        /// Inspects the supplied Cookie Collection for SQL injection.
        /// </summary>
        /// <param name="cookieCollection">The <see cref="HttpCookieCollection"/> to inspect.</param>
        /// <param name="problemParameters">A list of the cookie names from <paramref name="cookieCollection"/> which may create SQL injection attacks.</param>
        /// <param name="namesToSkip">A list of names to skip, if any.</param>
        /// <returns><c>true</c> if a potential SQL injection attack has been detected, otherwise <c>false</c>.</returns>
        private static bool InspectCookieCollectionForSqlInjection(HttpCookieCollection cookieCollection, out NameValueCollection problemParameters, NameConfigurationElementCollection namesToSkip)
        {
            NameValueCollection detectedInjectionCookies = new NameValueCollection();

            for (int i = 0; i < cookieCollection.Count; i++)
            {
                HttpCookie cookie = cookieCollection[i];

                if (namesToSkip != null &&
                    namesToSkip.Contains(cookie.Name))
                {
                    continue;
                }

                if (cookie.HasKeys)
                {
                    NameValueCollection problemSubKeys;
                    if (InspectNameValueCollectionForSqlInjection(cookie.Values, out problemSubKeys, null))
                    {
                        foreach (string name in problemSubKeys)
                        {
                            detectedInjectionCookies.Add(cookie.Name + @"\" + name, problemSubKeys[name]);
                        }
                    }
                }

                if (IsSqlInjectible(cookie.Name))
                {
                    detectedInjectionCookies.Add(cookie.Name, string.Empty);
                }

                if (IsSqlInjectible(cookie.Value))
                {
                    detectedInjectionCookies.Add(cookie.Name, cookie.Value);
                }
            }

            if (detectedInjectionCookies.Count == 0)
            {
                problemParameters = null;
                return false;
            }

            problemParameters = detectedInjectionCookies;
            return true;
        }
    }
}
