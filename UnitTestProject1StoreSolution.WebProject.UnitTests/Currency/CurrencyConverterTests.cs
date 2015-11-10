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
            var ci = CultureInfo.GetCultureInfo("en-US");

            var actual = CurrencyConverter.ConvertFromRu(value, ci);
            
            var expected = decimal.Zero;

            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void CurrencyConverter_ConvertToRu_ReturnNotZero()
        {
            var value = 5m;
            var ci = CultureInfo.GetCultureInfo("en-US");

            var actual = CurrencyConverter.ConvertToRu(value, ci);

            var expected = decimal.Zero;

            Assert.AreNotEqual(expected, actual);
        }
    }
}
