﻿using My;
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
    /// <summary>
    /// A wrapper for fome functionality from OpenQA.Selenium.Chrome
    /// </summary>
    public static class Web
    {       
        #region chromeDriver management
        /// <summary>
        /// last created chromeDriver
        /// </summary>
        public static ChromeDriverHelper LastChromeDriverHelper { get; private set; }
        public static List<ChromeDriverHelper> ChromeDriverHelpers { get; private set; } = new List<ChromeDriverHelper>();

        public class ChromeDriverHelper
        {
            public ChromeDriver ChromeDriver { get; set; }
            public List<string> AttachedProcessesIds { get; set; }
        }

        static List<string> lastCreatedDriverIds = new List<string>();//filled at chromedriver initialization
        /// <summary>
        /// All attached processes of all helpers
        /// </summary>
        public static List<string> chromeDriversIds
        {
            get
            {
                List<string> res = new List<string>();
                ChromeDriverHelpers.ForEach(s => res.AddRange(s.AttachedProcessesIds));
                return res;
            }
        }

        static string[] _processesToCheck = {   
        "chrome",
        "chromedriver",
        "conhost"};


        /// <summary>
        /// Returns a new chromedriver instance with given parameters, and saves its information.\n
        /// Each set_up must be followed by dispose or disposeAll.
        /// It's possible that some other chrome tabs(even in normal browsers) created during execution of this function are closed at later disposal
        /// It's better off launching projects with this function as administrator.
        /// To force so: create app.manifest and set it there <requestedExecutionLevel level = "requireAdministrator" uiAccess="false" />
        /// </summary>
        /// <param name="visible">False if chrome must run silently</param>
        /// <param name="ignoreSetificateErrors"></param>
        /// <param name="windowsUserName">Leave it empty not to use a consistant chrome profile</param>
        /// <returns></returns>
        public static ChromeDriverHelper chromedriver_set_up(bool visible = true, bool ignoreSetificateErrors = true
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
                catch (Exception e)
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
                catch (Exception e)
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
                catch (Exception e)
                {
                    throw new Exception("Can't set ignore certificate errors. " + e.Message);
                }
                #endregion
                //options.AddArgument("--window-position=-32000,-32000");
                #endregion
                //get chrome related processes before initialization. Can throw error that access denied. Dealt away with running as administaror
                var chromeRelatedIds = Process.GetProcesses()
                    .Where(s => _processesToCheck.FirstOrDefault(x => s.ProcessName.ToLower().Contains(x)) != null)
                    .ToList();
                waitSmoothly(1.5);
                //initialize chromedriver
                var driverStartTime = Process.GetCurrentProcess().StartTime;

                ChromeDriver chrome = new ChromeDriver(service, options);//cant instantize if onother cd exists for some reasons
                waitSmoothly(1.5);
                //get chrome related processes after initialization. Can throw error that access denied. Dealt away with running as administaror
                var latestChromeRelatedIds = Process.GetProcesses()
                    .Where(s => _processesToCheck.FirstOrDefault(x => s.ProcessName.ToLower().Contains(x)) != null
                    && s.StartTime > driverStartTime)
                    .ToList();
                waitSmoothly(1.5);
                //save newly created processes' ids to list
                lastCreatedDriverIds = new List<string>();
                latestChromeRelatedIds.ForEach(s =>
                {
                    var match = chromeRelatedIds.FirstOrDefault(a => a.Id == s.Id);
                    if (match == null)//if there was no such chrome related id before initialization
                        lastCreatedDriverIds.Add(s.Id.ToString());
                });
                //save new helper to list
                var lastHelper = new ChromeDriverHelper() { ChromeDriver = chrome, AttachedProcessesIds = lastCreatedDriverIds };
                ChromeDriverHelpers.Add(lastHelper);
                LastChromeDriverHelper = lastHelper;
                //save newly created processees' ids to a file
                File.AppendAllLines(Directory.GetCurrentDirectory() + "\\drivers.txt", lastCreatedDriverIds);
                return lastHelper;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void chromedriver_dispose(ChromeDriverHelper helper)
        {
            string ids = string.Join(", ", helper.AttachedProcessesIds);
            try
            {
                helper.ChromeDriver.Quit();
                helper.AttachedProcessesIds.ForEach(s =>
                {
                    var proc = Process.GetProcessById(int.Parse(s));
                    killProc(proc);
                });
                helper.AttachedProcessesIds = new List<string>();
                File.WriteAllLines(Directory.GetCurrentDirectory() + "\\drivers.txt", chromeDriversIds);
                ChromeDriverHelpers.Remove(helper);
                helper = null;
                LastChromeDriverHelper = ChromeDriverHelpers.LastOrDefault();
            }
            catch (Exception e)
            {
                throw new Exception("ids(" + ids + "): " + e.Message);
            }
        }

        static int linesCounter = 0;
        static int linesCount = 0;
        delegate void voidProcessInt(Process process, int integer);
        public static void chromedriver_disposeAll(int maximumTimeoutSeconds = 10)
        {

            string path = Directory.GetCurrentDirectory() + "\\drivers.txt";
            string[] lines = File.ReadAllLines(path);
            linesCount = lines.Length; linesCounter = 0;
            for (int i = 0; i < lines.Length; i++)
                try
                {
                    Process p = Process.GetProcessById(int.Parse(lines[i]));
                    if (p == null)
                    {
                        linesCounter++;
                        continue;
                    }
                    voidProcessInt vpi = killProc;
                    vpi.BeginInvoke(p, 2000, null, null);
                }
                catch
                {
                }
            var now = DateTime.Now;
            while (linesCounter != linesCount)
            {
                Application.DoEvents();
                Thread.Sleep(100);
                if (DateTime.Now - now > TimeSpan.FromSeconds(maximumTimeoutSeconds))
                    break;
            }
            LastChromeDriverHelper = null;
            ChromeDriverHelpers = new List<ChromeDriverHelper>();
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\drivers.txt", chromeDriversIds);
        }
        static void killProc(Process process, int timeOutMilliseconds = 2000)
        {
            try
            {
                process.WaitForExit(timeOutMilliseconds);

                if (!process.HasExited)
                {
                    if (process.Responding)
                        process.CloseMainWindow();
                }
                process.Kill();
            }
            catch { }
            finally
            {
                linesCounter++;
            }

        }
        #endregion

        #region useful functionality for chromedriver
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

        #region NavigateChrome(smooth chrome navigation)

        #region last chrome 
        /// <summary>
        /// Smooth chrome navigation. Last chrome is used. Can be stopped.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="minDelaySeconds"></param>
        /// <param name="maxDelaySeconds"></param>
        /// <param name="stop"></param>
        /// <param name="tryouts"></param>
        /// <returns></returns>
        public static bool NavigateLastChrome(string url, int minDelaySeconds, int maxDelaySeconds, ref bool stop, int tryouts = 3)
        {
            return NavigateChrome(Web.LastChromeDriverHelper.ChromeDriver, url, minDelaySeconds, maxDelaySeconds, ref stop, tryouts);
        }

        /// <summary>
        /// Smooth chrome navigation. Last chrome is used
        /// </summary>
        /// <param name="url"></param>
        /// <param name="minDelaySeconds"></param>
        /// <param name="maxDelaySeconds"></param>
        /// <param name="tryouts"></param>
        /// <returns></returns>
        public static bool NavigateLastChrome(string url, int minDelaySeconds, int maxDelaySeconds, int tryouts = 3)
        {
            return NavigateChrome(Web.LastChromeDriverHelper.ChromeDriver, url, minDelaySeconds, maxDelaySeconds, tryouts);
        }

        /// <summary>
        /// Smooth chrome navigation. Last chrome is used. Can be stopped.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="delaySeconds"></param>
        /// <param name="stop"></param>
        /// <param name="tryouts"></param>
        /// <returns></returns>
        public static bool NavigateLastChrome(string url, int delaySeconds, ref bool stop, int tryouts = 3)
        {
            return NavigateChrome(Web.LastChromeDriverHelper.ChromeDriver, url, delaySeconds, ref stop, tryouts);
        }

        /// <summary>
        /// Smooth chrome navigation. Last chrome is used
        /// </summary>
        /// <param name="url"></param>
        /// <param name="delaySeconds"></param>
        /// <param name="tryouts"></param>
        /// <returns></returns>
        public static bool NavigateLastChrome(string url, double delaySeconds, int tryouts = 3)
        {
            return NavigateChrome(Web.LastChromeDriverHelper.ChromeDriver, url, delaySeconds, tryouts);
        }
        #endregion

        #region target chrome
        /// <summary>
        /// Smooth chrome navigation. Can be stopped.
        /// </summary>
        /// <param name="cd"></param>
        /// <param name="url"></param>
        /// <param name="minDelaySeconds"></param>
        /// <param name="maxDelaySeconds"></param>
        /// <param name="stop"></param>
        /// <param name="tryouts"></param>
        /// <returns></returns>
        public static bool NavigateChrome(ChromeDriver cd, string url, int minDelaySeconds, int maxDelaySeconds, ref bool stop, int tryouts = 3)
        {
            double rand = new Random().Next(minDelaySeconds, maxDelaySeconds);
            return NavigateChrome(cd, url, rand, ref stop, tryouts);
        }

        /// <summary>
        /// Smooth chrome navigation.
        /// </summary>
        /// <param name="cd"></param>
        /// <param name="url"></param>
        /// <param name="minDelaySeconds"></param>
        /// <param name="maxDelaySeconds"></param>
        /// <param name="tryouts"></param>
        /// <returns></returns>
        public static bool NavigateChrome(ChromeDriver cd, string url, int minDelaySeconds, int maxDelaySeconds, int tryouts =3)
        {
            double rand = new Random().Next(minDelaySeconds, maxDelaySeconds);
            return NavigateChrome(cd, url, rand, tryouts);
        }


        /// <summary>
        /// Smooth chrome navigation. Can be stopped.
        /// </summary>
        /// <param name="cd"></param>
        /// <param name="url"></param>
        /// <param name="delay"></param>
        /// <param name="tryouts"></param>
        /// <returns></returns>
        public static bool NavigateChrome(ChromeDriver cd, string url, double delaySeconds, ref bool stop, int tryouts = 3)
        {
            #region check arguments
            if (cd == null)
                throw new ArgumentNullException("Chrome is null");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("URL is null or empty");
            if (delaySeconds <= 0 || tryouts <= 0)
                throw new ArgumentOutOfRangeException("Delay and tryouts cannot be less or equal 0");
            #endregion
            try
            {
                bool done = false;
                for (int i = 0; i < tryouts; i++)
                {
                    cd.Url = url;
                    waitSmoothly(delaySeconds, ref stop);
                    if (!cd.PageSource.Contains("No internet"))
                    {
                        done = true;
                        break;
                    }
                }
                if (done)
                    return true;
                else
                    throw new Exception("No internet connection after " + tryouts + " tryouts");

            }
            catch (Exception e)
            {
                throw new Exception("Cannot navigate chrome: " + e.Message);
            }
        }//main function with stop parameter

        /// <summary>
        /// Smooth chrome navigation
        /// </summary>
        /// <param name="cd"></param>
        /// <param name="url"></param>
        /// <param name="delay"></param>
        /// <param name="tryouts"></param>
        /// <returns></returns>
        public static bool NavigateChrome(ChromeDriver cd, string url, double delaySeconds, int tryouts = 3)
        {
            #region check arguments
            if (cd == null)
                throw new ArgumentNullException("Chrome is null");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("URL is null or empty");
            if (delaySeconds <= 0 || tryouts <= 0)
                throw new ArgumentOutOfRangeException("Delay and tryouts cannot be less or equal 0");
            #endregion
            try
            {
                bool done = false;
                for (int i = 0; i < tryouts; i++)
                {
                    cd.Url = url;
                    waitSmoothly(delaySeconds);
                    if (!cd.PageSource.Contains("No internet"))
                    {
                        done = true;
                        break;
                    }
                }
                if (done)
                    return true;
                else
                    throw new Exception("No internet connection after " + tryouts + " tryouts");

            }
            catch (Exception e)
            {
                throw new Exception("Cannot navigate chrome: " + e.Message);
            }
        }//main function
        #endregion

        #endregion

        #endregion

        #region waitSmoothly
        static void waitSmoothly(double seconds, ref bool stop)
        {
            for (double i = 0.1; i < seconds; i += 0.1)
            {
                Application.DoEvents();
                Thread.Sleep(100);
                if (stop)
                    break;
            }
        }

        static void waitSmoothly(double seconds)
        {
            for (double i = 0.1; i < seconds; i += 0.1)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
        }
        #endregion
    }
}
