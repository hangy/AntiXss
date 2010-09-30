// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlEncodingContext.cs" company="Microsoft Corporation">
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
//
// </copyright>
// <summary>
//   Class which represents a single control encoding configuration, containing the control type, property 
//   and type of encoding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System.Configuration;

    /// <summary>
    /// Defines types of encoding supported
    /// </summary>
    internal enum EncodingContext
    {
        /// <summary>
        /// Identifies Html encoding
        /// </summary>
        Html = 0,

        /// <summary>
        /// Identifies Html Attribute encoding.
        /// </summary>
        HtmlAttribute = 1,

        /// <summary>
        /// Identifies URL encoding.
        /// </summary>
        Url = 2,

        /// <summary>
        /// Identifies Xml encoding.
        /// </summary>
        Xml = 3,

        /// <summary>
        /// Identifies Xml Attribute encoding.
        /// </summary>
        XmlAttribute = 4,

        /// <summary>
        /// Identifies safe Html parsing.
        /// </summary>
        SafeHtml = 5
    }

    /// <summary>
    /// Class which represents a single encoding configuration. 
    /// </summary>
    internal sealed class ControlEncodingContext : ConfigurationElement
    {
        /// <summary>
        /// The configuration attribute name for the automatically generated identifier.
        /// </summary>
        private const string IdConfigurationAttribute = "id";

        /// <summary>
        /// The configuration attribute name for the class name setting.
        /// </summary>
        private const string ClassNameConfigurationAttribute = "fullClassName";

        /// <summary>
        /// The configuration attribute name for the property name setting.
        /// </summary>
        private const string PropertyNameConfigurationAttribute = "property";

        /// <summary>
        /// The configuration attribute name for the encoding context setting.
        /// </summary>
        private const string EncodingContextConfigurationAttribute = "encodingContext";

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlEncodingContext"/> class.
        /// </summary>
        public ControlEncodingContext()
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlEncodingContext"/> class.
        /// </summary>
        /// <param name="fullClassName">Full name of the class to be encoded.</param>
        /// <param name="propertyName">Name of the property to be encoded.</param>
        /// <param name="encodingContext">The encoding context.</param>
        public ControlEncodingContext(string fullClassName, string propertyName, EncodingContext encodingContext) :
            this(null, fullClassName, propertyName, encodingContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlEncodingContext"/> class.
        /// </summary>
        /// <param name="id">The identifier for this context entry.</param>
        /// <param name="fullClassName">Full name of the class to be encoded.</param>
        /// <param name="propertyName">Name of the property to be encoded.</param>
        /// <param name="encodingContext">The encoding context.</param>
        public ControlEncodingContext(string id, string fullClassName, string propertyName, EncodingContext encodingContext)
        {
            if (string.IsNullOrEmpty(id))
            {
                this.Id = fullClassName + "." + propertyName;
            }
            else
            {
                this.Id = id;
            }

            this.FullClassName = fullClassName;
            this.PropertyName = propertyName;
            this.EncodingContext = encodingContext;
        }

        /// <summary>
        /// Gets or sets an identifier for the control context.
        /// </summary>
        [ConfigurationProperty(IdConfigurationAttribute, IsRequired = false, DefaultValue = null, IsKey = true)]
        public string Id
        {
            get
            {
                if (this[IdConfigurationAttribute] == null)
                {
                    this.Id = this.FullClassName + "." + this.PropertyName;
                }

                return this[IdConfigurationAttribute] as string;
            }
            
            set
            {
                this[IdConfigurationAttribute] = value;
            }
        }

        /// <summary>
        /// Gets or sets the full type name of the control including the namespace.
        /// </summary>
        [ConfigurationProperty(ClassNameConfigurationAttribute, IsRequired = true)]
        public string FullClassName
        {
            get
            {
                return this[ClassNameConfigurationAttribute] as string;
            }

            set
            {
                this[ClassNameConfigurationAttribute] = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the property name of the control which needs to be encoded.
        /// </summary>
        [ConfigurationProperty(PropertyNameConfigurationAttribute, IsRequired = true)]
        public string PropertyName
        {
            get
            {
                return this[PropertyNameConfigurationAttribute] as string;
            }

            set
            {
                this[PropertyNameConfigurationAttribute] = value;
            }
        }

        /// <summary>
        /// Gets or sets the encoding type for this type of control and property.
        /// </summary>
        [ConfigurationProperty(EncodingContextConfigurationAttribute, IsRequired = true)]
        public EncodingContext EncodingContext
        {
            get
            {
                return (EncodingContext)this[EncodingContextConfigurationAttribute];
            }

            set
            {
                this[EncodingContextConfigurationAttribute] = value;
            }
        }
    }
}
