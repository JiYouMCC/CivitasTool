namespace MCCCivitasBlackTech
{
    public class Business
    {
        public string Seller;
        public Commodity Commodity;
        public int Level;
        public double Count;
        public double Price;

        public Business(string seller, Commodity commodity, int level, double count, double price)
        {
            this.Seller = seller;
            this.Commodity = commodity;
            this.Level = level;
            this.Count = count;
            this.Price = price;
            commodity.Add(this);
        }
    }
}