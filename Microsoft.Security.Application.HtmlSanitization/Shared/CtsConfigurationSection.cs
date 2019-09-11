// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CtsConfigurationSection.cs" company="Microsoft Corporation">
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
//   Provides access to the configuration section.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Internal
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Xml;
    
    /// <summary>
    /// Provides access to the configuration section.
    /// </summary>
    internal sealed class CtsConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// The subsections in the configuration file.
        /// </summary>
        private readonly Dictionary<string, IList<CtsConfigurationSetting>> subSections = new Dictionary<string, IList<CtsConfigurationSetting>>();

        /// <summary>
        /// The configuration properties.
        /// </summary>
        private static ConfigurationPropertyCollection properties;

        /// <summary>
        /// Gets a dictionary for the subsections in the configuration file.
        /// </summary>
        public Dictionary<string, IList<CtsConfigurationSetting>> SubSectionsDictionary
        {
            get
            {
                return this.subSections;
            }
        }

        /// <summary>
        /// Gets a collection of all configuration properties.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties ?? (properties = new ConfigurationPropertyCollection());
            }
        }

        /// <summary>
        /// Deserailizes a configuration section.
        /// </summary>
        /// <param name="reader">An XmlReader containing the section to deserialize.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Input reader is always from app config.")]
        protected override void DeserializeSection(XmlReader reader)
        {
            IList<CtsConfigurationSetting> unnamedSubSection = new List<CtsConfigurationSetting>();

            this.subSections.Add(string.Empty, unnamedSubSection);
            
            if (!reader.Read() || reader.NodeType != XmlNodeType.Element)
            {
                throw new ConfigurationErrorsException("error", reader);
            }

            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.IsEmptyElement)
                            {
                                CtsConfigurationSetting setting = DeserializeSetting(reader);
                                unnamedSubSection.Add(setting);
                            }
                            else
                            {
                                string subSectionName = reader.Name;

                                if (!this.subSections.TryGetValue(subSectionName, out IList<CtsConfigurationSetting> subSection))
                                {
                                    subSection = new List<CtsConfigurationSetting>();
                                    this.subSections.Add(subSectionName, subSection);
                                }

                                while (reader.Read())
                                {
                                    switch (reader.NodeType)
                                    {
                                        case XmlNodeType.Element:
                                            if (reader.IsEmptyElement)
                                            {
                                                CtsConfigurationSetting setting = DeserializeSetting(reader);
                                                subSection.Add(setting);
                                            }
                                            else
                                            {
                                                throw new ConfigurationErrorsException("error", reader);
                                            }

                                            break;
                                        case XmlNodeType.EndElement:
                                            break;
                                        case XmlNodeType.Text:
                                        case XmlNodeType.CDATA:
                                            throw new ConfigurationErrorsException("error", reader);
                                    }
                                }
                            }

                            break;
                        case XmlNodeType.EndElement:
                            break;
                        case XmlNodeType.Text:
                        case XmlNodeType.CDATA:
                            throw new ConfigurationErrorsException("error", reader);
                    }
                }
            }
        }

        /// <summary>
        /// Deserializes an individual configuration setting.
        /// </summary>
        /// <param name="reader">The XmlReader containing the setting to deserialize.</param>
        /// <returns>A <see cref="CtsConfigurationSetting"/> instance containing the configuration setting.</returns>
        private static CtsConfigurationSetting DeserializeSetting(XmlReader reader)
        {
            string settingName = reader.Name;

            CtsConfigurationSetting setting = new CtsConfigurationSetting(settingName);

            if (reader.AttributeCount > 0)
            {
                while (reader.MoveToNextAttribute())
                {
                    string argumentName = reader.Name;
                    string argumentValue = reader.Value;

                    setting.AddArgument(argumentName, argumentValue);
                }
            }

            return setting;
        }
    }
}
