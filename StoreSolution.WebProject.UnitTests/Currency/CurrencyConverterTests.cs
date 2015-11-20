using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreSolution.WebProject.Currency;

namespace StoreSolution.WebProject.UnitTests.Currency
{
    [TestClass]
    public class CurrencyConverterTests
    {
        [TestMethod]
        public void RefreshRate_RefreshRateUsd_ReturnNotNull()
        {
            var t = typeof (CurrencyConverter);
            var refreshMethod = t.GetMethod("RefreshRate", BindingFlags.NonPublic | BindingFlags.Static);

            var actual = (decimal?)refreshMethod.Invoke(null, new object[] { "ru-RU", "en-US" });
            
            Assert.AreNotEqual(null, actual);
        }

        [TestMethod]
        public void GetRealTimeRate_GetRateGbp_ReturnNotNullOrEmpty()
        {
            var t = typeof(CurrencyConverter);
            var refreshMethod = t.GetMethod("GetRealTimeRate", BindingFlags.NonPublic | BindingFlags.Static);

            var actual = (string)refreshMethod.Invoke(null, new object[] { "RUB", "GBP" });

            Assert.AreEqual(false, string.IsNullOrEmpty(actual));
        }

        [TestMethod]
        public void ConvertFromRu_ConvertFromRubToUsd_ReturnNotZero()
        {
            var value = 100m;
            var cultureName = "en-US";

            var actual = new CurrencyConverter().ConvertFromRu(value, cultureName);
            
            var expected = decimal.Zero;

            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertFromRuToUsdWithRate_100RubWithRate5_Return20Usd()
        {
            var value = 100m;
            var rate = 5;

            var actual = new CurrencyConverter().ConvertFromRu(value, rate);

            var expected = 20m;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertToRu_Convert50UsdToRub_ReturnNotZero()
        {
            var value = 50m;
            var cultureName = "en-US";

            var actual = new CurrencyConverter().ConvertToRu(value, cultureName);

            var expected = decimal.Zero;

            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void GetRate_GetRateGbpToRub_ReturnNotZero()
        {
            var cultureName = "en-GB";

            var actual = new CurrencyConverter().GetRate(cultureName);

            var expected = decimal.Zero;

            Assert.AreNotEqual(expected, actual);
        }
        
        [TestMethod]
        public void GetCultureNameForCurrency_GetCultureNameForUsd_ReturnEnUs()
        {
            var currencyName = "USD";

            var actual = new CurrencyConverter().GetCultureNameForCurrency(currencyName);

            var expected = "en-US";

            Assert.AreEqual(expected, actual);
        }
    }
}
