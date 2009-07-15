using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Reflection;
using Microsoft.Security.Application.SecurityRuntimeEngine.Configuration;
using Microsoft.Security.Application.SecurityRuntimeEngine;
using System.Drawing;
using Microsoft.Security.Application;
using System.Web.UI.WebControls;
using System.Data;

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PageProtection
{
    /// <summary>
    /// A static helper class which provides the XSS protection methods. 
    /// Contains methods to find and encode controls on a page.
    /// </summary>
    internal static class XssProtection
    {
       
        static List<string> lstSkippedControls = new List<string>();
        static ModuleConfiguration objConfig;

        /// <summary>
        /// Finds and encodes controls as per the defined configuration.
        /// </summary>
        /// <param name="cc">Control Collection in which to search the controls</param>
        private static void FindAndEncodeControls(Page p, ControlCollection cc)
        {
            if (objConfig == null)
                throw new ArgumentNullException("objConfig");
            if (cc == null)
                throw new ArgumentNullException("cc");
            int ccCount = cc.Count;
            for (int i = 0; i < ccCount; i++)
            {
                Control c = cc[i];
                
                bool blnSkip = false;
                //checking whether a control is excluded or not.
                if (c.UniqueID!=null)
                    blnSkip = XssProtection.IsControlExcluded(p, c);

                if (blnSkip==false) {
                    //If there are any controls in this control, 
                    //they will be recursively encoded
                    if (c.Controls.Count > 0)
                        XssProtection.FindAndEncodeControls(p, c.Controls);
                    string strType = c.GetType().ToString();
                    string strBaseType = c.GetType().BaseType.ToString();

                    //If the type is part of the configuration then it will be encoded.
                    if (objConfig.EncodingTypes.Contains(strType))
                    {
                        EncodeControl(c, strType);
                    }

                    if (objConfig.EncodeDerivedControls==true && objConfig.EncodingTypes.Contains(strBaseType))
                    {
                        EncodeControl(c, strBaseType);
                    }
                    //If the type is data grid then its cells will be encoded.
                    //This type of cell encoding is only required for controls that
                    //don't require <ItemTemplate/> i.e. those data controls which
                    //automatically generate columns.
                    if (c is DataGrid)
                    {
                        foreach (DataGridItem dgin in ((DataGrid)c).Items)
                        {
                            if (dgin.ItemType == ListItemType.AlternatingItem || dgin.ItemType == ListItemType.Item || dgin.ItemType == ListItemType.SelectedItem)
                            {
                                foreach (TableCell tc in dgin.Cells)
                                {
                                    if (tc.Text != "")
                                    {
                                        tc.Text = XssProtection.InternalHtmlEncode(tc.Text);
                                    }
                                }
                            }
                        }
                    }
                    //If the type is gridview then its cells will be encoded.
                    if (c is GridView)
                    {
                        foreach (GridViewRow gvr in ((GridView)c).Rows)
                        {
                            if (gvr.RowType == DataControlRowType.DataRow)
                            {
                                foreach (TableCell tc in gvr.Cells)
                                {
                                    if (tc.Text != "")
                                    {
                                        tc.Text = XssProtection.InternalHtmlEncode(tc.Text);
                                    }
                                }
                            }
                        }
                    }
                    
                }
            
            }
            
        }

        /// <summary>
        /// Encodes an entire given page.
        /// </summary>
        /// <param name="p">Page that needs to be encoded.</param>
        private static void EncodePage(Page p) 
        {
            if (objConfig == null)
                throw new ArgumentNullException("objConfig");
            if (p == null)
                throw new ArgumentNullException("p");

            //Encode page properties
            XssProtection.EncodeControl(p, typeof(Page).ToString());
            //Encode page controls
            XssProtection.FindAndEncodeControls(p, p.Controls);

            
        }

        /// <summary>
        /// Encodes a passed control based on the configuration.
        /// </summary>
        /// <param name="control">Control which needs to be encoded.</param>
        /// <param name="type">Type of the control.</param>
        private static void EncodeControl(Control control,string type) 
        {
                //Get the list of properties and encoding types
                List<ControlEncodingContext> lstEncodingTypes = objConfig.EncodingTypes.GetEncodingTypes(type);
                if (lstEncodingTypes.Count > 0)
                {
                    int lstCount = lstEncodingTypes.Count;
                    for(int index =0;index <lstCount;index++) 
                    {
                        ControlEncodingContext et = lstEncodingTypes[index];
                        if (et != null)
                        {
                            string strValue = string.Empty;
                            //Using reflection to get the property values
                            //Some perf tests show reflection is faster!!!
                            PropertyInfo objInfo = null;
                            objInfo = control.GetType().GetProperty(et.PropertyName);
                            strValue = (string)(objInfo.GetValue(control, null));

                            //Based on the encoding type, appropriate encoding method is being called
                            switch (et.EncodingContext)
                            {
                                case EncodingContexts.Html:
                                    //redirecting to internal helper function
                                    //to check for color coding.
                                    strValue= XssProtection.InternalHtmlEncode(strValue);
                                    break;
                                case EncodingContexts.HtmlAttribute:
                                    if (objConfig.DoubleEncodingFilter == true)
                                        strValue = System.Web.HttpUtility.HtmlDecode(strValue);
                                    strValue = AntiXss.HtmlAttributeEncode(strValue);
                                    break;
                                case EncodingContexts.Xml:
                                    if (objConfig.DoubleEncodingFilter == true)
                                        strValue = System.Web.HttpUtility.HtmlDecode(strValue);
                                    strValue = AntiXss.XmlEncode(strValue);
                                    break;
                                case EncodingContexts.XmlAttribute:
                                    if (objConfig.DoubleEncodingFilter == true)
                                        strValue = System.Web.HttpUtility.HtmlDecode(strValue);
                                    strValue = AntiXss.XmlAttributeEncode(strValue);
                                    break;
                                case EncodingContexts.Url:
                                    if (objConfig.DoubleEncodingFilter == true)
                                        strValue = System.Web.HttpUtility.UrlDecode(strValue);
                                    strValue = AntiXss.UrlEncode(strValue);
                                    break;
                                default:
                                    throw new IndexOutOfRangeException();
                            }
                            objInfo.SetValue(control, strValue, null);
                        }
                    }
                    
                }
            
        }


        /// <summary>
        /// Checks every field of the page for SupressAntiXssEncoding 
        /// attribute and stores the name of the field in a list.
        /// </summary>
        /// <param name="p">Page which needs to be checked.</param>
        public static bool IsControlExcluded(Page p, Control c) 
        {
            try
            {
                object[] attributes = c.GetType().GetCustomAttributes(typeof(SupressAntiXssEncodingAttribute), true);
                if (attributes.Length > 0)
                    return true;


                FieldInfo[] fi = p.GetType().GetFields(BindingFlags.Instance  | BindingFlags.NonPublic | BindingFlags.Public);

                for (int i = 0; i < fi.Length; i++)
                {
                    attributes = Attribute.GetCustomAttributes(fi[i], typeof(SupressAntiXssEncodingAttribute), true);
                    if (attributes.Length > 0)
                    {
                        object control = fi[i].GetValue(p);
                        if ((control != null) && ((Control)control).UniqueID == c.UniqueID)
                            return true;
                    }
                }
            }
            catch { throw;  }
            return false;
        }


        /// <summary>
        /// Protects a page by calling the EncodePage method.
        /// </summary>
        /// <param name="page">Page that needs to be protected.</param>
        /// <param name="config">Configuration of the protection.</param>
        public static void Protect(Page page, ModuleConfiguration config)
        {
            XssProtection.objConfig = config;
            if (page != null)
            {
                page.PreRender += new EventHandler(page_PreRender);
            }
            
        }

        

        /// <summary>
        /// This event is fired before a page is being rendered. 
        /// This event is used to encode the controls on the page. 
        /// </summary>
        /// <param name="sender">Page which is being pre-rendered.</param>
        /// <param name="e">Event arguments.</param>
        static void page_PreRender(object sender, EventArgs e)
        {
            try
            {
                Page p = (Page)sender;
                XssProtection.EncodePage(p);
            }
            catch { throw; }
        }

        /// <summary>
        /// Internal helper function to html encode input.
        /// </summary>
        /// <param name="input">Untrusted user input</param>
        /// <returns>Html encoded value</returns>
        static string InternalHtmlEncode(string input) 
        {
            string strMarkAntiXssOutputDiv = "<span name='#markantixssoutput'";
            string output = input;
            bool blnMarkAntiXssOutput = false;
            //checking whether the encoded input already contains markantixssouput tag
            if (input.Contains(strMarkAntiXssOutputDiv))
            {
                //stripping the tags.
                int iDiv = input.IndexOf(strMarkAntiXssOutputDiv) + strMarkAntiXssOutputDiv.Length;
                iDiv = input.IndexOf(">", iDiv) + 1;
                output = input.Substring(iDiv, input.Length - iDiv - 7);
                blnMarkAntiXssOutput = true;
            }
            //checking if double encoding is turned on and decoding the input
            //decoding the input is fine as long as we encode the input again.
            if (objConfig.DoubleEncodingFilter == true)
                output = System.Web.HttpUtility.HtmlDecode(output);
            if (objConfig.MarkAntiXssOutput == true || blnMarkAntiXssOutput == true)
                output = AntiXss.HtmlEncode(output, objConfig.MarkAntiXssOutputColor.ToKnownColor());
            else
                output = AntiXss.HtmlEncode(output);
            return output;
        }
    }
}
