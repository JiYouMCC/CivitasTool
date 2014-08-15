namespace MCCCivitasBlackTech
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class Neighbor
    {
        public List<Estate> Estates = new List<Estate>();
        private string name = string.Empty;
        private bool ready = false;
        private double health = 0;
        private double prosperity = 0;
        private double industry = 0;
        private int id = -1;

        public Neighbor(int id, string name, User user)
        {
            this.id = id;
            this.name = name;
        }

        public int Id
        {
            get { return this.id; }
        }

        public double Health
        {
            get { return this.health; }
        }

        public double Prosperity
        {
            get { return this.prosperity; }
        }

        public double Industry
        {
            get { return this.industry; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public bool Ready
        {
            get { return this.ready; }
        }

        public void SeeNeighbor(User user)
        {
            if (user.IsLogin)
            {
                new Thread(delegate()
                {
                    if (this.Id < 0)
                    {
                        return;
                    }

                    string text = string.Empty;
                    if (UrlHelpers.GetHtml(
                        "http://civitas.soobb.com/Districts/" + this.Id,
                        ref text,
                        user.CookieContainer) == 1)
                    {
                        this.health = Convert.ToDouble(UrlHelpers.CutHead(
                            UrlHelpers.CutBetween(text, "<div class=\"Subject\">卫生</div>", "%</div>"),
                            "ive\">"));
                        this.prosperity = Convert.ToDouble(UrlHelpers.CutHead(
                            UrlHelpers.CutBetween(text, "<div class=\"Subject\">繁荣</div>", "%</div>"),
                            "ive\">"));
                        this.industry = Convert.ToDouble(UrlHelpers.CutHead(
                            UrlHelpers.CutBetween(text, "<div class=\"Subject\">产业</div>", "%</div>"),
                            "ive\">"));
                    }

                    text = string.Empty;
                    if (UrlHelpers.GetHtml(
                        "http://civitas.soobb.com/Districts/" + this.Id + "/Estates/",
                        ref text,
                        user.CookieContainer) == 1)
                    {
                        string strCount = UrlHelpers.CutBetween(text, "<span class=\"Count\">(共 ", "条)</span>");
                        int count = Convert.ToInt32(strCount);
                        int pageCount = (count / 20) + 1;
                        for (int i = 1, j = 0; i <= pageCount; i++)
                        {
                            if (UrlHelpers.GetHtml(
                                "http://civitas.soobb.com/Districts/" + this.Id + "/Estates/?Action=Search&Page=" + i,
                                ref text,
                                user.CookieContainer) == 1)
                            {
                                string temp = UrlHelpers.CutHead(text, "<div class=\"Estate StatisticsRow\">");
                                for (int k = 0; k < 20 & j < count; k++, j++)
                                {
                                    string tempt1 = UrlHelpers.CutTail(temp, "<div class=\"Text\">");
                                    string name = UrlHelpers.CutBetween(UrlHelpers.CutHead(tempt1, "<h5>"), "Details/\">", "</a>");
                                    string typestr = UrlHelpers.CutBetween(tempt1, "</a>的", "</div>");
                                    string owner = UrlHelpers.CutHead(UrlHelpers.CutBetween(tempt1, " entityid=\"", "</a>的" + typestr), ">");
                                    string ownerPath = UrlHelpers.CutBetween(tempt1, "<div><a href=\"", "\" class=\"WithEntityCard\" entityid=\"");
                                    string estatePath = UrlHelpers.CutBetween(tempt1, "<h5><a href=\"", "\">" + name);
                                    double area = Convert.ToDouble(UrlHelpers.CutBetween(UrlHelpers.CutBetween(tempt1, "<div class=\"Text Text2\">", "占地面积"), "Number\">", "</p>"));
                                    EstateType type = EstateType.GetEstateType(typestr);
                                    if (null == type)
                                    {
                                        type = new EstateType(typestr);
                                    }

                                    this.Estates.Add(new Estate(name, type, area, estatePath, owner, ownerPath));
                                    temp = UrlHelpers.CutHead(temp, "<div class=\"Estate StatisticsRow\">");
                                }

                                Console.Write("[" + this.Name[0] + "]");
                            }

                            if (j == count)
                            {
                                break;
                            }
                        }
                    }

                    Console.WriteLine("\n窥探【" + this.Name + "】完毕，有种放学别走！");
                    this.ready = true;
                }).Start();
            }
        }

        public void CheckTax(TaxStandard standard)
        {
            if (this.ready)
            {
                foreach (Estate es in this.Estates)
                {
                    es.CheckTax(standard.GetStandard(es.Type));
                }
            }
        }
    }
}