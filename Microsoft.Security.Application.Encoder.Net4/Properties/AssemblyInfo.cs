// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Microsoft Corporation">
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
//   Assembly settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle(".NET 4.0 ASP.NET Encoder for AntiXSS")]
[assembly: AssemblyDescription("Class to allow .NET 4.0 to use AntiXSS as its default encoder.")]
#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("")]
#endif
[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyProduct("AntiXss Library v4.0")]
[assembly: AssemblyCopyright("Copyright © Microsoft Corporation 2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("bee978e3-69cc-41ca-a49a-00051a231315")]

[assembly: AssemblyVersion("4.0.0.0")]
[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: AllowPartiallyTrustedCallers]
