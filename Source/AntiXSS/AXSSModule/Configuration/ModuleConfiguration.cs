using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml;
using System.IO;
using System.Drawing;

namespace Microsoft.Security.Application.SecurityRuntimeEngine.Configuration
{

    /// <summary>
    /// Class representing the entire configuration file, including encoding types and options.
    /// Also contains helper methods to load configuration.
    /// </summary>
    internal sealed class ModuleConfiguration
    {
        #region XPathConstants
        //XPath Constants for reading the xml configuration file.
        const string ControlEncodingContextsXPath=@"/Configuration/ControlEncodingContexts/ControlEncodingContext";
        const string DoubleEncodingFilterXPath = @"/Configuration/DoubleEncodingFilter";
        const string PageSupressionsXPath = @"/Configuration/Suppressions/Exclude";
        const string SubclassedControlsXPath = @"/Configuration/EncodeDerivedControls";
        const string MarkAntiXssOutputXPath = @"/Configuration/MarkAntiXssOutput";
        #endregion

        bool blnDoubleEncodingFilter = false;
        bool blnEncodeSubclassedControls = false;
        bool blnMarkAntiXssOutput = false;
        Color clrMarkAntiXssOutput = Color.FromKnownColor(KnownColor.Yellow);
        ControlEncodingContexts encodingTypes = new ControlEncodingContexts();
        List<string> lstExclusions = new List<string>();

        /// <summary>
        /// Protects a page from any double encoding issues. 
        /// Double encoding usually results in a control encoded 
        /// in the code and also encoded in the module.
        /// </summary>
        public bool DoubleEncodingFilter
        {
            get { return blnDoubleEncodingFilter; }
            set { blnDoubleEncodingFilter = value; }
        }

        /// <summary>
        /// Encodes all controls derived from controls listed in 
        /// the configuration. Only direct subclasses will be encoded.
        /// </summary>
        public bool EncodeDerivedControls
        {
            get { return blnEncodeSubclassedControls; }
            set { blnEncodeSubclassedControls = value; }
        }

        /// <summary>
        /// Marks the antixss output with special tags to provide 
        /// test team the ability to visually distinguish the difference 
        /// between regular text and encoded text.
        /// </summary>
        public bool MarkAntiXssOutput
        {
            get { return blnMarkAntiXssOutput; }
            set { blnMarkAntiXssOutput = value; }
        }

        /// <summary>
        /// Color used to mark the output.
        /// </summary>
        public Color MarkAntiXssOutputColor
        {
            get { return clrMarkAntiXssOutput; }
            set { clrMarkAntiXssOutput = value; }
        }
        
        /// <summary>
        /// List of ControlEncodingContext which are the 
        /// types of controls that need to be encoded. 
        /// </summary>
        public ControlEncodingContexts EncodingTypes
        {
            get { return encodingTypes; }
            set { encodingTypes = value; }
        }

        /// <summary>
        /// Pages that will be excluded from encoding. All Controls 
        /// in the page will not be encoded. For control level exclusion 
        /// use SupressAntiXssEncoding attribute in the code.
        /// </summary>
        public List<string> Exclusions 
        {
            get { return lstExclusions; }
            set { lstExclusions = value; }
        }

        
        /// <summary>
        /// Loads the configuration from an XML file. 
        /// </summary>
        /// <param name="strConfigFile">Path where the xml config file is stored.</param>
        /// <returns>A Configuration object for the HTTP module.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when strConfigFile is empty or null.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when strConfigFile cannot be found in the specified path.</exception>
        /// <exception cref="System.Xml.XmlException">Thrown when the xml in the strConfigFile is not well formatted.</exception>
        public static ModuleConfiguration LoadConfiguration(string strConfigFile) 
        {
            if (string.IsNullOrEmpty(strConfigFile))
                throw new ArgumentNullException("strConfigFile");
            if (!File.Exists(strConfigFile))
                throw new FileNotFoundException("Config file not found");

            try
            {
                XPathDocument objXDoc = new XPathDocument(strConfigFile);
                XPathNavigator objNav = objXDoc.CreateNavigator();
                ModuleConfiguration objConfig = new ModuleConfiguration();
                XPathNodeIterator objNodes = objNav.Select(ControlEncodingContextsXPath);
                //Get EncodingTypes from configuration
                while (objNodes.MoveNext())
                {
                    ControlEncodingContext objEncType = new ControlEncodingContext(objNodes.Current.GetAttribute("FullClassName", ""), objNodes.Current.GetAttribute("PropertyName", ""), objNodes.Current.GetAttribute("EncodingContext", ""));
                    objConfig.EncodingTypes.Add(objEncType);
                }
                objNodes = null;

                XPathNavigator objNav1 = objNav.SelectSingleNode(DoubleEncodingFilterXPath);
                if (objNav1 != null)
                    bool.TryParse(objNav1.GetAttribute("Enabled", ""), out objConfig.blnDoubleEncodingFilter);

                objNav1 = objNav.SelectSingleNode(SubclassedControlsXPath);
                if (objNav1 != null)
                    bool.TryParse(objNav1.GetAttribute("Enabled", ""), out objConfig.blnEncodeSubclassedControls);

                objNav1 = objNav.SelectSingleNode(MarkAntiXssOutputXPath);
                if (objNav1 != null)
                {
                    bool.TryParse(objNav1.GetAttribute("Enabled", ""), out objConfig.blnMarkAntiXssOutput);
                    if (objNav1.GetAttribute("Color", "") != "")
                        objConfig.clrMarkAntiXssOutput = Color.FromName(objNav1.GetAttribute("Color", ""));
                    
                }



                objNodes = objNav.Select(PageSupressionsXPath);
                while (objNodes.MoveNext())
                {
                    objConfig.lstExclusions.Add(objNodes.Current.GetAttribute("Path", "").ToLower());
                }
                objNodes = null;

                objNav1 = null;
                objNav = null;
                objXDoc = null;
                return objConfig;
            }
            catch (XmlException xe)
            {
                throw xe;
            }
        }

        /// <summary>
        /// Method checks whether a page is excluded from encoding or not.
        /// </summary>
        /// <param name="pagePath">Vitual path of the page that needs to be checked.</param>
        /// <returns>Returns true if page is exlcuded otherwise returns false.</returns>
        public bool IsPageExcluded(string pagePath) 
        {
            if (lstExclusions.Contains(pagePath))
                return true;
            return false;
        }
    }

    
}
