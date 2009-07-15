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
using System.Text;
using System.Diagnostics;
using x = Microsoft.Security.Application;

namespace AntiXSSTestApp
{
    public partial class PerfTestRunner : System.Web.UI.Page
    {
        //set # of Iterations...
        const int ITERATIONS = 1000;

        private static TimeSpan seqTime = TimeSpan.Zero;
        int i = 1;

        //flag to surround results with Table <START> and <END>
        //bool bInitPrint = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                //Start writing to browser
                Response.Write("Running Performance Tests...... <br/>");

                string[] inputArray = new string[7];

                //iterate for diffrent input strings
                for (int i = 0; i < inputArray.Length; i++)
                {
                    inputArray = GetInputString(i);

                    Response.Write("Input Length: " + inputArray[i].Length.ToString() + "<br/>");

                    //start the table header...
                    startTable();

                    //start calling library methods...
                    perfRunner(inputArray[i], 0); // .NET HTTPUtility Encoding - HTMLEncode           

                    perfRunner(inputArray[i], 1); // New AntiXSS Encoding - HTMLEncode           
                    //perfRunner(inputArray[i], 2); // New AntiXSS Encoding - HTMLAttributeEncode
                    //perfRunner(inputArray[i], 3); // New AntiXSS Encoding - URLEncode
                    //perfRunner(inputArray[i], 4); // New AntiXSS Encoding - XMLEncode           
                    //perfRunner(inputArray[i], 5); // New AntiXSS Encoding - XMLAttributeEncode
                    //perfRunner(inputArray[i], 6); // New AntiXSS Encoding - JavaScriptEncode           
                    //perfRunner(inputArray[i], 7); // New AntiXSS Encoding - VisualBasicScriptEncode           
                    //perfRunner(inputArray[i], 8); // New AntiXSS Encoding - HtmlEncode(string input, System.Drawing.KnownColor.Blue  clr) **

                    //ending table
                    endTable();
                }

            }
        }

        private void perfRunner(string strTestString, int intTestType)
        {
            string strEncodedString = "";
            Stopwatch sw = new Stopwatch();

            switch (intTestType)
            {
                case 0:	// HTTP Utility - HTMLEncode
                    i = 1;
                    sw.Start();
                    while (i <= ITERATIONS)
                    {
                        // calling the ecoding function
                        HttpUtility.HtmlEncode(strTestString);

                        i = i + 1;
                    }
                    sw.Stop();
                    seqTime = sw.Elapsed;

                    //display results now
                    strEncodedString = HttpUtility.HtmlEncode(strTestString);
                    PrintResults("HttpUtility.HtmlEncode", strEncodedString, seqTime);

                    break;

                case 1: // AntiXSS - HTMLEncode
                    i = 1;
                    sw.Start();
                    while (i <= ITERATIONS)
                    {
                        // calling the ecoding function
                        x.AntiXss.HtmlEncode(strTestString);

                        i = i + 1;
                    }
                    sw.Stop();
                    seqTime = sw.Elapsed;

                    //display results now
                    strEncodedString = x.AntiXss.HtmlEncode(strTestString);
                    PrintResults("AntiXss.HtmlEncode", strEncodedString, seqTime);
                    break;

                case 2: // AntiXSS - HTMLAttributeEncode
                    i = 1;
                    sw.Start();
                    while (i <= ITERATIONS)
                    {
                        // calling the ecoding function
                        x.AntiXss.HtmlAttributeEncode(strTestString);

                        i = i + 1;
                    }
                    sw.Stop();
                    seqTime = sw.Elapsed;

                    //display results now
                    strEncodedString = x.AntiXss.HtmlAttributeEncode(strTestString);
                    PrintResults("AntiXss.HtmlAttributeEncode", strEncodedString, seqTime);
                    break;

                case 3: // AntiXSS Encoding - URLEncode
                    i = 1;
                    sw.Start();
                    while (i <= ITERATIONS)
                    {
                        // calling the ecoding function
                        x.AntiXss.UrlEncode(strTestString);

                        i = i + 1;
                    }
                    sw.Stop();
                    seqTime = sw.Elapsed;

                    //display results now
                    strEncodedString = x.AntiXss.UrlEncode(strTestString);
                    PrintResults("AntiXss.URLEncode", strEncodedString, seqTime);
                    
                    break;

                case 4: // AntiXSS - XMLEncode
                    i = 1;
                    sw.Start();
                    while (i <= ITERATIONS)
                    {
                        // calling the ecoding function
                        x.AntiXss.XmlEncode(strTestString);

                        i = i + 1;
                    }
                    sw.Stop();
                    seqTime = sw.Elapsed;

                    //display results now
                    strEncodedString = x.AntiXss.XmlEncode(strTestString);
                    PrintResults("AntiXss.XMLEncode", strEncodedString, seqTime);
                    break;

                case 5: // AntiXSS - XMLAttributeEncode
                    i = 1;
                    sw.Start();
                    while (i <= ITERATIONS)
                    {
                        // calling the ecoding function
                        x.AntiXss.XmlAttributeEncode(strTestString);

                        i = i + 1;
                    }
                    sw.Stop();
                    seqTime = sw.Elapsed;

                    //display results now
                    strEncodedString = x.AntiXss.XmlAttributeEncode(strTestString);
                    PrintResults("AntiXss.XMLAttributeEncode", strEncodedString, seqTime);
                    break;

                case 6: // AntiXSS Encoding - JavaScriptEncode
                    i = 1;
                    sw.Start();
                    while (i <= ITERATIONS)
                    {
                        // calling the ecoding function
                        x.AntiXss.JavaScriptEncode(strTestString);

                        i = i + 1;
                    }
                    sw.Stop();
                    seqTime = sw.Elapsed;

                    //display results now
                    strEncodedString = x.AntiXss.JavaScriptEncode(strTestString);
                    PrintResults("AntiXss.JavaScriptEncode", strEncodedString, seqTime);

                    break;

                case 7: // AntiXSS Encoding - VisualBasicScriptEncode
                    i = 1;
                    sw.Start();
                    while (i <= ITERATIONS)
                    {
                        // calling the ecoding function
                        x.AntiXss.VisualBasicScriptEncode(strTestString);

                        i = i + 1;
                    }
                    sw.Stop();
                    seqTime = sw.Elapsed;

                    //display results now
                    strEncodedString = x.AntiXss.VisualBasicScriptEncode(strTestString);
                    PrintResults("AntiXss.VisualBasicScriptEncode", strEncodedString, seqTime);

                    break;

                case 8: // AntiXSS Encoding - HTMLEncode with color
                    i = 1;
                    sw.Start();
                    while (i <= ITERATIONS)
                    {
                        // calling the ecoding function
                        x.AntiXss.HtmlEncode(strTestString, System.Drawing.KnownColor.Blue);

                        i = i + 1;
                    }
                    sw.Stop();
                    seqTime = sw.Elapsed;

                    //display results now
                    strEncodedString = x.AntiXss.HtmlEncode(strTestString, System.Drawing.KnownColor.Blue);
                    PrintResults("AntiXss.HtmlEncode(color)", strEncodedString, seqTime);
                    break;

                default:// do nothing
                    break;
            }
        }

        private static string Speedup(TimeSpan elapsed, TimeSpan elapsedTime)
        {
            long msecs = (long)(Math.Round(elapsed.TotalMilliseconds));
            long xsecs = (long)(Math.Round(elapsed.TotalMilliseconds / 10));
            return String.Format("{0}.{1:00} secs ({2} ms)", xsecs / 100, xsecs % 100, msecs);
        }

        private string[] GetInputString(int i)
        {
            string[] returnInput = new string[]
               {
                   "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._",//64 safe characters
                   "<a><script type='text/javascript'>alert('XSS');</script>XSS</a>", //64 characters with 25% encodings
                   "<a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a>",//128 characters with 25% encodings
                   "<a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a>",//512 characters with 25% encodings
                   "<a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a><a><script type='text/javascript'>alert('XSS');</script>XSS</a>",//1024 characters with 25% encodings
                   "~!@#$%^&*()_+=-`,./?><;'\":[]|}{~!@#$%^&*()_+=-`,./?><;'\":[]|}{",//64 characters with 100% encodings
                   "\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B\uFB5C\uFB66\uFB65\uFB56\uFB59\uFB5A\uFB5B",//unicode characters
                   "北京奥运完美谢幕 罗格称赞无以伦比"//international characters
               };
            return returnInput;
        }

        private void PrintResults(string strMessage, string strEncodedString, TimeSpan elapsedTime)
        {
            //printing results...
            Response.Write("<tr>");
            Response.Write("<td>" + strMessage + "</td>");
            Response.Write("<td width=50%>" + strEncodedString + "</td>");
            Response.Write("<td>" + strEncodedString.Length.ToString() + "</td>");
            Response.Write("<td>" + Speedup(elapsedTime, TimeSpan.Zero) + "</td>");
            Response.Write("</tr>");
        }
        
        private void startTable()
        {
            Response.Write("<table cellpadding=5 cellspacing=1 width=1000><tr bgcolor=gray>");
            Response.Write("<td>Encoding Method Used</td>");
            Response.Write("<td width=50%>Encoded Output</td>");
            Response.Write("<td>Output Length</td>");
            Response.Write("<td>Time Elapsed</td>");
            Response.Write("</tr>");
        }

        private void endTable()
        {
            Response.Write("</table>");

        }
       
    }
}
