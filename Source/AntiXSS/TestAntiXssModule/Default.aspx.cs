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
        protected global::System.Web.UI.WebControls.Label Label1;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = Microsoft.Security.Application.AntiXss.HtmlEncode(Request.QueryString["q"],System.Drawing.KnownColor.Beige);
            
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
