namespace MCCCivitasBlackTech
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// 市场
    /// </summary>
    public class Market
    {
        public int SupplyId;
        public int SupplyLevel;
        public DateTime Time = DateTime.Now;
        List<Commodity> Commodities = new List<Commodity>();

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
            List<Commodity> CommodityId = new List<Commodity>();
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
                foreach (Commodity c in m.Commodities)
                {
                    if (!FindCommodity(c.ID, c.Level, CommodityId))
                    {
                        CommodityId.Add(c);
                    }
                }
            }

            CommodityId.Sort();

            sw.WriteLine("<tr>");
            sw.Write("<td>");
            sw.Write("产品");
            sw.WriteLine("</td>");
            foreach (Commodity c in CommodityId)
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
                for (int i = 0; i < CommodityId.Count; i++)
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
                for (int i = 0, j = 0; i < CommodityId.Count;)
                {
                    sw.Write("<td>");
                    if (CommodityId[i].ID == m.Commodities[j].ID &&
                        CommodityId[i].Level == m.Commodities[j].Level)
                    {
                        sw.Write(string.Format("{0:F2}", m.Commodities[j].Count()));
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

                for (int i = 0, j = 0; i < CommodityId.Count;)
                {
                    sw.Write("<td>");
                    if (CommodityId[i].ID == m.Commodities[j].ID &&
                        CommodityId[i].Level == m.Commodities[j].Level)
                    {
                        sw.Write(string.Format("{0:F3}", m.Commodities[j].AvePrice()));
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
                for (int i = 0, j = 0; i < CommodityId.Count;)
                {
                    sw.Write("<td>");
                    if (CommodityId[i].ID == m.Commodities[j].ID &&
                        CommodityId[i].Level == m.Commodities[j].Level)
                    {
                        sw.Write(string.Format("{0:F2}", m.Commodities[j].HighPrice()));
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
                for (int i = 0, j = 0; i < CommodityId.Count;)
                {
                    sw.Write("<td>");
                    if (CommodityId[i].ID == m.Commodities[j].ID &&
                        CommodityId[i].Level == m.Commodities[j].Level)
                    {
                        sw.Write(string.Format("{0:F2}", m.Commodities[j].LowPrice()));
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
                for (int i = 0, j = 0; i < CommodityId.Count;)
                {
                    sw.Write("<td>");
                    if (CommodityId[i].ID == m.Commodities[j].ID &&
                        CommodityId[i].Level == m.Commodities[j].Level)
                    {
                        sw.Write(m.Commodities[j].BusinessCount());
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

        public Market(int supplyId, int supplyLevel)
        {
            this.SupplyId = supplyId;
            this.SupplyLevel = supplyLevel;
        }

        public List<Commodity> SeeCommodities(string cookieContainer)
        {
            Console.WriteLine("目前不支持翻页，如果商家超过一页。。。你懂的");
            string text = string.Empty;
            this.Commodities.Clear();
            if (UrlHelpers.GetHtml("http://civitas.soobb.com/Markets/Commodities/?SupplyID=" + this.SupplyId + "&SupplyLevel=" + this.SupplyLevel, ref text, cookieContainer) == 1)
            {
                string temp1 = UrlHelpers.CutBetween(text, "所需货物", "所需等级");
                do
                {
                    string temp2 = UrlHelpers.CutBetween(temp1, "class=\"Item\"", "</div>");
                    int id = Convert.ToInt32(UrlHelpers.CutBetween(temp2, "DemandID=", "&SupplyLevel"));
                    string name = UrlHelpers.CutBetween(temp2, "\">", "</a>");
                    this.Commodities.Add(new Commodity(id, name, 1));
                    this.Commodities.Add(new Commodity(id, name, 2));
                    temp1 = UrlHelpers.CutHead(temp1, "<div class=\"Item\"><a href=\"/Markets/Commodities/");
                } 
                while (temp1.Contains("<div class=\"Item\"><a href=\"/Markets/Commodities/"));
            }

            foreach (Commodity c in this.Commodities)
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
            for (int i = 0, j = this.Commodities.Count; i < j; i++)
            {
                if (this.Commodities[i].BusinessCount() == 0)
                {
                    this.Commodities.RemoveAt(i);
                    i--;
                    j--;
                }
            }

            this.Commodities.Sort();
            return this.Commodities;
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
            foreach (Commodity c in this.Commodities)
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