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
using System.Collections.Generic;

namespace TestAntiXssModule
{
    public partial class DataBinding : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            TemplateColumn tempClmn = (TemplateColumn)mygrid.Columns[2];
            CompiledTemplateBuilder clmnBuilder = (CompiledTemplateBuilder)tempClmn.ItemTemplate;
            Control tempControl = new Control();
            clmnBuilder.InstantiateIn(tempControl);
            foreach (Control c in tempControl.Controls)
            {
                Response.Write(c.GetType().ToString());
            }

        }

        protected void mygrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            this.Response.Write("hello from grid item bound");
        }

        void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Page.Response.Write("Hello from page rowdatabound.");
            Response.Write(e.Row.Cells.Count);
            
        }

        
        
    }
}
