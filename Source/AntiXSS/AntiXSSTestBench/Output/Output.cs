using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Microsoft.Security.Application.AntiXSSTestBench
{
    class Output
    {
        public static void WriteLine(string text)
        {
            Console.WriteLine(text);
            string FilePath = "Output.txt";
            FileStream fStream = new FileStream(FilePath, FileMode.Append, FileAccess.Write);
            BufferedStream bfs = new BufferedStream(fStream);
            StreamWriter sWriter = new StreamWriter(bfs);
            sWriter.WriteLine(text);
            sWriter.Close();
        }

    }
}
