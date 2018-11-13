using My;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace My
{
    public static class Web
    {
        public static Image downloadImageFromURL(string url, out int flagsCount)
        {
            flagsCount = -1;
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(url);

                using (MemoryStream mem = new MemoryStream(data))
                {
                    using (var yourImage = Image.FromStream(mem))
                    {
                        flagsCount = yourImage.Flags;
                        return yourImage;
                    }
                }
            }
        }

        public static ChromeDriver chromedriver_set_up(out Process process, out int process_id, bool visible = true)
        {
            try
            {
                var service = ChromeDriverService.CreateDefaultService();
                var options = new ChromeOptions();
                if (!visible)
                    options.AddArgument("headless");
                service.HideCommandPromptWindow = true;
                //options.AddArgument("--window-position=-32000,-32000");
                ChromeDriver chrome = new ChromeDriver(service, options);
                Thread.Sleep(2000);
                process_id = service.ProcessId;
                process = Process.GetProcessById(process_id);
                return chrome;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void chromedriver_dispose(ChromeDriver chrome, int process_id)
        {
            try
            {
                chrome.Close();
                var chromeProc = Process.GetProcessById(process_id);
                chromeProc.Kill();
                Thread.Sleep(2000);
                chrome = null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static IWebElement findLastElement(ChromeDriver chrome, string text, string tag = "", string id = "")
        {
            if (id != "")
            {
                return chrome.FindElementById(id);
            }
            if (tag != "")
            {
                var elems = chrome.FindElements(By.TagName(tag));
                return elems.Last(s => s.Text == text || s.Text.Trim() == text);
            }
            return chrome.FindElementByXPath(string.Format("//*[contains(text(), '{0}')]", text));
        }

    }
}
