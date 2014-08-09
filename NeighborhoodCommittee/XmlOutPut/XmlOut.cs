namespace MCCCivitasBlackTech
{
	using System;
	using System.Xml;
	using System.Collections.Generic;
	using System.IO;

	public class XmlOut
	{
		public XmlOut ()
		{
		}

		static public List<string> NeighborOut(User user)
		{
			List<string> xmllist = new List<string> ();
			if (user.Ready) {
				foreach (Neighbor neighbor in user.Neighbors) {
					XmlDocument document = new XmlDocument ();
					XmlNode table = document.CreateElement ("table");
					document.AppendChild (table);
					//区名，时间
					{
						XmlNode tr = document.CreateElement ("tr");
						table.AppendChild (tr);
						XmlNode tdname = document.CreateElement ("td");
						tdname.InnerText = neighbor.Name;
						tr.AppendChild (tdname);
						XmlNode tddate = document.CreateElement ("td");
						tddate.InnerText = DateTime.Now.ToString();
						tr.AppendChild (tddate);
					}
					//卫生繁荣产业
					{
						XmlNode tr = document.CreateElement ("tr");
						table.AppendChild (tr);
						{
							XmlNode tdname = document.CreateElement ("td");
							tdname.InnerText = "卫生指数" + neighbor.Health + "%";
							tr.AppendChild (tdname);
						}
						{
							XmlNode tdname = document.CreateElement ("td");
							tdname.InnerText = "繁荣指数" + neighbor.Prosperity + "%";
							tr.AppendChild (tdname);
						}
						{
							XmlNode tdname = document.CreateElement ("td");
							tdname.InnerText = "产业指数" + neighbor.Industry + "%";
							tr.AppendChild (tdname);
						}
					}
					List<List<Estate>> list = new List<List<Estate>> ();
					EstateType.estateTypes.Sort ();
					foreach (EstateType type in EstateType.estateTypes) {
						list.Add (new List<Estate> ());
					}
					foreach(Estate es in neighbor.Estates)
					{
						list [EstateType.estateTypes.IndexOf (es.Type)].Add (es);
					}
					foreach (List<Estate> item in list) {
						if (item.Count > 0) {
							double area = 0;
							item.Sort ();
							foreach (Estate estate in item)
								area += estate.Area;
							//汇总
							{
								XmlNode tr = document.CreateElement ("tr");
								table.AppendChild (tr);
								tr.AppendChild (document.CreateElement ("td"));
								tr.AppendChild (document.CreateElement ("td"));
								XmlNode tdname = document.CreateElement ("td");
								tdname.InnerText = item [0].Type.Name;
								tr.AppendChild (tdname);
								XmlNode tdcount = document.CreateElement ("td");
								tdcount.InnerText = item.Count.ToString();
								tr.AppendChild (tdcount);
								XmlNode tdarea = document.CreateElement ("td");
								tdarea.InnerText = area.ToString();
								tr.AppendChild (tdarea);
							}
						}
					}
					//合计
					{
						XmlNode tr = document.CreateElement ("tr");
						table.AppendChild (tr);
					}
					//标题栏
					{
						XmlNode tr = document.CreateElement ("tr");
						table.AppendChild (tr);
						XmlNode td1 = document.CreateElement ("td");
						td1.InnerText = "不动产名称";
						tr.AppendChild (td1);
						XmlNode td2 = document.CreateElement ("td");
						td2.InnerText = "不动产主人";
						tr.AppendChild (td2);
						XmlNode td3 = document.CreateElement ("td");
						td3.InnerText ="类型";
						tr.AppendChild (td3);
						XmlNode td4 = document.CreateElement ("td");
						td4.InnerText = "数量";
						tr.AppendChild (td4);
						XmlNode td5 = document.CreateElement ("td");
						td5.InnerText = "面积";
						tr.AppendChild (td5);
					}
					//不动产
					foreach (List<Estate> item in list) {
						if (item.Count > 0) {
							foreach (Estate estate in item) {
								XmlNode tr = document.CreateElement ("tr");
								table.AppendChild (tr);
								XmlNode td1 = document.CreateElement ("td");
								tr.AppendChild (td1);
								XmlElement td1a = document.CreateElement ("a");
								td1a.SetAttribute ("href", "http://civitas.soobb.com" + estate.EstatePath);
								td1a.InnerText = estate.Name;
								td1.AppendChild (td1a);
								XmlNode td2 = document.CreateElement ("td");
								tr.AppendChild (td2);
								XmlElement td2a = document.CreateElement ("a");
								td2a.SetAttribute ("href", "http://civitas.soobb.com" + estate.OwnerPath);
								td2a.InnerText = estate.Owner;
								td2.AppendChild (td2a);
								XmlNode td3 = document.CreateElement ("td");
								td3.InnerText = estate.Type.Name;
								tr.AppendChild (td3);
								tr.AppendChild (document.CreateElement ("td"));
								XmlNode td5 = document.CreateElement ("td");
								td5.InnerText =estate.Area.ToString();
								tr.AppendChild (td5);
							}
						}
					}
					document.Save (neighbor.Name+".xml");
					xmllist.Add(System.Environment.CurrentDirectory + "/"+neighbor.Name + ".xml.");
				}
			}
			return xmllist;
		}
	}
}

