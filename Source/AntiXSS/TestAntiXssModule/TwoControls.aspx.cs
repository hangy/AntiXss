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
    public partial class TwoControls : System.Web.UI.Page
    {
        //[Microsoft.Security.Application.SecurityRuntimeEngine.SupressAntiXssEncoding]
        protected global::System.Web.UI.WebControls.Label Label2;

        [Microsoft.Security.Application.SecurityRuntimeEngine.SupressAntiXssEncoding]
        protected System.Web.UI.WebControls.Label label10;
        protected void Page_Load(object sender, EventArgs e)
        {
            label10 = new Label();
            label10.Visible = true;
            label10.Enabled = true;
            label10.Text = "<script>alert('Syed')</script>";
            Panel1.Controls.Add(label10);
            label10.Text = "<script>alert('Syed')</script>";
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Label1.Text = "<script>alert('Syed')</script>";
            Label2.Text = "<script>alert('Syed')</script>";


        }
    }
}
