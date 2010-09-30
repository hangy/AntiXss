// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeExclusionCheckerTests.cs" company="Microsoft Corporation">
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
//   Tests to check the AttributeExclusionChecker class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// Contains the unit tests for the attribute exclusion utility class.
    /// </summary>
    [TestClass]
    public class AttributeExclusionCheckerTests
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        /// <value>The test context.</value>
        public TestContext TestContext
        {
            get;
            set;
        }

        /// <summary>
        /// Tests that passing a null page type will throw the correct exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PassingNullPageTypeToIsProcessorExcludedShouldThrowArgumentNullException()
        {
            Type type = this.GetType();

            AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(null, type);
        }

        /// <summary>
        /// Tests that passing a null plug-in type will throw the correct exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PassingNullPlugInTypeToIsProcessorExcludedShouldThrowArgumentNullException()
        {
            Type type = this.GetType();

            AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(type, null);
        }

        /// <summary>
        /// Tests that passing in both a null page and plug-in will throw the correct exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PassingNullPageTypeAndNullPlugInToIsProcessorExcludedShouldThrowArgumentNullException()
        {
            // Act
            AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(null, null);
        }

        /// <summary>
        /// Tests that passing in a plug-in and a page which has no suppressions returns that the plug-in is not excluded.
        /// </summary>
        [TestMethod]
        public void PassingAnUnsupressedPlugInAndAPageWithNoExclusionsReturnsFalse()
        {
            Type page = typeof(TestPageWithNoAttributes);
            Type plugIn = typeof(TestNotSuppressedPlugIn);

            bool actual = AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(page, plugIn);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Tests that passing in a plug-in and a page which has suppressions for other plug-ins returns that the plug-in is not excluded.
        /// </summary>
        [TestMethod]
        public void PassingAnUnsuppressedPlugInAndAPageWithExclusionForOtherPlugInsReturnsFalse()
        {
            Type page = typeof(TestPageWithSingleExclusion);
            Type plugIn = typeof(TestNotSuppressedPlugIn);

            bool actual = AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(page, plugIn);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Tests that passing in a plug-in and a page which has suppressed all plug-ins returns that the plug-in is excluded.
        /// </summary>
        [TestMethod]
        public void PassingAnUnsuppressedPlugInAndAPageWithGlobalExclusionReturnsTrue()
        {
            Type page = typeof(TestPageWithGlobalExclusion);
            Type plugIn = typeof(TestNotSuppressedPlugIn);

            bool actual = AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(page, plugIn);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests that passing in a plug-in that has been suppressed by another page and a page which has no exclusions returns false.
        /// </summary>
        [TestMethod]
        public void PassingASuppressedPlugInAndAPageWithNoExclusionsReturnsFalse()
        {
            Type page = typeof(TestPageWithNoAttributes);
            Type plugIn = typeof(TestSuppressedPlugIn);

            bool actual = AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(page, plugIn);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Tests that passing in a plug-in that has been suppressed by another page and a page which excludes it returns true.
        /// </summary>
        [TestMethod]
        public void PassingASuppressedPlugInAndAPageWithExclusionForThatPlugInsReturnsTrue()
        {
            Type page = typeof(TestPageWithSingleExclusion);
            Type plugIn = typeof(TestSuppressedPlugIn);

            bool actual = AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(page, plugIn);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests that passing in a plug-in that has been suppressed by another page and a page which suppresses globally returns true.
        /// </summary>
        [TestMethod]
        public void PassingASuppressedPlugInAndAPageWithGlobalExclusionReturnsTrue()
        {
            Type page = typeof(TestPageWithGlobalExclusion);
            Type plugIn = typeof(TestNotSuppressedPlugIn);

            bool actual = AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(page, plugIn);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests that a page with multiple suppresses returns the right results
        /// </summary>
        [TestMethod]
        public void PassingASuppressedPluginAndAPageWithMultipleExclusionsIncludingThePassedPluginReturnsTrue()
        {
            Type page = typeof(TestPageWithDoubleExclusion);
            Type firstPlugin = typeof(TestSuppressedPlugIn);
            Type secondPlugin = typeof(AnotherSuppressedPlugIn);
            Type neverSuppressed = typeof(TestNotSuppressedPlugIn);

            bool firstPluginIsExcluded = AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(page, firstPlugin);
            bool secondPluginIsExcluded = AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(page, secondPlugin);
            bool notExcludedPluginIsExcluded = AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(page, neverSuppressed);

            Assert.IsTrue(firstPluginIsExcluded);
            Assert.IsTrue(secondPluginIsExcluded);
            Assert.IsFalse(notExcludedPluginIsExcluded);
        }

        /// <summary>
        /// Tests that a page that has never been checked is not in the internal cache.
        /// </summary>
        [TestMethod]
        public void CheckInternalCachingDoesNotContainSomethingNeverChecked()
        {
            Type type = typeof(TestPageForInternalCachingNeverChecked);

            Assert.IsFalse(AttributeExclusionChecker_Accessor.HasTypeLevelCheckingTakenPlace(type));
        }

        /// <summary>
        /// Tests that a page that has been checked is in the internal cache.
        /// </summary>
        [TestMethod]
        public void CheckInternalCachingContainsResultsAfterATypeIsChecked()
        {
            Type type = typeof(TestPageForInternalCaching);
            Type plugIn = typeof(TestNotSuppressedPlugIn);

            Assert.IsFalse(AttributeExclusionChecker_Accessor.HasTypeLevelCheckingTakenPlace(type));

            AttributeExclusionChecker_Accessor.IsPlugInExcludedForType(type, plugIn);

            Assert.IsTrue(AttributeExclusionChecker_Accessor.HasTypeLevelCheckingTakenPlace(type));        
        }

        /// <summary>
        /// Tests that passing a null page type will throw the correct exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PassingNullPageTypeToGetExcludedUniqueIdsShouldThrowArgumentNullException()
        {
            Type type = this.GetType();

            AttributeExclusionChecker_Accessor.GetExcludedControlUniqueIdsForContainer(null, type);
        }

        /// <summary>
        /// Tests that passing a null plug-in type will throw the correct exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PassingNullPlugInTypeToGetExcludedUniqueIdsShouldThrowArgumentNullException()
        {
            using (Page page = new Page())
            {
                AttributeExclusionChecker_Accessor.GetExcludedControlUniqueIdsForContainer(page, null);
            }
        }

        /// <summary>
        /// Tests that passing in both a null page and plug-in will throw the correct exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PassingNullPageTypeAndNullPlugInToGetExcludedUniqueIdsShouldThrowArgumentNullException()
        {
            AttributeExclusionChecker_Accessor.GetExcludedControlUniqueIdsForContainer(null, null);
        }

        /// <summary>
        /// Tests that passing in a page with no controls will return an empty IEnumerable.
        /// </summary>
        [TestMethod]
        public void PassingAPageWithNoControlsToGetExcludedUniqueIdsWillReturnAnEmptyEnumerable()
        {
            using (Page page = new Page())
            {
                List<string> results =
                    new List<string>(AttributeExclusionChecker_Accessor.GetExcludedControlUniqueIdsForContainer(page, typeof(TestNotSuppressedPlugIn)));

                Assert.AreEqual(0, results.Count);
            }
        }

        /// <summary>
        /// Tests that passing in a page which has a control which has no attributes at all will return an empty IEnumerable.
        /// </summary>
        [TestMethod]
        public void PassingAPageWithANonExludedControlToGetExcludedUniqueIdsWillReturnAnEmptyEnumerable()
        {
            using (Page page = new PageWithAControl())
            {
                List<string> results =
                    new List<string>(AttributeExclusionChecker_Accessor.GetExcludedControlUniqueIdsForContainer(page, typeof(TestNotSuppressedPlugIn)));

                Assert.AreEqual(0, results.Count);
            }
        }

        /// <summary>
        /// Tests that passing in a page which has an excluded control which has no attributes at all will return the control id.
        /// </summary>
        [TestMethod]
        public void PassingAPageWithAnExludedControlToGetExcludedUniqueIdsWillReturnAnIEnumerableContainingTheUniqueIdOfTheControl()
        {
            const string ControlIdentifier = "uniqueId";
            using (Page page = new PageWithAGloballySupressedControl(ControlIdentifier))
            {
                List<string> results =
                    new List<string>(AttributeExclusionChecker_Accessor.GetExcludedControlUniqueIdsForContainer(page, typeof(TestNotSuppressedPlugIn)));

                Assert.AreEqual(1, results.Count);
                Assert.IsTrue(results.Contains(ControlIdentifier));
            }
        }

        /// <summary>
        /// Tests that an excluded control name is returned and an included control name is not returned when getting the excluded control IDs.
        /// </summary>
        [TestMethod]
        public void PassingAPageWithAnExludedControlAndAnUnexcludedControlToGetExcludedUniqueIdsWillReturnAnIEnumerableContainingTheUniqueIdOfTheExcludedControl()
        {
            const string SuppressedControlIdentifier = "suppressed";
            const string UnsuppressedControlIdentifier = "unsuppressed";

            using (Page page = new PageWithAGloballySupressedControlAndAnUnspressedControl(SuppressedControlIdentifier, UnsuppressedControlIdentifier))
            {
                List<string> results =
                    new List<string>(AttributeExclusionChecker_Accessor.GetExcludedControlUniqueIdsForContainer(
                        page,
                        typeof(TestNotSuppressedPlugIn)));

                Assert.AreEqual(1, results.Count);
                Assert.IsTrue(results.Contains(SuppressedControlIdentifier));
                Assert.IsFalse(results.Contains(UnsuppressedControlIdentifier));
            }
        }

        /// <summary>
        /// Tests that GetExcludedControlUniqueIdsForPage returns the current identifiers when it is passed 
        /// a page containing a globally excluded control, a control excluded for the plug-in type, a control excluded for another type and a 
        /// control which is not excluded.
        /// </summary>
        [TestMethod]
        public void PassingAComplicatedPageToGetExcludedUniqueIdsWillReturnAnIEnumerableContainingTheUniqueIdOfTheExcludedControl()
        {
            List<string> results;
            using (Page page = new ComplicatedPage())
            {
                results = new List<string>(AttributeExclusionChecker_Accessor.GetExcludedControlUniqueIdsForContainer(page, typeof(TestSuppressedPlugIn)));
            }

            Assert.AreEqual(2, results.Count);
            Assert.IsFalse(results.Contains("notSuppressed"));
            Assert.IsTrue(results.Contains("globallyExcluded"));
            Assert.IsTrue(results.Contains("excludedForTestSuppressedPlugIn"));
            Assert.IsFalse(results.Contains("excludedForTestNotSuppressedPlugIn"));
        }

        /// <summary>
        /// a page containing a globally excluded control, a control excluded for the plug-in type, a control excluded for another type and a 
        /// control which is not excluded.
        /// </summary>
        private sealed class ComplicatedPage : Page
        {
            /// <summary>
            /// Not excluded text box
            /// </summary>
            private readonly TextBox notExcluded = new TextBox { ID = "notSuppressed" };

            /// <summary>
            /// Globally suppressed test box
            /// </summary>
            [SuppressProtection]
            private readonly TextBox globallyExcluded = new TextBox { ID = "globallyExcluded" };

            /// <summary>
            /// Suppressed for the TestSuppressedPluginClass.
            /// </summary>
            [SuppressProtection(typeof(TestSuppressedPlugIn))]
            private readonly TextBox excludedForTestSupressedPlugin = new TextBox { ID = "excludedForTestSuppressedPlugIn" };

            /// <summary>
            /// Suppressed for the TestNotSuppressedPlugin.
            /// </summary>
            [SuppressProtection(typeof(TestNotSuppressedPlugIn))]
            private readonly TextBox excludedForTestNotSuppressedPlugin = new TextBox { ID = "excludedForTestNotSuppressedPlugIn" };

            /// <summary>
            /// Initializes a new instance of the <see cref="ComplicatedPage"/> class.
            /// </summary>
            public ComplicatedPage()
            {
                this.ID = "ComplicatedPage";
                this.Controls.Add(this.notExcluded);
                this.Controls.Add(this.globallyExcluded);
                this.Controls.Add(this.excludedForTestSupressedPlugin);
                this.Controls.Add(this.excludedForTestNotSuppressedPlugin);
            }

            /// <summary>
            /// Enables a server control to perform final clean up before it is released from memory.
            /// </summary>
            public override void Dispose()
            {
                this.notExcluded.Dispose();
                this.globallyExcluded.Dispose();
                this.excludedForTestSupressedPlugin.Dispose();
                this.excludedForTestNotSuppressedPlugin.Dispose();
                base.Dispose();
            }
        }

        /// <summary>
        /// A test page with a control.
        /// </summary>
        private sealed class PageWithAControl : Page
        {
            /// <summary>
            /// A Test Control.
            /// </summary>
            private readonly TextBox control = new TextBox { ID = "controlId" };

            /// <summary>
            /// Initializes a new instance of the <see cref="PageWithAControl"/> class.
            /// </summary>
            public PageWithAControl()
            {
                this.ID = "PageWithAControl";
                this.Controls.Add(this.control);
            }

            /// <summary>
            /// Enables a server control to perform final clean up before it is released from memory.
            /// </summary>
            public override void Dispose()
            {
                this.control.Dispose();
                base.Dispose();
            }
        }

        /// <summary>
        /// A test page with a control with a suppress attribute.
        /// </summary>
        private sealed class PageWithAGloballySupressedControl : Page
        {
            /// <summary>
            /// A Test Control.
            /// </summary>
            [SuppressProtection]
            private readonly TextBox suppressedControl;

            /// <summary>
            /// Initializes a new instance of the <see cref="PageWithAGloballySupressedControl"/> class.
            /// </summary>
            /// <param name="controlId">The control identifier to use.</param>
            public PageWithAGloballySupressedControl(string controlId)
            {
                this.ID = "PageWithGloballySuppressedControl";
                this.suppressedControl = new TextBox
                {
                    ID = controlId
                };
                this.Controls.Add(this.suppressedControl);
            }

            /// <summary>
            /// Enables a server control to perform final clean up before it is released from memory.
            /// </summary>
            public override void Dispose()
            {
                this.suppressedControl.Dispose();
                base.Dispose();
            }
        }

        /// <summary>
        /// A test page with a control with a suppress attribute.
        /// </summary>
        private sealed class PageWithAGloballySupressedControlAndAnUnspressedControl : Page
        {
            /// <summary>
            /// A test suppressed control.
            /// </summary>
            [SuppressProtection]
            private readonly TextBox suppressedControl;

            /// <summary>
            /// A test not suppressed control.
            /// </summary>
            private readonly TextBox notSuppressedControl;

            /// <summary>
            /// Initializes a new instance of the <see cref="PageWithAGloballySupressedControlAndAnUnspressedControl"/> class.
            /// </summary>
            /// <param name="suppressedControlId">The control identifier to use.</param>
            /// <param name="notSuppressedControlId">The control identifier for the not suppressed control.</param>
            public PageWithAGloballySupressedControlAndAnUnspressedControl(string suppressedControlId, string notSuppressedControlId)
            {
                this.ID = "PageWithGloballySuppressedControlAndUnsuppressedControl";
                this.suppressedControl = new TextBox
                {
                    ID = suppressedControlId
                };
                this.notSuppressedControl = new TextBox
                {
                    ID = notSuppressedControlId
                };

                this.Controls.Add(this.suppressedControl);
                this.Controls.Add(this.notSuppressedControl);
            }

            /// <summary>
            /// Enables a server control to perform final clean up before it is released from memory.
            /// </summary>
            public override void Dispose()
            {
                this.suppressedControl.Dispose();
                this.notSuppressedControl.Dispose();
                base.Dispose();
            }
        }

        /// <summary>
        /// A test page class with no attributes.
        /// </summary>
        private sealed class TestPageWithNoAttributes : Page
        {            
        }

        /// <summary>
        /// A test page class with global suppression
        /// </summary>
        [SuppressProtection]
        private sealed class TestPageWithGlobalExclusion : Page
        {            
        }

        /// <summary>
        /// A test page class with a single suppression
        /// </summary>
        [SuppressProtection(typeof(TestSuppressedPlugIn))]
        private sealed class TestPageWithSingleExclusion : Page
        {
        }

        /// <summary>
        /// A test page class with double suppression
        /// </summary>
        [SuppressProtection(typeof(TestSuppressedPlugIn))]
        [SuppressProtection(typeof(AnotherSuppressedPlugIn))]
        private sealed class TestPageWithDoubleExclusion : Page
        {
        }

        /// <summary>
        /// A test class for internal checking
        /// </summary>
        private sealed class TestPageForInternalCachingNeverChecked : Page
        {
        }

        /// <summary>
        /// A test class for internal checking
        /// </summary>
        private sealed class TestPageForInternalCaching : Page
        {            
        }

        /// <summary>
        /// A sample plug-in class for exclusion checks.
        /// </summary>
        private sealed class TestSuppressedPlugIn
        {            
        }

        /// <summary>
        /// A sample plug-in class for exclusion checks.
        /// </summary>
        private sealed class AnotherSuppressedPlugIn
        {
        }

        /// <summary>
        /// A sample plug-in class for exclusion checks.
        /// </summary>
        private sealed class TestNotSuppressedPlugIn
        {
        }
    }
}
