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
        
        /// <summary>
        /// last created chromeDriver
        /// </summary>
        public static ChromeDriver chromeDriver;
        public static List<ChromeDriver> chromeDrivers = new List<ChromeDriver>();
        public static List<string> chromeDriversIds = new List<string>();

        /// <summary>
        /// Returns a new chromedriver instance with given parameters, and saves its information.\n
        /// Each set_up must be followed by dispose or disposeAll
        /// </summary>
        /// <param name="visible">False if chrome must run silently</param>
        /// <param name="ignoreSetificateErrors"></param>
        /// <param name="windowsUserName">Leave it empty not to use a consistant chrome profile</param>
        /// <returns></returns>
        public static ChromeDriver chromedriver_set_up(bool visible = true, bool ignoreSetificateErrors = true
            , string windowsUserName = "")
        {
            try
            {  
                //create instance
                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                var options = new ChromeOptions();
                #region set options
                options.AddArgument("enable-extentions");
                options.AddArgument("--start-maximized");
                #region set visibility
                try
                {
                    if (!visible)
                        options.AddArgument("headless");
                }
                catch(Exception e)
                {
                    throw new Exception("Can't set visibility. " + e.Message);
                }
                #endregion
                #region set profile path
                string defaultProfilePath = @"C:\Users\<USERNAME>\AppData\Local\Temp\scoped_dir8764_15668\Default;";
                try
                {
                    if (!string.IsNullOrEmpty(windowsUserName))
                    {
                        string chromeProfilePath = defaultProfilePath.Replace("<USERNAME>", windowsUserName);
                        options.AddArgument("user-data-dir=" + chromeProfilePath);
                    }
                }
                catch(Exception e)
                {
                    throw new Exception("Can't set chrome profile path.\nIt must look like:\n" + defaultProfilePath + "\n" + e.Message);
                }
                #endregion
                #region set ignore sertificate errors
                try
                {
                    if (ignoreSetificateErrors)
                        options.AddArgument("ignore-certificate-errors");
                }
                catch(Exception e)
                {
                    throw new Exception("Can't set ignore certificate errors. " + e.Message);
                }
                #endregion
                //options.AddArgument("--window-position=-32000,-32000");
                #endregion
                ChromeDriver chrome = new ChromeDriver(service, options);                                
                Thread.Sleep(2000);

                #region set My.Web features for keeping more info about all Chromedrivers
                chromeDrivers.Add(chrome);
                chromeDriversIds.Add(service.ProcessId.ToString());                

                List<string> towrite = new List<string>();
                for (int i = 0; i < chromeDriversIds.Count; i++)
                {
                    towrite.Add(chromeDriversIds[i]);
                }
                File.AppendAllLines(Directory.GetCurrentDirectory() + "\\drivers.txt", towrite);
                #endregion

                return chrome;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message + "\nОтсутствует файл chromedriver.exe?");
            }
        }

        public static void chromedriver_dispose(ChromeDriver chrome, int process_id)
        {
            try
            {
                chromeDrivers.Remove(chrome);
                chromeDriversIds.RemoveAll(s => s == process_id.ToString());
                chrome.Quit();
                chrome = null;
                var chromeProc = Process.GetProcessById(process_id);
                killProc(chromeProc);
            }
            catch (Exception e)
            {
                throw new Exception(process_id + ": " + e.Message);
            }
            try
            {
                List<string> towrite = new List<string>();
                chromeDriversIds.ForEach(s => towrite.Add(s));
                File.WriteAllLines(Directory.GetCurrentDirectory() + "\\drivers.txt", towrite);
            }
            catch (Exception e)
            {
                throw new Exception(process_id + ": " + e.Message);
            }
        }
        public static void chromedriver_disposeAll()
        {
            string path = Directory.GetCurrentDirectory() + "\\drivers.txt";
            try
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    Process p = Process.GetProcessById(int.Parse(lines[i])); 
                    chromedriver_dispose(h.chromedriver, lines[i].ExtractDigits());
                }
            }
            catch { }
            try
            {
                var processList = Process.GetProcessesByName("chromedriver");
                foreach (var process in processList)
                {
                    killProc(process);
                }
            }
            catch { }
        }
        static void killProc(Process process, int timeOutMilliseconds = 10000)
        {
            process.WaitForExit(timeOutMilliseconds);

            if (!process.HasExited)
            {
                if (process.Responding)
                    process.CloseMainWindow();
                else
                    process.Kill();
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
