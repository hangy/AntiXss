// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AntiXssAssemblyInfo.cs" company="Microsoft Corporation">
//   Copyright (c) 2008, 2009, 2010 All Rights Reserved, Microsoft Corporation
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
//   AntiXSS solution wide assembly settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyCopyright("Copyright © Microsoft Corporation 2009, 2010, 2011")]
[assembly: AssemblyProduct("Microsoft Anti-XSS Library v4.3")]
#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("")]
#endif

[assembly: AssemblyInformationalVersion("4.3")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion("4.3.0.0")]
[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguageAttribute("en")]
