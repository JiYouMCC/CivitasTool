namespace MCCCivitasBlackTech
{
	using System;
	using System.Collections.Generic;

	public class Commodity :IComparable<Commodity>
	{
		#region IComparable implementation
		public int CompareTo (Commodity other)
		{
			if (this.ID.CompareTo(other.ID) == 0)
				return this.Level.CompareTo(other.Level);

			else return this.ID.CompareTo(other.ID);
		}
		#endregion
            
		public int ID;
		public string Name;
		public int Level;
		public List<Business> Businesses = new List<Business>();

		public Commodity(int id, string name, int level)
		{
			this.ID = id;
			this.Name = name;
			this.Level = level;
		}

		public Commodity()
		{
		}

		public void Add(Business business)
		{
			this.Businesses.Add(business);
		}

		public int BusinessCount()
		{
			return this.Businesses.Count;
		}

		public double Count()
		{
			double count = 0;
			if (this.Businesses.Count <= 0)
				return -1;

			foreach (Business b in Businesses)
				count += b.Count;

			return count;
		}

		public double AvePrice()
		{
			double sumprice = 0;
			double sumcount = 0;
			if (this.Businesses.Count <= 0)
				return -1;

			foreach (Business b in Businesses) {
				sumprice += b.Price * b.Count;
				sumcount += b.Count;
			}

			if (sumcount <= 0)
				return -1;

			return sumprice / sumcount;
		}

		public double HighPrice()
		{
			double highPrice = -1;
			if (this.Businesses.Count <= 0)
				return -1;

			foreach (Business b in Businesses)			
				if (b.Price > highPrice)
					highPrice = b.Price;

			return highPrice;
		}

		public double LowPrice()
		{
			double lowPrice = double.MaxValue;
			if (this.Businesses.Count <= 0)
				return -1;

			foreach (Business b in Businesses)
				if (b.Price < lowPrice)
					lowPrice = b.Price;

			if (lowPrice == double.MaxValue)
				return -1;

			return lowPrice;
		}
	}
}