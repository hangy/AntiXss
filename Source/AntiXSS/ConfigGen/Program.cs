using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Security.Application.SecurityRuntimeEngine.ConfigurationGenerator
{
    class Program
    {
        [STAThread()]
        public static void Main() 
        {
            frmMain main = new frmMain();
            System.Windows.Forms.Application.Run(main);
        }
    }
}
