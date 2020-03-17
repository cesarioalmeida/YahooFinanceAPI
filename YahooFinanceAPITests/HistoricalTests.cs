namespace YahooFinanceAPITests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using NUnit.Framework;

    using YahooFinanceAPI;

    [TestFixture]
    public class HistoricalTests
    {
        private const string Symbol = "O39.SI";

        [Test]
        public async Task GetDividendAsyncTest()
        {
            var dividends = await Historical.GetDividendAsync(Symbol, DateTime.Now.AddYears(-2), DateTime.Now).ConfigureAwait(false);

            Assert.NotNull(dividends);
            Assert.IsTrue(dividends.Any());
            Assert.IsTrue(dividends[0].Div > 0);
        }

        [Test]
        public async Task GetPriceAsyncTest()
        {
            var prices = await Historical.GetPriceAsync(Symbol, DateTime.Now.AddYears(-1), DateTime.Now).ConfigureAwait(false);

            Assert.NotNull(prices);
            Assert.IsTrue(prices.Any());
            Assert.IsTrue(prices[0].Close > 0);
        }

        [Test]
        public async Task GetRawAsyncTest()
        {
            var raw = await Historical.GetRawAsync(Symbol, DateTime.Now.AddDays(-20), DateTime.Now, "div|split&filter=split").ConfigureAwait(false);

            Assert.IsFalse(string.IsNullOrEmpty(raw));
        }
    }
}