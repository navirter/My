using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using System.Reflection;



namespace My
{
    public static class GeneralFunctions
    {
        public static class DateAndTime
        {
            public static readonly string dateTimeFormat = "yyyy.MM.dd: hh.mm.ss";
            public static readonly string dateFormat = "yyyy.MM.dd";
            public static readonly string timeSpanFormat = "dd: hh.mm.ss";

            /// <summary>
            /// Converts "yyyy.MM.dd: HH.mm.ss" or "yyyy.MM.dd" to DateTime. Ignores all exept for digits, "." and ":"
            /// </summary>
            /// <param name="source"></param>
            /// <param name="withTime"></param>
            /// <returns></returns>
            public static DateTime convertStringToDateTime(string source, bool withTime = true)
            {
                try
                {

                    string nsource = new string(source.ToCharArray().Where(s => Char.IsDigit(s) || s == '.' || s == ':').ToArray());
                    string[] parts = nsource.Split(new[] { ".", ":" }, StringSplitOptions.RemoveEmptyEntries);
                    int year = int.Parse(parts[0]);
                    int month = int.Parse(parts[1]);
                    int day = int.Parse(parts[2]);
                    int hour = 0; int minute = 0; int second = 0;
                    if (withTime)
                    {
                        int.TryParse(parts[3], out hour);
                        int.TryParse(parts[4], out minute);
                        try { int.TryParse(parts[5], out second); } catch { }
                    }
                    DateTime res = new DateTime(year, month, day, hour, minute, second);
                    return res;
                }
                catch { string full = ""; if (withTime) full = "full"; throw new Exception("can't " + full + " read datetime " + source); }
            }
            /// <summary>
            /// Converts "dd.HH.mm.ss" or "yyyy.MM.dd" to TimeSpan. Ignores all exept for digits, "." and ":"
            /// </summary>
            /// <param name="source"></param>
            /// <returns></returns>
            public static TimeSpan convertStringToTimeSpan(string source)
            {
                try
                {
                    string nsource = new string(source.ToCharArray().Where(s => Char.IsDigit(s) || s == '.' || s == ':').ToArray());
                    string[] parts = nsource.Split(new[] { ".", ":" }, StringSplitOptions.RemoveEmptyEntries);
                    int day = Convert.ToInt32(double.Parse(parts[0]));
                    int hour = 0; int.TryParse(parts[1], out hour);
                    int minute = 0; int.TryParse(parts[2], out minute);
                    int second = 0; try { int.TryParse(parts[3], out second); } catch { }
                    return new TimeSpan(day, hour, minute, second);
                }
                catch { throw new Exception("can't read timespan " + source); }
            }
            /// <summary>
            /// Format: yyyy.MM.dd: hh.mm.ss
            /// </summary>
            /// <param name="time"></param>
            /// <param name="withTime"></param>
            /// <returns></returns>
            public static string convertDateTimeToString(DateTime time, bool withTime = true, bool with_words = false)
            {
                string year = time.Year.ToString();
                while (year.Length < 4) year = "0" + year;
                string month = time.Month.ToString();
                if (month.Length == 1) month = "0" + month;
                string day = time.Day.ToString();
                if (day.Length == 1) day = "0" + day;
                string hour = time.Hour.ToString();
                if (hour.Length == 1) hour = "0" + hour;
                string minute = time.Minute.ToString();
                if (minute.Length == 1) minute = "0" + minute;
                string second = time.Second.ToString();
                if (second.Length == 1) second = "0" + second;

                if (!with_words)
                {
                    string res = year + "." + month + "." + day;
                    if (withTime) res += ": " + hour + "." + minute + "." + second;
                    return res;
                }
                else
                {
                    string res = year + "г." + month + "м." + day + "д.";
                    if (withTime) res += ": " + hour + "ч." + minute + "м." + second + "с.";
                    return res;
                }
            }
            /// <summary>
            /// Format: dd: hh.mm.ss
            /// </summary>
            /// <param name="time"></param>
            /// <returns></returns>
            public static string convertTimeSpanToString(TimeSpan time, bool with_words = false)
            {
                string day = Convert.ToInt32(Math.Truncate(time.TotalDays)).ToString();
                while (day.Length < 4) day = "0" + day;
                string hour = time.Hours.ToString();
                if (hour.Length == 1) hour = "0" + hour;
                string minute = time.Minutes.ToString();
                if (minute.Length == 1) minute = "0" + minute;
                string second = time.Seconds.ToString();
                if (second.Length == 1) second = "0" + second;
                if (!with_words)
                {
                    string res = day + ": " + hour + "." + minute + "." + second;
                    return res;
                }
                else
                {
                    string res = day + "д.: " + hour + "ч." + minute + "м." + second + "с.";
                    return res;
                }
            }            
        }

        public static class Web
        {
            public static Image downloadImageFromURL(string url)
            {
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] data = webClient.DownloadData(url);

                        using (MemoryStream mem = new MemoryStream(data))
                        {
                            using (var yourImage = Image.FromStream(mem))
                            {
                               return yourImage;
                            }
                        }
                    }
            }

            public static ChromeDriver chromedriver_set_up( out Process process, bool visible = true)
            {
                    var service = ChromeDriverService.CreateDefaultService();
                    var options = new ChromeOptions();
                    if (!visible)
                        options.AddArgument("headless");
                    service.HideCommandPromptWindow = true;
                    //options.AddArgument("--window-position=-32000,-32000");
                    ChromeDriver chrome = new ChromeDriver(service, options);
                    int id = service.ProcessId;
                    process = Process.GetProcessById(id);
                    return chrome;
            }

            public static void chromedriver_dispose(ChromeDriver chrome, int process_id)
            {
                    chrome.Close();
                    var chromeProc = Process.GetProcessById(process_id);
                    chromeProc.Kill();
                chrome = null;
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

        public static class IO
        {
            /// <summary>
            /// Writes the given object instance to an XML file.
            /// It FAILS at writing a TimeSpan instance. Even when they are a field
            /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
            /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
            /// <para>Object type must have a parameterless constructor.</para>            /// 
            /// </summary>
            /// <typeparam name="T">The type of object being written to the file.</typeparam>
            /// <param name="filePath">The file path to write the object instance to.</param>
            /// <param name="objectToWrite">The object instance to write to the file.</param>
            /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
            public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
            {
                TextWriter writer = null;
                try
                {
                    var serializer = new XmlSerializer(typeof(T));
                    writer = new StreamWriter(filePath, append, Encoding.GetEncoding(1251));
                    serializer.Serialize(writer, objectToWrite);
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }

            /// <summary>
            /// Reads an object instance from an XML file.
            /// <para>Object type must have a parameterless constructor.</para>
            /// </summary>
            /// <typeparam name="T">The type of object to read from the file.</typeparam>
            /// <param name="filePath">The file path to read the object instance from.</param>
            /// <returns>Returns a new instance of the object read from the XML file.</returns>
            public static T ReadFromXmlFile<T>(string filePath) where T : new()
            {
                TextReader reader = null;
                try
                {
                    var serializer = new XmlSerializer(typeof(T));
                    reader = new StreamReader(filePath, Encoding.GetEncoding(1251));
                    return (T)serializer.Deserialize(reader);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }

            public static string CurrentDirectory { get; private set; } = Directory.GetCurrentDirectory();
            public static string ApplicationData { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);            
        }

        public static class Copier
        {
            public static void copyFields<T>(T source, T destination, bool onlyPublic = false)
            {
                if (source.GetType() != destination.GetType())
                    throw new InvalidCastException("Types of objects are not the same!");
                if (!onlyPublic)
                {
                    var sourceProperties = source.GetType().GetProperties();
                    var destinationProperties = destination.GetType().GetProperties();
                    foreach (var sp in sourceProperties)
                        foreach (var dp in destinationProperties)
                            if (sp.Name == dp.Name && sp.PropertyType == dp.PropertyType)
                            {
                                dp.SetValue(destination, sp.GetValue(source));
                                break;
                            }
                }
                else
                {
                    var sourceProperties = source.GetType().GetProperties(BindingFlags.Public);
                    var destinationProperties = destination.GetType().GetProperties(BindingFlags.Public);
                    foreach (var sp in sourceProperties)
                        foreach (var dp in destinationProperties)
                            if (sp.Name == dp.Name && sp.PropertyType == dp.PropertyType)
                            {
                                dp.SetValue(destination, sp.GetValue(source));
                                break;
                            }
                }
            }
        }

        public static class Diagnostics
        {
            public static TimeSpan getIdleTime()
            {
                return IdleTimeFinder.getTimeSpent(IdleTimeFinder.GetIdleTime());
            }
            #region getting idle time

            internal struct LASTINPUTINFO
            {
                public uint cbSize;

                public uint dwTime;
            }
            /// <summary>
            /// Helps to find the idle time, (in milliseconds) spent since the last user input
            /// </summary>
            class IdleTimeFinder
            {
                [DllImport("User32.dll")]
                private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

                [DllImport("Kernel32.dll")]
                private static extern ulong GetLastError();

                public static uint GetIdleTime()
                {
                    LASTINPUTINFO lastInPut = new LASTINPUTINFO();
                    lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
                    GetLastInputInfo(ref lastInPut);

                    return ((uint)Environment.TickCount - lastInPut.dwTime);
                }
                /// <summary>
                /// Get the Last input time in milliseconds
                /// </summary>
                /// <returns></returns>
                public static long GetLastInputTime()
                {
                    LASTINPUTINFO lastInPut = new LASTINPUTINFO();
                    lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
                    if (!GetLastInputInfo(ref lastInPut))
                    {
                        throw new Exception(GetLastError().ToString());
                    }
                    return lastInPut.dwTime;
                }
                public static TimeSpan getTimeSpent(long ticks)
                {
                    return TimeSpan.FromMilliseconds(ticks);
                }
            }

            #endregion

            public static int get_cpu_percent_usage(Process p)
            {
                PerformanceCounter pc = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
                float fcpu = pc.NextValue();
                int cpu = Convert.ToInt32(fcpu);
                return cpu;
            }

            public static long get_memory_mb_usage(Process p)
            {
                PerformanceCounter pc = new PerformanceCounter("Process", "Working Set", p.ProcessName);
                long memory = pc.RawValue / 1024 / 1024;
                return memory;
            }
        }

        public static class Sorting
        {
            #region merge_sort
            public static IEnumerable<T> merge_sort<T>(IEnumerable<T> source) where T:IComparable
            {
                int count = source.Count();
                if (count <= 1)
                    return source;

                int half = count / 2;
                var left = source.Take(half);
                var right = source.Skip(half);

                left = merge_sort(left);
                right = merge_sort(right);

                return merge_parts(left, right);
            }
            static IEnumerable<T> merge_parts<T>(IEnumerable<T> left, IEnumerable<T> right) where T:IComparable
            {
                List<T> list_left = left.ToList(), list_right = right.ToList();      
                int total_count = list_left.Count() + list_right.Count();

                var result = new T[total_count];

                int left_indexer = 0;
                int right_indexer = 0;
                int result_indexer = 0;

                while (left_indexer < list_left.Count && right_indexer < list_right.Count)
                {
                    if (list_left[left_indexer].CompareTo(list_right[right_indexer]) <= 0)
                    {
                        result[result_indexer] = list_left[left_indexer];
                        result_indexer++; left_indexer++;
                    }
                    else
                    {
                        result[result_indexer] = list_right[right_indexer];
                        result_indexer++; right_indexer++;
                    }
                }
                while (left_indexer < list_left.Count)
                {
                    result[result_indexer] = list_left[left_indexer];
                    result_indexer++; left_indexer++;
                }
                while (right_indexer < list_right.Count)
                {
                    result[result_indexer] = list_right[right_indexer];
                    result_indexer++; right_indexer++;
                }
                return result;
            }
            #endregion
        }
    }
}
