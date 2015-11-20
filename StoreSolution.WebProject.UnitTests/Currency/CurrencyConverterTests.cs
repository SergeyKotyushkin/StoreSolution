using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreSolution.WebProject.Currency;
using StoreSolution.WebProject.Currency.Contracts;
using StructureMap;
using StructureMap.Graph;

namespace StoreSolution.WebProject.UnitTests.Currency
{
    [TestClass]
    public class CurrencyConverterTests
    {
        [TestMethod]
        public void Convert_GbpToUsd_NotZeroResultWithoutExceptions()
        {
            var converter = ArrangeConverter();

            const decimal expected = decimal.Zero;
            var actual = expected;
            try
            {
                actual = converter.Convert(new CultureInfo("en-GB"), new CultureInfo("en-US"), 100m, DateTime.Now);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertByRate_11_2_ConvertByRate_3_24_Return_36_29()
        {
            var converter = ArrangeConverter();
            const decimal expected = 36.29m;

            var actual = converter.ConvertByRate(11.2m, 3.24m);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertFromRubles_ToUsd_NotZeroResultWithoutExceptions()
        {
            var converter = ArrangeConverter();

            const decimal expected = decimal.Zero;
            var actual = expected;
            try
            {
                actual = converter.ConvertFromRubles(new CultureInfo("en-US"), 100m, DateTime.Now);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertToRubles_FromUsd_NotZeroResultWithoutExceptions()
        {
            var converter = ArrangeConverter();

            const decimal expected = decimal.Zero;
            var actual = expected;
            try
            {
                actual = converter.ConvertToRubles(new CultureInfo("en-US"), 100m, DateTime.Now);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void GetRate_FromRubToUsd_NotZeroResultWithoutExceptions()
        {
            var converter = ArrangeConverter();

            const decimal expected = decimal.Zero;
            var actual = expected;
            try
            {
                actual = converter.GetRate(new CultureInfo("ru-RU"), new CultureInfo("en-US"), DateTime.Now);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreNotEqual(expected, actual);
        }

        private static CurrencyConverter ArrangeConverter()
        {
            var container = new Container(
                x =>
                {
                    x.Scan(scan =>
                    {
                        scan.TheCallingAssembly();
                        scan.WithDefaultConventions();
                    });

                    x.For<ICurrencyService>().Use<CurrencyService>().Singleton();
                    x.For<IRateService>().Use<YahooRateService>().Singleton();
                });
            
            return new CurrencyConverter(container.GetInstance<ICurrencyService>());
        }
    }
}
