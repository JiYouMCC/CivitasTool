namespace MCCCivitasBlackTech
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	public class Neighbor
	{
		public List<Estate> Estates = new List<Estate>();
		public int Id = -1;
		string name = string.Empty;
		bool ready=false;
		double health=0;
		double prosperity=0;
		double industry = 0;

		public double Health
		{
			get{return this.health;}
		}

		public double Prosperity
		{
			get{return this.prosperity;}
		}

		public double Industry
		{
			get{return this.industry;}
		}

		public string Name
		{
			get{return this.name;}
		}

		public bool Ready
		{
			get{return this.ready;}
		}

		public Neighbor(int id,string name,User user)
		{
			this.Id = id;
			this.name = name;
		}

		public void SeeNeighbor(User user)
		{
			if (user.IsLogin) {
				new Thread (delegate() {
					if (Id < 0)
						return;

					string text = string.Empty;			
					if (UrlHelpers.GetHtml (
						"http://civitas.soobb.com/Districts/" + Id,
						ref text,
						user.CookieContainer) == 1) {
						this.health = Convert.ToDouble (UrlHelpers.CutHead (
							UrlHelpers.CutBetween (text, "<div class=\"Subject\">卫生</div>", "%</div>"),
							"ive\">"));
						this.prosperity = Convert.ToDouble (UrlHelpers.CutHead (
							UrlHelpers.CutBetween (text, "<div class=\"Subject\">繁荣</div>", "%</div>"),
							"ive\">"));
						this.industry = Convert.ToDouble (UrlHelpers.CutHead (
							UrlHelpers.CutBetween (text, "<div class=\"Subject\">产业</div>", "%</div>"),
							"ive\">"));
					}
					text = string.Empty;
					if (UrlHelpers.GetHtml (
						"http://civitas.soobb.com/Districts/" + Id + "/Estates/", 
						ref text, 
						user.CookieContainer) == 1) {
						string strCount = UrlHelpers.CutBetween (text, "<span class=\"Count\">(共 ", "条)</span>");
						int Count = Convert.ToInt32 (strCount);
						int pageCount = Count / 20 + 1;
						for (int i = 1, j = 0; i <= pageCount; i++) {
							if (UrlHelpers.GetHtml (
								"http://civitas.soobb.com/Districts/" + Id + "/Estates/?Action=Search&Page=" + i, 
								ref text, 
								user.CookieContainer) == 1) {
								string temp = UrlHelpers.CutHead (text, "<div class=\"Estate StatisticsRow\">");
								for (int k = 0; k < 20 & j < Count; k++, j++) {
									string tempt1 = UrlHelpers.CutTail (temp, "<div class=\"Text\">");
									string name = UrlHelpers.CutBetween (UrlHelpers.CutHead (tempt1, "<h5>"), "Details/\">", "</a>");//吃货大食堂！肉既是正义！
									string typestr = UrlHelpers.CutBetween (tempt1, "</a>的", "</div>");//宅院
									string owner = UrlHelpers.CutHead (UrlHelpers.CutBetween (tempt1,
									                                                          " entityid=\"",
									                                                          "</a>的" + typestr),
									                                   ">");
									string ownerPath = UrlHelpers.CutBetween (tempt1, "<div><a href=\"", "\" class=\"WithEntityCard\" entityid=\"");
									string estatePath = UrlHelpers.CutBetween (tempt1, "<h5><a href=\"", "\">" + name);
									double area = Convert.ToDouble (UrlHelpers.CutBetween (UrlHelpers.CutBetween (tempt1, "<div class=\"Text Text2\">", "占地面积"), "Number\">", "</p>"));
									EstateType type = EstateType.GetEstateType (typestr);
									if (null == type)
										type = new EstateType (typestr);

									Estates.Add (new Estate (name, type, area, estatePath, owner, ownerPath));
									temp = UrlHelpers.CutHead (temp, "<div class=\"Estate StatisticsRow\">");
                                  	}

                                Console.Write ("["+Name[0]+"]");                              
							}
							if (j == Count)
								break;
						}
					}
                    Console.WriteLine ("\n窥探【"+Name+"】完毕，有种放学别走！");
					this.ready = true;
				}).Start ();
			}
		}
	}
}

