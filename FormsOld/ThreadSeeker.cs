﻿using System;
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
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using My.DateAndTime;
using My.Diagnostics;
using My.Extensions.StringExtensions;


namespace My.Forms
{
    public partial class ThreadSeeker : UserControl, IDisposable
    {

        /// <summary>
        /// part of program may be empty
        /// </summary>
        public class ThreadSeekerColorisationSetting
        {
            public string partOfProgram = "";
            public string probablyContainedString = "";
            public Color colorToBeShown;
            public ThreadSeekerColorisationSetting(string PartOfProgram, string ProbablyContainedString, Color ColorToBeShown)
            {
                if (colorToBeShown == Color.Purple)
                    throw new Exception("Нельзя использовать Color.Purple для ThreadSeeker");
                partOfProgram = PartOfProgram;
                probablyContainedString = ProbablyContainedString;
                colorToBeShown = ColorToBeShown;
            }
        }
        public enum Side
        {
            Left, 
            Right, 
            Fill
        }

        public class Settings
        {
            public readonly Form Owner = null;
            public List<ThreadSeekerColorisationSetting> Colorization = null;

            #region public int DangerMemoryLoadoutMegabytes = 1000 and must be >0
            public int DangerMemoryLoadoutMegabytes
            {
                get
                {
                    return dangerMemoryLoadoutMegabytes;
                }
                set
                {
                    if (value <= 0)
                        throw new IndexOutOfRangeException("dangerMemoryLoadoutMegabytes cannot be 0 or less!");
                    dangerMemoryLoadoutMegabytes = value;
                }
            }
            int dangerMemoryLoadoutMegabytes = 1000;
            #endregion
            #region public int ReshowDelaySeconds = 30 and must be > 0
            public int ReshowDelaySeconds
            {
                get
                {
                    return reshowDelaySeconds;
                }
                set
                {
                    if (value <= 0)
                        throw new IndexOutOfRangeException("No int can be qual or less that 0");
                    reshowDelaySeconds = value;
                }
            }
            int reshowDelaySeconds = 30;
            #endregion
            #region public Graphics Graphics = null;
            public Graphics Graphics
            {
                get
                {
                    return graphics;
                }
                set
                {
                    graphics = value;
                }
            }
            Graphics graphics = null;
            #endregion
            public bool ShowSleepingInfo = false;
            public int UnpaseIfIdleMinutes = 1000000;
            public bool StartTimeframeRecalibration = false;
            public bool ShowIllClosedMessage = false;

            public Settings(Form owner, List<ThreadSeekerColorisationSetting> colorization)
            {
                Owner = owner ?? throw new NullReferenceException("owner cannot be null!");
                Colorization = colorization ?? throw new NullReferenceException("colorization cannot be null!");
                if(colorization.Any(s => s == null))
                    throw new NullReferenceException("colorization cannot be null!");
            }
        }

        public class Graphics
        {
            public int PositionX = 0;
            public int PositionY = 0;
            [Range(350, 50000)]
            public int Width = 350;
            [Range(600, 50000)]
            public int Height = 600;
            public AnchorStyles Anchors;
            public Side Side;


            public Graphics(int positionX, int positionY, int Width, int Height, AnchorStyles anchors, Side side)
            {
                this.PositionX = positionX;
                this.PositionY = positionY;
                this.Width = Width;
                this.Height = Height;
                this.Anchors = anchors;
                this.Side = side;
            }
        }

        public class Unit
        {
            public DateTime dateTime;
            public int cpu_usage;
            public long memory_usage;
            public bool is_important;
            public bool is_system_message;
            public string part;
            public string value;
            public Unit() { }
            public Unit(DateTime DateTime, int Cpu_usage, long Memory_usage, bool Is_important
                , bool Is_system_message, string Part, string Value)
            {
                this.dateTime = DateTime;
                this.cpu_usage = Cpu_usage;
                this.memory_usage = Memory_usage;
                this.is_important = Is_important;
                this.is_system_message = Is_system_message;
                this.part = Part;
                this.value = Value;
            }
            /// <summary>
            /// overridden
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string diagnostics = cpu_usage + "% " + memory_usage + " мб";//"100% 1000 mb"=12 char           
                while (diagnostics.Length < "100% 1000 mb_".Length)
                    diagnostics += "_";
                string impostring = "";
                if (is_system_message) impostring = "[system]";
                if (is_important) impostring += "[important]";
                string date = ConvertDateTimeToString.Do(dateTime);
                while (date.Length < "2018.07.14: 18.23.24_".Length) date = date + "_";
                string prog_part = "[" + part + "]";
                prog_part = "[" + part.Replace("__", "_") + "]";
                string message =
                     date
                    + diagnostics
                    + impostring
                    + prog_part
                    + value;
                return message;
            }
            /// <summary>
            /// A shortened version of regular ToString()
            /// </summary>
            /// <returns></returns>
            public string ToStringShort()
            {
                string impostring = "";
                if (is_system_message) impostring = "[system]";
                if (is_important) impostring += "[important]";
                string hour = dateTime.Hour.ToString(); if (hour.Length < 2) hour = "0" + hour;
                string minute = dateTime.Minute.ToString(); if (minute.Length < 2) minute = "0" + minute;
                string date = hour + ":" + minute;
                while (date.Length < "18.23_".Length) date = date + "_";
                string prog_part = "[" + part + "]";
                prog_part = "[" + part.Replace("__", "_") + "]";
                string message =
                     date
                    + impostring
                    + prog_part
                    + value;
                return message;
            }
            public static Unit Parse(string s)
            {
                //{MES}2018.07.06.00.53.59_0% 34 мб__________[system][worker.read_info]done
                try
                {
                    string[] splits = s.Replace("\r\n", "").Split(new[] { "мб" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] left_parts = splits[0].Split(new[] { "_", "%" }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime dt = DateAndTime.ConvertStringToDateTime.Do(left_parts[0]);
                    //for some reason the correct value from func is nor assigned to valueable
                    int cpu = Convert.ToInt32(left_parts[1]);
                    long mem = Convert.ToInt64(left_parts[2]);
                    bool system = false;
                    if (splits[1].Contains("[system]"))
                        system = true;
                    bool important = false;
                    if (splits[1].Contains("[important]"))
                        important = true;
                    string part_and_mes = splits[1].Substring(splits[1].IndexOf("[")).Replace("[system]", "").Replace("[important]", "");
                    string[] pams = part_and_mes.Split(new[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                    string part = pams[0];
                    string mes = pams[1];
                    Unit u = new Unit(dt, cpu, mem, important, system, part, mes);
                    return u;
                }
                catch { return null; }
            }
        }


        #region (un)initialisation

        

        public ThreadSeeker(Settings settings)
        {
            CheckForIllegalCrossThreadCalls = false;
            this.settings = settings;
            #region launch and initialisations         
            #region graphics
            var g = settings.Graphics;
            int x = g.PositionX;
            if (x <= 0) x = 0;
            int y = g.PositionY;
            if (y <= 0) y = 0;
            if (g.Side == Side.Left)
                x = g.PositionX;
            else if (g.Side == Side.Right)            
                x = settings.Owner.Width - x;              
            
            this.Location = new System.Drawing.Point(x, y);
            if (g.Side == Side.Fill)
                this.Dock = DockStyle.Fill;
            this.Size = new System.Drawing.Size(g.Width, g.Height);
            this.Anchor = g.Anchors;
            settings.Owner.Controls.Add(this);
            InitializeComponent();
            this.Show();
            #endregion
            if (settings.StartTimeframeRecalibration)
                button5.PerformClick();
            _start = DateTime.Now;
            #endregion
            #region read things
            _threads = new List<List<Unit>>();
            #region read last settings
            try
            {
                string[] lastSettings = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\ThreadSeeker.txt", Encoding.GetEncoding(1251));
                Stb.Text = lastSettings[0];
                POtb.Text = lastSettings[1];
            }
            catch { }
            #endregion
            string[] paths = getAllFilesInPeriod();
            if (paths.Length > 0)
            {
                using (Loading_progress tslp = new Loading_progress(paths.Length, "Логи"))
                {
                    tslp.Show();
                    for (int iter = 0; iter < paths.Length; iter++)
                    {
                        var oneDate = tryReadSelectedFile(paths[iter]);
                        _threads.Add(oneDate.ToList());
                        tslp.reshow_progress(iter);
                    }
                }
            }
            #endregion     
            if (settings.ShowIllClosedMessage)
                manageUnpropperClosing();
            initialized = true;
            #region manage threads
            try { _threadRenewData.Abort(); } catch { }
            _threadRenewData = new Thread(renewData);
            _threadRenewData.Name = "ThreadSeeker";
            _threadRenewData.Start();
            try { _threadRenewSleepAndCurrentActivity.Abort(); } catch { }
            _threadRenewSleepAndCurrentActivity = new Thread(renewSleepAndCurrentActivity);
            _threadRenewSleepAndCurrentActivity.Name = "SleepSeeker";
            _threadRenewSleepAndCurrentActivity.Start();
            #endregion
            pause_button = button4;
            threadSeeker = this;
            string part = "ThreadSeeker.start";
            addMessage(part, "__________________________________________________", true);
            addMessage(part, "Логгер запущен", true, false, true);
        }
        void manageUnpropperClosing()
        {
            bool closedCorrectly = false;
            for (int i = _threads.Count - 1; i >= 0; i--)
            {
                for (int j = _threads[i].Count - 1; j >= 0; j--)
                {
                    if (_threads[i][j].part.Contains("ThreadSeeker.closing"))
                    {
                        closedCorrectly = true;
                        break;
                    }
                    if (_threads[i][j].part.Contains("ThreadSeeker.start"))
                        break;
                }
            }
            if (!closedCorrectly) MessageBox.Show("Программа не была закрыта корректно");
        }
        
        /// <summary>
        /// must be used to stop cycles
        /// </summary>
        public void close()
        {
            addMessage("ThreadSeeker.closing", "Логгер закрыт", true, false, false);
            _closing = true;            
            try { _threadRenewData.Abort(); } catch { }
            try { _threadRenewSleepAndCurrentActivity.Abort(); } catch { }
            try
            {
                Dispose(true);
            }
            catch { }
        }
        ~ThreadSeeker()
        {
            close();
        }        

        string[] getAllFilesInPeriod()
        {
            List<string> res = new List<string>();
            try
            {
                DateTime start = new DateTime();
                DateTime end = new DateTime();
                bool bondsActive = false;
                #region initialise dates
                try
                {
                    string stext = Stb.Text;
                    string etext = POtb.Text;
                    string[] stexts = stext.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    string[] etexts = etext.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    start = new DateTime(int.Parse(stexts[0]), int.Parse(stexts[1]), int.Parse(stexts[2]), 0, 0, 0);
                    end = new DateTime(int.Parse(etexts[0]), int.Parse(etexts[1]), int.Parse(etexts[2]), 0, 0, 0);
                    bondsActive = true;
                }
                catch { }
                #endregion
                string logsPath = Directory.GetCurrentDirectory() + "\\logs";
                string[] files = Directory.GetFiles(logsPath, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                    try
                    {
                        if (bondsActive)
                        {
                            string[] parts = file.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                            int last = parts.Length - 1;
                            int day = parts[last].GetFirstDigits();
                            int month = parts[last - 1].GetFirstDigits();
                            int year = parts[last - 2].GetFirstDigits();
                            DateTime fileDate = new DateTime(year, month, day);
                            if (fileDate >= start && fileDate <= end)
                                res.Add(file);
                        }
                        else
                            res.Add(file);
                    }
                    catch (Exception e)
                    {
                        addMessage(file + ": " + e.Message, "ThreadSeeker", true, true, false);
                    }
            }
            catch { }
            return res.ToArray();
        }
        string getLastFile()
        {
            try
            {
                string logsPath = Directory.GetCurrentDirectory() + "\\logs";
                string[] years = Directory.GetDirectories(logsPath);
                if (years.Length == 0) { Thread.Sleep(15000); return ""; }
                string[] months = Directory.GetDirectories(years[years.Length - 1]);
                string[] days = Directory.GetFiles(months[months.Length - 1]);
                return days[days.Length - 1];
            }
            catch { }
            return "";
        }
        Unit[] tryReadSelectedFile(string file)
        {
            List<Unit> res = new List<Unit>();
            try
            {
                //res = GeneralFunctions.IO.ReadFromXmlFile<List<Unit>>(file);
                string s = File.ReadAllText(file, Encoding.GetEncoding(1251));
                string[] splits = s.Split(new[] { "{MES}" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < splits.Length; i++)
                    try
                    {
                        var u = Unit.Parse(splits[i]);
                        if (u != null)
                            res.Add(u);
                    }
                    catch { }
            }
            catch
            { }

            return res.ToArray();
        }
        #endregion

        #region fields
                    
        public static ThreadSeeker threadSeeker;
        public static bool ShowSleepingInfo { get; set; } = false;
        /// <summary>
        /// it will be shown in bottom line
        /// </summary>
        public static string CurrentActivity { get; set; } = "";
        public static string CurrentActivityToolTip { get; set; } = "";
        public static bool Pause
        {
            get
            {
                return pause;
            }
            private set
            {
                pause = value;
            }
        }
        static bool pause = false;
        public static bool Stop
        {
            get
            {
                return stop;
            }
            set
            {
                stop = value;
                if (stop)
                {
                    Pause = false;
                    ThreadSeeker.addMessage("UI", "Stop button was pressed!", true, false);
                }
                else
                {

                }
            }
        }
        static bool stop = false;
        public static long MaximumMemoryLoadoutForWorkTime { get; set; }
        public static long MaximumCPULoadoutForWorkTime { get; set; }


        public Settings settings = null;
        public bool initialized { get; private set; } = false;

        static Button pause_button = null;
        static TimeSpan _sleptTime { get; set; } = new TimeSpan();
        static DateTime _start = DateTime.Now;
        static bool _somethingChanged = false;
        static List<Process> _additionalProcessesToConsider = new List<Process>();

        static List<List<Unit>> _threads = new List<List<Unit>>();

        static bool _showDetailedInfo = false;
        static bool _showImportantOnly = false;
        static bool _closing = false;

        Thread _threadRenewData;
        Thread _threadRenewSleepAndCurrentActivity;



        #region delegates
        delegate void voidstringstringbool(string s1, string s2, bool b, bool system, bool isCurrentActivity);
        public delegate void voidrefbool(ref bool b);
        #endregion

        #endregion

        #region public static funcs

        #region AddMsessage
        /// <summary>
        /// Class containing complete addMessage functions for different cases. CurrentActivity & system = true and errors important
        /// </summary>        
        public static class AddMessage
        {
            public static void Start(string particularPart, bool system = true)
            {
                ThreadSeeker.addMessage(particularPart, "start", false, system);
            }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public static void NotNeeded(string particularPart, bool system = true)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            {
                ThreadSeeker.addMessage( particularPart, "not needed", false, system);
            }
            public static void Cancel(string particularPart, bool system = false)
            {
                ThreadSeeker.addMessage(particularPart, "cancel", false, system, true);
            }
            public static void Stop(string particularPart, bool system = false)
            {
                ThreadSeeker.addMessage(particularPart, "stop", false, system, true);
            }
            public static void Fail(string particularPart, string failMessage = "", bool system = true)
            {
                ThreadSeeker.addMessage( particularPart, "fail. " + failMessage, true, system, false);
            }
            public static void Done(string particularPart, bool system = true)
            {
                ThreadSeeker.addMessage( particularPart, "done", false, system);
            }
        }
        #endregion

        #region addThread
        [Obsolete("Use addMessage")]
        static public void addThread(string message, string programPart, bool important = false, bool system = false, bool isCurrentActivity = true)
        {
            addMessage(programPart, message, important, system, isCurrentActivity);
        }
        #endregion

        #region addMessage
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="programPart">Must be separated with dots</param>
        /// <param name="important"></param>
        /// <param name="system"></param>
        /// <param name="isCurrentActivity"></param>
        static public void addMessage(string programPart, string message, bool important = false, bool system = false, bool isCurrentActivity = true)
        {
            try
            {
                if (isCurrentActivity)
                    CurrentActivity = programPart + "." + message;
                #region getDiagnostics = memory and cpu
                Process p = Process.GetCurrentProcess();                
                float fcpu = Diagnostics.GetCPUUsagePercent.Do(p);
                long memory = Diagnostics.GetMemoryUsage.DoMegabytes(p);
                foreach (var v in _additionalProcessesToConsider)
                    try
                    {
                        int addfCPU = Diagnostics.GetCPUUsagePercent.Do(v);
                        fcpu += addfCPU;
                        long addMemory = Diagnostics.GetMemoryUsage.DoMegabytes(v);
                        memory += addMemory;
                    }
                    catch { }
                int cpu = Convert.ToInt32(fcpu);
                #endregion
                DateTime now = DateTime.Now;
                Unit unit = new Unit(now, cpu, memory, important, system, programPart.Replace("__", "_"), message.Replace("[", "").Replace("]", ""));

                if (memory > MaximumMemoryLoadoutForWorkTime)                
                    MaximumMemoryLoadoutForWorkTime = memory;
                if (cpu > MaximumCPULoadoutForWorkTime)
                    MaximumCPULoadoutForWorkTime = cpu;
                
                string message_to_write = "{MES}" + unit.ToString();

                appendLineToFile(message_to_write);
                find_and_add_thread_to_right_date_list(unit);
                if (!system || (system && _showDetailedInfo))
                    _somethingChanged = true;
            }
            catch
            {

            }
        }
        static string create_folder_and_get_path()
        {
            string month = DateTime.Now.Month.ToString(); if (month.Length < 2) month = "0" + month;
            string day = DateTime.Now.Day.ToString(); if (day.Length < 2) day = "0" + day;
            string path = Directory.GetCurrentDirectory() + "\\logs\\" + DateTime.Today.Year + "\\" + month;
            Directory.CreateDirectory(path);
            path += "\\" + day + ".txt";
            return path;
        }
        static void find_and_add_thread_to_right_date_list(Unit unit)
        {
            foreach (var v in _threads)
                try
                {
                    if (v[0].dateTime.Date == unit.dateTime.Date)
                    {
                        v.Add(unit);
                        if (!unit.is_system_message)
                            _somethingChanged = true;
                        return;
                    }
                }
                catch { }
            List<Unit> n = new List<Unit>();
            n.Add(unit);
            _threads.Add(n);
            if (!unit.is_system_message)
                _somethingChanged = true;
        }
        static void appendLineToFile(string message)
        {
            string path = create_folder_and_get_path();
            File.AppendAllLines(path, new[] { message }, Encoding.GetEncoding(1251));
        }
        #endregion

        #region notifyImportantMessage
        static public void notifyImportantMessage()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\temp\\lookAtLog.txt"))
                File.Create(Directory.GetCurrentDirectory() + "\\temp\\lookAtLog.txt");
        }
        #endregion
        
        #region add and remove process to consider
        public static void try_add_process_to_consider(Process p)
        {
            if (p != null && !_additionalProcessesToConsider.Contains(p))
                _additionalProcessesToConsider.Add(p);
        }
        public static void try_remove_process_to_consider(Process p)
        {
            if (p != null)
                _additionalProcessesToConsider.Remove(p);
        }
        #endregion
        
        #endregion

        #region private continious funcs
        void renewData()
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            //threads.Clear();
            string initialText = groupBox1.Text;
            listView1.Items.Add("Подготовка логов. Стоит ограничить временной диапазон, если долго.");
            var defaultGroupBox1Text = groupBox1.Text;
            while (!_closing)
                exhibite(defaultGroupBox1Text);
        }
        void exhibite(string defaultGroupBoxText)
        {
            _somethingChanged = false;
            try
            {
                groupBox1.Text = defaultGroupBoxText + ". Max CPU=" + MaximumCPULoadoutForWorkTime + ". Max memory=" + MaximumMemoryLoadoutForWorkTime;
                #region exhibition   
                if (_closing) return;
                if (_threads.Count == 0)
                { sleep(); return; }
                int howMuch = listView1.Size.Height / 16;  //appr. 18 units Y for one line                     
                if (howMuch == 0)
                    howMuch = listView1.MinimumSize.Height / 16;
                //18 = lv.size.height/17 // bout 9 strings left, 18 present // height = 306
                #region (anti)requesting
                string request = textBox1.Text.ToLower();
                string antirequest = textBox2.Text.ToLower();

                string[] requests = request.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < requests.Length; i++) requests[i] = requests[i].Trim();
                string[] antirequests = antirequest.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < antirequests.Length; i++) antirequests[i] = antirequests[i].Trim();
                #endregion
                List<string> res = new List<string>();
                #region get threads for selected dates
                try
                {
                    DateTime mindt = DateAndTime.ConvertStringToDateTime.Do(Stb.Text, false);
                    DateTime maxdt = DateAndTime.ConvertStringToDateTime.Do(POtb.Text, false);
                    for (int i = _threads.Count - 1; i >= 0; i--)
                        try
                        {
                            if (_closing || _somethingChanged || res.Count >= howMuch) break;
                            if (_threads[i][0].dateTime.Date > mindt && _threads[i][0].dateTime.Date < maxdt)
                            {
                                var reversed = new List<Unit>();
                                for (int j = _threads[i].Count - 1; j >= 0; j--)
                                {
                                    if (checking(_threads[i][j], requests, antirequests))
                                        reversed.Add(_threads[i][j]);
                                }
                                for (int a = 0; a < reversed.Count; a++)
                                {
                                    if (res.Count >= howMuch)
                                        break;
                                    string sreversed = "";
                                    if (cb_compress.Checked && !reversed[a].part.EndsWith(".start") && !reversed[a].part.EndsWith(".closing"))
                                        sreversed = reversed[a].ToStringShort();
                                    else
                                        sreversed = reversed[a].ToString();
                                    res.Add(sreversed);
                                }
                            }
                        }
                        catch { }
                }
                catch(Exception e)
                {
                    listView1.Items.Clear();
                    listView1.Items.Add("Can't get threads for selected dates: " + e.Message);
                }
                #endregion
                listView1.Items.Clear();
                #region fillList
                for (int a = 0; a < res.Count; a++)
                {
                    if (_somethingChanged || _closing) break;
                    try { listView1.Items.Add(res[a]); } catch { continue; }
                    #region colorize list
                    try
                    {
                        var elem = listView1.Items[listView1.Items.Count - 1];
                        if (res[a].Contains("[important]"))
                        {
                            elem.ForeColor = Color.Purple;
                            continue;
                        }
                        foreach (var v in settings.Colorization)
                        {
                            string[] parts = res[a].Split(new[] { "]" }, StringSplitOptions.None);
                            string p0 = parts[0].ToLower() + "]";
                            string p1 = res[a].Replace(p0, "").ToLower();
                            if (p0.Contains(v.partOfProgram.ToLower())
                                && p1.Contains(v.probablyContainedString.ToLower()))
                                elem.ForeColor = v.colorToBeShown;
                        }
                    }
                    catch(Exception e)
                    {
                        listView1.Items.Clear();
                        listView1.Items.Add("Can't colorize list: " + e.Message);
                    }
                    #endregion
                }
                #endregion
                listView1.Refresh();
                sleep();
                #endregion
            }
            catch(Exception e)
            {
                try
                {
                    listView1.Items.Clear();
                    listView1.Items.Add("Can't exhibite threads: " + e.Message);
                }
                catch { }
            }
        }
        bool checking(Unit unit, string[] requests, string[] antirequests)
        {
            if (!_showDetailedInfo && unit.is_system_message)
                return false;
            if (_showImportantOnly && !unit.is_important)
                return false;
            try
            {
                DateTime since = DateAndTime.ConvertStringToDateTime.Do(Stb.Text, false);
                DateTime to = DateAndTime.ConvertStringToDateTime.Do(POtb.Text, false);
                if (unit.dateTime < since || unit.dateTime > to)
                    return false;
            }
            catch { }
            string thread = unit.ToString();
            #region requesting and antirequesting  
            foreach (var r in requests)
            {
                if (r.Contains("*"))
                {
                    Regex regex = new Regex(r.Replace("*", ".+"), RegexOptions.IgnoreCase);
                    var matches = regex.Matches(thread);
                    if (matches.Count == 0)
                        return false;
                }
                else if (!thread.ToLower().Contains(r))
                    return false;
            }
            foreach (var r in antirequests)
            {
                if (r.Contains("*"))
                {
                    Regex regex = new Regex(r.Replace("*", ".+"), RegexOptions.IgnoreCase);
                    var matches = regex.Matches(thread);
                    if (matches.Count > 0)
                        return false;
                }
                else if (thread.ToLower().Contains(r))
                    return false;
            }
            #endregion
            return true;
        }

        void renewSleepAndCurrentActivity()
        {
            while (!_closing)
                try
                {
                    tb_CurrentActivity.Text = CurrentActivity;
                    if (ShowSleepingInfo && _sleptTime.TotalSeconds >= 1)
                    {
                        string sleepstring = "(сон " + DateAndTime.ConvertTimeSpanToString.Do(_sleptTime) + ")";
                        if (tb_CurrentActivity.Text.Contains("(сон"))
                            tb_CurrentActivity.Text = tb_CurrentActivity.Text.Remove(tb_CurrentActivity.Text.IndexOf("(сон"));
                        tb_CurrentActivity.Text += sleepstring;
                    }
                    var idle = My.Diagnostics.GetIdleTime.Do();
                    if (settings.UnpaseIfIdleMinutes > 0 &&
                        idle > new TimeSpan(0, settings.UnpaseIfIdleMinutes, 0))
                        Pause = false;
                    if (_closing) return;
                    Thread.Sleep(1000);
                }
                catch { return; }
        }
        void sleep()
        {
            for (int i = 0; i < settings.ReshowDelaySeconds; i++)
            {
                Thread.Sleep(1000);        
                    if (_somethingChanged || _closing)
                        return;     
            }
        }        

        #endregion
        
        #region events      

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _showImportantOnly = checkBox1.Checked;
            _somethingChanged = true;
        }

        private void save_settings(object sender, EventArgs e)
        {
            try
            {
                _somethingChanged = true;
                string path = Directory.GetCurrentDirectory() + "\\ThreadSeeker.txt";
                string from = Stb.Text;
                string to = POtb.Text;
                File.WriteAllLines(path, new[] { from, to }, Encoding.GetEncoding(1251));
            }
            catch { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            recalibrateTimeFrame();
            _somethingChanged = true;
        }
        public void recalibrateTimeFrame()
        {
            var spreadTime = new TimeSpan(3, 0, 0, 0);

            var today = DateTime.Today;
            var weekago = today - spreadTime;
            var weeklater = today + spreadTime;
            string sago = DateAndTime.ConvertDateTimeToString.Do(weekago, false);
            string slater = DateAndTime.ConvertDateTimeToString.Do(weeklater, false);
            Stb.Text = sago;
            POtb.Text = slater;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string path = getLastFile();
                System.Diagnostics.Process.Start(path);
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + "\\logs";
                Process.Start(path);
            }
            catch { }
        }
       
        private void listView1_Click(object sender, EventArgs e)
        {

            try
            {
                string txt = listView1.SelectedItems[0].ToString();
                string[] links = txt.Split(new[] { " ", "}", "{", "=", "(", ")"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var link in links)
                    if (link.Contains("http://") && link.Contains("https://"))
                    {
                        Clipboard.SetText(link);
                        break;
                    }
                toolTip1.Show(listView1.SelectedItems[0].ToString(), (Control)sender);
            }
            catch { }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(listView1.SelectedItems[0].ToString());
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            addMessage("ThreadSeeker.manually", textBox1.Text, true, false, false);
            textBox1.Text = "";
            somethingChanged(sender, e);
        }
        
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !Char.IsControl(e.KeyChar))
                e.Handled = true;         
        }

        private void StextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            dt_KeyPress(sender, ref e);
            //checkBox2.Checked = true;
        }

        private void POtextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            dt_KeyPress(sender, ref e);
            //checkBox2.Checked = true;
        }
        void dt_KeyPress(object sender, ref KeyPressEventArgs e)
        {            
            var tb = sender as TextBox;
            int ss = tb.SelectionStart;
            string txt = tb.Text;
            if (Char.IsControl(e.KeyChar))            
                return;            
            if (!Char.IsDigit(e.KeyChar) || txt.Length > 11)
            {
                e.Handled = true;
                return;
            }
        }

        void somethingChanged(object sender, EventArgs e)
        {
            _somethingChanged = true;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            _showDetailedInfo = checkBox3.Checked;
            _somethingChanged = true;
        }
                
        private void button4_Click(object sender, EventArgs e)
        {
            Pause = !Pause;
            if (Pause)
            {
                button4.Text = "||";
            }
            else
            {
                button4.Text = ">";
            }
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _somethingChanged = true;
        }


        private void listView1_MouseHover(object sender, EventArgs e)
        {
            try
            {
                string text = "Клик для копирования ссылки(если есть).\nДвойной клик для поллного копирования.\n"
                    + listView1.SelectedItems[0].Text;
                toolTip1.Show(text, (Control)sender, 3000);
            }
            catch { }
        }

        private void tb_CurrentActivity_MouseHover(object sender, EventArgs e)
        {
            if (CurrentActivityToolTip != "")
                toolTip1.Show(CurrentActivityToolTip, (Control)sender, 3000);
            else
                toolTip1.Show(tb_CurrentActivity.Text, (Control)sender, 3000);

        }


        private void b_Stop_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Вы уверены в остановке программы?", "Треуется подтверждение", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                Stop = true;
                new Thread(stopColorization).Start();
            }
        }

        void stopColorization()
        {
            var color = b_Stop.BackColor;
            b_Stop.BackColor = Color.Green;
            Thread.Sleep(1000);
            b_Stop.BackColor = color;
        }

        #endregion

    }
}