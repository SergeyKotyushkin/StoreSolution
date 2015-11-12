using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreSolution.WebProject.Currency;

namespace UnitTestProject1StoreSolution.WebProject.UnitTests.Currency
{
    [TestClass]
    public class CurrencyConverterTests
    {
        [TestMethod]
        public void CurrencyConverter_ConvertFromRu_ReturnNotZero()
        {
            var value = 100m;
            var cultureName = "en-US";

            var actual = CurrencyConverter.ConvertFromRu(value, cultureName);
            
            var expected = decimal.Zero;

            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void CurrencyConverter_ConvertToRu_ReturnNotZero()
        {
            var value = 5m;
            var cultureName = "en-US";

            var actual = CurrencyConverter.ConvertToRu(value, cultureName);

            var expected = decimal.Zero;

            Assert.AreNotEqual(expected, actual);
        }
    }
}
