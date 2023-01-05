#define USE_TIMER

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Text.RegularExpressions;

namespace AxxonLayoutUtility
{
    public partial class Form2 : Form
    {
        //public const int TIMEOUT = 15000;
        public const int MAX_MONITOR = 4;

        public int TIMEOUT = 15000;
        public int monitorCount = 1;

        public string clientLayoutJson = "";

        public List<LayoutInfo> layoutList, layoutListR;

        //public List<LayoutInfo> layoutList_Display1, layoutList_Display2, layoutList_Display3, layoutList_Display4;
        public List<LayoutInfo>[] layoutList_Display_Array = new List<LayoutInfo>[MAX_MONITOR];

        public List<DisplayInfo> displayList;

        //public Thread layoutPlayer1, layoutPlayer2, layoutPlayer3, layoutPlayer4;
        public Thread[] layoutPlayer_Array = new Thread[MAX_MONITOR];

#if (USE_TIMER == true)
        // timer
        public System.Windows.Forms.Timer[] layoutPlayerTimer_Array = new System.Windows.Forms.Timer[MAX_MONITOR];
        IEnumerator<LayoutInfo>[] layoutEnumerator = new IEnumerator<LayoutInfo>[MAX_MONITOR];
#endif

        public bool isAllPlaying = false;
        //public bool isLayout1Playing = false, isLayout2Playing = false, isLayout3Playing = false, isLayout4Playing = false;
        public bool[] isLayoutPlaying_Array = new bool[MAX_MONITOR];

        //public ManualResetEvent resetEvent_Display1, resetEvent_Display2, resetEvent_Display3, resetEvent_Display4;
        public ManualResetEvent[] resetEvent_Display_Array = new ManualResetEvent[MAX_MONITOR];

        //public string currentLayoutNo;
        public string targetLayoutNo;
        public bool[] isTargetLayoutChange = new bool[MAX_MONITOR];

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form2()
        {
            InitializeComponent();
        }

        ~Form2()
        {
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            ReadConfig();

            SetNetSh();
            GetCurrentLayout();
            GetCurrentDisplay();

            for(int i = 0; i < MAX_MONITOR; i++)
            {
                resetEvent_Display_Array[i] = new ManualResetEvent(false);
            }
        }

        private void SetNetSh()
        {
            Console.WriteLine("{0}\\{1} on {2}", Environment.UserDomainName, Environment.UserName, Environment.MachineName);

            ProcessStartInfo startInfo = new ProcessStartInfo("netsh.exe");
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;
            startInfo.Arguments = "http add urlacl url=http://+:8888/ user=" + Environment.UserDomainName + "\\" + Environment.UserName;
            Process.Start(startInfo);
        }

        private void GetCurrentLayout()
        {
            string url = "http://localhost:8888/GetLayouts";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            String respStr = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                respStr = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
            }


            JObject jsonObject = JObject.Parse(respStr);
            // get JSON result objects into a list
            IList<JToken> results = jsonObject["LayoutInfo"].Children().ToList();

            // serialize JSON results into .NET objects
            layoutList = new List<LayoutInfo>();
            layoutListR = new List<LayoutInfo>();
            foreach (JToken result in results)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                LayoutInfo searchResult = result.ToObject<LayoutInfo>();
                layoutList.Add(searchResult);
                layoutListR.Add(searchResult);
            }

            // sort layout by name
            layoutList.Sort(delegate (LayoutInfo a, LayoutInfo b) { return a.Name.CompareTo(b.Name); });
            layoutListR.Sort(delegate (LayoutInfo a, LayoutInfo b) { return b.Name.CompareTo(a.Name); });

            for (int i = 0; i < layoutList_Display_Array.Length; i++)
            {
                layoutList_Display_Array[i] = new List<LayoutInfo>();
            }

            foreach (LayoutInfo layout in layoutList)
            {
                string layoutNoString = layout.Name.Split('_')[0];

                Regex regex = new Regex(@"[0-9]{2}_[0-9]{1}");

                if (regex.IsMatch(layout.Name))
                {
                    Console.WriteLine(layout.Name + " Match");
                }
                else
                {
                    Console.WriteLine(layout.Name + " NOT Match");
                    continue;
                }

                if (comboBox_layout.FindString(layoutNoString) < 0)
                {
                    comboBox_layout.Items.Add(layoutNoString);
                }

                Console.WriteLine(layout.Id + " : " + layout.Name);

                if (layout.Name.EndsWith("_1"))
                {
                    //layoutList_Display1.Add(layout);
                    layoutList_Display_Array[0].Add(layout);
                    Console.WriteLine("--> Display1");
                }
                else if (layout.Name.EndsWith("_2"))
                {
                    //layoutList_Display2.Add(layout);
                    layoutList_Display_Array[1].Add(layout);
                    Console.WriteLine("--> Display2");
                }
                else if (layout.Name.EndsWith("_3"))
                {
                    //layoutList_Display3.Add(layout);
                    layoutList_Display_Array[2].Add(layout);
                    Console.WriteLine("--> Display3");
                }
                else if (layout.Name.EndsWith("_4"))
                {
                    //layoutList_Display4.Add(layout);
                    layoutList_Display_Array[3].Add(layout);
                    Console.WriteLine("--> Display4");
                }

            }

            foreach (LayoutInfo info in layoutList)
            {
                Console.WriteLine(info.Name);
            }

            foreach (LayoutInfo info in layoutListR)
            {
                Console.WriteLine(info.Name);
            }
        }

        private void GetCurrentDisplay()
        {
            string url = "http://localhost:8888/GetDisplays";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            String respStr = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                respStr = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
            }


            JObject jsonObject = JObject.Parse(respStr);
            // get JSON result objects into a list
            IList<JToken> results = jsonObject["DisplayInfo"].Children().ToList();

            // serialize JSON results into .NET objects
            displayList = new List<DisplayInfo>();
            foreach (JToken result in results)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                DisplayInfo searchResult = result.ToObject<DisplayInfo>();
                displayList.Add(searchResult);
            }

            // sort layout by name
            displayList.Sort(delegate (DisplayInfo a, DisplayInfo b) { return a.Id.CompareTo(b.Id); });

            foreach (DisplayInfo info in displayList)
            {
                Console.WriteLine(info.Id + ", Mainform " + info.IsMainForm);
            }
        }

        private void LayoutPlayerThread(int monitorNo)
        {
            Console.WriteLine((monitorNo + 1) + " monitor thread created");
            foreach (LayoutInfo thisloop in GetLoopLayout_Display_Array(monitorNo))
            {
                if (isLayoutPlaying_Array[monitorNo] == false)
                {
                    break;
                }

                string url = @"http://localhost:8888/SwitchLayout?layoutId=" + thisloop.Id + @"&displayId=" + displayList[monitorNo].Id;  //\\.\DISPLAY2";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                String respStr = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    respStr = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                }

                Console.WriteLine((monitorNo + 1) + " monitor cmd sent : " + url);
                Thread.Sleep(TIMEOUT); // + (monitorNo + 1) * 100);

                resetEvent_Display_Array[monitorNo].WaitOne();
            }
        }

#if (USE_TIMER == true)
        // timer
        private void LayoutPlayerTimer(object sender, EventArgs e, int monitorNo)
        {
            //foreach (LayoutInfo thisloop in GetLoopLayout_Display_Array(monitorNo))
            //{
                layoutEnumerator[monitorNo].MoveNext();
                LayoutInfo thisloop = layoutEnumerator[monitorNo].Current;
                
                string url = @"http://localhost:8888/SwitchLayout?layoutId=" + thisloop.Id + @"&displayId=" + displayList[monitorNo].Id;  //\\.\DISPLAY2";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                String respStr = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    respStr = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                }

                Console.WriteLine((monitorNo + 1) + " monitor cmd sent : " + url + " " + thisloop.Name);
            //}
        }
#endif

        public IEnumerable<LayoutInfo> GetLoopLayout_Display_Array(int monitorNo)
        {
            while (true)
            {
                foreach (var layout in layoutList_Display_Array[monitorNo])
                {
                    if (isTargetLayoutChange[monitorNo])
                    {
                        if (layout.Name.Split('_')[0] != targetLayoutNo)
                        {
                            continue;
                        }
                        else
                        {
                            isTargetLayoutChange[monitorNo] = false;
                        }
                    }
                    yield return layout;
                }
            }
        }

        public IEnumerable<LayoutInfo> GetLoopLayout_Display()
        {
            while (true)
            {
                foreach (var layout in layoutList)
                {
                    yield return layout;
                }
            }
        }

        public IEnumerable<LayoutInfo> GetLoopLayoutR_Display()
        {
            while (true)
            {
                foreach (var layout in layoutListR)
                {
                    yield return layout;
                }
            }
        }

        private void button_displayAll_Click(object sender, EventArgs e)
        {
            if (isAllPlaying == false)
            {
                isAllPlaying = true;
                button_displayAll.Text = "일괄 정지";

                for (int i = 0; i < displayList.Count; i++)
                {
                    int monitorNo = i;
                    if (isLayoutPlaying_Array[monitorNo] == false)
                    {
                        isLayoutPlaying_Array[monitorNo] = true;

#if (USE_TIMER == false)
                        if (layoutPlayer_Array[monitorNo] == null)
                        {
                            layoutPlayer_Array[monitorNo] = new Thread(() => { LayoutPlayerThread(monitorNo); });
                            layoutPlayer_Array[monitorNo].Start();
                            resetEvent_Display_Array[monitorNo].Set();
                        }
                        else
                        {
                            resetEvent_Display_Array[monitorNo].Set();
                        }
#endif

#if (USE_TIMER == true)
                        // timer
                        if (layoutPlayerTimer_Array[monitorNo] == null)
                        {
                            layoutEnumerator[monitorNo] = GetLoopLayout_Display_Array(monitorNo).GetEnumerator();
                            layoutPlayerTimer_Array[monitorNo] = new System.Windows.Forms.Timer();
                            layoutPlayerTimer_Array[monitorNo].Interval = TIMEOUT;
                            layoutPlayerTimer_Array[monitorNo].Tick += new EventHandler((eventSender, eventExp) => { LayoutPlayerTimer(eventSender, eventExp, monitorNo); });
                            layoutPlayerTimer_Array[monitorNo].Start();
                            LayoutPlayerTimer(null, null, monitorNo);
                        }
                        else
                        {
                            layoutPlayerTimer_Array[monitorNo].Start();
                            LayoutPlayerTimer(null, null, monitorNo);
                        }
#endif

                    }
                }
            }
            else
            {
                isAllPlaying = false;
                button_displayAll.Text = "일괄 시작";

                for (int i = 0; i < displayList.Count; i++)
                {
                    int monitorNo = i;
                    isLayoutPlaying_Array[monitorNo] = false;

#if (USE_TIMER == false)
                    //layoutPlayer1.Abort();
                    //layoutPlayer1 = null;
                    resetEvent_Display_Array[monitorNo].Reset();
#endif

#if (USE_TIMER == true)
                    // timer
                    layoutPlayerTimer_Array[monitorNo].Stop();
#endif
                }
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
#if (USE_TIMER == false)
            for (int i = 0; i < layoutPlayer_Array.Length; i++)
            {
                if (layoutPlayer_Array[i] != null)
                {
                    layoutPlayer_Array[i].Abort();
                    layoutPlayer_Array[i] = null;
                }
            }
#endif

#if (USE_TIMER == true)
            for (int i = 0; i < layoutPlayerTimer_Array.Length; i++)
            {
                // timer
                if (layoutPlayerTimer_Array[i] != null)
                {
                    layoutPlayerTimer_Array[i].Stop();
                    layoutPlayerTimer_Array[i] = null;
                }
            }
#endif
        }

        private void button_Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button_displayAll_TextChanged(object sender, EventArgs e)
        {
            if (button_displayAll.Text.Contains("시작"))
            {
                button_displayAll.BackColor = Color.DarkOrange;
            }
            else if (button_displayAll.Text.Contains("정지"))
            {
                button_displayAll.BackColor = Color.Cyan;
            }
        }

        private void comboBox_layout_SelectedIndexChanged(object sender, EventArgs e)
        {
            targetLayoutNo = (comboBox_layout.SelectedIndex + 1).ToString("00");
            //MessageBox.Show(targetLayoutNo);

            // stop all thread
            for (int i = 0; i < displayList.Count; i++)
            {
                int monitorNo = i;
                isLayoutPlaying_Array[monitorNo] = false;
#if (USE_TIMER == false)
                if (layoutPlayer_Array[i] != null)
                {
                    layoutPlayer_Array[i].Abort();
                    layoutPlayer_Array[i] = null;
                }
                resetEvent_Display_Array[i].Reset();
#endif
#if (USE_TIMER == true)
                // timer
                if (layoutPlayerTimer_Array[i] != null)
                {
                    layoutPlayerTimer_Array[i].Stop();
                    layoutPlayerTimer_Array[i] = null;
                }
#endif
            }

            // restart
            isAllPlaying = true;
            button_displayAll.Text = "일괄 정지";

            for (int i = 0; i < displayList.Count; i++)
            {
                int monitorNo = i;
                if (isLayoutPlaying_Array[monitorNo] == false)
                {
                    isTargetLayoutChange[monitorNo] = true;
                    isLayoutPlaying_Array[monitorNo] = true;

#if (USE_TIMER == false)
                    if (layoutPlayer_Array[monitorNo] == null)
                    {
                        layoutPlayer_Array[monitorNo] = new Thread(() => { LayoutPlayerThread(monitorNo); });
                        layoutPlayer_Array[monitorNo].Start();
                        resetEvent_Display_Array[monitorNo].Set();
                    }
                    else
                    {
                        resetEvent_Display_Array[monitorNo].Set();
                    }
#endif

#if (USE_TIMER == true)
                    // timer
                    if (layoutPlayerTimer_Array[monitorNo] == null)
                    {
                        layoutEnumerator[monitorNo] = GetLoopLayout_Display_Array(monitorNo).GetEnumerator();
                        layoutPlayerTimer_Array[monitorNo] = new System.Windows.Forms.Timer();
                        layoutPlayerTimer_Array[monitorNo].Interval = TIMEOUT;
                        layoutPlayerTimer_Array[monitorNo].Tick += new EventHandler((eventSender, eventExp) => { LayoutPlayerTimer(eventSender, eventExp, monitorNo); });
                        layoutPlayerTimer_Array[monitorNo].Start();
                        LayoutPlayerTimer(null, null, monitorNo);
                    }
                    else
                    {
                        layoutPlayerTimer_Array[monitorNo].Start();
                        LayoutPlayerTimer(null, null, monitorNo);
                    }
#endif
                }
            }
        }

        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void ReadConfig()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(@".\config.xml");

                XmlNodeList nodeList = xml.GetElementsByTagName("Config");

                foreach (XmlNode node in nodeList)
                {
                    TIMEOUT = Convert.ToInt32(node["Timeout"].InnerText, 10) * 1000;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("config read error");
            }

            Console.WriteLine("TIMEOUT " + TIMEOUT);

        }

    }
}
