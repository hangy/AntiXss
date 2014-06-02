// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeChartHelperTests.cs" company="Microsoft Corporation">
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
//   Tests the range helpers
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.Tests 
{
    using System.Linq;

    using Microsoft.Security.Application.CodeCharts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the range helpers.
    /// </summary>
    [TestClass]    
    public class CodeChartHelperTests 
    {
        /// <summary>
        /// Tests GetRange() returns the correct range with no exclusions.
        /// </summary>
        [TestMethod]
        public void GetRange()
        {
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, CodeChartHelper.GetRange(1, 4).ToList());
        }

        /// <summary>
        /// Tests GetRange() returns the correct range and excludes the specified numbers.
        /// </summary>
        [TestMethod]
        public void GetRangeWithExclusion()
        {
            CollectionAssert.AreEqual(new[] { 1, 2, 5 }, CodeChartHelper.GetRange(1, 5, i => i == 3 || i == 4).ToList());
        }
    }
}
