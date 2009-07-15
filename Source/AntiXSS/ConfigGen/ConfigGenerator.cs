using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Drawing;

namespace Microsoft.Security.Application.SecurityRuntimeEngine.ConfigurationGenerator
{

    /// <summary>
    /// This internal class is used to generate SRE module config. 
    /// </summary>
    internal class ConfigGenerator
    {
        
        List<ControlEncodingType> lstEncodings = new List<ControlEncodingType>();

        public ConfigGenerator() { this.LoadConfig(); }

        public void GenerateConfig(List<string> controls, ConfigOptions configOptions, string configFilename) 
        {
            try
            {
                List<ControlEncodingType> lstControlEncodings = new List<ControlEncodingType>();
                foreach (string s in controls) 
                {
                    if (this.IsControlInConfig(s)) 
                    {
                        lstControlEncodings.AddRange(this.GetControlEncodings(s));                        
                    }
                }

                if (lstControlEncodings.Count > 0) 
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "no"));
                    XmlElement configNode = xmlDoc.CreateElement("Configuration");
                    XmlElement controlEncodingContextsNode = xmlDoc.CreateElement("ControlEncodingContexts");
                    foreach (ControlEncodingType cet in lstControlEncodings) 
                    {
                        XmlElement controlEncodingContextNode = xmlDoc.CreateElement("ControlEncodingContext");
                        XmlAttribute fullClassName = xmlDoc.CreateAttribute("FullClassName");
                        fullClassName.Value = cet.controlFullName;
                        XmlAttribute propertyName = xmlDoc.CreateAttribute("PropertyName");
                        propertyName.Value = cet.propertyName;
                        XmlAttribute encodingType = xmlDoc.CreateAttribute("EncodingContext");
                        encodingType.Value = cet.encodingType;
                        controlEncodingContextNode.Attributes.Append(fullClassName);
                        controlEncodingContextNode.Attributes.Append(propertyName);
                        controlEncodingContextNode.Attributes.Append(encodingType);
                        controlEncodingContextsNode.AppendChild(controlEncodingContextNode);
                    }
                    configNode.AppendChild(controlEncodingContextsNode);
                    
                    XmlElement optionsNode = xmlDoc.CreateElement("DoubleEncodingFilter");
                    XmlAttribute enabledAttribute = xmlDoc.CreateAttribute("Enabled");
                    enabledAttribute.Value = configOptions.doubleEncodingFilter.ToString();
                    optionsNode.Attributes.Append(enabledAttribute);
                    configNode.AppendChild(optionsNode);

                    optionsNode = xmlDoc.CreateElement("EncodeDerivedControls");
                    enabledAttribute = xmlDoc.CreateAttribute("Enabled");
                    enabledAttribute.Value = configOptions.encodeDerivedControls.ToString();
                    optionsNode.Attributes.Append(enabledAttribute);
                    configNode.AppendChild(optionsNode);

                    optionsNode = xmlDoc.CreateElement("MarkAntiXssOutput");
                    enabledAttribute = xmlDoc.CreateAttribute("Enabled");
                    enabledAttribute.Value = configOptions.markAntiXssOutput.ToString();
                    optionsNode.Attributes.Append(enabledAttribute);

                    enabledAttribute = xmlDoc.CreateAttribute("Color");
                    enabledAttribute.Value = configOptions.markAntiXssOutputColor.Name;
                    optionsNode.Attributes.Append(enabledAttribute);

                    configNode.AppendChild(optionsNode);

                    xmlDoc.AppendChild(configNode);
                    xmlDoc.Save(configFilename);
                }

            }
            catch 
            {
                throw; 
            }
            
        }
        
        private void LoadConfig() 
        {
            XPathDocument xDoc = new XPathDocument(AppDomain.CurrentDomain.BaseDirectory + "EncodingControls.xml");
            XPathNodeIterator xNodes = xDoc.CreateNavigator().Select("/Configuration/ControlEncodingContexts/ControlEncodingContext");
            while (xNodes.MoveNext()) 
            {
                
                lstEncodings.Add(new ControlEncodingType(xNodes.Current.GetAttribute("FullClassName", ""), xNodes.Current.GetAttribute("PropertyName", ""), xNodes.Current.GetAttribute("EncodingContext", "")));

            }
        }

        private bool IsControlInConfig(string controlType)
        {
            foreach (ControlEncodingType cet in this.lstEncodings) 
            {
                if (cet.controlFullName == controlType)
                    return true;
            }
            return false;
        }

        private List<ControlEncodingType> GetControlEncodings(string controlType) 
        {
            List<ControlEncodingType> returnControls = new List<ControlEncodingType>();
            foreach (ControlEncodingType cet in this.lstEncodings)
            {
                if (cet.controlFullName == controlType)
                    returnControls.Add(cet);
            }
            return returnControls;
        }

    }

    internal struct ControlEncodingType 
    {
        public string controlFullName;
        public string propertyName;
        public string encodingType;
        public ControlEncodingType(string s1, string s2, string s3) 
        {
            controlFullName = s1;
            propertyName = s2;
            encodingType = s3;
        }        
    }

    internal struct ConfigOptions 
    {
        public bool doubleEncodingFilter;
        public bool encodeDerivedControls;
        public bool markAntiXssOutput;
        public Color markAntiXssOutputColor;
    }
}
