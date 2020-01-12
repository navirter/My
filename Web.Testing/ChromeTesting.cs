using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Web;

namespace My.Web.Testing
{
    [TestClass]
    public class ChromeTesting
    {
        [TestMethod]
        public void n1_Chromedriver_setup_navigate_openTabs_selectTabs_dispose()
        {
            Web.Chrome.ChromeDriverHelper helper = null;
            #region create chrome
            try
            {
                helper = Web.Chrome.ChromedriverSetUp(new Chrome.ChromeDriverSetUpOptions());
                if (helper == null)
                    Assert.Fail("Chrome is null created.");
                var handles = helper.ChromeDriver.WindowHandles;
                if (handles.Count == 0)
                    Assert.Fail("No chrome handles");
            }
            catch (Exception e)
            {
                Assert.Fail("Can't create chrome: " + e.Message);
            }
            #endregion
            #region navigate
            string uri = "https://www.google.com/";
            try
            {
                helper.Navigate(uri);
                string page = helper.ChromeDriver.PageSource;
                string currentUri = helper.ChromeDriver.Url;
                if (string.IsNullOrEmpty(page) || page == "data;,")
                    Assert.Fail("Can't download page");
            }
            catch (Exception e)
            {
                Assert.Fail("Can't navigate chrome: " + e.Message);
            }
            #endregion
            #region open 2 new tabs
            try
            {
                helper.OpenNewTab();
                helper.OpenNewTab();
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                Assert.Fail("Can't open 2 new tabs: " + e.Message);
            }
            #endregion
            #region select new tab
            try
            {
                helper.SelectChromeHandle(1);
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                Assert.Fail("Can't select the new tab: " + e.Message);
            }
            #endregion
            #region select first tab with closing option
            try
            {
                helper.SelectChromeHandle(0, true);
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                Assert.Fail("Can't select the first tab with closing option: " + e.Message);
            }
            #endregion
            #region dispose of chrome
            try
            {
                Web.Chrome.ChromedriverDispose(helper);
            }
            catch (Exception e)
            {
                Assert.Fail("Can't dispose chrome: " + e.Message);
            }
            #endregion
        }

        [TestMethod]
        public void n3_chromedriver_disposeAll()
        {
            try
            {
                Web.Chrome.ChromedriverDisposeAll();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
