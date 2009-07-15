using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Globalization;

namespace Microsoft.Security.Application.SecurityRuntimeEngine.ConfigurationGenerator
{
    internal static class LocalizationUtil
    {
        private static ResourceManager resman=null;
        static LocalizationUtil() 
        {
            try
            {
                resman = Microsoft.Security.Application.SecurityRuntimeEngine.ConfigurationGenerator.Properties.Resources.ResourceManager;//new ResourceManager("Microsoft.Security.Application.SecurityRuntimeEngine.ConfigurationGenerator.Properties.Resources", typeof(Properties.Resources).Assembly);
            }
            catch { }
        }
        public static string GetString(string locName) 
        {
            if (resman != null)
                return resman.GetString(locName);
            else
                return string.Empty;
        }

        public static string GetHelpString(string locName)
        {
            return LocalizationUtil.GetString(locName + "_Help");
        }

    }
}
