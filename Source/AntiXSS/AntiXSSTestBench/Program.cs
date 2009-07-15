using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Security.Application.AntiXSSTestBench
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //start tests
                Console.WriteLine("Please choose a Test Bench:");
                Console.WriteLine("1: Performance Test Bench");
                Console.WriteLine("2: XSS Validation Test Bench");

                //read the user input
                int i = Int32.Parse(Console.ReadLine());

                //create Test Runner object
                TestRunner T1 = new TestRunner(1);

                switch (i)
                {
                    case 1:
                        T1.RunBenchmarks(i);
                        break;
                    case 2:
                        T1.RunBenchmarks(i);
                        break;
                    default:
                        break;
                }

                //write to console...
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
        }
    }
}
