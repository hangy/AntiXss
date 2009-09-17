// ***************************************************************
// <copyright file="Configuration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Internal
{
    using System;
    using System.Diagnostics;
    using System.Configuration;
    using System.Collections.Generic;
    using System.Xml;

    
    internal class CtsConfigurationSetting
    {
        private string name;
        private IList<CtsConfigurationArgument> arguments;

        internal CtsConfigurationSetting(string name)
        {
            this.name = name;
            this.arguments = new List<CtsConfigurationArgument>();
        }

        internal void AddArgument(string name, string value)
        {
            this.arguments.Add(new CtsConfigurationArgument(name, value));
        }

        public string Name { get { return this.name; } }

        public IList<CtsConfigurationArgument> Arguments { get { return this.arguments; } }
    }

    
    internal class CtsConfigurationArgument
    {
        private string name;
        private string value;

        internal CtsConfigurationArgument(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name { get { return this.name; } }
        public string Value { get { return this.value; } }
    }


    
    internal sealed class CtsConfigurationSection : ConfigurationSection
    {
        private static ConfigurationPropertyCollection properties;

        private Dictionary<string, IList<CtsConfigurationSetting>> subSections = new Dictionary<string, IList<CtsConfigurationSetting>>();

        public Dictionary<string, IList<CtsConfigurationSetting>> SubSectionsDictionary
        {
            get { return this.subSections; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (properties == null)
                {
                    properties = new ConfigurationPropertyCollection();
                }

                return properties;
            }
        }

        protected override void DeserializeSection(System.Xml.XmlReader reader)
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
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.IsEmptyElement)
                        {
                            

                            CtsConfigurationSetting setting = DeserializeSetting(reader);

                            unnamedSubSection.Add(setting);
                        }
                        else
                        {
                            string subSectionName = reader.Name;

                            IList<CtsConfigurationSetting> subSection;

                            if (!this.subSections.TryGetValue(subSectionName, out subSection))
                            {
                                subSection = new List<CtsConfigurationSetting>();

                                this.subSections.Add(subSectionName, subSection);
                            }

                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    if (reader.IsEmptyElement)
                                    {
                                        CtsConfigurationSetting setting = DeserializeSetting(reader);

                                        subSection.Add(setting);
                                    }
                                    else
                                    {
                                        throw new ConfigurationErrorsException("error", reader);
                                    }
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    break;
                                }
                                else if ((reader.NodeType == XmlNodeType.CDATA) || (reader.NodeType == XmlNodeType.Text))
                                {
                                    throw new ConfigurationErrorsException("error", reader);
                                }
                            }
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    else if ((reader.NodeType == XmlNodeType.CDATA) || (reader.NodeType == XmlNodeType.Text))
                    {
                        throw new ConfigurationErrorsException("error", reader);
                    }
                }
            }
        }

        private CtsConfigurationSetting DeserializeSetting(System.Xml.XmlReader reader)
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
