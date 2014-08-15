namespace MCCCivitasBlackTech
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    public class TaxStandard
    {
        private List<Standard> standardList = new List<Standard>();

        public static TaxStandard LoadStandard(string path)
        {
            TaxStandard tax = new TaxStandard();
            return tax;
        }

        public void Update(EstateType type, double area, double price)
        {
            Standard s = this.Find(type);
            if (s != null)
            {
                s = new Standard();
                s.Area = area;
                s.Price = price;
                s.Type = type;
                this.standardList.Add(s);
            }
            else
            {
                s.Area = area;
                s.Price = price;
            }
        }       

        public double GetStandard(EstateType type)
        {
            Standard s = this.Find(type);
            if (s == null)
            {
                return 0;
            }
            else
            {
                return s.Price / s.Area;
            }
        }      

        public void SaveStandard(string path)
        {
            XmlDocument document = new XmlDocument();
            XmlNode table = document.CreateElement("standard");
            document.AppendChild(table);
            foreach (Standard s in this.standardList)
            {
                XmlNode item = document.CreateElement("item");
                XmlNode type = document.CreateElement("type");
                type.InnerText = s.Type.Name;
                item.AppendChild(type);
                XmlNode price = document.CreateElement("price");
                price.InnerText = s.Price.ToString();
                item.AppendChild(price);
                XmlNode area = document.CreateElement("area");
                area.InnerText = s.Area.ToString();
                item.AppendChild(area);
                table.AppendChild(item);
            }

            document.Save(path);
        }

        private Standard Find(EstateType type)
        {
            foreach (Standard item in this.standardList)
            {
                if (item.Type == type)
                {
                    return item;
                }
            }

            return null;
        }

        internal class Standard
        {
            public EstateType Type;
            public double Area;
            public double Price;
        }
    }
}
