// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClickJackInspectorSettings.cs" company="Microsoft Corporation">
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
//   Settings for the ClickJack Header Inspector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System;
    using System.Configuration;

    /// <summary>
    /// A list of possible click-jack header values
    /// </summary>
    [Serializable]
    public enum ClickJackHeaderValue
    {
        /// <summary>
        /// Denies rendering if the page is enclosed in a frame.
        /// </summary>
        Deny = 0,

        /// <summary>
        /// Blocks rendering only if the origin of the top level-browsing-context is different than the origin of the content containing the X-FRAME-OPTIONS directive.
        /// </summary>
        SameOrigin = 1
    }

    /// <summary>
    /// Settings for the ClickJack Header Inspector
    /// </summary>
    public class ClickJackInspectorSettings : BasePlugInConfiguration
    {
        /// <summary>
        /// The property name for the clickjack setting attribute.
        /// </summary>
        private const string HeaderValueProperty = "headerValue";

        /// <summary>
        /// Gets or sets the header value to insert.
        /// </summary>
        /// <value>The header value to insert.</value>
        [ConfigurationProperty(HeaderValueProperty, IsRequired = false, DefaultValue = ClickJackHeaderValue.Deny)]
        public ClickJackHeaderValue HeaderValue
        {
            get
            {
                return (ClickJackHeaderValue)this[HeaderValueProperty];
            }

            set
            {
                this[HeaderValueProperty] = value;                
            }
        }
    }
}
