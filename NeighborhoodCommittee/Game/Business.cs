namespace MCCCivitasBlackTech
{
    public class Business
    {
        public Business(string seller, Commodity commodity, int level, double count, double price)
        {
            this.Seller = seller;
            this.Commodity = commodity;
            this.Level = level;
            this.Count = count;
            this.Price = price;
            commodity.Add(this);
        }

        public string Seller
        {
            get;
            set;
        }

        public Commodity Commodity
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }

        public double Count
        {
            get;
            set;
        }

        public double Price
        {
            get;
            set;
        }       
    }
}