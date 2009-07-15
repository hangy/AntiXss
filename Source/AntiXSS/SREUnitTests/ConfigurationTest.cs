using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Microsoft.Security.Application.SecurityRuntimeEngine.PageProtection;
using Microsoft.Security.Application.SecurityRuntimeEngine.Configuration;
namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for ConfigurationTest and is intended
    ///to contain all ConfigurationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConfigurationTest
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
        ///A test for EncodingTypes
        ///</summary>
        [TestMethod()]
        public void EncodingTypesTest()
        {
            ModuleConfiguration target = new ModuleConfiguration(); 
            ControlEncodingContexts expected = null; 
            ControlEncodingContexts actual;
            target.EncodingTypes = expected;
            actual = target.EncodingTypes;
            Assert.AreEqual(expected, actual,"ModuleConfiguration.EncodingTypes is not working properly");
        }

        /// <summary>
        ///A test for LoadConfiguration
        ///</summary>
        [TestMethod()]
        public void LoadConfigurationTest()
        {
            string strConfigFile = @"antixssmodulegood.config";
            string expectedClassName = "System.Web.UI.WebControls.Label";
            ModuleConfiguration actual;
            actual = ModuleConfiguration.LoadConfiguration(strConfigFile);
            Assert.AreEqual(expectedClassName, actual.EncodingTypes[0].FullClassName,"ModuleConfiguration.LoadConfiguration is not working properly");
        }

        /// <summary>
        ///A negative test for LoadConfiguration
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException),"ModuleConfiguration.LoadConfiguration did not throw filenot found exception")]
        public void LoadConfigurationTest2()
        {
            string strConfigFile = "";
            ModuleConfiguration actual;
            actual = ModuleConfiguration.LoadConfiguration(strConfigFile);
        }

        /// <summary>
        ///A negative test for LoadConfiguration
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(XmlException), "ModuleConfiguration.LoadConfiguration did not throw filenot found exception")]
        public void LoadConfigurationTest4()
        {
            string strConfigFile = @"antixssmodulebad.config";
            ModuleConfiguration actual;
            actual = ModuleConfiguration.LoadConfiguration(strConfigFile);
        }

        /// <summary>
        ///A negative test for LoadConfiguration
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.IO.FileNotFoundException), "ModuleConfiguration.LoadConfiguration did not throw filenot found exception")]
        public void LoadConfigurationTest3()
        {
            string strConfigFile = "test.xml";
            ModuleConfiguration actual;
            actual = ModuleConfiguration.LoadConfiguration(strConfigFile);
        }

        /// <summary>
        ///A test for ModuleConfiguration Constructor
        ///</summary>
        [TestMethod()]
        public void ConfigurationConstructorTest()
        {
            ModuleConfiguration target = new ModuleConfiguration();
            Assert.IsNotNull(target, "ModuleConfiguration constructor is not working properly");
        }

    }
}
