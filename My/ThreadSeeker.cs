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


namespace My
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
        public class Graphics
        {
            public int HorizontalMargin = 0;
            public int VerticalMargin = 0;
            public int Width = 0;
            public int Height = 0;
            public AnchorStyles Anchors;

            public Graphics(int horizontalMargin, int verticalMargin, int Width, int Height, AnchorStyles anchors)
            {
                this.HorizontalMargin = horizontalMargin;
                this.VerticalMargin = verticalMargin;
                this.Width = Width;
                this.Height = Height;
                this.Anchors = anchors;
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
                string date = DateAndTime.convertDateTimeToString(dateTime);
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
            public static Unit Parse(string s)
            {
                //{MES}2018.07.06.00.53.59_0% 34 мб__________[system][worker.read_info]done
                try
                {
                    string[] splits = s.Replace("\r\n", "").Split(new[] { "мб" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] left_parts = splits[0].Split(new[] { "_", "%" }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime dt = DateAndTime.convertStringToDateTime(left_parts[0]);
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

        #region initialization fields
        public Form owner;
        public bool initialized { get; private set; } = false;
        static Button pause_button = null;
        List<ThreadSeekerColorisationSetting> threadSeekerColorisationSettings = new List<ThreadSeekerColorisationSetting>();
        #endregion

        public ThreadSeeker(ThreadSeekerColorisationSetting[] colorizationSettings, int ReshowDelaySec
            , int dangerMemoryLoadMB, Graphics graphics, Form Owner,
            bool ShowSleepingInfo = false, bool UnpauseIfIdle = false, bool pressRecalibrationAtStart = false)
        {
            #region validation
            if (colorizationSettings == null || colorizationSettings.Contains(null))
                throw new NullReferenceException("ColorizationSettings cannot be null");
            if (reshowDelay <= 0 || dangerMemoryLoadMB <= 0)
                throw new IndexOutOfRangeException("No int can be qual or less that 0");
            if (graphics.HorizontalMargin < 0 || graphics.VerticalMargin < 0 || graphics.Width <= 0 || graphics.Height <= 0)
                throw new ArgumentOutOfRangeException("No graphics int can be qual or less that 0");
            if (Owner == null)
                throw new ArgumentNullException("Owner can't be null");
            #endregion
            #region launch and initialisations
            threadSeekerColorisationSettings = colorizationSettings.ToList();
            reshowDelay = ReshowDelaySec;
            dangerMemoryMB = dangerMemoryLoadMB;
            #region graphics
            int x = owner.Width - graphics.HorizontalMargin - graphics.Width;
            if (x <= 0) x = 0;
            int y = owner.Height - graphics.VerticalMargin - graphics.Height;
            if (y <= 0) y = 0;
            this.Location = new System.Drawing.Point(x, y);
            this.Size = new System.Drawing.Size(graphics.Width, graphics.Height);
            this.Anchor = graphics.Anchors;
            owner = Owner; owner.Controls.Add(this);
            InitializeComponent();
            this.Show();
            #endregion
            if (pressRecalibrationAtStart)
                button5.PerformClick();
            if (colorizationSettings != null)
            dangerMemoryMB = dangerMemoryLoadMB;
            showSleepingInfo = ShowSleepingInfo;
            launchIfIdle = UnpauseIfIdle;
            start = DateTime.Now;
            reshowDelay = ReshowDelaySec;
            #endregion
            #region read things
            threads = new List<List<Unit>>();
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
                        threads.Add(oneDate.ToList());
                        tslp.reshow_progress(iter);
                    }
                }
            }
            #endregion
            #region if closed correctly             
            bool closedCorrectly = false;
            for (int i = threads.Count - 1; i >= 0; i--)
            {
                for (int j = threads[i].Count - 1; j >= 0; j--)
                {
                    if (threads[i][j].part.Contains("ThreadSeeker.closing"))
                    {
                        closedCorrectly = true;
                        break;
                    }
                    if (threads[i][j].part.Contains("ThreadSeeker.start"))
                        break;
                }
            }
            if (!closedCorrectly) MessageBox.Show("Программа не была закрыта корректно");
            #endregion
            initialized = true;
            #region manage threads
            try { thread_renew_data.Abort(); } catch { }
            thread_renew_data = new Thread(renewData);
            thread_renew_data.Name = "ThreadSeeker";
            thread_renew_data.Start();
            try { thread_renew_sleep.Abort(); } catch { }
            thread_renew_sleep = new Thread(renewSleep);
            thread_renew_sleep.Name = "SleepSeeker";
            thread_renew_sleep.Start();
            #endregion
            pause_button = button4;
            string part = "ThreadSeeker.start";
            addMessage(part, "__________________________________________________", true);
            addMessage(part, "Логгер запущен", true, false, true);
        }
        
        /// <summary>
        /// must be used to stop cycles
        /// </summary>
        public void close()
        {
            addMessage("ThreadSeeker.closing", "Логгер закрыт", true, false, false);
            closing = true;
            try { thread_renew_data.Abort(); } catch { }
            try { thread_renew_sleep.Abort(); } catch { }
            Dispose(true);
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
                            int day = parts[last].getFirstDigits();
                            int month = parts[last - 1].getFirstDigits();
                            int year = parts[last - 2].getFirstDigits();
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
                    
        public static bool showSleepingInfo { get; private set; } = false;
        /// <summary>
        /// it will be shown in bottom line
        /// </summary>
        public static string currentActivity = "";
        public static TimeSpan sleptTime = new TimeSpan();
        

        static DateTime start = DateTime.Now;
        static int dangerMemoryMB = 750;
        static bool launchIfIdle = false;
        static bool somethingChanged = false;
        static List<Process> additional_processes_to_consider = new List<Process>();

        static List<List<Unit>> threads = new List<List<Unit>>();

        static bool showDetailedInfo = false;
        static bool showImportantOnly = false;
        static int reshowDelay = 15;
        static bool closing = false;

        Thread thread_renew_data;
        Thread thread_renew_sleep;

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
            public static void Done(string particularPart)
            {
                ThreadSeeker.addMessage("done", particularPart, false, true);
            }
            public static void Fail(string particularPart, string failMessage = "")
            {
                ThreadSeeker.addMessage("fail. " + failMessage, particularPart, true, true);
            }
            public static void Start(string particularPart)
            {
                ThreadSeeker.addMessage("start", particularPart, false, true);
            }
            public static void NotNeeded(string particularPart)
            {
                ThreadSeeker.addMessage("not needed", particularPart, false, true);
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
                    currentActivity = programPart + "." + message;
                #region getDiagnostics = memory and cpu
                Process p = Process.GetCurrentProcess();                
                float fcpu = Diagnostics.get_cpu_percent_usage(p);
                long memory = Diagnostics.get_memory_mb_usage(p);
                foreach (var v in additional_processes_to_consider)
                    try
                    {
                        int addfCPU = Diagnostics.get_cpu_percent_usage(v);
                        fcpu += addfCPU;
                        long addMemory = Diagnostics.get_memory_mb_usage(v);
                        memory += addMemory;
                    }
                    catch { }
                int cpu = Convert.ToInt32(fcpu);
                #endregion
                DateTime now = DateTime.Now;
                Unit unit = new Unit(now, cpu, memory, important, system, programPart.Replace("__", "_"), message.Replace("[", "").Replace("]", ""));
                string message_to_write = "{MES}" + unit.ToString();

                appendLineToFile(message_to_write);
                find_and_add_thread_to_right_date_list(unit);
                if (!system || (system && showDetailedInfo))
                    somethingChanged = true;
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
            foreach (var v in threads)
                try
                {
                    if (v[0].dateTime.Date == unit.dateTime.Date)
                    {
                        v.Add(unit);
                        if (!unit.is_system_message)
                            somethingChanged = true;
                        return;
                    }
                }
                catch { }
            List<Unit> n = new List<Unit>();
            n.Add(unit);
            threads.Add(n);
            if (!unit.is_system_message)
                somethingChanged = true;
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
            if (p != null && !additional_processes_to_consider.Contains(p))
                additional_processes_to_consider.Add(p);
        }
        public static void try_remove_process_to_consider(Process p)
        {
            if (p != null)
                additional_processes_to_consider.Remove(p);
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
            while (!closing)
                try
                {
                    somethingChanged = false;
                    #region exhibition   
                    if (closing) return;      
                    if (threads.Count == 0)
                    { sleep(); continue; }
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
                    List<Unit> seldates = new List<Unit>();
                    #region get threads for selected dates
                    try
                    {
                        DateTime mindt = DateAndTime.convertStringToDateTime(Stb.Text, false);
                        DateTime maxdt = DateAndTime.convertStringToDateTime(POtb.Text, false);
                        for (int i = threads.Count - 1; i >= 0; i--)
                            try
                            {
                                if (closing) break;
                                if (threads[i][0].dateTime.Date > mindt && threads[i][0].dateTime.Date < maxdt)
                                {
                                    var reversed = new List<Unit>();
                                    for (int j = threads[i].Count - 1; j >= 0; j--)
                                        reversed.Add(threads[i][j]);
                                    seldates.AddRange(reversed);
                                }
                            }
                            catch { }
                    }
                    catch
                    {
                        seldates.Clear();
                        foreach (var v in threads)
                            seldates.AddRange(v);
                    }
                    #endregion
                    List<string> res = new List<string>();
                    #region count and filter
                    int found = 0;
                    //int numUPDOWN = Convert.ToInt32(numericUpDown1.Value);
                    int numUPDOWN = 0;
                    int starter = 1 + numUPDOWN;
                    string txt = groupBox1.Text;
                    for (int a = 0; a < seldates.Count; a++)
                        try
                        {
                            if (somethingChanged || closing) break;
                            if (a % 100 == 0) groupBox1.Text = txt + ".Обработка " + a + " сообщений.";
                            if (checking(seldates[a], requests, antirequests))
                            {
                                if (res.Count < howMuch)
                                    res.Add(seldates[a].ToString());
                                found++;
                            }
                        }
                        catch { }
                    groupBox1.Text = txt;
                    #endregion
                    //threads.Clear();
                    listView1.Items.Clear();
                    #region fillList
                    for (int a = 0; a < res.Count; a++)
                    {
                        if (somethingChanged || closing) break;
                        try { listView1.Items.Add(res[a]); } catch { continue; }
                        #region colorise
                        var elem = listView1.Items[listView1.Items.Count - 1];
                        if (res[a].Contains("[important]"))
                        {
                            elem.ForeColor = Color.Purple;
                            continue;
                        }
                        if (threadSeekerColorisationSettings == null)
                            continue;
                        foreach (var v in threadSeekerColorisationSettings)
                            try
                            {
                                string[] parts = res[a].Split(new[] { "]" }, StringSplitOptions.None);
                                string p0 = parts[0].ToLower() + "]";
                                string p1 = res[a].Replace(p0, "").ToLower();
                                if (p0.Contains(v.partOfProgram.ToLower())
                                    && p1.Contains(v.probablyContainedString.ToLower()))
                                    elem.ForeColor = v.colorToBeShown;
                            }
                            catch { }
                        #endregion
                    }
                    #endregion
                    listView1.Refresh();
                    groupBox1.Text = initialText + " Найдено:" + found.ToString();
                    sleep();
                    #endregion
                }
                catch
                { }

        }
        bool checking(Unit unit, string[] requests, string[] antirequests)
        {
            if (!showDetailedInfo && unit.is_system_message)
                return false;
            if (showImportantOnly && !unit.is_important)
                return false;
            try
            {
                DateTime since = DateAndTime.convertStringToDateTime(Stb.Text, false);
                DateTime to = DateAndTime.convertStringToDateTime(POtb.Text, false);
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

        void renewSleep()
        {
            while (!closing)
                try
                {
                    textBox3.Text = currentActivity;
                    if (showSleepingInfo && sleptTime.TotalSeconds >= 1)
                    {
                        string sleepstring = "(сон " + DateAndTime.convertTimeSpanToString(sleptTime) + ")";
                        if (textBox3.Text.Contains("(сон"))
                            textBox3.Text = textBox3.Text.Remove(textBox3.Text.IndexOf("(сон"));
                        textBox3.Text += sleepstring;
                    }
                    if (launchIfIdle)
                    {
                        var idle = Diagnostics.getIdleTime();
                        if (idle > new TimeSpan(1, 30, 0))
                            pause = false;
                    }
                    if (closing) return;
                    Thread.Sleep(1000);
                }
                catch { return; }
        }       
        void sleep()
        {
            for (int i = 0; i < reshowDelay; i++)
            {
                Thread.Sleep(1000);        
                    if (somethingChanged || closing)
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
            showImportantOnly = checkBox1.Checked;
            somethingChanged = true;
        }

        private void save_settings(object sender, EventArgs e)
        {
            try
            {
                somethingChanged = true;
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
            somethingChanged = true;
        }
        public void recalibrateTimeFrame()
        {
            var spreadTime = new TimeSpan(3, 0, 0, 0);

            var today = DateTime.Today;
            var weekago = today - spreadTime;
            var weeklater = today + spreadTime;
            string sago = DateAndTime.convertDateTimeToString(weekago, false);
            string slater = DateAndTime.convertDateTimeToString(weeklater, false);
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
            addMessage(textBox1.Text, "ThreadSeeker.manually", true, false, false);
            _somethingChanged(sender, e);
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

        void _somethingChanged(object sender, EventArgs e)
        {
            somethingChanged = true;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            showDetailedInfo = checkBox3.Checked;
            somethingChanged = true;
        }

        /// <summary>
        /// valuable for managing user set pauses in application
        /// </summary>
        public static bool pause { get; private set; } = false;
        public static void pause_set(bool value)
        {
            pause = value;
            if (value)
                pause_button.Text = "||";
            else
                pause_button.Text = ">";            
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text == ">")
                pause_set(true);

            else
                pause_set(false);
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            somethingChanged = true;
        }


        #endregion

    }
}