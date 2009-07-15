using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Security.Application.SecurityRuntimeEngine.Configuration
{
    /// <summary>
    /// Class which represents a single encoding configuration. 
    /// Specifies the control type, property and type of encoding.
    /// </summary>
    internal sealed class ControlEncodingContext
    {
        string strFullClassName = "";

        /// <summary>
        /// Full type name of the control including the namespace.
        /// </summary>
        [XmlAttribute()]
        public string FullClassName
        {
            get { return strFullClassName; }
            set { strFullClassName = value; }
        }
        string strPropertyName = "";

        /// <summary>
        /// Property name of the control which needs to be encoded.
        /// </summary>
        [XmlAttribute()]
        public string PropertyName
        {
            get { return strPropertyName; }
            set { strPropertyName = value; }
        }
        EncodingContexts encodingContext = EncodingContexts.Html;

        /// <summary>
        /// Encoding type for this type of control and property.
        /// </summary>
        [XmlAttribute()]
        public EncodingContexts EncodingContext
        {
            get { return encodingContext; }
            set { encodingContext = value; }
        }

        /// <summary>
        /// Read only property which returns a unique ID which 
        /// is a concatenation of class name and property name.
        /// </summary>
        [XmlIgnore()]
        public string ID
        {
            get { return strFullClassName + "." + strPropertyName; }
        }

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        public ControlEncodingContext() { }

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="fullName">Full type name of the control.</param>
        /// <param name="propertyName">Property name of the control.</param>
        /// <param name="context">Type of encoding for the property.</param>
        public ControlEncodingContext(string fullName, string propertyName, EncodingContexts context)
        {
            this.strFullClassName = fullName;
            this.strPropertyName = propertyName;
            this.encodingContext = context;
        }

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="fullName">Full type name of the control.</param>
        /// <param name="propertyName">Property name of the control.</param>
        /// <param name="encContext">Type of encoding for the property.</param>
        public ControlEncodingContext(string fullName, string propertyName, string encContext)
        {
            this.strFullClassName = fullName;
            this.strPropertyName = propertyName;
            if (Enum.IsDefined(typeof(EncodingContexts),encContext))
                this.encodingContext=(EncodingContexts)Enum.Parse(typeof(EncodingContexts),encContext);
        }

    }

    /// <summary>
    /// Defines types of encoding supported
    /// </summary>
    public enum EncodingContexts:int
    {
        /// <summary>
        /// Identifies Html encoding
        /// </summary>
        [XmlEnum(Name = "Html")]
        Html = 0,
        /// <summary>
        /// Identifies Html Attribute encoding.
        /// </summary>
        [XmlEnum(Name = "HtmlAttribute")]
        HtmlAttribute = 1,
        /// <summary>
        /// Identifies Url encoding.
        /// </summary>
        [XmlEnum(Name = "Url")]
        Url = 2,
        /// <summary>
        /// Identifies Xml encoding.
        /// </summary>
        [XmlEnum(Name = "Xml")]
        Xml = 3,
        /// <summary>
        /// Identifies Xml Attribute encoding.
        /// </summary>
        [XmlEnum(Name = "XmlAttribute")]
        XmlAttribute = 4,
        /// <summary>
        /// Identifies safe Html parsing.
        /// </summary>
        [XmlEnum(Name="SafeHtml")]
        SafeHtml = 5
    }
}
