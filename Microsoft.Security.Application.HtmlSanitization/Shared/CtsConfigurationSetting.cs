// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CtsConfigurationSetting.cs" company="Microsoft Corporation">
//   Copyright (c) 2008, 2009, 2010 All Rights Reserved, Microsoft Corporation
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
//   Contains a configuration name and its arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Internal
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains a configuration name and its arguments.
    /// </summary>
    internal class CtsConfigurationSetting
    {
        /// <summary>
        /// The configuration name.
        /// </summary>
        private readonly string configurationName;

        /// <summary>
        /// The configuration arguments.
        /// </summary>
        private readonly IList<CtsConfigurationArgument> arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="CtsConfigurationSetting"/> class.
        /// </summary>
        /// <param name="name">The setting name.</param>
        internal CtsConfigurationSetting(string name)
        {
            this.configurationName = name;
            this.arguments = new List<CtsConfigurationArgument>();
        }

        /// <summary>
        /// Gets the name of the setting.
        /// </summary>
        /// <value>The name of the setting.</value>
        public string Name
        {
            get
            {
                return this.configurationName;
            }
        }

        /// <summary>
        /// Gets the argument list for the setting.
        /// </summary>
        /// <value>The argument list.</value>
        public IList<CtsConfigurationArgument> Arguments
        {
            get
            {
                return this.arguments;
            }
        }

        /// <summary>
        /// Adds the specified argument to the configuration setting.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <param name="value">The argument value.</param>
        internal void AddArgument(string name, string value)
        {
            this.arguments.Add(new CtsConfigurationArgument(name, value));
        }
    }
}