// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityRuntimeInspection.cs" company="Microsoft Corporation">
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
//   Helper class containing the pipeline inspection methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.UI;

    using PlugIns;

    using Properties;

    /// <summary>
    /// Helper class containing the pipeline inspection methods.
    /// </summary>
    internal static class SecurityRuntimeInspection
    {
        /// <summary>
        /// The context key used to flow the suspect inspection result count through a request lifecycle.
        /// </summary>
        private const string SuspectCountContextIndex = "__SRE_suspectCount";

        /// <summary>
        /// The context key used to indicate the request has been stopped by an inspection result and no more inspections need to take place.
        /// </summary>
        private const string RequestStoppedIndex = "__SRE_requestStopped";

        /// <summary>
        /// The context key used to flow the content type to a filter.
        /// </summary>
        private const string RequestContentTypeIndex = "__SRE_contentType";

        /// <summary>
        /// Gets the current suspect inspection count for this request/response from the specified context.
        /// </summary>
        /// <param name="context">The context to retrieve from.</param>
        /// <returns>The current suspect inspection count for this request from the specified context.</returns>
        internal static int GetSuspectCountBeforeInspection(HttpContextBase context)
        {
            if (context.Items[SuspectCountContextIndex] == null)
            {
                return 0;
            }

            return (int)context.Items[SuspectCountContextIndex];
        }

        /// <summary>
        /// Sets the current suspect inspection count for this request/response into the specified context.
        /// </summary>
        /// <param name="context">The context in which to store the suspect count.</param>
        /// <param name="suspectCount">The suspect inspection count.</param>
        internal static void SetSuspectCountAfterInspection(HttpContextBase context, int suspectCount)
        {
            context.Items[SuspectCountContextIndex] = suspectCount;
        }

        /// <summary>
        /// Gets the content type of the response from the specified context.
        /// </summary>
        /// <param name="context">The context to retrieve from.</param>
        /// <returns>The content type of the response from the specified context.</returns>
        internal static string GetContentType(HttpContextBase context)
        {
            return context.Items[RequestContentTypeIndex] as string;
        }

        /// <summary>
        /// Sets the content type of the response into the specified context.
        /// </summary>
        /// <param name="context">The context to store into.</param>
        /// <param name="contentType">The content type of the response from the specified context.</param>
        internal static void SetContentType(HttpContextBase context, string contentType)
        {
            context.Items[RequestContentTypeIndex] = contentType;
        }

        /// <summary>
        /// Returns a value indicating if the request has been stopped by a previous event.
        /// </summary>
        /// <param name="context">The current http context.</param>
        /// <returns><c>true</c> if the request has been stopped, otherwise <c>false</c>.</returns>
        internal static bool IsRequestStopped(HttpContextBase context)
        {
            if (context.Items[RequestStoppedIndex] == null)
            {
                return false;
            }

            return (bool)context.Items[RequestStoppedIndex];
        }

        /// <summary>
        /// Marks the current request as stopped in the specified context and stops further event processing.
        /// </summary>
        /// <param name="reason">The result of the inspection that triggered the stop.</param>
        /// <param name="context">The current http context.</param>
        internal static void StopRequest(IInspectionResult reason, HttpContextBase context)
        {
            Logger.Log(LogLevel.Fatal, reason.StopReason);
            context.ApplicationInstance.CompleteRequest();
            context.Items[RequestStoppedIndex] = true;
        }

        /// <summary>
        /// Checks that a request is not excluded by global configuration.
        /// </summary>
        /// <param name="path">The request path to check</param>
        /// <returns><c>true</c> if the request path is excluded, otherwise false.</returns>
        internal static bool IsRequestPathGloballyExcluded(string path)
        {
            return IsRequestPathExcluded(path, SecurityRuntimeSettings.Settings.ExcludedPaths);
        }

        /// <summary>
        /// Checks that a request is not excluded by configuration.
        /// </summary>
        /// <param name="path">The request path to check</param>
        /// <param name="excludedPaths">The excluded path list to check.</param>
        /// <returns><c>true</c> if the request path is excluded, otherwise false.</returns>
        internal static bool IsRequestPathExcluded(string path, ExcludedPathCollection excludedPaths)
        {
            if (excludedPaths == null)
            {
                return false;
            }

            return excludedPaths.IndexOf(path) > -1;
        }

        /// <summary>
        /// Inspects the pipeline using the specified plug-ins.
        /// </summary>
        /// <param name="securityRuntimePlugIns">The collection of plug-ins to run.</param>
        /// <param name="conversionTarget">The conversion target to use when creating an inspector adapter.</param>
        /// <param name="request">The current request.</param>
        /// <param name="response">The current response, if any.</param>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="page">The current page, if any.</param>
        /// <param name="suspectMessage">The suspect message.</param>
        /// <param name="createException">A function to create a suitable exception for the pipeline stage.</param>
        /// <param name="eventAbortUnsupportedInCassini">If set to <c>true</c> indicates aborting in the current pipeline stage is unsupported in Cassini and a message should be displayed in the browser..</param>
        internal static void InspectPipeline(
            IEnumerable<ISecurityRuntimePlugIn> securityRuntimePlugIns,
            InspectorConversionTarget conversionTarget,
            HttpRequestBase request,
            HttpResponseBase response,
            HttpContextBase context,
            Page page,
            string suspectMessage,
            Func<string, Exception> createException,
            bool eventAbortUnsupportedInCassini)
        {
            // Make sure any previous inspectors haven't failed.
            if (IsRequestStopped(context))
            {
                return;
            }

            // Check for global path exclusions.
            if (IsRequestPathGloballyExcluded(request.Path))
            {
                return;
            }

            // Retrieve the number of suspect inspections that this request has had so far.
            int suspectRequestCount = GetSuspectCountBeforeInspection(context);

            // Loop through each plug-in, if the plug-in has not been excluded for that particular plug, 
            // wrap it in the correct adapter for this stage then inspect the pipeline.
            foreach (IInspectionResult result in from securityRuntimePlugIn in securityRuntimePlugIns
                                                 where securityRuntimePlugIn != null &&
                                                       !IsRequestPathExcluded(request.Path, securityRuntimePlugIn.ExcludedPaths)
                                                 select AdapterFactory.Convert(securityRuntimePlugIn, conversionTarget).Inspect(request, response, page))
            {
                switch (result.Severity)
                {
                    case InspectionResultSeverity.Halt:
                        StopRequest(result, context);
                        if (eventAbortUnsupportedInCassini)
                        {
                            NotifyCassiniUsers(request, response, result.StopReason);
                        }

                        throw createException(result.StopReason);
                    case InspectionResultSeverity.Suspect:
                        suspectRequestCount++;
                        break;
                    default:
                        break;
                }
            }

            // If we're over the maximum number of suspect results throw an exception and stop processing.
            if (SecurityRuntimeSettings.Settings.AllowedSuspectResults != -1 &&
                suspectRequestCount > SecurityRuntimeSettings.Settings.AllowedSuspectResults)
            {
                StopRequest(new TooManySuspectInspectionsResult(suspectMessage), context);
                if (eventAbortUnsupportedInCassini)
                {
                    NotifyCassiniUsers(request, response, suspectMessage);
                }

                throw new ResponseStoppedException(suspectMessage);
            }

            // And finally if we're still good, and we're keeping the suspect inspections count between stages then save it away.
            if (!SecurityRuntimeSettings.Settings.ResetSuspectCountBetweenStages)
            {
                SetSuspectCountAfterInspection(context, suspectRequestCount);
            }
        }

        /// <summary>
        /// Notifies users running under Cassini, Visual Studio's built in development web server an exception occurred.
        /// </summary>
        /// <param name="request">The current ASP.NET request.</param>
        /// <param name="response">The current ASP.NET response.</param>
        /// <param name="message">The notification message.</param>
        /// <remarks>Cassini doesn't mirror IIS behaviour in certain events - this method will clear the response and write its
        /// own warning to the response stream ensuring, at least, there is some semblance of response interruption as 
        /// expected.</remarks>
        [Conditional("DEBUG")]
        private static void NotifyCassiniUsers(HttpRequestBase request, HttpResponseBase response, string message)
        {
            if (request == null || response == null || !string.IsNullOrEmpty(request.ServerVariables["SERVER_SOFTWARE"]))
            {
                return;
            }

            response.Clear();
            response.Write(string.Format(CultureInfo.CurrentUICulture, Resources.CassiniWarning, message));
            response.End();
        }
    }
}
