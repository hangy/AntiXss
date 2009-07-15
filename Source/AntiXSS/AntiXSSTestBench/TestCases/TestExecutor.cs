using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Microsoft.Security.Application;

namespace Microsoft.Security.Application.AntiXSSTestBench
{
    /// <summary>
    /// Main class which executes tests.
    /// </summary>
    public class TestExecutor
    {
        const int ITERATIONS = 5000;

        public void DoXSSAttackVectors(string text)
        {
            // calling the ecoding function
            Output.WriteLine("<output>" + AntiXss.HtmlEncode(text) + "</output>");
        }

        public void DoNETHTMLEncoding(string text)
        {
            // Calling .NET HTTPUtility Encoding - HTMLEncode 
            Output.WriteLine("<output>" + HttpUtility.HtmlEncode(text) + "</output>");

            //now Iterate to elapse time
            int i = 1;
            while (i <= ITERATIONS)
            {
                // calling the ecoding function
                HttpUtility.HtmlEncode(text);

                i = i + 1;
            }

        }
        public void DoAntiXSSHTMLEncoding(string text)
        {
            // Calling AntiXSS Encoding - HTMLEncode 
            Output.WriteLine("<output>" + AntiXss.HtmlEncode(text) + "</output>");

            //now Iterate to elapse time
            int i = 1;
            while (i <= ITERATIONS)
            {
                // calling the ecoding function
                AntiXss.HtmlEncode(text);

                i = i + 1;
            }

        }
        
    }
}
