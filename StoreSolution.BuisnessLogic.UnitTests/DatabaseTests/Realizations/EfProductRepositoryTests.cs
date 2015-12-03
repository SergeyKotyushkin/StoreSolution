using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoreSolution.BusinessLogic.Database.EfContexts;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.Database.Realizations;

namespace StoreSolution.BuisnessLogic.UnitTests.DatabaseTests.Realizations
{
    [TestClass]
    public class EfProductRepositoryTests
    {
        [TestMethod]
        public void Products_GetCarAndTomato_ReturnCarAndTomato()
        {
            var expected = new List<Product>
            {
                new Product {Id = 0, Name = "Car", Category = "Transport", Price = 5000},
                new Product {Id = 1, Name = "Tomato", Category = "Food", Price = 1.5m}
            };
            var efProductRepository = ArrangeEfProductRepository(expected);
            
            var actual = efProductRepository.GetAll().ToArray();

            Assert.AreEqual(2, actual.Count());
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        private static EfProductRepository ArrangeEfProductRepository(IEnumerable<Product> products)
        {
            var data = products.AsQueryable();

            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<EfProductContext>();
            mockContext.Setup(m => m.ProductTable).Returns(mockSet.Object);

            return new EfProductRepository(mockContext.Object);
        }
    }
}
