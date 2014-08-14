namespace MCCCivitasBlackTech
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The Market
    /// </summary>
    public class Market
    {
        private List<Commodity> commodities = new List<Commodity>();

        public Market(int supplyId, int supplyLevel)
        {
            this.SupplyId = supplyId;
            this.SupplyLevel = supplyLevel;
            this.Time = DateTime.Now;
        }

        public int SupplyId
        {
            get;
            set;
        }

        public int SupplyLevel
        {
            get;
            set;
        }

        public DateTime Time
        {
            get;
            set;
        }

        public static bool FindCommodity(int id, int level, List<Commodity> commodities)
        {
            foreach (Commodity c in commodities)
            {
                if (c.ID == id && c.Level == level)
                {
                    return true;
                }
            }

            return false;
        }

        public static void Output(List<Market> markets, string filePath)
        {
            List<Commodity> commodityId = new List<Commodity>();
            if (markets == null || markets.Count <= 0)
            {
                return;
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            FileStream fs = new FileStream(filePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("<table>");

            // 统计商品
            foreach (Market m in markets)
            {
                foreach (Commodity c in m.commodities)
                {
                    if (!FindCommodity(c.ID, c.Level, commodityId))
                    {
                        commodityId.Add(c);
                    }
                }
            }

            commodityId.Sort();

            sw.WriteLine("<tr>");
            sw.Write("<td>");
            sw.Write("产品");
            sw.WriteLine("</td>");
            foreach (Commodity c in commodityId)
            {
                sw.Write("<td>");
                sw.Write(c.Name + c.Level + "星");
                sw.WriteLine("</td>");
            }

            sw.WriteLine("</tr>");

            foreach (Market m in markets)
            {
                sw.WriteLine("<tr>");

                // 时间
                for (int i = 0; i < commodityId.Count; i++)
                {
                    sw.Write("<td>");
                    if (i == 0)
                    {
                        sw.Write(m.Time.ToString());
                    }

                    sw.WriteLine("</td>");
                }

                sw.WriteLine("</tr>");
                sw.WriteLine("<tr>");

                // 总数
                sw.Write("<td>");
                sw.Write("总数");
                sw.Write("</td>");
                for (int i = 0, j = 0; i < commodityId.Count;)
                {
                    sw.Write("<td>");
                    if (commodityId[i].ID == m.commodities[j].ID &&
                        commodityId[i].Level == m.commodities[j].Level)
                    {
                        sw.Write(string.Format("{0:F2}", m.commodities[j].Count()));
                        i++;
                        j++;
                    }
                    else
                    {
                        i++;
                    }

                    sw.WriteLine("</td>");
                }

                sw.WriteLine("</tr>");
                sw.WriteLine("<tr>");

                // 平均价
                sw.Write("<td>");
                sw.Write("平均价");
                sw.Write("</td>");

                for (int i = 0, j = 0; i < commodityId.Count;)
                {
                    sw.Write("<td>");
                    if (commodityId[i].ID == m.commodities[j].ID &&
                        commodityId[i].Level == m.commodities[j].Level)
                    {
                        sw.Write(string.Format("{0:F3}", m.commodities[j].AvePrice()));
                        i++;
                        j++;
                    }
                    else
                    {
                        i++;
                    }

                    sw.WriteLine("</td>");
                }

                sw.WriteLine("</tr>");
                sw.WriteLine("<tr>");

                // 最高价
                sw.Write("<td>");
                sw.Write("最高价");
                sw.Write("</td>");
                for (int i = 0, j = 0; i < commodityId.Count;)
                {
                    sw.Write("<td>");
                    if (commodityId[i].ID == m.commodities[j].ID &&
                        commodityId[i].Level == m.commodities[j].Level)
                    {
                        sw.Write(string.Format("{0:F2}", m.commodities[j].HighPrice()));
                        i++;
                        j++;
                    }
                    else
                    {
                        i++;
                    }

                    sw.WriteLine("</td>");
                }

                sw.WriteLine("</tr>");
                sw.WriteLine("<tr>");

                // 最低价
                sw.Write("<td>");
                sw.Write("最低价");
                sw.Write("</td>");
                for (int i = 0, j = 0; i < commodityId.Count;)
                {
                    sw.Write("<td>");
                    if (commodityId[i].ID == m.commodities[j].ID &&
                        commodityId[i].Level == m.commodities[j].Level)
                    {
                        sw.Write(string.Format("{0:F2}", m.commodities[j].LowPrice()));
                        i++;
                        j++;
                    }
                    else
                    { 
                        i++;
                    }

                    sw.WriteLine("</td>");
                }

                sw.WriteLine("</tr>");
                sw.WriteLine("<tr>");

                // 商家数
                sw.Write("<td>");
                sw.Write("商家数");
                sw.Write("</td>");
                for (int i = 0, j = 0; i < commodityId.Count;)
                {
                    sw.Write("<td>");
                    if (commodityId[i].ID == m.commodities[j].ID &&
                        commodityId[i].Level == m.commodities[j].Level)
                    {
                        sw.Write(m.commodities[j].BusinessCount());
                        i++;
                        j++;
                    }
                    else
                    {
                        i++;
                    }

                    sw.WriteLine("</td>");
                }

                sw.WriteLine("</tr>");
            }

            sw.WriteLine("</table>");
            sw.Flush();
            Console.WriteLine("已经导出至" + filePath + ",用Excel打开哦，长得难看不是我的错~~~~~之所以要横过来是为了统计多个。。");

            sw.Close();
        }       

        public List<Commodity> SeeCommodities(string cookieContainer)
        {
            Console.WriteLine("目前不支持翻页，如果商家超过一页。。。你懂的");
            string text = string.Empty;
            this.commodities.Clear();
            if (UrlHelpers.GetHtml("http://civitas.soobb.com/Markets/Commodities/?SupplyID=" + this.SupplyId + "&SupplyLevel=" + this.SupplyLevel, ref text, cookieContainer) == 1)
            {
                string temp1 = UrlHelpers.CutBetween(text, "所需货物", "所需等级");
                do
                {
                    string temp2 = UrlHelpers.CutBetween(temp1, "class=\"Item\"", "</div>");
                    int id = Convert.ToInt32(UrlHelpers.CutBetween(temp2, "DemandID=", "&SupplyLevel"));
                    string name = UrlHelpers.CutBetween(temp2, "\">", "</a>");
                    this.commodities.Add(new Commodity(id, name, 1));
                    this.commodities.Add(new Commodity(id, name, 2));
                    temp1 = UrlHelpers.CutHead(temp1, "<div class=\"Item\"><a href=\"/Markets/Commodities/");
                } 
                while (temp1.Contains("<div class=\"Item\"><a href=\"/Markets/Commodities/"));
            }

            foreach (Commodity c in this.commodities)
            {
                // Console.WriteLine("\n" + c.Name + c.Level);
                text = string.Empty;

                // 暂不支持翻页
                if (UrlHelpers.GetHtml(
                    "http://m.civitas.soobb.com/Markets/Commodities/?InventoryID=20708&SupplyID=" + this.SupplyId + "&DemandID=" + c.ID + "&SupplyLevel=" + this.SupplyLevel + "&DemandLevel=" + c.Level,
                    ref text,
                    cookieContainer) == 1)
                {
                    if (!text.Contains("<div class=\"Column Item1\">"))
                    {
                        continue;
                    }

                    string temp = UrlHelpers.CutKeepHead(text, "<div class=\"Column Item1\">");
                    while (temp.Contains("<div class=\"Column Item1\">"))
                    {
                        temp = UrlHelpers.CutHead(temp, "<div class=\"Column Item1\">");
                        string sellerName = UrlHelpers.CutHead(
                            UrlHelpers.CutHead(
                            UrlHelpers.CutBetween(temp, "<div class=\"Content\">", "</a>"),
                            "<h4>"),
                            ">");
                        double count = Convert.ToDouble(UrlHelpers.CutBetween(temp, "<span class=\"Price1\">", "</span>"));
                        double price = Convert.ToDouble(UrlHelpers.CutBetween(temp, "<span class=\"Price2\">", "</span>"));
                        
                        // Console.WriteLine(sellerName + count + "*" + price);
                        new Business(sellerName, c, c.Level, count, price);
                    }
                }
            }

            // clear 
            for (int i = 0, j = this.commodities.Count; i < j; i++)
            {
                if (this.commodities[i].BusinessCount() == 0)
                {
                    this.commodities.RemoveAt(i);
                    i--;
                    j--;
                }
            }

            this.commodities.Sort();
            return this.commodities;
        }

        public void Print()
        {
            Console.WriteLine(DateTime.Now);
            Console.WriteLine(string.Format("{0,11}", "产品") +
                string.Format("{0}", "总数\t") +
                string.Format("{0}", "平均价\t") +
                string.Format("{0}", "最高价\t") +
                string.Format("{0}", "最低价\t") +
                string.Format("{0}", "商家数\t"));
            foreach (Commodity c in this.commodities)
            {
                Console.WriteLine(string.Format("{0,11}", c.Name + c.Level + "星\t") +
                    string.Format("{0,10}", string.Format("{0:F2}", c.Count())) +
                    string.Format("{0,10}", string.Format("{0:F3}", c.AvePrice())) +
                    string.Format("{0,10}", string.Format("{0:F2}", c.HighPrice())) +
                    string.Format("{0,10}", string.Format("{0:F2}", c.LowPrice())) +
                    string.Format("{0,10}", c.BusinessCount().ToString()));
            }
        }
    }
}