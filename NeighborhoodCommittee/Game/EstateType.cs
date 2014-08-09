namespace MCCCivitasBlackTech
{	
	using System;
	using System.Collections.Generic;

	public class EstateType:IComparable<EstateType>
	{
		static public List<EstateType> estateTypes = new List<EstateType> ();
		private string name;
		public string Name
		{
			get{return this.name;}
		}

		public EstateType(string name)
		{
			this.name = name;
			if (GetEstateType (name) == null)
				estateTypes.Add (this);
			else
				return;
		}

		public static EstateType GetEstateType(string name)
		{
			foreach (EstateType  estatetype in estateTypes) 
				if (estatetype.Name == name)
					return estatetype;

			return null;
		}

		public int CompareTo (EstateType other)
		{
			return this.name.CompareTo (other.name);
		}
	}
}