using NUnit.Framework;
using BlaiseDataDelivery;

namespace NUnitTestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddTest()
        {
            int res = Program.Add(1, 2);
            Assert.AreEqual(res, 3);
        }
        [Test]
        public void AddNegTest()
        {
            int res = Program.Add(1, 32);
            Assert.AreNotEqual(res, 3);
        }
    }
}