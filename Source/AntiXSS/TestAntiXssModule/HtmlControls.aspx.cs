﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace TestAntiXssModule
{
    public partial class HtmlControls : System.Web.UI.Page
    {
         protected void Page_Load(object sender, EventArgs e)
        {
            homepage.HRef = Request.QueryString["q"];
        }
    }
}
