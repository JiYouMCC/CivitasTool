namespace MCCCivitasBlackTech
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    public class UrlHelpers
    {
        public static string REFERER = "http://www.soobb.com/Accounts/Login/";
        private const int RetryTimes = 10;

        public static string CutTail(string source, string cut)
        {
            return source.Substring(0, source.IndexOf(cut));
        }

        public static int GetHtml(string path, ref string url, string cookieContainer)
        {
            url = string.Empty;
            int i = 0;
            do
            {
                try
                {
                    StreamReader streamReader;
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(path);
                    request.Accept = @"text/plain, */*; q=0.01";
                    request.UserAgent = "Mozilla/5.0";
                    request.Headers.Add("Accept-Encoding", "utf-8");
                    request.Headers.Add("Accept-Language", "zh-CN");
                    request.Headers["x-requested-with"] = "XMLHttpRequest";
                    request.Headers["Accept-Language"] = "zh-cn";
                    request.UserAgent = "Mozilla/4.0 ";
                    request.Headers["Pragma"] = "no-cache";
                    request.KeepAlive = true;
                    request.Referer = REFERER;
                    if (!string.IsNullOrEmpty(cookieContainer))
                    {
                        request.Headers.Add("cookie", cookieContainer);
                    }

                    WebResponse response = request.GetResponse();
                    streamReader = new StreamReader(response.GetResponseStream());
                    url = streamReader.ReadToEnd();
                    streamReader.Close();
                    return 1;
                }
                catch (Exception ex)
                {
                    url = ex.Message;
                }
            }
            while (i++ < RetryTimes);
            return -1;
        }

        public static string CutKeepHead(string source, string cut)
        {
            return source.Substring(source.IndexOf(cut));
        }

        public static string CutHead(string source, string cut)
        {
            return source.Substring(source.IndexOf(cut) + cut.Length);
        }       

        public static string CutBetween(string source, string begin, string end)
        {
            string result = UrlHelpers.CutHead(source, begin);
            result = UrlHelpers.CutTail(result, end);
            return result;
        }
    }
}