// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityRuntimeEngine.cs" company="Microsoft Corporation">
//   Copyright (c) 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   A collection of excluded paths sourced from configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI;

    using PlugIns;
    using Properties;

    /// <summary>
    /// The central <see cref="IHttpModule"/> for the Security Runtime Engine.
    /// This module loads any discovered SRE plug-ins and hooks them into the appropriate parts of the ASP.NET pipeline for their function.
    /// </summary>
    internal sealed class SecurityRuntimeEngine : IHttpModule
    {
        /// <summary>
        /// Gets or sets the response inspectors.
        /// </summary>
        /// <value>The discovered response inspectors.</value>
        /// <remarks>This is initially populated via MEF in the <see cref="SecurityRuntimeEngine.Compose"/> process.</remarks>
        [ImportMany(typeof(IResponseHeaderInspector))]
        internal IEnumerable<ISecurityRuntimePlugIn> ResponseHeaderInspectors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the request inspectors.
        /// </summary>
        /// <value>The request inspectors.</value>
        /// <remarks>This is initially populated via MEF in the <see cref="SecurityRuntimeEngine.Compose"/> process.</remarks>
        [ImportMany(typeof(IRequestInspector))]
        internal IEnumerable<ISecurityRuntimePlugIn> RequestInspectors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the page inspectors.
        /// </summary>
        /// <value>The page inspectors.</value>
        /// <remarks>This is initially populated via MEF in the <see cref="SecurityRuntimeEngine.Compose"/> process.</remarks>
        [ImportMany(typeof(IPageInspector))]
        internal IEnumerable<ISecurityRuntimePlugIn> PageInspectors
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the response inspectors.
        /// </summary>
        /// <value>The response inspectors.</value>
        /// <remarks>
        /// <para>This is initially populated via MEF in the <see cref="SecurityRuntimeEngine.Compose"/> process.</para>
        /// <para>As response inspectors work as stream filters </para>
        /// </remarks>
        [ImportMany(typeof(IResponseInspector))]
        internal IEnumerable<IResponseInspector> ResponseInspectors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the discovered loggers.
        /// </summary>
        /// <value>The discovered loggers.</value>
        /// <remarks>This is populated via MEF in the <see cref="SecurityRuntimeEngine.Compose"/> process.</remarks>
        [ImportMany(typeof(ILogger))]
        internal IEnumerable<ILogger> DiscoveredLoggers
        {
            get;
            set;
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
            // clean-up code here.
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, 
        /// and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            // Basic sanity check.
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // Get the plug-in directory if specified, otherwise use wplPlugins under the current application bin directory
            string plugInDirectory = SecurityRuntimeSettings.Settings.PlugInDirectory;
            if (String.IsNullOrEmpty(plugInDirectory))
            {
                plugInDirectory = Path.Combine(HttpRuntime.BinDirectory, "wplPlugins");
            }
            else if (!Path.IsPathRooted(plugInDirectory))
            {
                plugInDirectory = Path.Combine(HttpRuntime.BinDirectory, plugInDirectory);                
            }

            // If the directory doesn't exist, well there's no point in even attempting to load any plug-ins.
            if (Directory.Exists(plugInDirectory))
            {
                this.Compose(plugInDirectory);
                this.PreparePlugins(SecurityRuntimeSettings.Settings.DisabledPlugIns);
            }

            // Map the appropriate events if there are plug-ins which need them.
            if (this.RequestInspectors != null && this.RequestInspectors.Count() > 0)
            {
                Logger.Log(LogLevel.Informational, Resources.RequestInspectorsFound, this.RequestInspectors.Count());
                context.BeginRequest += this.OnBeginRequest;
            }

            if (this.ResponseInspectors != null && this.ResponseInspectors.Count() > 0)
            {
                Logger.Log(LogLevel.Informational, Resources.ResponseInspectorsFound, this.ResponseInspectors.Count());
            }

            if (this.PageInspectors != null && this.PageInspectors.Count() > 0)
            {
                Logger.Log(LogLevel.Informational, Resources.PageInspectorsFound, this.PageInspectors.Count());
            }

            if ((this.ResponseInspectors != null && this.ResponseInspectors.Count() > 0) ||
                (this.PageInspectors != null && this.PageInspectors.Count() > 0))
            {
                context.PostMapRequestHandler += this.OnPostMapRequestHander;
            }

            if (this.ResponseHeaderInspectors != null && this.ResponseHeaderInspectors.Count() > 0)
            {
                Logger.Log(LogLevel.Informational, Resources.ResponseHeaderInspectorsFound, this.ResponseHeaderInspectors.Count());
                context.PreSendRequestHeaders += this.OnPreSendRequestHeaders;
            }
        }

        /// <summary>
        /// Checks the <paramref name="disabledPlugIns"/> to see if the type of <paramref name="potentialPlugIn"/> is listed.
        /// </summary>
        /// <param name="disabledPlugIns">The <see cref="DisabledPlugIn"/>s to search for.</param>
        /// <param name="potentialPlugIn">A potential plug-in.</param>
        /// <returns><c>true</c> if the plug-in is disabled, otherwise <c>false</c>.</returns>
        internal static bool IsPluginDisabled(IEnumerable<DisabledPlugIn> disabledPlugIns, object potentialPlugIn)
        {
            if (disabledPlugIns.Count() == 0)
            {
                return false;
            }

            // HACK: This needs to be better thought out!
            string potentialPlugInType = potentialPlugIn.GetType().AssemblyQualifiedName;

            if (potentialPlugInType == null)
            {
                return false;
            }

            for (int i = 0; i < disabledPlugIns.Count(); i++)
            {
                string disabledPlugInType = disabledPlugIns.ElementAt(i).PlugInType;

                if (potentialPlugInType.IndexOf(disabledPlugInType, StringComparison.Ordinal) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Configures and returns a list of non-disabled plug-ins.
        /// </summary>
        /// <typeparam name="T">The type of the plug-ins.</typeparam>
        /// <param name="unconfigured">The un-configured plug-ins.</param>
        /// <param name="disabledPlugIns">The disabled plug-ins.</param>
        /// <returns>A list of non-disabled plug-ins.</returns>
        internal static T[] GetActiveConfiguredInspectors<T>(IEnumerable<T> unconfigured, IEnumerable<DisabledPlugIn> disabledPlugIns)
        {
            if (unconfigured == null || unconfigured.Count() == 0)
            {
                return null;
            }

            T[] inspectors = unconfigured.Where(plugIn => !IsPluginDisabled(disabledPlugIns, plugIn)).ToArray();
            Array.ForEach(inspectors, plugIn => ConfigurePlugIn(plugIn));

            return inspectors;
        }

        /// <summary>
        /// Configures the specified plug-in if it implements the <see cref="IConfigurablePlugIn"/> interface.
        /// </summary>
        /// <param name="plugIn">The plug-in to configure.</param>
        internal static void ConfigurePlugIn(object plugIn)
        {
            IConfigurablePlugIn configurableModule = plugIn as IConfigurablePlugIn;
            if (configurableModule == null)
            {
                return;
            }

            BasePlugInConfiguration configurationSection =
                ConfigurationManager.GetSection(configurableModule.ConfigurationSectionName) as
                BasePlugInConfiguration;
            if (configurationSection != null)
            {
                configurableModule.Settings = configurationSection;
            }
        }

        /// <summary>
        /// Initialises plug-ins as necessary, removing disabled ones and configuring any which implement <see cref="IConfigurablePlugIn"/>.
        /// </summary>
        /// <param name="disabledPlugIns">The disabled plug-ins list.</param>
        internal void PreparePlugins(IEnumerable<DisabledPlugIn> disabledPlugIns)
        {
            this.RequestInspectors = GetActiveConfiguredInspectors(this.RequestInspectors, disabledPlugIns);
            this.ResponseHeaderInspectors = GetActiveConfiguredInspectors(this.ResponseHeaderInspectors, disabledPlugIns);
            this.PageInspectors = GetActiveConfiguredInspectors(this.PageInspectors, disabledPlugIns);
            this.ResponseInspectors = GetActiveConfiguredInspectors(this.ResponseInspectors, disabledPlugIns);
        }

        /// <summary>
        /// Loads any available plug-ins.
        /// </summary>
        /// <param name="plugInDirectory">The plug-in directory to load from.</param>
        private void Compose(string plugInDirectory)
        {
            // And let MEF do its magic.
            using (DirectoryCatalog plugInCatalog = new DirectoryCatalog(plugInDirectory))
            using (CompositionContainer container = new CompositionContainer(plugInCatalog))
            {
                container.ComposeParts(this);
            }

            // Pass off the loggers to the static Log class, then throw our copy away.
            Logger.ConfigureLoggers(this.DiscoveredLoggers);
            this.DiscoveredLoggers = null;
        }

        /// <summary>
        /// Occurs as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>The BeginRequest event signals the creation of any given new request. At this point any configured <see cref="IRequestInspector"/>s are ran.</remarks>
        private void OnBeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContextBase context = new HttpContextWrapper(application.Context);

            if (context.Request == null)
            {
                return;
            }

            HttpRequestBase request = context.Request;

            SecurityRuntimeInspection.InspectPipeline(
                this.RequestInspectors,
                InspectorConversionTarget.RequestInspector,
                request,
                null,
                context,
                null,
                Resources.RequestStoppedMessage,
                x => new RequestStoppedException(x),
                false);
        }

        /// <summary>
        /// Occurs just before ASP.NET sends HTTP headers to the client.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>At this point the <see cref="IResponseHeaderInspector.Inspect"/> method of any configured <see cref="IResponseHeaderInspector"/>s are ran.</remarks>
        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContextBase context = new HttpContextWrapper(application.Context);

            if (context.Request == null)
            {
                return;
            }

            HttpRequestBase request = context.Request;

            if (context.Response == null)
            {
                return;
            }

            HttpResponseBase response = context.Response;

            SecurityRuntimeInspection.InspectPipeline(
                this.ResponseHeaderInspectors,
                InspectorConversionTarget.ResponseHeaderInspector,
                request,
                response,
                context,
                null,
                Resources.ResponseHeaderInspectionStoppedMessage,
                x => new ResponseStoppedException(x),
                true);
        }

        /// <summary>
        /// Occurs when ASP.NET has mapped the current request to the appropriate <see cref="IHttpHandler"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Once a handler has been assigned the type of the request can be determined. At this point, if the request is for
        /// an ASP.NET WebForms page any configured <see cref="IPageInspector"/>s are ran.</remarks>
        private void OnPostMapRequestHander(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;

            if (context == null)
            {
                return;
            }

            Page page = context.Handler as Page;
            if (page == null)
            {
                return;
            }

            if (this.PageInspectors != null && this.PageInspectors.Count() > 0)
            {
                page.PreRender += this.OnPagePreRender;
            }

            if (this.ResponseInspectors != null && this.ResponseInspectors.Count() > 0)
            {
                page.Init += this.OnPageInit;
                page.PreRenderComplete += this.OnPagePreRenderComplete;
            }
        }

        /// <summary>
        /// Occurs when the server control is initialized, which is the first step in its lifecycle.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnPageInit(object sender, EventArgs e)
        {
            Page page = sender as Page;

            if (page == null)
            {
                return;
            }

            HttpContextBase context = new HttpContextWrapper(HttpContext.Current);

            // Make sure any previous inspectors haven't failed.
            if (SecurityRuntimeInspection.IsRequestStopped(context))
            {
                return;
            }

            // Check for global path exclusions.
            if (SecurityRuntimeInspection.IsRequestPathGloballyExcluded(page.Request.Path))
            {
                return;
            }

            // Exclude any response inspectors that have been suppressed by attribute based suppression.
            IEnumerable<IResponseInspector> nonSuppressedResponseInspectors =
                from responseInspector in this.ResponseInspectors
                where !AttributeExclusionChecker.IsPlugInExcludedForType(page.GetType(), responseInspector.GetType())
                select responseInspector;

            if (nonSuppressedResponseInspectors.Count() <= 0)
            {
                return;
            }

            Stream responseSource = page.Response.Filter;
            page.Response.Filter = new ResponseInspectorFilter(nonSuppressedResponseInspectors, responseSource, context);
        }

        /// <summary>
        /// Occurs when the server control is unloaded from memory.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnPagePreRenderComplete(object sender, EventArgs e)
        {
            Page page = sender as Page;

            if (page == null)
            {
                return;
            }

            HttpContextBase context = new HttpContextWrapper(HttpContext.Current);

            // Make sure any previous inspectors haven't failed.
            if (SecurityRuntimeInspection.IsRequestStopped(context))
            {
                return;
            }

            // Check for global path exclusions.
            if (SecurityRuntimeInspection.IsRequestPathGloballyExcluded(page.Request.Path))
            {
                return;
            }

            // Exclude any response inspectors that have been suppressed by attribute based suppression.
            IEnumerable<IResponseInspector> nonSuppressedResponseInspectors =
                from responseInspector in this.ResponseInspectors
                where !AttributeExclusionChecker.IsPlugInExcludedForType(page.GetType(), responseInspector.GetType())
                select responseInspector;

            if (nonSuppressedResponseInspectors.Count() <= 0)
            {
                return;
            }

            SecurityRuntimeInspection.SetContentType(context, context.Response.ContentType);
        }

        /// <summary>
        /// Occurs when the server control is about to render to its containing <see cref="Page"/> control. 
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnPagePreRender(object sender, EventArgs e)
        {
            Page page = sender as Page;

            if (page == null)
            {
                return;
            }
            
            // Exclude any page inspectors that have been suppressed by attribute based suppression.
            IEnumerable<ISecurityRuntimePlugIn> nonSuppressedPageInspectors =
                from pageInspector in this.PageInspectors
                where !AttributeExclusionChecker.IsPlugInExcludedForType(page.GetType(), pageInspector.GetType())
                select pageInspector;                

            HttpContextBase context = new HttpContextWrapper(HttpContext.Current);

            SecurityRuntimeInspection.InspectPipeline(
                nonSuppressedPageInspectors,
                InspectorConversionTarget.PageInspector,
                context.Request,
                context.Response,
                context,
                page,
                Resources.PageInspectionStoppedMessage,
                x => new PageProcessingStoppedException(x),
                false);
        }
    }
}
