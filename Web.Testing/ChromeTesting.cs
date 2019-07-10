using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Web;

namespace My.Web.Testing
{
        [TestClass]
        public class ChromeTesting
        {
            [TestMethod]
            public void n1_Chromedriver_setup_navigate_dispose()
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
                catch (Exception e)
                {
                    Assert.Fail("Can't create chrome: " + e.Message);
                }
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
                try
                {
                    Web.Chrome.ChromedriverDispose(helper);
                }
                catch (Exception e)
                {
                    Assert.Fail("Can't dispose chrome: " + e.Message);
                }
            }

            [TestMethod]
            public void n2_Chromedriver_setup_navigate_dispose_fastProxy()
            {
                Web.Chrome.ChromeDriverHelper helper = null;
                try
                {
                    string byPassExtensionPath = @"C:\Users\navir\AppData\Local\Google\Chrome\User Data\Default\Extensions\mkelkmkgljeohnaeehnnkmdpocfmkmmf\5.0.4_0";
                    helper = Web.Chrome.Chromedriver_set_up(true, true, new[] { byPassExtensionPath });
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
                try
                {
                    helper.SelectChromeHandle(1, false);
                }
                catch (Exception e)
                {
                    Assert.Fail("Can't select first handle");
                }
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
                try
                {
                    Web.Chrome.ChromedriverDispose(helper);
                }
                catch (Exception e)
                {
                    Assert.Fail("Can't dispose chrome: " + e.Message);
                }
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
