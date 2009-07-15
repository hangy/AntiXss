using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using System.IO;

namespace Microsoft.Security.Application.AntiXss
{
    [RunInstaller(true)]
    public class NGenCustomAction:Installer
    {
        string ngenPath = @"%WINDIR%\Microsoft.NET\Framework\v2.0.50727\ngen.exe";
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            if (Context.Parameters.ContainsKey("NGENDLL"))
            { 
                if (!File.Exists(Environment.ExpandEnvironmentVariables(ngenPath)))
                    throw new FileNotFoundException(".NET Framework 2.0 directory does not contain ngen utility");

                ProcessStartInfo psInfo = new ProcessStartInfo(Environment.ExpandEnvironmentVariables(ngenPath));
                psInfo.Arguments = "install \"" + Context.Parameters["NGENDLL"] + "\"";
                psInfo.CreateNoWindow = true;
                psInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(psInfo);
            }
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            if (Context.Parameters.ContainsKey("NGENDLL"))
            {
                if (!File.Exists(Environment.ExpandEnvironmentVariables(ngenPath)))
                    throw new FileNotFoundException(".NET Framework 2.0 directory does not contain ngen utility");

                ProcessStartInfo psInfo = new ProcessStartInfo(Environment.ExpandEnvironmentVariables(ngenPath));
                psInfo.Arguments = "uninstall \"" + Context.Parameters["NGENDLL"] + "\"";
                psInfo.CreateNoWindow = true;
                psInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(psInfo);
            }
        }


    }
}
