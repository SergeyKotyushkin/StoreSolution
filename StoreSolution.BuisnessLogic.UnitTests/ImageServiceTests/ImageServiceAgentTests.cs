using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreSolution.BusinessLogic.ImageService;

namespace StoreSolution.BuisnessLogic.UnitTests.ImageServiceTests
{
    [TestClass]
    public class ImageServiceAgentTests
    {
        [TestMethod]
        public void GetSize_Fit1000x1000ImageTo250x250Frame_ReturnNewSize250x170()
        {
            var agent = new ImageServiceAgent();

            var expected = new Size(250, 187);
            var actual = agent.GetSize(new Size(1000, 750), 250);

            Assert.AreEqual(expected, actual);
        }
    }
}
