﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My.Web
{
    public class Chrome
    {
        public class ChromeDriverHelper
        {
            #region fields
            public ChromeDriver ChromeDriver { get; set; }
            public List<string> AttachedProcessesIds { get; set; }
            public string CurrentUrl
            {
                get
                {
                    return ChromeDriver.Url;
                }
                set
                {
                    ChromeDriver.Url = value;
                }
            }
            public string CurrentPageSource { get { return ChromeDriver.PageSource; } }
            #endregion

            #region Navigate(URL or smoothly with parameters)
            public void Navigate(string URL)
            {
                this.CurrentUrl = URL;
            }
            
            /// <summary>
            /// Smooth chrome navigation. Can be stopped.
            /// </summary>
            /// <param name="cd"></param>
            /// <param name="URL"></param>
            /// <param name="delaySeconds"></param>
            /// <param name="stop"></param>
            /// <param name="tryouts"></param>
            /// <returns></returns>
            public bool Navigate(string URL, double delaySeconds, int tryouts = 3)
            {
                #region check arguments
                if (ChromeDriver == null)
                    throw new ArgumentNullException("Chrome is null");
                if (string.IsNullOrEmpty(URL))
                    throw new ArgumentNullException("URL is null or empty");
                if (delaySeconds <= 0 || tryouts <= 0)
                    throw new ArgumentOutOfRangeException("Delay and tryouts cannot be less or equal 0");
                #endregion
                try
                {
                    //toleratable errors counters
                    int noInternetTimes = 0, connectionResetTimes = 0, mainFrameErrorTimes = 0, tooLongToRespondTimes = 0;
                    for (int i = 0; i < tryouts; i++)
                    {
                        #region tryout logic = navigation + checking for stop, taleratable errors, and exceptional errors                                 
                        ChromeDriver.Url = URL;
                        WaitSmoothly.Do(delaySeconds);
                        if (ThreadSeeker.Stop) return false;

                        isExceptionalError(ChromeDriver.PageSource, ChromeDriver.Url);
                        if (isContinuableError(ref noInternetTimes, ref connectionResetTimes, ref mainFrameErrorTimes, ref tooLongToRespondTimes))
                            continue;
                        #endregion
                        //if no errors
                        return true;
                    }
                    #region build and throw Exception message, since I failed to download the page tryouts times
                    string message = "No connection after " + tryouts + " tryouts";
                    if (noInternetTimes > 0)
                        message += "\nNo internet connection times " + noInternetTimes;
                    if (connectionResetTimes > 0)
                        message += "\nConnection reset times " + connectionResetTimes;
                    if (mainFrameErrorTimes > 0)
                        message += "\nCommon error times " + mainFrameErrorTimes;
                    throw new Exception(message);
                    #endregion
                }
                catch (Exception e)
                {
                    throw new Exception("Cannot navigate chrome: " + e.Message);
                }
            }
            
            bool isContinuableError(ref int noInternetTimes, ref int connectionResetTimes, ref int mainFrameErrorTimes, ref int tooLongToRespond)
            {
                var cd = ChromeDriver;
                if (cd.PageSource.Contains("No internet"))
                {
                    noInternetTimes++;
                    return true;
                }
                if (cd.PageSource.Contains("The connection was reset."))
                {
                    connectionResetTimes++;
                    return true;
                }
                if (cd.PageSource.Contains("took too long to respond."))
                {
                    tooLongToRespond++;
                    return true;
                }

                try
                {
                    var mfe = FindLastOrDefaultElement("", "div", "main-frame-error");
                    if (mfe != null)
                    {
                        mainFrameErrorTimes++;
                        return true;
                    }
                }
                catch { }
                return false;
            }

            void isExceptionalError(string pageSource, string URL)
            {
                if (URL.StartsWith("blocked"))
                    throw new Exception("Proxy failed to fight through cencorship!");
                if (pageSource.Contains("unexpectedly closed the connection."))
                    throw new Exception("Site unexpectedly closed the connection.");
            }
            #endregion

            #region useful functionality for chromedriver

            /// <summary>
            /// At least 1 parameter must not be empty. Serches by Id first, then by tag, then by text. In the last case it returns the first occurence
            /// </summary>
            /// <param name="text"></param>
            /// <param name="tag"></param>
            /// <param name="id"></param>
            /// <returns></returns>
            public IWebElement FindLastOrDefaultElement(string text = "", string tag = "", string id = "")
            {
                if (id != "")
                {
                    return ChromeDriver.FindElementById(id);
                }
                if (tag != "")
                {
                    var elems = ChromeDriver.FindElements(By.TagName(tag));
                    if (text != "")
                        return elems.LastOrDefault(s => s.Text == text || s.Text.Trim() == text);
                    else
                        return elems.LastOrDefault();
                }
                var texts = ChromeDriver.FindElementsByXPath(string.Format("//*[contains(text(), '{0}')]", text));
                var lastText = texts.LastOrDefault();
                if (lastText != null)
                    return lastText;
                else
                    return null;
            }



            #endregion
        }

        public static List<ChromeDriverHelper> ChromeDriverHelpers { get; private set; } = new List<ChromeDriverHelper>();
        public static ChromeDriverHelper LastChromeDriverHelper { get { return ChromeDriverHelpers.LastOrDefault(); } }
        public static ChromeDriver LastChrome { get { return LastChromeDriverHelper.ChromeDriver; } }
        
        #region setting up and disposal

        static string[] _processesToCheck = {
        "chrome",
        "chromedriver",
        "conhost"};
        #region public static List<string> chromeDriversIds
        static List<string> lastCreatedDriverIds = new List<string>();
        ///filled at chromedriver initialization
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
        #endregion

        #region  public static ChromeDriverHelper chromedriver_set_up
        /// <summary>
        /// Returns a new chromedriver instance with given parameters, and saves its information.\n
        /// Each set_up must be followed by dispose or disposeAll.
        /// It's possible that some other chrome tabs(even in normal browsers) created during execution of this function are closed at later disposal
        /// It's better off launching projects with this function as administrator.
        /// To force so: create app.manifest and set it there <requestedExecutionLevel level = "requireAdministrator" uiAccess="false" />
        /// </summary>
        /// <param name="visible">False if chrome must run silently</param>
        /// <param name="ignoreSetificateErrors"></param>
        /// <param name="proxyServer">IPAddress:Port</param>
        /// <param name="proxyWithAuthentication"></param>
        /// <param name="extensionPaths">Leave it empty or null not to use a consistant chrome profile</param>
        /// <returns></returns>
        public static ChromeDriverHelper chromedriver_set_up(bool visible = true, bool ignoreSetificateErrors = true
            , string[] extensionPaths = null, string proxyServer = "", bool proxyWithAuthentication = false)
        {
            try
            {                
                //create instance
                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                var options = new ChromeOptions();
                #region set options
                options.AddArgument("--enable-extensions");
                options.AddArgument("--start-maximized");
                #region proxy
                if (proxyServer != "")
                {
                    Proxy proxy = new Proxy()
                    {
                        Kind = ProxyKind.Manual,
                        IsAutoDetect = false,
                        HttpProxy = proxyServer,//"127.0.0.1:3330",
                        SslProxy = proxyServer//"127.0.0.1:3330"
                    };
                    options.Proxy = proxy;
                    if (proxyWithAuthentication)
                        options.AddArguments("--proxy-server=http://user:password@yourProxyServer.com:8080");
                }
                #endregion
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
                #region set extensions
                try
                {
                    if (extensionPaths != null)
                        foreach (var extension in extensionPaths)
                            if (!string.IsNullOrEmpty(extension))
                                options.AddArgument("--load-extension=" + extension);
                    //options.AddExtension(extension);                            
                }
                catch (Exception e)
                {
                    throw new Exception("Can't set chrome extensions: " + e.Message);
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
                #endregion
                
                //get chrome related processes before initialization. Can throw error that access denied. Dealt away with running as administaror
                var chromeRelatedIds = Process.GetProcesses()
                    .Where(s => _processesToCheck.FirstOrDefault(x => s.ProcessName.ToLower().Contains(x)) != null)
                    .ToList();
                WaitSmoothly.Do(1.5);
                //initialize chromedriver
                var driverStartTime = Process.GetCurrentProcess().StartTime;

                ChromeDriver chrome = new ChromeDriver(service, options, new TimeSpan(0, 1, 0));
                WaitSmoothly.Do(1.5);
                //get chrome related processes after initialization. Can throw error that access denied. Dealt away with running as administaror
                var latestChromeRelatedIds = Process.GetProcesses()
                    .Where(s => _processesToCheck.FirstOrDefault(x => s.ProcessName.ToLower().Contains(x)) != null
                    && s.StartTime > driverStartTime)
                    .ToList();
                WaitSmoothly.Do(1.5);
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
                //save newly created processees' ids to a file
                File.AppendAllLines(Directory.GetCurrentDirectory() + "\\drivers.txt", lastCreatedDriverIds);
                return lastHelper;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        public static void chromedriver_dispose(ChromeDriverHelper helper)
        {
            string ids = string.Join(", ", helper.AttachedProcessesIds);
            try
            {
                helper.ChromeDriver.Quit();
                helper.AttachedProcessesIds.ForEach(s =>
                {
                    try
                    {
                        var proc = Process.GetProcessById(int.Parse(s));
                        killProc(proc);
                    }
                    catch(Exception e)
                    { }
                });
                helper.AttachedProcessesIds = new List<string>();
                File.WriteAllLines(Directory.GetCurrentDirectory() + "\\drivers.txt", chromeDriversIds);
                ChromeDriverHelpers.Remove(helper);
                helper = null;
            }
            catch (Exception e)
            {
                throw new Exception("ids(" + ids + "): " + e.Message);
            }
        }

        #region public static void chromedriver_disposeAll(int maximumTimeoutSeconds = 10)
        static int linesCounter = 0;
        static int linesCount = 0;
        delegate void voidProcessInt(Process process, int integer);

        public static void chromedriver_disposeAllAsync(int maximumTimeoutSeconds = 10)
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
            Thread finisher = new Thread(awaitThings);
            finisher.Start(maximumTimeoutSeconds);
        }
        static void awaitThings(object intSeconds)
        {
            if (intSeconds is int == false)
                return;
            int seconds = (int)intSeconds;

            var now = DateTime.Now;
            while (linesCounter != linesCount)
            {
                Application.DoEvents();
                Thread.Sleep(100);
                if (DateTime.Now - now > TimeSpan.FromSeconds(seconds))
                    break;
            }
            ChromeDriverHelpers = new List<ChromeDriverHelper>();
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\drivers.txt", chromeDriversIds);
        }
        #endregion

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
    }
}
