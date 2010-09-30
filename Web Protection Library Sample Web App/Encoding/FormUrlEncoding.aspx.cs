// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormUrlEncoding.aspx.cs" company="Microsoft Corporation">
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
//
// </copyright>
// <summary>
//   Demonstrates application/x-www-form-urlencoded encoding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Web_Protection_Library_Sample_Web_App.Encoding
{
    using System;

    using Microsoft.Security.Application;

    /// <summary>
    /// Demonstrates application/x-www-form-urlencoded encoding.
    /// </summary>
    public partial class FormUrlEncoding : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                return;
            }

            this.EncodedResult.Visible = true;
            this.EncodedOutput.Text = Encoder.HtmlEncode(Encoder.HtmlFormUrlEncode(this.QueryData.Text));
        }
    }
}