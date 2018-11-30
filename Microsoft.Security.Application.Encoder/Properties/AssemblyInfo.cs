// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Microsoft Corporation">
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
//
// </copyright>
// <summary>
//   Assembly settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("AntiXss Library for .NET 4.0")]
[assembly: AssemblyDescription("Encoding classes for safe-listing encoding of HTML, XML and other output types.")]

[assembly: Guid("9C2BF6C0-0B3B-4401-8B50-12A16CFD4730")]

[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityTransparent]

[assembly: InternalsVisibleTo("AntiXSSUnitTests")]

#if DEBUG
// Needed for code coverage
[assembly: SecurityRules(SecurityRuleSet.Level1, SkipVerificationInFullTrust = true)]
#endif

