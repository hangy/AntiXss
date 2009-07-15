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
using Microsoft.Security.Application;
using System.IO;
using System.Text;

namespace AntiXSSTestApp
{
    public partial class _Default : System.Web.UI.Page
    {
        public string note = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {


            //Vineet Batta<ああBatta // あぃい<>ぅう
            //string unicodeString = "あ";
            //string output = AntiXss.UrlEncode(unicodeString, 932);
            //Response.Write(output);


            //string input = "The contains SHIFT-JIS, digit SEVEN ( &#12471; -- &#x30B2;&#x30B3;&#x30B4;&#x30B5;&#x30B6;&#x30B7; )";
            //string input = "ゲコゴサザシ";


            string input = "ʃ";
            //StreamReader strReader = new StreamReader(@"C:\Source\AntiXSS\Internal\V3.0\Source\AntiXSS\AntiXSSLibrary\bin\Debug\Inputfile.txt");
            //input = strReader.ReadLine(); // str should be input string to the method.
            //strReader.Close();
            
            
            //input = "<a href='http://www.microsoft.com'>NO XSS </a> <a> UNBALANCED TAG <br> Hello <script> XSS </script>";
            //input = "a<script/>a1b<a href='http://www.microsoft.com'>this is my lin</script>fa<script/>a1b<a href='http://www.microsoft.com'>this is my link</script>fa<script/>a1";
            //input = "<font><b> <a> This is XSS TEST. <script> alert('Inject Script'); </script> ! FAIL ME DUDE </a> </font>";
            //input = " <html><body><b><a href=’http://www.live.com’> Search</a></body></html>";
            //input = "<a href='fred.htm'>fred</a>";
            //string output = string.Empty;
            //TextReader stringReader =  new StringReader(input);
            //TextWriter stringWriter = new StringWriter();




            //Stream TSSW = null ;
            //TextReader TR = null;

            try
            {                


                //WRITE
                //string TSstrpath = @"C:\Source\AntiXSS\Internal\V3.0\Source\AntiXSS\AntiXSSLibrary\bin\Debug\GetSafeHTMLTRSW.txt";
                //TSSW = new FileStream(TSstrpath, FileMode.Create, FileAccess.Write);

                //READ
                //string strpath = @"C:\Source\AntiXSS\Internal\V3.0\Source\AntiXSS\AntiXSSLibrary\bin\Debug\Inputfile.txt";
                //TR = new StreamReader(strpath);
                //lblMessage.Text = str;
                //lnkId.InnerHtml = str;
                //lnkbtn.Text = str;
                //Label1.Text = str;
                //Response.Write(output);

                //HtmlToHtml htmlObject = null;                        
                //htmlObject = new HtmlToHtml();                

                // Set the properties.
                //htmlObject.FilterHtml = true;
                //htmlObject.OutputHtmlFragment = true;
                //htmlObject.NormalizeHtml = true;                

                //htmlObject.Convert(stringReader, stringWriter);
                //htmlObject = null; 

                note = "http://www.yahoo.com?id=" + AntiXss.UrlEncode("vbatta ʃ good<a>");
                //note = AntiXss.HtmlAttributeEncode(input, 65001);







                //string errorMessage = "No Items found for " + txtData.Text + "";
                //Type cstype = this.GetType();
                //Page pge = (Page)this.Page;
                //CreateMessageAlert(ref pge, errorMessage, "error", cstype);


            }
            catch (Exception ex)
            {
            }
            finally
            {
                /*
                if (TSSW != null)
                    TSSW.Close();
                if (TR != null)
                    TR.Close();
                 */ 
            }

            Response.Write(note);            
        }

        // Utility class.
        public  void CreateMessageAlert(ref System.Web.UI.Page pge, string message, string key, Type type)
        {
            message = message.Replace("'", "\\'");
            string script = "<script language=JavaScript>alert('" + AntiXss.JavaScriptEncode(message) + "');</script>";

            if (!pge.ClientScript.IsStartupScriptRegistered(key))
                pge.ClientScript.RegisterStartupScript(type, key, script);
        }
    }
}
