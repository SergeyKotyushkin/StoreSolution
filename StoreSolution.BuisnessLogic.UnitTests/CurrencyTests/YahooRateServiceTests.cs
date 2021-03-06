﻿using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.StructureMap;

namespace StoreSolution.BuisnessLogic.UnitTests.CurrencyTests
{
    [TestClass]
    public class YahooRateServiceTests
    {
        [TestMethod]
        public void GetRate_FromGbpToUsd_ReturnNotZeroWithoutExceptions()
        {
            StructureMapFactory.Init();

            var service = StructureMapFactory.Resolve<IRateService>();

            const decimal expected = decimal.Zero;
            var actual = expected;
            try
            {
                actual = service.GetRate(new CultureInfo("en-GB"), new CultureInfo("en-US"));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreNotEqual(expected, actual);
        }
    }
}
