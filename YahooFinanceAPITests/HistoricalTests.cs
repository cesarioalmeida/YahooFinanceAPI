namespace YahooFinanceAPI.Tests
{
    using System;
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class HistoricalTests
    {
        private static string symbol = "O39.SI";

        [Test]
        public void GetDividendAsyncTest()
        {
            var dividends = Historical.GetDividendAsync(symbol, DateTime.Now.AddYears(-2), DateTime.Now).Result;

            Assert.NotNull(dividends);
            Assert.IsTrue(dividends.Any());
            Assert.IsTrue(dividends[0].Div > 0);
        }

        [Test]
        public void GetPriceAsyncTest()
        {
            var prices = Historical.GetPriceAsync(symbol, DateTime.Now.AddYears(-1), DateTime.Now).Result;

            Assert.NotNull(prices);
            Assert.IsTrue(prices.Any());
            Assert.IsTrue(prices[0].Close > 0);
        }

        [Test]
        public void GetRawAsyncTest()
        {
            var raw = Historical.GetRawAsync(symbol, DateTime.Now.AddDays(-20), DateTime.Now, "div|split&filter=split").Result;

            Assert.IsFalse(string.IsNullOrEmpty(raw));
        }
    }
}