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

namespace AxxonLayoutUtility
{
    public partial class Form1 : Form
    {
        public const int TIMEOUT = 3000;
        public const int MAX_MONITOR = 4;

        public int monitorCount = 1;

        public string clientLayoutJson = "";

        public List<LayoutInfo> layoutList, layoutListR;

        //public List<LayoutInfo> layoutList_Display1, layoutList_Display2, layoutList_Display3, layoutList_Display4;
        public List<LayoutInfo>[] layoutList_Display_Array = new List<LayoutInfo>[MAX_MONITOR];

        public List<DisplayInfo> displayList;

        //public Thread layoutPlayer1, layoutPlayer2, layoutPlayer3, layoutPlayer4;
        public Thread[] layoutPlayer_Array = new Thread[MAX_MONITOR];

        public bool isAllPlaying = false;
        //public bool isLayout1Playing = false, isLayout2Playing = false, isLayout3Playing = false, isLayout4Playing = false;
        public bool[] isLayoutPlaying_Array = new bool[MAX_MONITOR];

        //public ManualResetEvent resetEvent_Display1, resetEvent_Display2, resetEvent_Display3, resetEvent_Display4;
        public ManualResetEvent[] resetEvent_Display_Array = new ManualResetEvent[MAX_MONITOR];

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();
        }

        ~Form1()
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetNetSh();
            GetCurrentLayout();
            GetCurrentDisplay();

/*
            resetEvent_Display1 = new ManualResetEvent(false);
            resetEvent_Display2 = new ManualResetEvent(false);
            resetEvent_Display3 = new ManualResetEvent(false);
            resetEvent_Display4 = new ManualResetEvent(false);
*/

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

/*
            layoutList_Display1 = new List<LayoutInfo>();
            layoutList_Display2 = new List<LayoutInfo>();
            layoutList_Display3 = new List<LayoutInfo>();
            layoutList_Display4 = new List<LayoutInfo>();
*/

            for (int i = 0; i < layoutList_Display_Array.Length; i++)
            {
                layoutList_Display_Array[i] = new List<LayoutInfo>();
            }

            // divide layout (for odd/even)
            /*for(int i = 0; i < layoutList.Count; i++)
            {
                Console.WriteLine(layoutList[i].Id + " : " + layoutList[i].Name);

                if (i % 2 == 0)
                {
                    layoutList_Display1.Add(layoutList[i]);
                    Console.WriteLine("--> Display1");
                }
                else
                {
                    layoutList_Display2.Add(layoutList[i]);
                    Console.WriteLine("--> Display2");
                }
            }*/

            foreach (LayoutInfo layout in layoutList)
            {
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

            switch (displayList.Count)
            {
                case 3:
                    button_display4.Visible = false;
                    break;
                case 2:
                    button_display4.Visible = false;
                    button_display3.Visible = false;
                    break;
                case 1:
                    button_display4.Visible = false;
                    button_display3.Visible = false;
                    button_display2.Visible = false;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //OnClickButton1();
            OnClickButton(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //OnClickButton2();
            OnClickButton(1);
        }

        private void button_display3_Click(object sender, EventArgs e)
        {
            OnClickButton(2);
        }

        private void button_display4_Click(object sender, EventArgs e)
        {
            OnClickButton(3);
        }

#if false
        private void LayoutPlayerThread1()
        {
            foreach (LayoutInfo thisloop in GetLoopLayout_Display())
            {
                resetEvent_Display1.WaitOne();

                if (isLayout1Playing == false)
                {
                    break;
                }

                string url = @"http://localhost:8888/SwitchLayout?layoutId=" + thisloop.Id + @"&displayId=" + displayList[0].Id; // \\.\DISPLAY1";
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

                Thread.Sleep(TIMEOUT);
            }
        }

        private void LayoutPlayerThread2()
        {
            foreach (LayoutInfo thisloop in GetLoopLayoutR_Display())
            {
                resetEvent_Display2.WaitOne();

                if (isLayout2Playing == false)
                {
                    break;
                }

                string url = @"http://localhost:8888/SwitchLayout?layoutId=" + thisloop.Id + @"&displayId=" + displayList[1].Id;  //\\.\DISPLAY2";
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

                Thread.Sleep(TIMEOUT);
            }
        }
#endif

        private void LayoutPlayerThread(int monitorNo)
        {
            Console.WriteLine((monitorNo + 1) + " monitor thread created");
            foreach (LayoutInfo thisloop in GetLoopLayout_Display_Array(monitorNo))
            {
                resetEvent_Display_Array[monitorNo].WaitOne();

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
                Thread.Sleep(TIMEOUT);
            }
        }
#if false
        public IEnumerable<LayoutInfo> GetLoopLayout()
        {
            while (true)
            {
                foreach (var layout in layoutList)
                {
                    yield return layout;
                }
            }
        }
#endif

#if false
        public IEnumerable<LayoutInfo> GetLoopLayout_Display1()
        {
            while (true)
            {
                foreach (var layout in layoutList_Display1)
                {
                    yield return layout;
                }
            }
        }

        public IEnumerable<LayoutInfo> GetLoopLayout_Display2()
        {
            while (true)
            {
                foreach (var layout in layoutList_Display2)
                {
                    yield return layout;
                }
            }
        }

        public IEnumerable<LayoutInfo> GetLoopLayout_Display3()
        {
            while (true)
            {
                foreach (var layout in layoutList_Display3)
                {
                    yield return layout;
                }
            }
        }

        public IEnumerable<LayoutInfo> GetLoopLayout_Display4()
        {
            while (true)
            {
                foreach (var layout in layoutList_Display4)
                {
                    yield return layout;
                }
            }
        }
#endif

        public IEnumerable<LayoutInfo> GetLoopLayout_Display_Array(int monitorNo)
        {
            while (true)
            {
                foreach (var layout in layoutList_Display_Array[monitorNo])
                {
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

                /*
                if (isLayout1Playing == false)
                {
                    isLayout1Playing = true;
                    button_display1.Text = "1번 모니터 정지";
                    if (layoutPlayer1 == null)
                    {
                        layoutPlayer1 = new Thread(LayoutPlayerThread1);
                        layoutPlayer1.Start();
                        resetEvent_Display1.Set();
                    }
                    else
                    {
                        resetEvent_Display1.Set();
                    }
                }

                if (isLayout2Playing == false)
                {
                    isLayout2Playing = true;
                    button_display2.Text = "2번 모니터 정지";
                    if (layoutPlayer2 == null)
                    {
                        layoutPlayer2 = new Thread(LayoutPlayerThread2);
                        layoutPlayer2.Start();
                        resetEvent_Display2.Set();
                    }
                    else
                    {
                        resetEvent_Display2.Set();
                    }
                }*/

                for (int i = 0; i < displayList.Count; i++)
                {
                    int monitorNo = i;
                    if (isLayoutPlaying_Array[monitorNo] == false)
                    {
                        isLayoutPlaying_Array[monitorNo] = true;
                        switch (monitorNo)
                        {
                            case 0:
                                button_display1.Text = (monitorNo + 1) + "번 모니터 정지";
                                break;
                            case 1:
                                button_display2.Text = (monitorNo + 1) + "번 모니터 정지";
                                break;
                            case 2:
                                button_display3.Text = (monitorNo + 1) + "번 모니터 정지";
                                break;
                            case 3:
                                button_display4.Text = (monitorNo + 1) + "번 모니터 정지";
                                break;
                        }
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
                    }
                }
            }
            else
            {
                isAllPlaying = false;
                button_displayAll.Text = "일괄 시작";

                /*
                if (isLayout1Playing == true)
                {
                    isLayout1Playing = false;
                    button_display1.Text = "1번 모니터 시작";
                    //layoutPlayer1.Abort();
                    //layoutPlayer1 = null;
                    resetEvent_Display1.Reset();
                }

                if (isLayout2Playing == true)
                {
                    isLayout2Playing = false;
                    button_display2.Text = "2번 모니터 시작";
                    //layoutPlayer2.Abort();
                    //layoutPlayer2 = null;
                    resetEvent_Display2.Reset();
                }*/

                for (int i = 0; i < displayList.Count; i++)
                {
                    int monitorNo = i;
                    isLayoutPlaying_Array[monitorNo] = false;
                    //button_display1.Text = "1번 모니터 시작";
                    switch (monitorNo)
                    {
                        case 0:
                            button_display1.Text = (monitorNo + 1) + "번 모니터 시작";
                            break;
                        case 1:
                            button_display2.Text = (monitorNo + 1) + "번 모니터 시작";
                            break;
                        case 2:
                            button_display3.Text = (monitorNo + 1) + "번 모니터 시작";
                            break;
                        case 3:
                            button_display4.Text = (monitorNo + 1) + "번 모니터 시작";
                            break;
                    }
                    //layoutPlayer1.Abort();
                    //layoutPlayer1 = null;
                    resetEvent_Display_Array[monitorNo].Reset();
                }
            }
        }

#if false
        private void OnClickButton1()
        {
            if (isLayout1Playing == false)
            {
                isLayout1Playing = true;
                button_display1.Text = "1번 모니터 정지";
                if (layoutPlayer1 == null)
                {
                    layoutPlayer1 = new Thread(LayoutPlayerThread1);
                    layoutPlayer1.Start();
                    resetEvent_Display1.Set();
                }
                else
                {
                    resetEvent_Display1.Set();
                }
            }
            else
            {
                isLayout1Playing = false;
                button_display1.Text = "1번 모니터 시작";
                //layoutPlayer1.Abort();
                //layoutPlayer1 = null;
                resetEvent_Display1.Reset();
            }
        }

        private void OnClickButton2()
        {
            if (isLayout2Playing == false)
            {
                isLayout2Playing = true;
                button_display2.Text = "2번 모니터 정지";
                if (layoutPlayer2 == null)
                {
                    layoutPlayer2 = new Thread(LayoutPlayerThread2);
                    layoutPlayer2.Start();
                    resetEvent_Display2.Set();
                }
                else
                {
                    resetEvent_Display2.Set();
                }
            }
            else
            {
                isLayout2Playing = false;
                button_display2.Text = "2번 모니터 시작";
                //layoutPlayer2.Abort();
                //layoutPlayer2 = null;
                resetEvent_Display2.Reset();
            }
        }
#endif

        private void OnClickButton(int monitorNo)
        {
            //PropertyInfo property = this.GetType().GetProperty("button_display" + (monitorNo+1));

            if (isLayoutPlaying_Array[monitorNo] == false)
            {
                isLayoutPlaying_Array[monitorNo] = true;
                //button_display2.Text = (monitorNo+1) + "번 모니터 정지";
                switch (monitorNo)
                {
                    case 0:
                        button_display1.Text = (monitorNo + 1) + "번 모니터 정지";
                        break;
                    case 1:
                        button_display2.Text = (monitorNo + 1) + "번 모니터 정지";
                        break;
                    case 2:
                        button_display3.Text = (monitorNo + 1) + "번 모니터 정지";
                        break;
                    case 3:
                        button_display4.Text = (monitorNo + 1) + "번 모니터 정지";
                        break;
                }
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
            }
            else
            {
                isLayoutPlaying_Array[monitorNo] = false;
                //button_display2.Text = (monitorNo + 1) + "번 모니터 시작";
                switch (monitorNo)
                {
                    case 0:
                        button_display1.Text = (monitorNo + 1) + "번 모니터 시작";
                        break;
                    case 1:
                        button_display2.Text = (monitorNo + 1) + "번 모니터 시작";
                        break;
                    case 2:
                        button_display3.Text = (monitorNo + 1) + "번 모니터 시작";
                        break;
                    case 3:
                        button_display4.Text = (monitorNo + 1) + "번 모니터 시작";
                        break;
                }
                //layoutPlayer2.Abort();
                //layoutPlayer2 = null;
                resetEvent_Display_Array[monitorNo].Reset();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            if (layoutPlayer1 != null)
            {
                layoutPlayer1.Abort();
                layoutPlayer1 = null;
            }

            if (layoutPlayer2 != null)
            {
                layoutPlayer2.Abort();
                layoutPlayer2 = null;
            }

            if (layoutPlayer3 != null)
            {
                layoutPlayer3.Abort();
                layoutPlayer3 = null;
            }

            if (layoutPlayer4 != null)
            {
                layoutPlayer4.Abort();
                layoutPlayer4 = null;
            }*/

            for (int i = 0; i < layoutPlayer_Array.Length; i++)
            {
                if (layoutPlayer_Array[i] != null)
                {
                    layoutPlayer_Array[i].Abort();
                    layoutPlayer_Array[i] = null;
                }
            }
        }

        private void button_Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button_display1_TextChanged(object sender, EventArgs e)
        {
            if (button_display1.Text.Contains("시작"))
            {
                button_display1.BackColor = Color.DarkOrange;
            }
            else if (button_display1.Text.Contains("정지"))
            {
                button_display1.BackColor = Color.Cyan;
            }
        }

        private void button_display2_TextChanged(object sender, EventArgs e)
        {
            if (button_display2.Text.Contains("시작"))
            {
                button_display2.BackColor = Color.DarkOrange;
            }
            else if (button_display2.Text.Contains("정지"))
            {
                button_display2.BackColor = Color.Cyan;
            }
        }

        private void button_display3_TextChanged(object sender, EventArgs e)
        {
            if (button_display3.Text.Contains("시작"))
            {
                button_display3.BackColor = Color.DarkOrange;
            }
            else if (button_display3.Text.Contains("정지"))
            {
                button_display3.BackColor = Color.Cyan;
            }
        }

        private void button_display4_TextChanged(object sender, EventArgs e)
        {
            if (button_display4.Text.Contains("시작"))
            {
                button_display4.BackColor = Color.DarkOrange;
            }
            else if (button_display4.Text.Contains("정지"))
            {
                button_display4.BackColor = Color.Cyan;
            }
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

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

    }
}
