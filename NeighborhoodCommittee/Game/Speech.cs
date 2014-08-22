//-----------------------------------------------------------------------
// <copyright file="Speech.cs" company="No Company">
//  Copyright (c)
// </copyright>
// <author>Ji You</author>
//-----------------------------------------------------------------------
namespace MCCCivitasBlackTech
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// The speech
    /// </summary>
    public class Speech : IComparable<Speech>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Speech" /> class
        /// </summary>
        /// <param name="id">The speech id</param>
        /// <param name="owner">The speech owner</param>
        /// <param name="text">The speech text</param>
        /// <param name="like">The count of like</param>
        /// <param name="watch">The count of watch</param>
        /// <param name="dislike">The count of dislike</param>
        /// <param name="time">The speech CIVITAS time</param>
        public Speech(int id, string owner, string text, int like, int watch, int dislike, CivitasTime time)
        {
            this.Id = id;
            this.Owner = owner;
            this.Text = text;
            this.Time = time;
            this.Like = like;
            this.Watch = watch;
            this.DisLike = dislike;
        }

        /// <summary>
        /// Gets the speech percent
        /// </summary>
        public static double SpeechPer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the speech ID
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the speech owner
        /// </summary>
        public string Owner { get; private set; }

        /// <summary>
        /// Gets the speech text
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the speech CIVITAS time
        /// </summary>
        public CivitasTime Time { get; private set; }

        /// <summary>
        /// Gets count of like
        /// </summary>
        public int Like { get; private set; }

        /// <summary>
        /// Gets count of watch
        /// </summary>
        public int Watch { get; private set; }

        /// <summary>
        /// Gets count of dislike
        /// </summary>
        public int DisLike { get; private set; }

        /// <summary>
        /// To Get the speeches, default max thread count is 50
        /// </summary>
        /// <param name="cookieContainer">The user connection cookie container</param>
        public static void FindSpeech(string cookieContainer)
        {
            FindSpeech(50, cookieContainer);
        }

        /// <summary>
        /// To Get the speeches
        /// </summary>
        /// <param name="threadCount">The max thread count</param>
        /// <param name="cookieContainer">The user connection cookie container</param>
        public static void FindSpeech(int threadCount, string cookieContainer)
        {
            List<Speech> speechList = new List<Speech>();
            int pageCount = GetSpeechPageCount(1, cookieContainer);
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
                    SpeechPer = (double)(100 * (double)finishPageCount / (double)pageCount);
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
                        new Thread(() => MethodSpeech(ref speechList, cookieContainer, tempPoint, ref finishPageCount, ref currentThreadCount)).Start();
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
                    sw.WriteLine("D" + s.Time.Day.ToString("D3") + " " + s.Time.Hour.ToString("D2") + ":" + s.Time.Minute.ToString("D2") + " " + s.Owner + ":" + s.Text.Replace("\n", " ").Replace("\r", " "));
                }

                Console.WriteLine("\n" + System.Environment.CurrentDirectory + "/speech.txt");
            }
        }

        #region IComparable implementation
        /// <summary>
        /// The comparable implementation of speech
        /// </summary>
        /// <param name="other">The other speech</param>
        /// <returns>The compare result</returns>
        public int CompareTo(Speech other)
        {
            return this.Id.CompareTo(other.Id);
        }
        #endregion

        /// <summary>
        /// The thread method of speech
        /// </summary>
        /// <param name="speechList">The reference speech list</param>
        /// <param name="cookieContainer">The user connection cookie container</param>
        /// <param name="number">The speech page number</param>
        /// <param name="finish">The reference finish count</param>
        /// <param name="current">The reference current working speech count</param>
        private static void MethodSpeech(ref List<Speech> speechList, string cookieContainer, int number, ref int finish, ref int current)
        {
            string text = string.Empty;
            if (UrlHelpers.GetHtml("http://civitas.soobb.com/Forums/Speeches/?SpeechType=1&Page=" + number, ref text, cookieContainer) == 1)
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

            finish++;
            current--;
        }

        /// <summary>
        /// The the speech page count
        /// </summary>
        /// <param name="speechType">The speech type</param>
        /// <param name="cookieContainer">The user connection cookie container</param>
        /// <returns>The speech page count</returns>
        private static int GetSpeechPageCount(int speechType, string cookieContainer)
        {
            string text = string.Empty;
            if (UrlHelpers.GetHtml("http://civitas.soobb.com/Forums/Speeches/?SpeechType=" + speechType, ref text, cookieContainer) == 1)
            {
                text = UrlHelpers.CutBetween(text, "<li class=\"Break\">...</li>", "</ul>");
                text = UrlHelpers.CutHead(text, "<li><a");
                text = UrlHelpers.CutHead(text, "<li><a");
                text = UrlHelpers.CutHead(text, "<li><a");
                text = UrlHelpers.CutBetween(text, ">", "</a>");
                return Convert.ToInt32(text);
            }

            return 0;
        }
    }
}
