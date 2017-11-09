namespace YahooFinanceAPI.Models
{
    using System;

    public class HistoryPrice
    {
        public double AdjClose { get; set; }

        public double Close { get; set; }

        public DateTime Date { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Open { get; set; }

        public double Volume { get; set; }
    }
}