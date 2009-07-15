using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using System.IO;
using System.Text;
using Microsoft.Security.Application.SecurityRuntimeEngine;


namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for AntiXssModuleTest and is intended
    ///to contain all AntiXssModuleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AntiXssModuleTest
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
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            AntiXssModule_Accessor target = new AntiXssModule_Accessor(); // TODO: Initialize to an appropriate value
            target.Dispose();
            Assert.IsNotNull(target, "AntiXssModule_Accessor.Dispose is not working as expected");
        }

        /// <summary>
        ///A test for AntiXssModule_Accessor Constructor
        ///</summary>
        [TestMethod()]
        public void AntiXssModuleConstructorTest()
        {
            AntiXssModule_Accessor target = new AntiXssModule_Accessor();
            Assert.IsNotNull(target, "AntiXssModule_Accessor Constructor is not working as expected");
        }


    }

}
