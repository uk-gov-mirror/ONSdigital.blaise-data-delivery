using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlaiseDataDelivery;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlaiseDataDelivery.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void AddTest()
        {
            int res = Program.Add(1, 2);
            Assert.AreEqual(res, 3);
        }
        [TestMethod()]
        public void AddNegTest()
        {
            int res = Program.Add(1, 32);
            Assert.AreNotEqual(res, 3);
        }
    }
}