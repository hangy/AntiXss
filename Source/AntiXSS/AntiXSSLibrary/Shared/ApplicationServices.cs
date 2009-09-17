// ***************************************************************
// <copyright file="ApplicationServices.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Internal
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Reflection;
    using System.Configuration;

    internal interface IApplicationServices
    {
        Stream CreateTemporaryStorage();

        IList<CtsConfigurationSetting> GetConfiguration(string subSectionName);
        void RefreshConfiguration();

        void LogConfigurationErrorEvent();

        
        
        
        
    }

    internal class DefaultApplicationServices : IApplicationServices
    {
        internal static IList<CtsConfigurationSetting> emptySubSection = new List<CtsConfigurationSetting>();

        private object lockObject = new object();
        internal volatile Dictionary<string, IList<CtsConfigurationSetting>> configurationSubSections;

        public Stream CreateTemporaryStorage()
        {
            return DefaultApplicationServices.CreateTemporaryStorage(null, null);
        }

        public static Stream CreateTemporaryStorage(Func<int, byte[]> acquireBuffer, Action<byte[]> releaseBuffer)
        {
            TemporaryDataStorage storage = new TemporaryDataStorage(acquireBuffer, releaseBuffer);

            Stream stream = storage.OpenWriteStream(false);

            
            
            
            storage.Release();
            
            return stream;
        }

        public IList<CtsConfigurationSetting> GetConfiguration(string subSectionName)
        {
            IList<CtsConfigurationSetting> subSection;

            if (this.configurationSubSections == null)
            {
                lock (lockObject)
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
                                this.configurationSubSections = new Dictionary<string, IList<CtsConfigurationSetting>>();
                                this.configurationSubSections.Add(string.Empty, new List<CtsConfigurationSetting>());
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

                            
                            this.configurationSubSections = new Dictionary<string, IList<CtsConfigurationSetting>>();
                            this.configurationSubSections.Add(string.Empty, new List<CtsConfigurationSetting>());
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
                subSection = emptySubSection;
            }

            return subSection;
        }

        public void RefreshConfiguration()
        {
            
            ConfigurationManager.RefreshSection("appSettings");

            
            this.configurationSubSections = null;
        }

        public void LogConfigurationErrorEvent()
        {
            
        }
    }

    internal static class ApplicationServices
    {
        private static IApplicationServices provider = ApplicationServices.LoadServices();

        public static IApplicationServices Provider { get { return ApplicationServices.provider; } }

        private static IApplicationServices LoadServices()
        {
            return new DefaultApplicationServices();
        }

        public static CtsConfigurationSetting GetSimpleConfigurationSetting(string subSectionName, string settingName)
        {
            CtsConfigurationSetting result = null;
            IList<CtsConfigurationSetting> settings = Microsoft.Exchange.Data.Internal.ApplicationServices.Provider.GetConfiguration(subSectionName);

            foreach (CtsConfigurationSetting setting in settings)
            {
                if (string.Equals(setting.Name, settingName, StringComparison.OrdinalIgnoreCase))
                {
                    if (result != null)
                    {
                        
                        ApplicationServices.Provider.LogConfigurationErrorEvent();
                        break;
                    }

                    result = setting;
                }
            }

            return result;
        }

        internal static int ParseIntegerSetting(CtsConfigurationSetting setting, int defaultValue, int min, bool kilobytes)
        {
            if (setting.Arguments.Count != 1 || !setting.Arguments[0].Name.Equals("Value", StringComparison.OrdinalIgnoreCase))
            {
                
                
                ApplicationServices.Provider.LogConfigurationErrorEvent();

                return defaultValue;
            }

            if (setting.Arguments[0].Value.Trim().Equals("unlimited", StringComparison.OrdinalIgnoreCase))
            {
                return int.MaxValue;
            }

            int value;

            if (!int.TryParse(setting.Arguments[0].Value.Trim(), out value))
            {
                
                
                ApplicationServices.Provider.LogConfigurationErrorEvent();

                return defaultValue;
            }

            if (value < min)
            {
                
                
                ApplicationServices.Provider.LogConfigurationErrorEvent();

                return defaultValue;
            }

            if (kilobytes)
            {
                if (value > int.MaxValue / 1024)
                {
                    
                    
                    value = int.MaxValue;
                }
                else
                {
                    
                    value = value * 1024;
                }
            }

            return value;
        }
    }
}
