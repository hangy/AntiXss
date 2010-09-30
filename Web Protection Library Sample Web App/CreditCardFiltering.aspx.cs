// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreditCardFiltering.aspx.cs" company="Microsoft Corporation">
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
//   Demonstrates the Credit Card filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Web_Protection_Library_Sample_Web_App
{
    using System;
    using System.Web.UI;

    /// <summary>
    /// Demonstrates the credit card filter.
    /// </summary>
    public partial class CreditCardFiltering : Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                this.Reflected.Visible = true;
                this.ReflectedOutput.Text = Microsoft.Security.Application.Encoder.HtmlEncode(this.CreditCardNumber.Text);
            }
        }
    }
}