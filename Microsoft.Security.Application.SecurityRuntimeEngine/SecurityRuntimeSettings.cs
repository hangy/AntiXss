// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityRuntimeSettings.cs" company="Microsoft Corporation">
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
//   The settings for the SRE itself.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System.Configuration;

    /// <summary>
    /// The settings for the SRE itself.
    /// </summary>
    internal sealed class SecurityRuntimeSettings : BaseSecurityRuntimeConfiguration
    {
        /// <summary>
        /// The section name for the setting section in the configuration file.
        /// </summary>
        private const string ConfigurationSection = "sreSettings";

        /// <summary>
        /// The property name for the plug-in directory configuration attribute.
        /// </summary>
        private const string PlugInDirectoryProperty = "plugInDirectory";

        /// <summary>
        /// The property name for the suspect results count configuration attribute.
        /// </summary>
        private const string SuspectResultsProperty = "allowedSuspectResults";

        /// <summary>
        /// The property name for the disabled plug-in configuration collection.
        /// </summary>
        private const string DisabledPlugInsProperty = "disabledPlugIns";

        /// <summary>
        /// The property name for the reset suspect count configuration attribute.
        /// </summary>
        private const string ResetSuspectCountProperty = "resetSuspectCountBetweenStages";

        /// <summary>
        /// The property name for the log level configuration attribute.
        /// </summary>
        private const string LogLevelProperty = "logLevel";

        /// <summary>
        /// An instance of the internal settings.
        /// </summary>
        private static SecurityRuntimeSettings internalSettings =
            ConfigurationManager.GetSection(ConfigurationSection) as SecurityRuntimeSettings;

        /// <summary>
        /// Gets the settings for the Security Runtime Engine.
        /// </summary>
        /// <value>The settings for the Security Runtime Engine.</value>
        public static SecurityRuntimeSettings Settings
        {
            get
            {
                return internalSettings ?? (internalSettings = new SecurityRuntimeSettings());
            }
        }

        /// <summary>
        /// Gets the directory from which to load plug-ins.
        /// </summary>
        /// <value>
        /// The directory from which to load plug-ins.
        /// </value>
        [ConfigurationProperty(PlugInDirectoryProperty, IsRequired = false)]
        public string PlugInDirectory
        {
            get
            {
                return (string)this[PlugInDirectoryProperty];
            }
        }

        /// <summary>
        /// Gets the number suspect results allowed before an exception is thrown.
        /// </summary>
        /// <value>The number of suspect results allowed before an exception is thrown.</value>
        [ConfigurationProperty(SuspectResultsProperty, DefaultValue = -1, IsRequired = false)]
        [IntegerValidator(MinValue = -1, MaxValue = int.MaxValue, ExcludeRange = false)]
        public int AllowedSuspectResults
        {
            get
            {
                return (int)this[SuspectResultsProperty];
            }
        }

        /// <summary>
        /// Gets the disabled plug-ins collection.
        /// </summary>
        /// <value>The disabled plug-ins.</value>
        [ConfigurationProperty(DisabledPlugInsProperty, IsRequired = false)]
        public DisabledPlugInCollection DisabledPlugIns
        {
            get
            {
                return this[DisabledPlugInsProperty] as DisabledPlugInCollection;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the suspect inspection count should be reset at each stage of the ASP.NET pipeline.
        /// </summary>
        /// <value><c>true</c> if the count should be reset, otherwise <c>false</c>.</value>
        [ConfigurationProperty(ResetSuspectCountProperty, IsRequired = false, DefaultValue = false)]
        public bool ResetSuspectCountBetweenStages
        {
            get
            {
                return (bool)this[ResetSuspectCountProperty];
            }
        }

        /// <summary>
        /// Gets a value indicating the log threshold where messages at this threshold or higher are logged.
        /// </summary>
        /// <value>
        /// The logging threshold.
        /// </value>
        [ConfigurationProperty(LogLevelProperty, IsRequired = false, DefaultValue = LogLevel.Error)]
        public LogLevel LogLevel
        {
            get
            {
                return (LogLevel)this[LogLevelProperty];
            }
        }

        /// <summary>
        /// Gets the excluded paths for the plug-in.
        /// </summary>
        /// <value>The excluded paths for the plug-in.</value>
        [ConfigurationProperty(DefaultExcludedPathsProperty, IsDefaultCollection = true, IsRequired = false)]
        public override ExcludedPathCollection ExcludedPaths
        {
            get
            {
                return this[DefaultExcludedPathsProperty] as ExcludedPathCollection;
            }
        }
    }
}
