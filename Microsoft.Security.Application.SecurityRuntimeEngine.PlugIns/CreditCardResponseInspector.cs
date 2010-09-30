// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreditCardResponseInspector.cs" company="Microsoft Corporation">
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
//   A response inspector to halt any response which contains a credit card number.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System;
    using System.ComponentModel.Composition;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    using SecurityRuntimeEngine;

    /// <summary>
    /// A response inspector to halt any response which contains a credit card number.
    /// </summary>
    [Export(typeof(IResponseInspector))]
    public sealed class CreditCardResponseInspector : IResponseInspector, IConfigurablePlugIn
    {
        /// <summary>
        /// The configuration section name,
        /// </summary>
        private const string ConfigSectionName = "sreCCInspectorSettings";

        /// <summary>
        /// A regular expression for credit card number detection.
        /// </summary>
        /// <remarks>
        /// Original regular expression sourced from http://www.regexlib.com/REDetails.aspx?regexp_id=1288
        /// </remarks>
        private readonly Regex cardRegex = new Regex(@"/*(^|[^0-9])((4[0-9]|5[12345]|6[0245])[0-9]{2}[^0-9]?[0-9]{4}[^0-9]?[0-9]{4}[^0-9]?[0-9]{4}|3[47][0-9]{2}[^0-9]?[0-9]{6}[^0-9]?[0-9]{5})([^0-9]|$)/*", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline); 

        /// <summary>
        /// The encoding for the response.
        /// </summary>
        private readonly UTF8Encoding encoding = new UTF8Encoding();

        /// <summary>
        /// Internal, strongly typed settings.
        /// </summary>        
        private CreditCardInspectorSettings internalSettings = new CreditCardInspectorSettings();

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
                this.internalSettings = value as CreditCardInspectorSettings;
            }
        }

        /// <summary>
        /// Inspects an HTTP response for potential problems.
        /// </summary>
        /// <param name="request">The request that triggered this response.</param>
        /// <param name="contentType">The content type of the response.</param>
        /// <param name="response">A byte array containing the response to inspect.</param>
        /// <returns>
        /// An <see cref="IInspectionResult"/> containing the results of the inspection.
        /// </returns>
        /// <remarks>
        /// The response is writable - any changes made to it will be sent to the client.
        /// </remarks>
        public IInspectionResult Inspect(HttpRequestBase request, string contentType, ref byte[] response)
        {
            if (!string.IsNullOrEmpty(contentType))
            {
                string loweredContentType = contentType.ToUpperInvariant();
                if (loweredContentType != "TEXT/HTML" && 
                    loweredContentType != "TEXT/PLAIN" &&
                    loweredContentType != "TEXT/XML" && 
                    loweredContentType != "APPLICATION/XML" &&
                    loweredContentType != "APPLICATION/RSS+XML" && 
                    loweredContentType != "APPLICATION/XHTML+XML")
                {
                    return new ResponseInspectionResult(InspectionResultSeverity.Continue);
                }
            }

            try
            {
                string responseAsString = this.encoding.GetString(response);
                Match match = this.cardRegex.Match(responseAsString);
                if (match.Success)
                {
                    return new ResponseInspectionResult();
                }
            }
            catch (DecoderFallbackException)
            {                
            }
            catch (ArgumentException)
            {
            }

            return new ResponseInspectionResult(InspectionResultSeverity.Continue);
        }
    }
}
