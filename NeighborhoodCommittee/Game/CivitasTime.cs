namespace MCCCivitasBlackTech.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CivitasTime
    {
        public CivitasTime(DateTime relTime)
        {
            this.RelTime = relTime;
        }

        public CivitasTime(int day, int hour, int minute)
        {
            this.RelTime = new DateTime(2014, 8, 15, hour, minute, 0).AddDays(day - 515);
        }

        public DateTime RelTime
        {
            get;
            private set;
        }       

        public int Day
        {
            get
            {
                TimeSpan timespan = this.RelTime - new DateTime(2014, 8, 15, this.Hour, this.Minute, 0);
                return timespan.Days + 515;
            }
        }

        public int Hour
        {
            get { this.RelTime.Hour; }
        }

        public int Minute
        {
            get { this.RelTime.Minute; }
        }
    }
}