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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AxxonLayoutUtility
{
    public partial class Form1 : Form
    {
        public const int TIMEOUT = 30000;

        public string clientLayoutJson = "";
        public List<LayoutInfo> layoutList, layoutListR, layoutList_Display1, layoutList_Display2;
        public List<DisplayInfo> displayList;
        public Thread layoutPlayer1, layoutPlayer2;
        public bool isLayout1Playing = false, isLayout2Playing = false, isAllPlaying = false;

        public ManualResetEvent resetEvent_Display1, resetEvent_Display2;

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

            resetEvent_Display1 = new ManualResetEvent(false);
            resetEvent_Display2 = new ManualResetEvent(false);
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

            layoutList_Display1 = new List<LayoutInfo>();
            layoutList_Display2 = new List<LayoutInfo>();

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

        private void button1_Click(object sender, EventArgs e)
        {
            OnClickButton1();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OnClickButton2();
        }

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
                }
            }
            else
            {
                isAllPlaying = false;
                button_displayAll.Text = "일괄 시작";

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
                }
            }
        }

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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
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
        }

        private void button_Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
