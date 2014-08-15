namespace MCCCivitasBlackTech
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Speech:IComparable<Speech>
    {
        public string Owner { get; private set; }
        public string Text { get; private set; }
        public CivitasTime Time { get; private set; }
        public int Like { get; private set; }
        public int Watch { get; private set; }
        public int DisLike { get; private set; }

        public Speech(string owner, string text, int like, int watch, int dislike, CivitasTime time)
        {
            this.Owner = owner;
            this.Text = text;
            this.Time = time;
            this.Like = like;
            this.Watch = watch;
            this.DisLike = dislike;
        }

        #region IComparable implementation

        public int CompareTo (Speech other)
        {
            return this.Time.RelTime.CompareTo (other.Time.RelTime);
        }

        #endregion
    }
}
