﻿using OpenQA.Selenium;
using OpenQA.Selenium.Html5;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using My.Waiting;
//using System.Windows.Forms;

namespace My.Web
{
    public class Chrome 
    {
        public class ChromeDriverHelper
        {
            #region fields
            public List<string> VisitedUrls { get; private set; } = new List<string>();
            public ChromeDriver ChromeDriver { get; set; }
            public List<string> AttachedProcessesIds { get; set; }
            public string CurrentUrl
            {
                get
                {
                    string url = ChromeDriver.Url;
                    return url;
                }
                set
                {
                    string url = value;
                    ChromeDriver.Url = url;
                    VisitedUrls.Add(ChromeDriver.Url);
                }
            }
            public class ChromeDriverInfo
            {
                public ChromeDriverInfo(bool HasApplicationCache, ICapabilities Capabilities, IReadOnlyCollection<string> WindowHandles
                    , string CurrentWindowHandle, IFileDetector FileDetector, bool HasLocationContext, bool HasWebStorage, bool IsActionExecutor,
                     string PageSource, SessionId SessionID, string Title, string Url)
                {

                    #region hasApplicationCache
                    try
                    {
                        this.HasApplicationCache = false;
                        this.HasApplicationCache = HasApplicationCache;
                    }
                    catch { }
                    #endregion
                    #region capabilities
                    try
                    {
                        this.Capabilities = null;
                        this.Capabilities = Capabilities;
                    }
                    catch { }
                    #endregion
                    #region windowHandles
                    try
                    {
                        this.WindowHandles = null;
                        this.WindowHandles = WindowHandles;
                    }
                    catch { }
                    #endregion
                    #region currentWindowHandle
                    try
                    {
                        this.CurrentWindowHandle = null;
                        this.CurrentWindowHandle = CurrentWindowHandle;
                    }
                    catch { }
                    #endregion
                    #region fileDetector
                    try
                    {
                        this.FileDetector = null;
                        this.FileDetector = FileDetector;
                    }
                    catch { }
                    #endregion
                    #region hasLocationContext
                    try
                    {
                        this.HasLocationContext = false;
                        this.HasLocationContext = HasLocationContext;
                    }
                    catch { }
                    #endregion
                    #region hasWebStorage
                    try
                    {
                        this.HasWebStorage = false;
                        this.HasWebStorage = HasWebStorage;
                    }
                    catch { }
                    #endregion
                    #region isActionExecutor
                    try
                    {
                        this.IsActionExecutor = false;
                        this.IsActionExecutor = IsActionExecutor;
                    }
                    catch { }
                    #endregion
                    #region pageSource
                    try
                    {
                        this.PageSource = null;
                        this.PageSource = PageSource;
                    }
                    catch { }
                    #endregion
                    #region sessionId
                    try
                    {
                        this.SessionID = null;
                        this.SessionID = SessionID;
                    }
                    catch { }
                    #endregion
                    #region title
                    try
                    {
                        this.Title = null;
                        this.Title = Title;
                    }
                    catch { }
                    #endregion
                    #region UrlBefore
                    try
                    {
                        this.UrlBefore = null;
                        this.UrlBefore = Url;
                    }
                    catch { }
                    #endregion
                }
                public bool HasApplicationCache { get; private set; }
                public ICapabilities Capabilities { get; private set; }
                public IReadOnlyCollection<string> WindowHandles { get; private set; }
                public string CurrentWindowHandle { get; private set; }
                public IFileDetector FileDetector { get; private set; }
                public bool HasLocationContext { get; private set; }
                public bool HasWebStorage { get; private set; }
                public bool IsActionExecutor { get; private set; }
                //public ILocationContext LocationContext { get; set; }
                //public ChromeNetworkConditions NetworkConditions { get; set; }
                public string PageSource { get; private set; }
                public SessionId SessionID { get; private set; }
                public string Title { get; private set; }
                public string UrlBefore { get; private set; }
                public string UrlAfter { get; internal set; }
                //public IWebStorage WebStorage { get; set; }
            }
            //public ChromeDriverInfo CurrentChromeDriverInfo { get; private set; }

            #endregion

            #region Navigate

            public bool NavigateIfNewPage(string URL)
            {
                return NavigateIfNewPage(URL, 1, 1);
            }

            public bool NavigateIfNewPage(string URL, double delaySeconds, int tryouts = 1)
            {
                validate(URL, delaySeconds, tryouts);
                string curUrl = this.CurrentUrl;
                if (curUrl != URL)
                    return Navigate(URL, delaySeconds, tryouts);
                else
                    return true;
            }

            void validate(string URL, double delaySeconds, int tryouts)
            {
                if (this.ChromeDriver is null)
                    throw new NullReferenceException("ChromeDriver is null!");
                string currentUrl = this.CurrentUrl;
                if (currentUrl is null)
                    throw new NullReferenceException("ChromeDriver.Url is null!");
                if (URL is null)
                    throw new NullReferenceException("target url is null!");
                if (delaySeconds <= 0 || tryouts <= 0)
                    throw new ArgumentOutOfRangeException("Delay and tryouts cannot be less or equal 0");
            }

            public bool Navigate(string URL)
            {
                return Navigate(URL, 1, 1);
            }

            /// <summary>
            /// Smooth chrome navigation. Can be stopped.
            /// </summary>
            /// <param name="URL"></param>
            /// <param name="delaySeconds"></param>
            /// <param name="tryouts"></param>
            /// <returns></returns>
            public bool Navigate(string URL, double delaySeconds, int tryouts = 1)
            {
                try
                {
                    //toleratable errors counters
                    int noInternetTimes = 0, connectionResetTimes = 0, mainFrameErrorTimes = 0, tooLongToRespondTimes = 0;
                    var cd = ChromeDriver;
                    for (int i = 0; i < tryouts; i++)
                    {
                        this.CurrentUrl = URL;
                        string cururl = this.CurrentUrl;
                        VisitedUrls.Add(cururl);
                        Wait.Do(delaySeconds);
                        if (Wait.Stop) return false;

                        isExceptionalError(cd.PageSource, cd.Url);
                        if (isContinuableError(ref noInternetTimes, ref connectionResetTimes, ref mainFrameErrorTimes, ref tooLongToRespondTimes))
                            continue;
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

            public void SelectChromeHandle(int number = 0, bool closeOther = false)
            {
                var windows = ChromeDriver.WindowHandles.Select(s => s).ToList();
                if (windows.Count > 1 && closeOther)
                    for (int i = windows.Count - 1; i > 0; i--)
                        try
                        {
                            ChromeDriver = ChromeDriver.SwitchTo().Window(windows[i]) as ChromeDriver;
                            ChromeDriver.Close();
                            ChromeDriver = ChromeDriver.SwitchTo().Window(windows[i - 1]) as ChromeDriver;
                        }
                        catch { }
                else
                {
                    ChromeDriver = ChromeDriver.SwitchTo().Window(windows[number]) as ChromeDriver;
                }
            }

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

            public void OpenNewTab()
            {                
                ((IJavaScriptExecutor)ChromeDriver).ExecuteScript("window.open();"); //to be extracted to the library
            }

            #endregion
        }
        public class ChromeDriverSetUpOptions
        {
            public bool Visible = true;
            public bool IgnoreSertivicateErrors = true;
            public string[] ExtensionsPaths = null;
            public string ProxyServer = null;
            public string ProxyLogin = null;
            public string ProxyPassword = null;
            public TimeSpan CommandTimeout = new TimeSpan();
        }

        public static List<ChromeDriverHelper> ChromeDriverHelpers { get; private set; } = new List<ChromeDriverHelper>();
        public static ChromeDriverHelper LastChromeDriverHelper { get { return ChromeDriverHelpers.LastOrDefault(); } }
        public static ChromeDriver LastChrome { get { return LastChromeDriverHelper.ChromeDriver; } }

        #region setting up and disposal

        static string[] _processesToCheck = {
        "chrome",
        "chromedriver",
        "conhost"
        };
        #region public static List<string> chromeDriversIds
        static List<string> lastCreatedDriverIds = new List<string>();
#pragma warning disable IDE1006 // Naming Styles
        ///filled at chromedriver initialization
        /// <summary>
        /// All attached processes of all helpers
        /// </summary>
        public static List<string> chromeDriversIds
#pragma warning restore IDE1006 // Naming Styles
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
        /// Returns a new chromedriver instance with given parameters, and saves its information.
        /// Each set_up must be followed by dispose or disposeAll.
        /// It's possible that some other chrome tabs(even in normal browsers) created during execution of this function are closed at later disposal
        /// It's better off launching projects with this function as administrator.
        /// To force so: create app.manifest and set it there: tag + "requestedExecutionLevel level = "requireAdministrator" uiAccess="false" />"
        /// /**/
        /// </summary>
        /// <param name="visible">False if chrome must run silently</param>
        /// <param name="ignoreSetificateErrors"></param>
        /// <param name="proxyServer">IPAddress:Port</param>
        /// <param name="proxyWithAuthentication"></param>
        /// <param name="extensionPaths">Leave it empty or null not to use a consistant chrome profile</param>
        /// <returns></returns>
        public static ChromeDriverHelper ChromedriverSetUp(ChromeDriverSetUpOptions setUpOptions)
        {
            try
            {
                #region create service and options
                //create instance
                var service = ChromeDriverService.CreateDefaultService();
                //service.HideCommandPromptWindow = true;
                var isRunning = service.IsRunning;

                var options = new ChromeOptions();
                #region set options
                try
                {

                    options.AddArgument("--enable-extensions");
                    options.AddArgument("--start-maximized");
                    #region proxy
                    if (!string.IsNullOrEmpty(setUpOptions.ProxyServer))
                    {
                        Proxy proxy = new Proxy()
                        {
                            Kind = ProxyKind.Manual,
                            IsAutoDetect = false,
                            SocksUserName = setUpOptions.ProxyLogin != null ? setUpOptions.ProxyLogin : "",
                            SocksPassword = setUpOptions.ProxyPassword != null ? setUpOptions.ProxyPassword : "",
                            HttpProxy = setUpOptions.ProxyServer,//"127.0.0.1:3330",
                            SslProxy = setUpOptions.ProxyServer//"127.0.0.1:3330"
                        };
                        options.Proxy = proxy;
                    }
                    #endregion
                    #region set visibility
                    try
                    {
                        if (!setUpOptions.Visible)
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
                        if (setUpOptions.ExtensionsPaths != null)
                            foreach (var extension in setUpOptions.ExtensionsPaths)
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
                        if (setUpOptions.IgnoreSertivicateErrors)
                            options.AddArgument("--ignore-certificate-errors");
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Can't set ignore certificate errors. " + e.Message);
                    }
                    #endregion
                }
                catch (Exception e)
                { throw new Exception("Can't set options: " + e.Message); }
                #endregion
                #endregion
                
                var chromeRelatedIds = getChromeRelatedProcesses();
                //Wait.Do(1.5);
                //initialize chromedriver

                ChromeDriver chrome = null;
                //chrome = new ChromeDriver();//debug
                if (setUpOptions.CommandTimeout != new TimeSpan())
                    chrome = new ChromeDriver(service, options, setUpOptions.CommandTimeout);
                else
                    chrome = new ChromeDriver(service, options);
                //chrome.Manage().Cookies.DeleteAllCookies();
                Wait.Do(1.5);
                if (chrome is null || chrome.Url is null)
                    throw new Exception("Fail to create Chrome!");
                //get chrome related processes after initialization. Can throw error that access denied. Dealt away with running as administaror
                var latestChromeRelatedIds = getChromeRelatedProcesses();
                Wait.Do(1.5);
                //save newly created processes' ids to list
                lastCreatedDriverIds = new List<string>();
                lastCreatedDriverIds = latestChromeRelatedIds
                    .Where(s => !chromeRelatedIds.Any(a => a.Id == s.Id))
                    .Select(s => s.Id.ToString())
                    .ToList();
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
        static List<Process> getChromeRelatedProcesses()
        {
            try
            {
                return Process.GetProcesses()
                    .Where(s => _processesToCheck.FirstOrDefault(x => s.ProcessName.ToLower().Contains(x)) != null)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception("Can't get processes: " + e.Message);
            }
        }
        #endregion

        public static void ChromedriverDispose(ChromeDriverHelper helper)
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
                    catch
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

        #region public static void Chromedriver_disposeAll(int maximumTimeoutSeconds = 10)
        static int linesCounter = 0;
        static int linesCount = 0;
        delegate void voidProcessInt(Process process, int integer, bool fromFile);

        public static void ChromedriverDisposeAll(int maximumTimeoutSeconds = 10)
        {
            #region kill processes from file
            try
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
                        vpi.BeginInvoke(p, 2000, true, null, null);
                    }
                    catch
                    {
                    }
            }
            catch
            { }
            #endregion
            #region kill conhost
            var processes = Process.GetProcesses();
            var conhosts = processes.Where(s=> s.ProcessName.Contains("conhost")).ToList();
            foreach (var v in conhosts)
                try
                {
                    voidProcessInt vpi = killProc;
                    vpi.BeginInvoke(v, 2000, false, null, null);
                }
                catch { }
            #endregion
            #region kill chromedriver
            var chromedriver = Process.GetProcesses().Where(s => s.ProcessName.Contains("chromedriver")).ToList();
            foreach (var v in chromedriver)
                try
                {
                    voidProcessInt vpi = killProc;
                    vpi.BeginInvoke(v, 2000, false, null, null);
                }
                catch { }
            #endregion
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
                Thread.Sleep(100);
                if (DateTime.Now - now > TimeSpan.FromSeconds(seconds))
                    break;
            }
            ChromeDriverHelpers = new List<ChromeDriverHelper>();
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\drivers.txt", chromeDriversIds);
        }
        #endregion

        static void killProc(Process process, int timeOutMilliseconds = 2000, bool fromFile = false)
        {
            try
            {
                //process.WaitForExit(timeOutMilliseconds);

                //if (!process.HasExited)
                //{
                //    if (process.Responding)
                //        process.CloseMainWindow();
                //}
                process.Kill();
            }
            catch(Exception e)
            { }
            finally
            {
                if (fromFile)
                    linesCounter++;
            }

        }

        #endregion
    }
}
