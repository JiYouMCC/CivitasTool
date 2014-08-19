namespace MCCCivitasBlackTech
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;

    public class User
    {
        private const string LOGIN = "http://www.soobb.com/Accounts/AjaxAuthenticate/";
        private string cookieContainer = string.Empty;
        private string emailAddress;
        private string password;
        private bool isLogin;
        private List<Neighbor> neighbors = new List<Neighbor>();
        private string city = string.Empty;
        private bool ready = false;
        public double speechPer;

        public User(string emailAddress, string password)
        {
            this.emailAddress = emailAddress;
            this.password = password;
        }

        public bool Ready
        {
            get { return this.ready; }
        }

        public bool NeighborsReady
        {
            get
            {
                if (this.ready)
                {
                    foreach (Neighbor neighbor in this.neighbors)
                    {
                        if (!neighbor.Ready)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public List<Neighbor> Neighbors
        {
            get { return new List<Neighbor>(this.neighbors); }
        }

        public string City
        {
            get { return this.city; }
        }

        public bool IsLogin
        {
            get { return this.isLogin; }
        }

        public string CookieContainer
        {
            get { return this.cookieContainer; }
        }

        public bool Login()
        {
            if (!this.isLogin)
            {
                try
                {
                    string responseText = string.Empty;
                    string content = "Continue=http%3A%2F%2Fcivitas.soobb.com%2F&Language=zh-cn&Email="
                        + System.Uri.EscapeDataString(this.emailAddress)
                        + "&Password=" + this.password
                        + "&RememberMe=on";
                    byte[] byteContent = Encoding.Default.GetBytes(content);
                    StreamReader streamReader;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(LOGIN);
                    request.Accept = @"text/plain, */*; q=0.01";
                    request.UserAgent = "Mozilla/5.0";
                    request.Headers.Add("Accept-Encoding", "utf-8");
                    request.Headers.Add("Accept-Language", "zh-CN");
                    request.Headers["x-requested-with"] = "XMLHttpRequest";
                    request.Headers["Accept-Language"] = "zh-cn";
                    request.Headers["Accept-Encoding"] = "gzip, deflate";
                    request.UserAgent = "Mozilla/4.0 ";
                    request.Headers["Pragma"] = "no-cache";
                    request.Method = "POST";
                    request.KeepAlive = true;
                    request.Referer = UrlHelpers.REFERER;
                    request.ContentLength = byteContent.Length;
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.GetRequestStream().Write(byteContent, 0, byteContent.Length);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    streamReader = new StreamReader(response.GetResponseStream());
                    responseText = streamReader.ReadToEnd();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string cookieString = response.Headers.Get("Set-Cookie");

                        foreach (string s in cookieString.Split(';'))
                        {
                            if (s.Contains("CIVITAS-Authentication"))
                            {
                                string name = "CIVITAS-Authentication";
                                string value = UrlHelpers.CutHead(s, "CIVITAS-Authentication=");
                                this.cookieContainer = name + "=" + value;
                                this.isLogin = true;
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                this.isLogin = false;
                return false;
            }

            return true;
        }

        public void FindNeighbor()
        {
            new Thread(delegate()
            {
                string text = string.Empty;
                this.neighbors.Clear();
                if (UrlHelpers.GetHtml("http://civitas.soobb.com/Forums/", ref text, this.cookieContainer) == 1)
                {
                    this.city = UrlHelpers.CutBetween(text, "<title>", "的广场");
                    string temp = UrlHelpers.CutHead(text, "<h4>街区排行</h4>");
                    while (temp.Contains("<a href=\"/Districts/"))
                    {
                        int id = Convert.ToInt32(UrlHelpers.CutBetween(temp, "<a href=\"/Districts/", "/"));
                        string name = UrlHelpers.CutBetween(temp, "<a href=\"/Districts/" + id + "/\">", "</a>");
                        bool contains = false;
                        foreach (Neighbor nei in this.neighbors)
                        {
                            if (nei.Id == id)
                            {
                                contains = true;
                            }
                        }

                        if (!contains)
                        {
                            this.neighbors.Add(new Neighbor(id, name, this));
                        }

                        temp = UrlHelpers.CutHead(temp, "<a href=\"/Districts/");
                    }

                    this.ready = true;
                }
            }).Start();
        }
              
        public void FindSpeech()
        {
            this.FindSpeech(50);
        }
        
        public void FindSpeech(int threadCount)
        {
            List<Speech> speechList = new List<Speech>();
            int pageCount = this.GetSpeechPage();
            int maxThreadCount = threadCount;
            int currentThreadCount = 0;
            int finishPageCount = 0;
            Console.WriteLine("Page Count:" + pageCount);
            int pagePoint = 1;

            Thread time = new Thread(delegate()
                {
                    while (true)
                    {
                        if (finishPageCount == pageCount)
                        {
                            break;
                        }

                        Thread.Sleep(1000);
                        ConsoleHelpers.OutPersent((double)(100 * (double)finishPageCount / (double)pageCount));
                        this.speechPer = (double)(100 * (double)finishPageCount / (double)pageCount);
                    }
                });
            time.Start();

            for (; pagePoint <= pageCount; pagePoint++)
            {
                // 防止多线程的时候point混乱
                int tempPoint = pagePoint;
                while (true)
                {
                    if (currentThreadCount < maxThreadCount)
                    {
                        currentThreadCount++;
                        new Thread(() => this.MethodSpeech(ref speechList, tempPoint, ref finishPageCount, ref currentThreadCount)).Start();
                        break;
                    }
                }                
            }

            time.Join();
            speechList.Sort();
            using (StreamWriter sw = new StreamWriter("speech.txt", true))
            {
                foreach (var s in speechList)
                {
                    sw.WriteLine("D" + s.Time.Day.ToString("D3") + " " + s.Time.Hour.ToString("D2") + ":" + s.Time.Minute.ToString("D2") + " " + s.Owner + ":" + s.Text.Replace("\n"," ").Replace("\r"," "));
                }

                Console.WriteLine("\n"+System.Environment.CurrentDirectory + "/speech.txt");
            }
        }

        private void MethodSpeech(ref List<Speech> speechList, int number, ref int finish, ref int current)
        {
            string text = string.Empty;
            if (this.IsLogin)
            {
                if (UrlHelpers.GetHtml("http://civitas.soobb.com/Forums/Speeches/?SpeechType=1&Page=" + number, ref text, this.cookieContainer) == 1)
                {
                    text = UrlHelpers.CutBetween(text, "<div class=\"Speeches\"", "<h4>系统消息");
                    while (text.Contains("<div class=\"Speech\""))
                    {
                        text = UrlHelpers.CutHead(text, "<div class=\"Speech\"");
                        int id = Convert.ToInt32(UrlHelpers.CutBetween(text, "speechid=\"", "\""));
                        string name = UrlHelpers.CutHead(UrlHelpers.CutBetween(text, "<p class=\"Name\"><a href", "</a>："), "\">");
                        string t = UrlHelpers.CutTail(UrlHelpers.CutHead(text, "：</p>\r\n\t\t\t<p>"), "</p>");
                        int like = Convert.ToInt32(UrlHelpers.CutBetween(text, "class=\"Normal\">欢呼</a><span type=\"1\" class=\"Number\">(", ")</span>"));
                        int watch = Convert.ToInt32(UrlHelpers.CutBetween(text, "class=\"Normal\">关注</a><span type=\"3\" class=\"Number\">(", ")</span>"));
                        int dislike = Convert.ToInt32(UrlHelpers.CutBetween(text, "class=\"Normal\">倒彩</a><span type=\"2\" class=\"Number\">(", ")</span>"));
                        string time = UrlHelpers.CutBetween(text, "演讲，第", "</p>");
                        int day = -1;
                        int hour = 0;
                        int min = 0;
                        try
                        {
                            day = Convert.ToInt32(UrlHelpers.CutTail(time, "天"));
                            hour = Convert.ToInt32(UrlHelpers.CutBetween(time, "天 ", ":"));
                            min = Convert.ToInt32(UrlHelpers.CutHead(time, ":"));
                        }
                        catch
                        {
                            Console.WriteLine("\n时间有个错误，可能是BOSS发言，不带天数和时间");
                        }

                        speechList.Add(new Speech(id, name, t, like, watch, dislike, new CivitasTime(day, hour, min)));
                    }
                }
            }

            finish++;
            current--;
        }

        private int GetSpeechPage()
        {
            string text = string.Empty;
            if (this.IsLogin)
            {
                if (UrlHelpers.GetHtml("http://civitas.soobb.com/Forums/Speeches/?SpeechType=1", ref text, this.cookieContainer) == 1)
                {
                    text = UrlHelpers.CutBetween(text, "<li class=\"Break\">...</li>", "</ul>");
                    text = UrlHelpers.CutHead(text, "<li><a");
                    text = UrlHelpers.CutHead(text, "<li><a");
                    text = UrlHelpers.CutHead(text, "<li><a");
                    text = UrlHelpers.CutBetween(text, ">", "</a>");
                    return Convert.ToInt32(text);
                }
            }

            return 0;
        }
    }
}
