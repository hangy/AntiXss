// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockHttpResponse.cs" company="Microsoft Corporation">
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
//   A mock HttpResponse.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns.UnitTests
{
    using System.Collections.Specialized;
    using System.Web;

    /// <summary>
    /// A mock HttpResponse which supports headers.
    /// </summary>
    public class MockHttpResponse : HttpResponseBase
    {
        /// <summary>
        /// Header collection
        /// </summary>
        private readonly NameValueCollection headerCollection = new NameValueCollection();

        /// <summary>
        /// Http Cookie collection
        /// </summary>
        private readonly HttpCookieCollection cookieCollection = new HttpCookieCollection();

        /// <summary>
        /// Gets the collection of response headers.
        /// </summary>
        /// <returns>The response headers.</returns>
        public override NameValueCollection Headers
        {
            get
            {
                return this.headerCollection;
            }
        }

        /// <summary>
        /// Gets the response cookie collection.
        /// </summary>
        /// <returns>The response cookie collection.</returns>
        public override HttpCookieCollection Cookies
        {
            get
            {
                return this.cookieCollection;
            }
        }

        /// <summary>
        /// When overridden in a derived class, adds an HTTP header to the current response. This method is provided for compatibility with earlier versions of ASP.
        /// </summary>
        /// <param name="name">The name of the HTTP header to add <paramref name="value"/> to.</param>
        /// <param name="value">The string to add to the header.</param>
        /// <exception cref="T:System.NotImplementedException">Always.</exception>
        public override void AddHeader(string name, string value)
        {
            this.Headers.Add(name, value);
        }

        /// <summary>
        /// When overridden in a derived class, adds an HTTP header to the current response.
        /// </summary>
        /// <param name="name">The name of the HTTP header to add to the current response.</param>
        /// <param name="value">The value of the header.</param>
        /// <exception cref="T:System.NotImplementedException">Always.</exception>
        public override void AppendHeader(string name, string value)
        {
            this.Headers.Add(name, value);
        }

        /// <summary>
        /// When overridden in a derived class, adds an HTTP cookie to the current response.
        /// </summary>
        /// <param name="name">The name of the HTTP cookie to add to the current response.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <exception cref="T:System.NotImplementedException">Always.</exception>
        public void AppendCookie(string name, string value)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = value;
            this.AppendCookie(cookie);
        }

        /// <summary>
        /// When overridden in a derived class, adds an HTTP cookie to the HTTP response cookie collection.
        /// </summary>
        /// <param name="cookie">The cookie to add to the response.</param>
        /// <exception cref="T:System.NotImplementedException">Always.</exception>
        public override void AppendCookie(HttpCookie cookie)
        {
            this.Cookies.Add(cookie);
        }
    }
}
