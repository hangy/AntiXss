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

    
    public partial class _Default : System.Web.UI.Page
    {
        
        protected System.Web.UI.WebControls.Label Label1;

        [Microsoft.Security.Application.SecurityRuntimeEngine.SupressAntiXssEncoding()]
        protected System.Web.UI.WebControls.Label Label2=new Label();

        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = Request.QueryString["q"];
            Label2.Text = Request.QueryString["q"];
            this.Controls.Add(Label2);
        }

    }

    public class userdefinedlabelclass : System.Web.UI.WebControls.Label
    {
        string _text;
        public new string Text
        {
            get { return _text; }
            set { _text = value; }
        }

    }

}
