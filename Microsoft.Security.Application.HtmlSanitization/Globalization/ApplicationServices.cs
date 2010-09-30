// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationServices.cs" company="Microsoft">
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
//   Provides functions for parsing application configuration data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides functions for parsing application configuration data.
    /// </summary>
    internal static class ApplicationServices
    {
        /// <summary>
        /// Loads the application service provider.
        /// </summary>
        private static readonly IApplicationServices ServicesProvider = LoadServices();

        /// <summary>
        /// Gets the application service provider.
        /// </summary>
        public static IApplicationServices Provider
        {
            get
            {
                return ServicesProvider;
            }
        }

        /// <summary>
        /// Gets the specified configuration setting.
        /// </summary>
        /// <param name="subSectionName">Name of the configuration sub section.</param>
        /// <param name="settingName">Name of the configuration setting.</param>
        /// <returns>A <see cref="CtsConfigurationSetting"/> for the sepecified setting from the specified sub section.</returns>
        public static CtsConfigurationSetting GetSimpleConfigurationSetting(string subSectionName, string settingName)
        {
            CtsConfigurationSetting result = null;
            IList<CtsConfigurationSetting> settings = Provider.GetConfiguration(subSectionName);

            foreach (CtsConfigurationSetting setting in
                settings.Where(setting => string.Equals(setting.Name, settingName, StringComparison.OrdinalIgnoreCase)))
            {
                if (result != null)
                {                        
                    Provider.LogConfigurationErrorEvent();
                    break;
                }

                result = setting;
            }

            return result;
        }

        /// <summary>
        /// Initializes the application services.
        /// </summary>
        /// <returns>An instance of the default Application Services class.</returns>
        private static IApplicationServices LoadServices()
        {
            return new DefaultApplicationServices();
        }
    }
}
