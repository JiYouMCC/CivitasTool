namespace MCCCivitasBlackTech
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Food
    {
        public string Name;
        public double Hunger;
        public double Happy;
        public double Health;
        public double Stream;
        public int Level;
        public double Price;

        public Food(
            string name,
            double hunger,
            double happy,
            double health,
            double stream,
            int level,
            double price)
        {
            this.Name = name;
            this.Hunger = hunger;
            this.Happy = happy;
            this.Stream = stream;
            this.Level = level;
            this.Price = price;
        }
    }

    public class Recipy
    {
        private List<Food> foods = new List<Food>();
        private List<double> account = new List<double>();
    }
}