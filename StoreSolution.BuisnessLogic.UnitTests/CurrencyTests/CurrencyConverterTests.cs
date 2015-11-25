using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreSolution.BusinessLogic.Currency;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.StructureMap;

namespace StoreSolution.BuisnessLogic.UnitTests.CurrencyTests
{
    [TestClass]
    public class CurrencyConverterTests
    {
        [TestMethod]
        public void Convert_GbpToUsd_NotZeroResultWithoutExceptions()
        {
            var converter = ArrangeConverter();

            var cultureFrom = new CultureInfo("ru-RU");
            var cultureTo = new CultureInfo("en-US");
            const decimal value = 100m;

            const decimal expected = decimal.Zero;

            try
            {
                var actual = converter.Convert(cultureFrom, cultureTo, value, DateTime.Now);
                Assert.AreNotEqual(expected, actual);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void ConvertByRate_11_2_ConvertByRate_3_24_Return_36_29()
        {
            var converter = ArrangeConverter();

            const decimal value = 11.2m;
            const decimal rate = 3.24m;

            const decimal expected = 36.29m;

            var actual = converter.ConvertByRate(value, rate);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertFromRubles_ToUsd_NotZeroResultWithoutExceptions()
        {
            var converter = ArrangeConverter();

            var cultureTo = new CultureInfo("en-US");
            const decimal value = 100m;

            const decimal expected = decimal.Zero;

            try
            {
                var actual = converter.ConvertFromRubles(cultureTo, value, DateTime.Now);
                Assert.AreNotEqual(expected, actual);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void ConvertToRubles_FromUsd_NotZeroResultWithoutExceptions()
        {
            var converter = ArrangeConverter();

            var cultureFrom = new CultureInfo("en-US");
            const decimal value = 100m;

            const decimal expected = decimal.Zero;

            try
            {
                var actual = converter.ConvertToRubles(cultureFrom, value, DateTime.Now);
                Assert.AreNotEqual(expected, actual);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void GetRate_FromRubToUsd_NotZeroResultWithoutExceptions()
        {
            var converter = ArrangeConverter();

            var cultureFrom = new CultureInfo("ru-RU");
            var cultureTo = new CultureInfo("en-US");

            const decimal expected = decimal.Zero;

            try
            {
                var actual = converter.GetRate(cultureFrom, cultureTo, DateTime.Now);
                Assert.AreNotEqual(expected, actual);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private static CurrencyConverter ArrangeConverter()
        {
            StructureMapFactory.Init();
            
            return new CurrencyConverter(StructureMapFactory.Resolve<ICurrencyService>());
        }
    }
}
