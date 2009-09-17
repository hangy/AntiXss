using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Reflection;
using Microsoft.Security.Application.SecurityRuntimeEngine.Configuration;
using Microsoft.Security.Application.SecurityRuntimeEngine.PageProtection;
using System.Diagnostics;

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{

    /// <summary>
    /// This class is responsible for intercepting page requests 
    /// and protects the page from specific attacks. It also reads 
    /// the configuration file in the Init method and stores it in 
    /// the Application variable for reuse, configuration file is 
    /// only loaded once. Implements IHttpModule interface and hooks 
    /// to the Page.PreRender event to protect the page from different 
    /// attacks. During the Page_PreRender event, the XssProtection 
    /// class is called to protect the page.
    /// </summary>
    public class AntiXssModule:IHttpModule
    {

        private HttpApplication objApp = null;
        private ModuleConfiguration objConfig = null;
        
        #region IHttpModule Members
        
        /// <summary>
        /// Dereferences the variables.
        /// </summary>
        public void Dispose()
        {
            objApp = null;
            objConfig = null;
        }

        /// <summary>
        /// Initializes the module by loading the configuration 
        /// and adding the event handler for PostMapRequestHandler. 
        /// </summary>
        /// <param name="context">Context of the current request</param>
        public void Init(HttpApplication context)
        {
            
            this.LoadConfig(context, AppDomain.CurrentDomain.BaseDirectory + "antixssmodule.config");
            if (objConfig != null)
            {
                objApp = context;
                objApp.PostMapRequestHandler += new EventHandler(objApp_PostMapRequestHandler);
            }
        }

        /// <summary>
        /// This event is fired when a handler is loaded by ASP.Net, 
        /// a Page class is a handler in itself which is then checked 
        /// for suppressions and exclusions. It also adds an event 
        /// handler for PreRender event.
        /// </summary>
        /// <param name="sender">Page that is being handled.</param>
        /// <param name="e">Arugments for the event.</param>
        void objApp_PostMapRequestHandler(object sender, EventArgs e)
        {
            IHttpHandler pageHandler =null;
            if (objApp == null)
                return;
            if (objApp.Context.Handler is System.Web.UI.Page)
                pageHandler = objApp.Context.Handler;

            if (pageHandler != null)
            {
                if (objConfig == null)
                    return;

                if (objConfig != null)
                {
                    string strVirPath = objApp.Context.Request.FilePath;
                    if (objConfig.IsPageExcluded(strVirPath.ToLower()))
                    {
                        return;
                    }
                }
                object[] attributes = ((Page)pageHandler).GetType().GetCustomAttributes(typeof(SupressAntiXssEncodingAttribute), true);
                if (attributes.Length > 0)
                    return;
                Page page = (Page)pageHandler;
                XssProtection.Protect(page, objConfig);
            }

        }

        
        /// <summary>
        /// Loads the configuration file it one does not exist in the application variable.
        /// </summary>
        /// <param name="context">Context of the page.</param>
        /// <param name="configFile">Location of the file where configuration is stored.</param>
        private void LoadConfig(HttpApplication context,string configFile) 
        {
            try
            {
                if (context != null)
                {
                    if (context.Application != null && context.Application.Count > 0 && context.Application["AntiXssModuleConfig"] != null)
                    {
                        try
                        {
                            objConfig = (ModuleConfiguration)context.Application["AntiXssModuleConfig"];
                        }
                        catch { }
                    }

                    if (objConfig == null)
                    {
                        objConfig = ModuleConfiguration.LoadConfiguration(configFile);
                        context.Application["AntiXssModuleConfig"] = objConfig;
                    }
                }
            }
            catch { }
        }

        #endregion
    }
}
