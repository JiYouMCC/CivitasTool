namespace MCCCivitasBlackTech
{
    using System;

    public class Estate : IComparable<Estate>
    {
        public Estate(string name, EstateType type, double area, string estatePath, string owner, string ownerPath)
        {
            this.Name = name;
            this.Area = area;
            this.Type = type;
            this.EstatePath = estatePath;
            this.Owner = owner;
            this.OwnerPath = ownerPath;
        }       

        public double Tax
        {
            get;
            private set;
        }
        
        public string Name
        {
            get;
            set;
        }

        public EstateType Type
        {
            get;
            set;
        }

        public double Area
        {
            get;
            set;
        }

        public string EstatePath
        {
            get;
            set;
        }

        public string Owner
        {
            get;
            set;
        }

        public string OwnerPath
        {
            get;
            set;
        }

        public double CheckTax(double standard)
        {
            this.Tax = this.Area * standard;
            return this.Tax;
        }
      
        public void Print()
        {
            Console.WriteLine(this.Name + " " + this.Owner + " " + this.EstatePath + " " + this.Type.Name);
        }

        #region IComparable implementation
        public int CompareTo(Estate other)
        {
            if (this.Owner.CompareTo(other.Owner) == 0)
            {
                return this.EstatePath.CompareTo(other.EstatePath);
            }

            return this.Owner.CompareTo(other.Owner);
        }
        #endregion
    }
}