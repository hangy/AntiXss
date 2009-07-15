using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml.XPath;

namespace Microsoft.Security.Application.SecurityRuntimeEngine.ConfigurationGenerator
{
    /// <summary>
    /// This internal class is used for analyzing an assembly to identify 
    /// ASP.NET controls which need encoding. This constructor of the 
    /// class calls LoadConfig() method to load the configuration from the 
    /// master configuration file “EncodingControls.xml”.  
    /// </summary>
    internal class AssemblyAnalysis
    {

        Dictionary<string, bool> lstControlTypes = new Dictionary<string,bool>();
        string currentAssemblyFile = string.Empty;

        public AssemblyAnalysis() { this.LoadConfig();}

        private void LoadConfig() 
        {
            XPathDocument xDoc = new XPathDocument(AppDomain.CurrentDomain.BaseDirectory + "EncodingControls.xml");
            XPathNodeIterator xNodes = xDoc.CreateNavigator().Select("/Configuration/ControlEncodingContexts/ControlEncodingContext");
            while (xNodes.MoveNext()) 
            {
                if (!lstControlTypes.ContainsKey(xNodes.Current.GetAttribute("FullClassName","")))
                    lstControlTypes.Add(xNodes.Current.GetAttribute("FullClassName",""),false);
            }
        }

        public class AnalysisEventArgs : EventArgs 
        {
            ControlDetail cd;
            public AnalysisEventArgs(ControlDetail controlDetail) { cd = controlDetail; }
            public ControlDetail ControlDetail
            {
                get { return cd; }
            }
        }

        public event EventHandler<AnalysisEventArgs> ControlFound;

        private void OnControlFound(ControlDetail controlDetail) 
        {
            if (this.ControlFound != null)
                this.ControlFound(this, new AnalysisEventArgs(controlDetail));
        }

        public void Analyze(string assemblyFileName) 
        {
            try
            {
                currentAssemblyFile = assemblyFileName;
                List<ControlDetail> lstControls = new List<ControlDetail>();
                if (File.Exists(assemblyFileName))
                {
                    AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(CurrentDomain_ReflectionOnlyAssemblyResolve);
                    Assembly oAssembly = Assembly.ReflectionOnlyLoadFrom(assemblyFileName);
                    foreach (Type t in oAssembly.GetExportedTypes())
                    {
                        if (t.BaseType != null && t.BaseType.ToString() == "System.Web.UI.Page")
                        {
                            foreach (FieldInfo f in t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                            {
                                if (lstControlTypes.ContainsKey(f.FieldType.ToString()))
                                {
                                    
                                    ControlDetail cd = new ControlDetail();
                                    cd.controlName = f.Name;
                                    cd.controlType = f.FieldType.ToString();
                                    cd.pageName = t.Name;
                                    this.OnControlFound(cd);
                                }
                            }
                        }
                    }
                }
                else
                    throw new FileNotFoundException(assemblyFileName);
            }
            catch
            { throw;  }
        }

        Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                foreach (Assembly a in AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies())
                {
                    if (a.FullName == args.Name)
                        return a;
                }
                if (Directory.Exists(Path.GetDirectoryName(currentAssemblyFile)) && File.Exists(Path.Combine(Path.GetDirectoryName(currentAssemblyFile), args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll")))
                    return Assembly.ReflectionOnlyLoadFrom(Path.Combine(Path.GetDirectoryName(currentAssemblyFile), args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll"));
                else
                    Assembly.ReflectionOnlyLoad(args.Name);
            }
            
            catch { throw; }
            return null;
        }

    }

    internal struct ControlDetail
    {
        public string controlName;
        public string controlType;
        public string pageName;
    }
}
