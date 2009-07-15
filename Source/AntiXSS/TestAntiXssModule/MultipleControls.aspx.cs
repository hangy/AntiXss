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
    public partial class MultipleControls : System.Web.UI.Page
    {
        [Microsoft.Security.Application.SecurityRuntimeEngine.SupressAntiXssEncoding()]
        protected global::System.Web.UI.WebControls.CheckBox CheckBox1;

        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = Request.QueryString["q"];
            LinkButton1.Text = Request.QueryString["q"];
            HyperLink1.NavigateUrl = Request.QueryString["q"];
            HyperLink1.Text = "link";
            RadioButton1.Text = Request.QueryString["q"];
            CheckBox1.Text = Request.QueryString["q"];
        }
    }
}
