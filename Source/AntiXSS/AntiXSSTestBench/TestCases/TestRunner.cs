using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Security.Application;
using System.IO;

namespace Microsoft.Security.Application.AntiXSSTestBench
{
    /// <summary>
    /// Summary description for TestRunner.
    /// </summary>
    public sealed class TestRunner : TestRunnerBase
    {
        public TestRunner(int repeats)
            : base(repeats)
        {

        }

        public void RunBenchmarks(int i)
        {
            // First cleanup output file
            CleanUp();

            Output.WriteLine("Test starts: " + DateTime.Now.ToString());
            Output.WriteLine("----------------------------------------");
            //----------------------------------------------------------------

            if (i == 1)
            {
                //Running Performance Tests
                PerfTestRunner();
            }
            else if (i == 2)
            {
                //Running the RSnake's Exploits
                XSSTestRunner();
            }
            //----------------------------------------------------------------
            Output.WriteLine("----------------------------------------");
            Output.WriteLine("Test ends: " + DateTime.Now.ToString());

            //finally open the results
            System.Diagnostics.Process.Start(@"Output.txt");
        }

        private void PerfTestRunner()
        {
            Output.WriteLine("Running Performance Tests...");
            Output.WriteLine("----------------------------------------");
            string[] inputArray = new string[7];
            //iterate for diffrent input strings
            for (int i = 0; i < inputArray.Length; i++)
            {
                inputArray = GetInputString(i);
                //output input string length
                Output.WriteLine("<inputLength>" + inputArray[i].Length.ToString() + "</inputLength>");

                TestExecutor runner = new TestExecutor();
                RunBenchmark(new DoWork(runner.DoNETHTMLEncoding), inputArray[i]);
                //now run AntiXSS html encoding
                RunBenchmark(new DoWork(runner.DoAntiXSSHTMLEncoding), inputArray[i]);
                Output.WriteLine(" ");
            }
        }

        private void XSSTestRunner()
        {
            Output.WriteLine("Encoding RSnake's XSS Attack Vectors");
            Output.WriteLine("----------------------------------------");
            using (StreamReader s = new StreamReader(@"XSSAttacks.txt"))
            {
                string input;
                while ((input = s.ReadLine()) != null)
                {
                    TestExecutor runner = new TestExecutor();
                    runner.DoXSSAttackVectors(input);

                    //RunBenchmark(new DoWork(runner.DoXSSAttackVectors), input);
                }
            }
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

        private void CleanUp()
        {
            FileInfo fi = new FileInfo(@"Output.txt");
            fi.Delete();
        }

    }
}
