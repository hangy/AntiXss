
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Microsoft.Security.Application.SecurityRuntimeEngine.PageProtection;
using Microsoft.Security.Application.SecurityRuntimeEngine.Configuration;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for EncodingTypesTest and is intended
    ///to contain all EncodingTypesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EncodingTypesTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetEncodingTypes
        ///</summary>
        [TestMethod()]
        public void GetEncodingTypesTest()
        {
            ControlEncodingContexts target = new ControlEncodingContexts(); // TODO: Initialize to an appropriate value\
            target.Add(new ControlEncodingContext("System.Web.UI.WebControls.Label", "Text", EncodingContexts.Html));
            string fullName = "System.Web.UI.WebControls.Label"; // TODO: Initialize to an appropriate value

            List<ControlEncodingContext> actual;
            actual = target.GetEncodingTypes(fullName);
            Assert.AreEqual("System.Web.UI.WebControls.Label.Text", actual[0].ID,"EncodingTypes.GetEncodingTypes did not work properly");
        }

        /// <summary>
        ///A negative test for GetEncodingTypes
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentNullException),"EncodingTypes.GetEncodingTypes is not working propery")]
        public void GetEncodingTypesTest2()
        {
            ControlEncodingContexts target = new ControlEncodingContexts(); // TODO: Initialize to an appropriate value\
            target.Add(new ControlEncodingContext("System.Web.UI.WebControls.Label", "Text", EncodingContexts.Html));
            string fullName = ""; 

            List<ControlEncodingContext> actual;
            actual = target.GetEncodingTypes(fullName);

        }


        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod()]
        public void ContainsTest1()
        {
            ControlEncodingContexts target = new ControlEncodingContexts(); 
            target.Add(new ControlEncodingContext("System.Web.UI.WebControls.Label", "Text", EncodingContexts.Html));
            
            string fullName = "System.Web.UI.WebControls.Label"; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Contains(fullName);
            Assert.AreEqual(expected, actual,"EncodingTypes.Contains is not working properly");
        }
        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod()]
        public void ContainsTest2()
        {
            ControlEncodingContexts target = new ControlEncodingContexts();
            target.Add(new ControlEncodingContext("System.Web.UI.WebControls.Label", "Text", EncodingContexts.Html));

            string fullName = "System.Web.UI.WebControls.TextBox"; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Contains(fullName);
            Assert.AreEqual(expected, actual, "EncodingTypes.Contains is not working properly");
        }

        /// <summary>
        ///A negative test for Contains
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentNullException),"EncodingTypes.Contains is not working as expected")]
        public void ContainsTest3()
        {
            ControlEncodingContexts target = new ControlEncodingContexts();
            string fullName = ""; 
            bool actual = target.Contains(fullName);
        }

        /// <summary>
        ///A negative test for Contains
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentNullException), "EncodingTypes.Contains is not working as expected")]
        public void ContainsTest4()
        {
            ControlEncodingContexts target = new ControlEncodingContexts();
            string fullName = "";
            string propertyName = "";
            bool actual = target.Contains(fullName,propertyName);
        }

        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod()]
        public void ContainsTest5()
        {
            ControlEncodingContexts target = new ControlEncodingContexts();
            target.Add(new ControlEncodingContext("System.Web.UI.WebControls.Label", "Text", EncodingContexts.Html));

            string fullName = "System.Web.UI.WebControls.TextBox"; // TODO: Initialize to an appropriate value
            string propertyName = "Text"; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Contains(fullName, propertyName);
            Assert.AreEqual(expected, actual, "EncodingTypes.Contains is not working properly");
        }


        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod()]
        public void ContainsTest()
        {
            ControlEncodingContexts target = new ControlEncodingContexts();
            target.Add(new ControlEncodingContext("System.Web.UI.WebControls.Label", "Text", EncodingContexts.Html));

            string fullName = "System.Web.UI.WebControls.Label"; // TODO: Initialize to an appropriate value
            string propertyName = "Text"; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Contains(fullName, propertyName);
            Assert.AreEqual(expected, actual,"EncodingTypes.Contains is not working properly");
        }

        /// <summary>
        ///A test for EncodingTypes Constructor
        ///</summary>
        [TestMethod()]
        public void EncodingTypesConstructorTest()
        {
            ControlEncodingContexts target = new ControlEncodingContexts();
            Assert.IsNotNull(target, "EncodingTypes constructor is not working properly");
        }
    }
}
