// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockHttpRequest.cs" company="Microsoft Corporation">
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
//
// </copyright>
// <summary>
//   Implements the basics of an Http request for testing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns.UnitTests
{
    using System.Collections.Specialized;
    using System.Web;

    /// <summary>
    /// Implements the basics of an Http request for testing.
    /// </summary>
    public class MockHttpRequest : HttpRequestBase
    {
        /// <summary>
        /// Holds the request headers.
        /// </summary>
        private readonly NameValueCollection internalHeaders = new NameValueCollection();

        /// <summary>
        /// Holds the query string parameters.
        /// </summary>
        private readonly NameValueCollection queryStringParameters = new NameValueCollection();

        /// <summary>
        /// Holds the form variables.
        /// </summary>
        private readonly NameValueCollection formCollection = new NameValueCollection();

        /// <summary>
        /// Holds the request cook collection.
        /// </summary>
        private readonly HttpCookieCollection httpCookieCollection = new HttpCookieCollection();

        /// <summary>
        /// Gets the collection of HTTP headers that were sent by the client.
        /// </summary>
        /// <returns>The request headers.</returns>
        public override NameValueCollection Headers
        {
            get
            {
                return this.internalHeaders;
            }
        }

        /// <summary>
        /// Gets the collection of cookies that were sent by the client.
        /// </summary>
        /// <returns>The client's cookies.</returns>
        public override HttpCookieCollection Cookies
        {
            get
            {
                return this.httpCookieCollection;
            }
        }

        /// <summary>
        /// Gets the collection of HTTP query-string variables.
        /// </summary>
        /// <returns>The query-string variables that were sent by the client in the URL of the current request. </returns>
        public override NameValueCollection QueryString
        {
            get
            {
                return this.queryStringParameters;
            }
        }

        /// <summary>
        /// Gets the collection of form variables that were sent by the client.
        /// </summary>
        /// <returns>The form variables.</returns>
        public override NameValueCollection Form
        {
            get
            {
                return this.formCollection;
            }
        }
    }
}
