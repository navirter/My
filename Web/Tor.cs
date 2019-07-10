using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Web
{

    public class Tor
    {
        public IWebDriver Driver { get; set; }
        public Process TorProcess { get; set; }
        //public WebDriverWait Wait { get; set; }

        public void Setup()
        {
            String torBinaryPath = @"C:\Users\navir\Desktop\Tor Browser\Browser\firefox.exe";
            this.TorProcess = new Process();
            this.TorProcess.StartInfo.FileName = torBinaryPath;
            this.TorProcess.StartInfo.Arguments = "-n";
            this.TorProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            this.TorProcess.Start();

            var PROXY = "127.0.0.1:9150";// # IP:PORT or HOST:PORT
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--proxy-server=" + PROXY);
            this.Driver = new ChromeDriver(options);
            

            /*
            FirefoxProfile profile = new FirefoxProfile();
            profile.SetPreference("network.proxy.type", 1);
            profile.SetPreference("network.proxy.socks", "127.0.0.1");
            profile.SetPreference("network.proxy.socks_port", 9150);
            FirefoxOptions options = new FirefoxOptions();
            options.Profile = profile;
            string s = IO.CurrentDirectoryFolder;
            this.Driver = new FirefoxDriver(options);
            //this.Wait = new WebDriverWait(this.Driver, TimeSpan.FromSeconds(60));
            */
        }

        public void Teardown()
        {
            this.Driver.Quit();
            this.TorProcess.Kill();
        }

        public void Navigate()
        {
            this.Driver.Navigate().GoToUrl(@"whatismyipaddressSiteUrl");
        }

        public IWebElement FindElementByXpath(string xpath)
        {
            var expression = By.XPath(xpath);
            //this.Wait.Until(x => x.FindElement(expression));
            var element = this.Driver.FindElement(expression);
            return element;
            //Assert.AreNotEqual<string>("84.40.65.000", element.Text);
        }
    }
}
