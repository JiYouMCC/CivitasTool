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
    }
}
