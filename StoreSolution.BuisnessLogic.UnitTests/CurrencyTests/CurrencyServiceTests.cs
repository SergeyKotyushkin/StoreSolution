using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.StructureMap;

namespace StoreSolution.BuisnessLogic.UnitTests.CurrencyTests
{
    [TestClass]
    public class CurrencyServiceTests
    {
        [TestMethod]
        public void GetRate_FromGbpToUsdOneTime_ReturnNotZeroWithoutExceptions()
        {
            StructureMapFactory.Init();

            var service = StructureMapFactory.Resolve<ICurrencyService>();

            const decimal expected = decimal.Zero;
            var actual = expected;
            try
            {
                actual = service.GetRate(new CultureInfo("en-GB"), new CultureInfo("en-US"), DateTime.Now);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void GetRate_FromGbpToUsdTwoTimes_ReturnNotZeroWithoutExceptions()
        {
            StructureMapFactory.Init();

            var service = StructureMapFactory.Resolve<ICurrencyService>();

            const decimal expected = decimal.Zero;
            var actual = expected;
            var dateTimeFirstCall = DateTime.Now;

            try
            {
                service.GetRate(new CultureInfo("en-GB"), new CultureInfo("en-US"), dateTimeFirstCall);
                actual = service.GetRate(new CultureInfo("en-GB"), new CultureInfo("en-US"), dateTimeFirstCall.AddHours(3));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void GetRealTimeRate_FromUsdToRub_ReturnNotZeroWithoutExceptions()
        {
            StructureMapFactory.Init();

            var service = StructureMapFactory.Resolve<ICurrencyService>();

            const decimal expected = decimal.Zero;
            var actual = expected;
            try
            {
                actual = service.GetRealTimeRate(new CultureInfo("en-GB"), new CultureInfo("en-US"), DateTime.Now);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void CheckIsRateActual_CallFirstTime_ReturnFalseWithoutExceptions()
        {
            StructureMapFactory.Init();

            var service = StructureMapFactory.Resolve<ICurrencyService>();

            const bool expected = false;
            var actual = expected;

            try
            {
                actual = service.CheckIsRateActual(new CultureInfo("en-GB"), new CultureInfo("en-US"), DateTime.Now);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreEqual(expected, actual);
        }
        
        [TestMethod]
        public void CheckIsRateActual_Call_5_MinutesAfterLastUpdateRate_ReturnTrueWithoutExceptions()
        {
            StructureMapFactory.Init();

            var service = StructureMapFactory.Resolve<ICurrencyService>();

            const bool expected = true;
            var actual = expected;
            var cultureForm = new CultureInfo("en-GB");
            var cultureTo = new CultureInfo("en-US");
            var dateTimeLastUpdateRate = DateTime.Now;
            const int differenceInMinutes = 5;

            try
            {
                service.GetRealTimeRate(cultureForm, cultureTo, dateTimeLastUpdateRate);
                actual = service.CheckIsRateActual(cultureForm, cultureTo, dateTimeLastUpdateRate.AddMinutes(differenceInMinutes));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreEqual(expected, actual);
        }

        [TestMethod] public void CheckIsRateActual_Call_32_MinutesAfterLastUpdateRate_ReturnFalseWithoutExceptions()
        {
            StructureMapFactory.Init();

            var service = StructureMapFactory.Resolve<ICurrencyService>();

            const bool expected = false;
            var actual = expected;
            var cultureForm = new CultureInfo("en-GB");
            var cultureTo = new CultureInfo("en-US");
            var dateTimeLastUpdateRate = DateTime.Now;
            const int differenceInMinutes = 32;

            try
            {
                service.GetRealTimeRate(cultureForm, cultureTo, dateTimeLastUpdateRate);
                actual = service.CheckIsRateActual(cultureForm, cultureTo, dateTimeLastUpdateRate.AddMinutes(differenceInMinutes));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreEqual(expected, actual);
        }
    }
}
