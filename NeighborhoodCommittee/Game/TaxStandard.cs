namespace MCCCivitasBlackTech
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    public class TaxStandard
    {
        class Standard
        {
            public EstateType type;
            public double area;
            public double price;
        }

        List<Standard> standardList = new List<Standard>();

        public void Update(EstateType type, double area, double price)
        {
            Standard s = this.find(type);
            if (s!=null)
            {
                s = new Standard();
                s.area = area;
                s.price = price;
                s.type = type;
                standardList.Add(s);
            }
            else
            {
                s.area = area;
                s.price = price;
            }
        }

        private Standard find(EstateType type)
        {
            foreach (Standard item in standardList)
            {
                if (item.type == type)
                {
                    return item;
                }
            }

            return null;
        }

        public double GetStandard(EstateType type)
        {
            Standard s = this.find(type);
            if (s==null)
            {
                return 0;
            }
            else
            {
                return s.price / s.area;
            }
        }

        static public TaxStandard LoadStandard(string path)
        {
            TaxStandard tax = new TaxStandard();
            return tax;
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
                type.InnerText = s.type.Name;
                item.AppendChild(type);
                XmlNode price = document.CreateElement("price");
                price.InnerText = s.price.ToString();
                item.AppendChild(price);
                XmlNode area = document.CreateElement("area");
                area.InnerText = s.area.ToString();
                item.AppendChild(area);
                table.AppendChild(item);
            }

            document.Save(path);
        }
    }
}
