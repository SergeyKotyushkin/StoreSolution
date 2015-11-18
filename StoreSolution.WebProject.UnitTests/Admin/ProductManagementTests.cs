using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreSolution.WebProject.Admin;
using StoreSolution.WebProject.Master;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.DatabaseProject.Realizations;
using StoreSolution.WebProject.Currency;
using StoreSolution.WebProject.Currency.Contracts;
using StructureMap;

namespace StoreSolution.WebProject.UnitTests.Admin
{
    [TestClass]
    public class ProductManagementTests
    {
        [TestMethod]
        public void CheckIsNewProductValid_CheckCorrectProduduct_GetTrueAnd123Rub()
        {
            var productManagement = (ProductManagement)ArrangeProductManagmentForCheckIsNewProductValidMethod()[0];
            var checkIsNewProductValidMethod = (MethodInfo)ArrangeProductManagmentForCheckIsNewProductValidMethod()[1];
            var parameters = new object[] { "Salt", "Food", "123", 0m };

            var actual = checkIsNewProductValidMethod.Invoke(productManagement, parameters);

            Assert.AreEqual(true, actual);
            Assert.AreEqual(123m, (decimal)parameters[3]);
        }

        private static ArrayList ArrangeProductManagmentForCheckIsNewProductValidMethod()
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            ObjectFactory.Initialize(x =>
            {
                x.For<IProductRepository>().Use<EfProductRepository>();
                x.For<ICurrencyConverter>().Use<CurrencyConverter>();
            });

            var moq = new Moq.Mock<StoreMaster>();
            moq.Setup(m => m.GetCurrencyCultureInfo()).Returns(new CultureInfo("ru-RU"));

            //var mocks = new MockFactory();
            //var masterMock = mocks.CreateMock<StoreMaster>();
            //masterMock.Expects.One.Method(m => m.GetCurrencyCultureInfo()).WillReturn(new CultureInfo("ru-RU"));

            var t = typeof (ProductManagement);
            var ctor = t.GetConstructors(bindingFlags).FirstOrDefault(c => !c.GetParameters().Any());

            if (ctor == null) return null;

            var pm = (ProductManagement) ctor.Invoke(null);

            var fieldInfo = t.GetField("_master", bindingFlags);

            if (fieldInfo == null) return null;

            //fieldInfo.SetValue(pm, masterMock.MockObject);
            fieldInfo.SetValue(pm, moq.Object);

            var checkIsNewProductValidMethod = t.GetMethod("CheckIsNewProductValid", bindingFlags);
            if (checkIsNewProductValidMethod == null) return null;

            return new ArrayList{pm, checkIsNewProductValidMethod};
        }
    }
}
