using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Security.Application;

namespace AntiXSSTestApp
{
    public partial class TestMe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("Test for XSS!!!<br/><br/>");
        }

         protected void Submit_Click(object sender, EventArgs e)
        {
             if(txtBox1.Text != "")
             {
                 string strInputText = txtBox1.Text;
                 //Response.Write(AntiXss.HtmlEncode(strInputText)); //plain html encoding
                 //Response.Write(AntiXss.UrlEncode(strInputText));
                 Response.Write(AntiXss.HtmlAttributeEncode(strInputText,65001)); //plain html encoding
                 //Response.Write(AntiXss.HtmlAttributeEncode(strInputText, 932));
                 //Response.Write(AntiXss.UrlEncode(strInputText, 932));
             }
        }

    }
}
