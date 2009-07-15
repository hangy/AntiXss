
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Security.Application.SecurityRuntimeEngine.PageProtection;
using Microsoft.Security.Application.SecurityRuntimeEngine.Configuration;
namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for EncodingTypeTest and is intended
    ///to contain all EncodingTypeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EncodingTypeTest
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
        ///A test for PropertyName
        ///</summary>
        [TestMethod()]
        public void PropertyNameTest()
        {
            ControlEncodingContext target = new ControlEncodingContext();
            string expected = "Text"; 
            string actual;
            target.PropertyName = expected;
            actual = target.PropertyName;
            Assert.AreEqual(expected, actual,"EncodingType.PropertyName is not working properly");
        }

        /// <summary>
        ///A test for ID
        ///</summary>
        [TestMethod()]
        public void IDTest()
        {
            ControlEncodingContext target = new ControlEncodingContext("System.Web.UI.WebControls.Label", "Text", EncodingContexts.Html); 
            string actual;
            actual = target.ID;
            Assert.AreEqual("System.Web.UI.WebControls.Label.Text", actual, "EncodingType.ID is not working properly");
        }

        /// <summary>
        ///A test for FullClassName
        ///</summary>
        [TestMethod()]
        public void FullClassNameTest()
        {
            ControlEncodingContext target = new ControlEncodingContext(); 
            string expected = "System.Web.UI.WebControls.Label";
            string actual;
            target.FullClassName = expected;
            actual = target.FullClassName;
            Assert.AreEqual(expected, actual, "EncodingType.FullClassName is not working properly");
        }

        /// <summary>
        ///A test for EncodingContext
        ///</summary>
        [TestMethod()]
        public void EncodingContextTest()
        {
            ControlEncodingContext target = new ControlEncodingContext(); // TODO: Initialize to an appropriate value
            EncodingContexts expected = EncodingContexts.Html; // TODO: Initialize to an appropriate value
            EncodingContexts actual;
            target.EncodingContext = expected;
            actual = target.EncodingContext; 
            Assert.AreEqual(expected, actual, "EncodingType.EncodingContext is not working properly");
        }

        /// <summary>
        ///A test for EncodingType Constructor
        ///</summary>
        [TestMethod()]
        public void EncodingTypeConstructorTest1()
        {
            ControlEncodingContext target = new ControlEncodingContext();
            Assert.IsNotNull(target, "EncodingType constructor is not working properly");
        }

        /// <summary>
        ///A test for EncodingType Constructor
        ///</summary>
        [TestMethod()]
        public void EncodingTypeConstructorTest()
        {
            string fullName = "System.Web.UI.WebControls.Label";
            string propertyName = "Text";
            EncodingContexts context = EncodingContexts.Html;
            ControlEncodingContext target = new ControlEncodingContext(fullName, propertyName, context);
            Assert.AreEqual("System.Web.UI.WebControls.Label.Text", target.ID, "EncodingType Constructor is not working properly");
        }
    }
}
