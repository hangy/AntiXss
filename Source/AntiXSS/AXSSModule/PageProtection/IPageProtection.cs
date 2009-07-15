using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Microsoft.Security.Application.SecurityRuntimeEngine.Configuration;

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PageProtection
{
    /// <summary>
    /// Interface for XSS page protection class. This interface 
    /// can be implemented by the classes which provide page protection.
    /// </summary>
    internal interface IPageProtection
    {
        /// <summary>
        /// Method that protects the user of the page from malicious activity.
        /// </summary>
        /// <param name="page">Page that needs protection</param>
        /// <param name="config">Configuration file with all the parameters</param>
        void Protect(Page page,ModuleConfiguration config);
    }
}
