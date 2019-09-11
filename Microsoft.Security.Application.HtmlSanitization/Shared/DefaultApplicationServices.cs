// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultApplicationServices.cs" company="Microsoft Corporation">
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
//   Wrapper for CTS application settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;

    /// <summary>
    /// Wrapper for CTS application settings.
    /// </summary>
    internal class DefaultApplicationServices : IApplicationServices
    {
        /// <summary>
        /// A blank sub section.
        /// </summary>
        private static readonly IList<CtsConfigurationSetting> EmptySubSection = new List<CtsConfigurationSetting>();

        /// <summary>
        /// The lock used for thread safe syncronization.
        /// </summary>
        private readonly object lockObject = new object();

        /// <summary>
        /// The configuration sub sections from the CTS application settings.
        /// </summary>
        private volatile Dictionary<string, IList<CtsConfigurationSetting>> configurationSubSections;

        // Orphaned WPL code.
#if false
        /// <summary>
        /// Creates the temporary storage stream.
        /// </summary>
        /// <param name="acquireBuffer">The acquire buffer.</param>
        /// <param name="releaseBuffer">The release buffer.</param>
        /// <returns>A temporary storage stream.</returns>
        public static Stream CreateTemporaryStorage(Func<int, byte[]> acquireBuffer, Action<byte[]> releaseBuffer)
        {
            TemporaryDataStorage storage = new TemporaryDataStorage(acquireBuffer, releaseBuffer);

            Stream stream = storage.OpenWriteStream(false);

            storage.Release();

            return stream;
        }

        /// <summary>
        /// Creates the temporary storage stream.
        /// </summary>
        /// <returns>
        /// A <see cref="Stream"/> for temporary storage.
        /// </returns>
        public Stream CreateTemporaryStorage()
        {
            return CreateTemporaryStorage(null, null);
        }
#endif

        /// <summary>
        /// Gets the configuration subsection specified.
        /// </summary>
        /// <param name="subSectionName">Name of the subsection.</param>
        /// <returns>
        /// A list of <see cref="CtsConfigurationSetting"/>s for the specified section.
        /// </returns>
        public IList<CtsConfigurationSetting> GetConfiguration(string subSectionName)
        {
            IList<CtsConfigurationSetting> subSection;

            if (this.configurationSubSections == null)
            {
                lock (this.lockObject)
                {
                    if (this.configurationSubSections == null)
                    {
                        try
                        {
                            CtsConfigurationSection section = ConfigurationManager.GetSection("CTS") as CtsConfigurationSection;

                            if (section != null)
                            {
                                this.configurationSubSections = section.SubSectionsDictionary;
                            }
                            else
                            {
                                this.configurationSubSections = new Dictionary<string, IList<CtsConfigurationSetting>>
                                                                    {
                                                                        { string.Empty, new List<CtsConfigurationSetting>() }
                                                                    };
                            }

                            string path = ConfigurationManager.AppSettings["TemporaryStoragePath"];

                            if (!string.IsNullOrEmpty(path))
                            {
                                CtsConfigurationSetting newSetting = new CtsConfigurationSetting("TemporaryStorage");
                                newSetting.AddArgument("Path", path);

                                subSection = this.configurationSubSections[string.Empty];

                                subSection.Add(newSetting);
                            }

                            ConfigurationManager.RefreshSection("CTS");
                        }
                        catch (ConfigurationErrorsException /*exception*/)
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();

                            this.configurationSubSections = new Dictionary<string, IList<CtsConfigurationSetting>>
                                                                {
                                                                    { string.Empty, new List<CtsConfigurationSetting>() }
                                                                };
                        }
                    }
                }
            }

            if (subSectionName == null)
            {
                subSectionName = string.Empty;
            }

            if (!this.configurationSubSections.TryGetValue(subSectionName, out subSection))
            {
                subSection = EmptySubSection;
            }

            return subSection;
        }

        /// <summary>
        /// Refreshes the configuration from the application configuration file.
        /// </summary>
        public void RefreshConfiguration()
        {
            ConfigurationManager.RefreshSection("appSettings");

            this.configurationSubSections = null;
        }

        /// <summary>
        /// Logs an error during configuration processing.
        /// </summary>
        public void LogConfigurationErrorEvent()
        {
        }
    }
}
