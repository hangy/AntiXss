using System;
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
    public partial class SubClassedLabel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RedLabel1.Text = Request.QueryString["q"];
        }
    }

    [Microsoft.Security.Application.SecurityRuntimeEngine.SupressAntiXssEncoding()]
    public class RedLabel:Label 
    {
        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div id=\"" + this.ClientID + "\" style=\"background-color:Red\">");
            writer.Write(this.Text);
            writer.Write("</div>");
        }
    }
}
