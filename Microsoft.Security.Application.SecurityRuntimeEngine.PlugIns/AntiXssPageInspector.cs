// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AntiXssPageInspector.cs" company="Microsoft Corporation">
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
//   A preventative page inspector which ensures the correct encoding of Web Forms control properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Properties;

    /// <summary>
    /// A preventative page inspector which ensures the correct encoding of Web Forms control properties.
    /// </summary>
    [Export(typeof(IPageInspector))]
    internal class AntiXssPageInspector : IPageInspector, IConfigurablePlugIn
    {
        /// <summary>
        /// The section name for the setting section in the configuration file.
        /// </summary>
        private const string ConfigSectionName = "sreAntiXssSettings";

        /// <summary>
        /// Internal, strongly typed settings.
        /// </summary>
        private AntiXssInspectorSettings internalSettings = new AntiXssInspectorSettings();

        /// <summary>
        /// Gets the configuration section name for the plug-in.
        /// </summary>
        /// <value>The configuration section name for the plug-in.</value>
        public string ConfigurationSectionName
        {
            get
            {
                return ConfigSectionName;
            }
        }

        /// <summary>
        /// Gets the list of excluded paths for the plug-in.
        /// </summary>
        /// <value>The list of excluded paths for the plug-in.</value>
        public ExcludedPathCollection ExcludedPaths
        {
            get
            {
                return this.internalSettings == null ? null : this.internalSettings.ExcludedPaths;
            }
        }

        /// <summary>
        /// Gets or sets the settings for the plug-in.
        /// </summary>
        /// <value>The settings for the plug-in.</value>
        public BasePlugInConfiguration Settings
        {
            get
            {
                return this.internalSettings;
            }

            set
            {
                this.internalSettings = (AntiXssInspectorSettings)value;
            }
        }

        /// <summary>
        /// Inspects an HTTP page for potential problems.
        /// </summary>
        /// <param name="page">The <see cref="Page"/> to inspect.</param>
        /// <returns>
        /// An <see cref="IInspectionResult"/> containing the results of the inspection.
        /// </returns>
        public IInspectionResult Inspect(Page page)
        {
            // First encode properties on the page if configured.
            EncodeControl(page, this.internalSettings);
            
            // Now look for controls contained within the page and encode those.
            this.FindAndEncodeControls(page, this.internalSettings);

            // TODO: RETURN VALUE
            return new PageInspectionResult(InspectionResultSeverity.Continue);
        }

        /// <summary>
        /// Encodes a passed control based on the configuration.
        /// </summary>
        /// <param name="control">Control which needs to be encoded.</param>
        /// <param name="settings">The encoding settings and rules to use.</param>
        private static void EncodeControl(Control control, AntiXssInspectorSettings settings)
        {
            EncodeControl(control, control.GetType().ToString(), settings);
        }

        /// <summary>
        /// Encodes a passed control based on the configuration.
        /// </summary>
        /// <param name="control">Control which needs to be encoded.</param>
        /// <param name="typeToUse">The type to use.</param>
        /// <param name="settings">The encoding settings and rules to use.</param>
        private static void EncodeControl(Control control, string typeToUse, AntiXssInspectorSettings settings)
        {
            if (settings.EncodingTypes.Count <= 0)
            {
                return;
            }

            // Get the list of properties and encoding types
            Collection<ControlEncodingContext> encodedPropertiesAndTypes = settings.EncodingTypes.GetEncodingTypes(typeToUse);
            if (encodedPropertiesAndTypes.Count <= 0)
            {
                return;
            }

            foreach (ControlEncodingContext encodingContext in encodedPropertiesAndTypes)
            {
                if (encodingContext == null)
                {
                    continue;
                }

                // Using reflection to get the property values
                // Some perf tests show reflection is faster!!!
                PropertyInfo propertyInfo = control.GetType().GetProperty(encodingContext.PropertyName);
                string controlValue = (string)propertyInfo.GetValue(control, null);

                // Based on the encoding type, appropriate encoding method is being called
                switch (encodingContext.EncodingContext)
                {
                    case EncodingContext.Html:
                        // redirecting to internal helper function
                        // to check for color coding.
                        controlValue = HtmlHightlightAndEncode(controlValue, settings.DoubleEncodingFilter, settings.MarkAntiXssOutput, settings.MarkAntiXssOutputColor);
                        break;
                    case EncodingContext.HtmlAttribute:
                        if (settings.DoubleEncodingFilter)
                        {
                            controlValue = HttpUtility.HtmlDecode(controlValue);
                        }

                        controlValue = Encoder.HtmlAttributeEncode(controlValue);
                        break;
                    case EncodingContext.Xml:
                        if (settings.DoubleEncodingFilter)
                        {
                            controlValue = HttpUtility.HtmlDecode(controlValue);
                        }

                        controlValue = Encoder.XmlEncode(controlValue);
                        break;
                    case EncodingContext.XmlAttribute:
                        if (settings.DoubleEncodingFilter)
                        {
                            controlValue = HttpUtility.HtmlDecode(controlValue);
                        }

                        controlValue = Encoder.XmlAttributeEncode(controlValue);
                        break;
                    case EncodingContext.Url:
                        if (settings.DoubleEncodingFilter)
                        {
                            controlValue = HttpUtility.UrlDecode(controlValue);
                        }

                        controlValue = Encoder.UrlEncode(controlValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("settings", Resources.UnknownEncodingContext);
                }

                propertyInfo.SetValue(control, controlValue, null);
            }
        }

        /// <summary>
        /// Internal helper function to html encode input.
        /// </summary>
        /// <param name="input">Un-trusted user input</param>
        /// <param name="filterDoubleEncoding">A value indicating if double encoding filtering should be use.</param>
        /// <param name="markOutput">A value indicating if encoded output should be marked.</param>
        /// <param name="markColor">The color to use if marking is enabled.</param>
        /// <returns>Html encoded value</returns>
        private static string HtmlHightlightAndEncode(string input, bool filterDoubleEncoding, bool markOutput, Color markColor)
        {
            const string MarkedEncodingTag = "<span name='#markantixssoutput'";
            string output = input;
            bool shouldMarkOutput = false;

            // checking whether the encoded input already contains markantixssouput tag
            if (input.Contains(MarkedEncodingTag))
            {
                // stripping the tags.
                int currentDiv = input.IndexOf(MarkedEncodingTag, StringComparison.OrdinalIgnoreCase) + MarkedEncodingTag.Length;
                currentDiv = input.IndexOf(">", currentDiv, StringComparison.Ordinal) + 1;
                output = input.Substring(currentDiv, input.Length - currentDiv - 7);
                shouldMarkOutput = true;
            }

            // checking if double encoding is turned on and decoding the input
            // decoding the input is fine as long as we encode the input again.
            if (filterDoubleEncoding)
            {
                output = HttpUtility.HtmlDecode(output);
            }

            if (markOutput || shouldMarkOutput)
            {
                output = WrapHtmlEncodeWithHighlightColour(output, markColor.ToKnownColor());
            }
            else
            {
                output = Encoder.HtmlEncode(output);
            }

            return output;
        }

        /// <summary>
        /// Encodes an input string and embeds it in a colored &lt;SPAN&gt; tag for use in HTML.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="clr">KnownColor like System.Drawing.KnownColor.CadetBlue</param>
        /// <returns>
        ///  The encoded string is embedded within a &lt;SPAN&gt; tag and style settings for use in HTML.
        /// </returns>
        private static string WrapHtmlEncodeWithHighlightColour(string input, KnownColor clr)
        {
            // HTMLEncode will handle the encoding
            // This check is for making sure that bgcolor is required or not.
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request.QueryString["MarkAntiXssOutput"] != null)
                {
                    string returnInput = "<span name='#markantixssoutput' style ='background-color : " + Color.FromKnownColor(clr).Name + "'>" + Encoder.HtmlEncode(input) + "</span>";
                    return returnInput;
                }
            }

            return Encoder.HtmlEncode(input);
        }

        /// <summary>
        /// Encodes the table cells within a grid view.
        /// </summary>
        /// <param name="gridView">The grid view to encode.</param>
        /// <param name="settings">The encoding settings and rules to use.</param>
        private static void EncodeGridView(GridView gridView, AntiXssInspectorSettings settings)
        {
            foreach (TableCell tc in from GridViewRow gvr in gridView.Rows
                                     where gvr.RowType == DataControlRowType.DataRow
                                     from tc in gvr.Cells.Cast<TableCell>().Where(tc => !string.IsNullOrEmpty(tc.Text))
                                     select tc)
            {
                tc.Text = HtmlHightlightAndEncode(tc.Text, settings.DoubleEncodingFilter, settings.MarkAntiXssOutput, settings.MarkAntiXssOutputColor);
            }
        }

        /// <summary>
        /// Encodes the table cells within a data grid.
        /// </summary>
        /// <param name="dataGrid">The data grid to encode.</param>
        /// <param name="settings">The encoding settings and rules to use.</param> 
        private static void EncodeDataGrid(DataGrid dataGrid, AntiXssInspectorSettings settings)
        {
            foreach (TableCell tc in from DataGridItem dgin in dataGrid.Items
                                     where (dgin.ItemType == ListItemType.AlternatingItem || dgin.ItemType == ListItemType.Item) || dgin.ItemType == ListItemType.SelectedItem
                                     from TableCell tc in dgin.Cells
                                     where !string.IsNullOrEmpty(tc.Text)
                                     select tc)
            {
                tc.Text = HtmlHightlightAndEncode(tc.Text, settings.DoubleEncodingFilter, settings.MarkAntiXssOutput, settings.MarkAntiXssOutputColor);
            }
        }

        /// <summary>
        /// Finds controls in the specified control collection and encodes them as necessary.
        /// </summary>
        /// <param name="container">The control container</param>
        /// <param name="settings">The encoding settings and rules to use.</param>
        private void FindAndEncodeControls(Control container, AntiXssInspectorSettings settings)
        {
            List<string> excludedUniqueControlIds =
                new List<string>(AttributeExclusionChecker.GetExcludedControlUniqueIdsForContainer(container, this.GetType()));

            this.FindAndEncodeControls(container.Controls, excludedUniqueControlIds, settings);
        }

        /// <summary>
        /// Finds controls in the specified control collection and encodes them as necessary.
        /// </summary>
        /// <param name="controlCollection">The control collection to encode.</param>
        /// <param name="excludedUniqueControlIds">A list of excluded control Ids.</param>
        /// <param name="settings">The encoding settings and rules to use.</param>
        private void FindAndEncodeControls(ControlCollection controlCollection, ICollection<string> excludedUniqueControlIds, AntiXssInspectorSettings settings)
        {
            foreach (Control control in
                controlCollection.Cast<Control>().Where(control => !excludedUniqueControlIds.Contains(control.UniqueID) && control.Visible))
            {
                if (control.HasControls() && control.Controls.Count > 0)
                {
                    this.FindAndEncodeControls(control, settings);
                }

                string controlType = control.GetType().ToString();

                string baseControlType = null;
                if (control.GetType().BaseType != null)
                {
                    baseControlType = control.GetType().BaseType.ToString();
                }

                if (settings.EncodingTypes.Contains(controlType))
                {
                    EncodeControl(control, settings);
                }
                else if (baseControlType != null && settings.EncodeDerivedControls && settings.EncodingTypes.Contains(baseControlType))
                {
                    EncodeControl(control, baseControlType, settings);
                }

                // If the type is data grid then its cells will be encoded.
                // This type of cell encoding is only required for controls that
                // don't require <ItemTemplate/> i.e. those data controls which
                // automatically generate columns.
                DataGrid controlAsDataGrid = control as DataGrid;
                if (controlAsDataGrid != null)
                {
                    EncodeDataGrid(controlAsDataGrid, settings);
                }

                // If the type is gridview then its cells will be encoded.
                GridView controlAsGridView = control as GridView;
                if (controlAsGridView != null)
                {
                    EncodeGridView(controlAsGridView, settings);
                }
            }
        }
    }
}
