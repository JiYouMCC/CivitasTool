namespace MCCCivitasBlackTech
{
	using System;

	public class Estate:IComparable<Estate>
	{
		public string Name;
		public EstateType Type;
		public double Area;
		public string EstatePath;
		public string Owner;
		public string OwnerPath;

		public Estate(string name, EstateType type, double area,string estatePath,string owner,string ownerPath)
		{
			this.Name = name;
			this.Area = area;
			this.Type = type;
			this.EstatePath = estatePath;
			this.Owner = owner;
			this.OwnerPath = ownerPath;
		}

		public void Print()
		{
			Console.WriteLine (Name + " " + Owner + " " + EstatePath + " " + Type.Name);
		}

		#region IComparable implementation
		public int CompareTo (Estate other)
		{
			if (this.Owner.CompareTo (other.Owner) == 0)
				return this.EstatePath.CompareTo (other.EstatePath);

			return this.Owner.CompareTo (other.Owner);
		}
		#endregion
	}
}

