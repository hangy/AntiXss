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
    /// Authenticates the user and redirects to the requested page.
    /// User authentication data is stored in the web.config file.
    /// </summary>
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string venueStyle = "boldoblique";
            //Checking the querysting for a location
            if (Request.QueryString["location"] != null)
            {
                //Setting the venue using location from the query string
                //User input is being encoded using HtmlEncode as the Label.Text
                //places data directly inside HTML elements.
                lblVenue.Text = AntiXss.HtmlEncode(Request.QueryString["location"],System.Drawing.KnownColor.Yellow);
            }
            //Getting the style from querystring
            venueStyle = Request.QueryString["style"];
            //Encoding class attribute using AntiXss.HtmlAttributeEncode
            //As the input could be from QueryString, venueStyle is being encoded.
            lblVenue.Attributes["class"] = AntiXss.HtmlAttributeEncode(venueStyle);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //Authenticating the user
            if (FormsAuthentication.Authenticate(txtUsername.Text, txtPassword.Text))
            {
                //Setting the authentication cookie
                FormsAuthentication.SetAuthCookie(txtUsername.Text, false);
                //Storing the username in the cookie
                Response.Cookies["UserSettings"]["Username"] = txtUsername.Text;
                //Checking if redirection url is passed in the querystring
                if (Request.QueryString["RedirectUrl"] != null)
                {
                    //Encoding the QueryString value
                    //AntiXss.UrlEncode is being used as the data is being passed
                    //as a URL to Response.Redirect
                    string tempUrl = AntiXss.UrlEncode(Request.QueryString["RedirectUrl"]);
                    Response.Redirect(tempUrl);
                }
                else
                    Response.Redirect("default.aspx");
            }
            else
            {
                lblError.Text = ("Invalid UserID and Password");
            }
        }
    }
}
