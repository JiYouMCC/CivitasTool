namespace MCCCivitasBlackTech
{
    using System;
    using System.Collections.Generic;

    public class EstateType : IComparable<EstateType>
    {
        public static List<EstateType> EstateTypes = new List<EstateType>();
        private string name;
       
        public EstateType(string name)
        {
            this.name = name;
            if (GetEstateType(name) == null)
            {
                EstateTypes.Add(this);
            }
            else
            {
                return;
            }
        }

        public string Name
        {
            get { return this.name; }
        }

        public static EstateType GetEstateType(string name)
        {
            foreach (EstateType estatetype in EstateTypes)
            {
                if (estatetype.Name == name)
                {
                    return estatetype;
                }
            }

            return null;
        }

        public int CompareTo(EstateType other)
        {
            return this.name.CompareTo(other.name);
        }
    }
}