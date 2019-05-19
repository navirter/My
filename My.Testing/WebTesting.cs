using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;

namespace My.Testing
{
    [TestClass]
    public class WebTesting
    {
        #region chromedriver
        [TestMethod]
        public void chromedriver_setup_navigate_dispose_setup()
        {
            Web.Chrome.ChromeDriverHelper helper = null;
            try
            {
                helper = Web.Chrome.Chromedriver_set_up();                
                if (helper == null)
                    Assert.Fail("Chrome is null created.");
                var handles = helper.ChromeDriver.WindowHandles;
                if (handles.Count == 0)
                    Assert.Fail("No chrome handles");
            }
            catch(Exception e)
            {
                Assert.Fail("Can't create chrome: " + e.Message);
            }
            string uri = "https://www.google.com/";
            try
            {
                helper.Navigate(uri);
                string page = helper.CurrentChromeDriverInfo.PageSource;
                string currentUri = helper.CurrentChromeDriverInfo.UrlAfter;
                if (currentUri != uri || string.IsNullOrEmpty(page) || page == "data;,")
                    Assert.Fail("Can't navigate chrome");
            }
            catch(Exception e)
            {
                Assert.Fail("Can't navigate chrome: " + e.Message);
            }
            try
            {
                Web.Chrome.Chromedriver_dispose(helper);
            }
            catch (Exception e)
            {
                Assert.Fail("Can't dispose chrome: " + e.Message);
            }
        }
        #endregion

        //[TestClass]
        //public class TorTesting
        //{
        //    Web.Tor tor = null;

        //    [TestMethod]
        //    public void Initializate()
        //    {
        //        tor = new Web.Tor();
        //        tor.Setup();
        //        Assert.IsNotNull(tor.Driver);
        //    }

        //    [TestMethod]
        //    public void TearDown()
        //    {
        //        if (tor != null)
        //        {
        //            tor.Teardown();
        //            Assert.IsNull(tor.Driver);
        //        }
        //    }
        //}
    }
}
