// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AntiXssInspectorSettings.cs" company="Microsoft Corporation">
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
//   Settings for the AntiXSS page inspector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System.Configuration;
    using System.Drawing;

    /// <summary>
    /// Settings for the AntiXSS page inspector.
    /// </summary>
    internal sealed class AntiXssInspectorSettings : BasePlugInConfiguration
    {
        /// <summary>
        /// The property name for the plug-in directory setting.
        /// </summary>
        private const string DoubleEncodingFilterAttributeName = "filterDoubleEncoding";

        /// <summary>
        /// The property name for the encode derived controls setting.
        /// </summary>
        private const string EncodeDerivedControlsAttributeName = "encodeDerivedControls";

        /// <summary>
        /// The property name for the mark output setting.
        /// </summary>
        private const string MarkOutputAttributeName = "markAntiXssOutput";

        /// <summary>
        /// The property name for the marked colour setting.
        /// </summary>
        private const string MarkedOutputColourAttributeName = "markAntiXssOutputColor";

        /// <summary>
        /// The property name for the encoding types collection.
        /// </summary>
        private const string EncodingTypesCollectionName = "encodingTypes";        

        /// <summary>
        /// Gets a value indicating whether a page is protected from any double encoding issues. 
        /// </summary>
        /// <remarks>
        /// Double encoding usually results in a control encoded 
        /// in the code and also encoded in the module.
        /// </remarks>
        [ConfigurationProperty(DoubleEncodingFilterAttributeName, IsRequired = false, DefaultValue = true)]
        public bool DoubleEncodingFilter
        {
            get
            {
                return (bool)this[DoubleEncodingFilterAttributeName];
            }
        }

        /// <summary>
        /// Gets a value indicating whether derived controls are encoded.
        /// </summary>
        /// <remarks>
        /// Only direct subclasses will be encoded.
        /// </remarks>
        [ConfigurationProperty(EncodeDerivedControlsAttributeName, IsRequired = false, DefaultValue = true)]
        public bool EncodeDerivedControls
        {
            get
            {
                return (bool)this[EncodeDerivedControlsAttributeName];
            }
        }

        /// <summary>
        /// Gets a value indicating whether control values which are encoded by the WPL are highlighted 
        /// to allow for visual detection.
        /// </summary>
        [ConfigurationProperty(MarkOutputAttributeName, IsRequired = false, DefaultValue = false)]
        public bool MarkAntiXssOutput
        {
            get
            {
                return (bool)this[MarkOutputAttributeName];
            }
        }

        /// <summary>
        /// Gets the color used to mark the output.
        /// </summary>
        [ConfigurationProperty(MarkedOutputColourAttributeName, IsRequired = false)]
        public Color MarkAntiXssOutputColor
        {
            get
            {
                return (Color)this[MarkedOutputColourAttributeName];
            }
        }

        /// <summary>
        /// Gets the list of controls types that need to be encoded. 
        /// </summary>
        [ConfigurationProperty(EncodingTypesCollectionName, IsRequired = true)]
        public ControlEncodingContextCollection EncodingTypes
        {
            get
            {
                return this[EncodingTypesCollectionName] as ControlEncodingContextCollection;
            }
        }
    }
}
