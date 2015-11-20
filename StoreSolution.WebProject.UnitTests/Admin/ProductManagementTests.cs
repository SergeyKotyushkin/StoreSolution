using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreSolution.WebProject.Admin;
using StoreSolution.WebProject.Master;
using StoreSolution.WebProject.StructureMap;

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
            StructureMapFactory.Init();

            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            var moq = new Moq.Mock<StoreMaster>();
            moq.Setup(m => m.GetCurrencyCultureInfo()).Returns(new CultureInfo("ru-RU"));

            var t = typeof (ProductManagement);
            var ctor = t.GetConstructors(bindingFlags).FirstOrDefault(c => !c.GetParameters().Any());

            if (ctor == null) return null;

            var pm = (ProductManagement) ctor.Invoke(null);

            var fieldInfo = t.GetField("_master", bindingFlags);

            if (fieldInfo == null) return null;

            fieldInfo.SetValue(pm, moq.Object);

            var checkIsNewProductValidMethod = t.GetMethod("CheckIsNewProductValid", bindingFlags);
            if (checkIsNewProductValidMethod == null) return null;

            return new ArrayList{pm, checkIsNewProductValidMethod};
        }
    }
}
