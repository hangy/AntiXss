using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using Microsoft.Security.Application;
//===============================================================================
// Microsoft Connected Info Security Group samples
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
 
namespace Feedback
{
    /// <summary>
    /// Default page of the website. Welcomes the user and provides link to the feedback summary page.
    /// </summary>
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Creating welcome script using string builder object.
            StringBuilder sbScript = new StringBuilder();
            sbScript.Append("function welcomeUserMessage() {");
            //AntiXss.JavaScriptEncode returns encoded value which is safe for JavaScript context.
            //As the username is coming from Cookies which is an untrusted source it is being encoded.
            //Similary in cases of VBScript AntiXss.VisualBasicScriptEncode should be used.
            sbScript.AppendLine("alert('Welcome '+" + AntiXss.JavaScriptEncode(Request.Cookies["UserSettings"]["Username"]) + "+' to the feedback management site');");
            sbScript.AppendLine("}");
            //Registering the script.
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "welcomeUserMessage()", sbScript.ToString(), true);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("summary.aspx?sname=Secure%20Application%20Development");
        }
    }
}
