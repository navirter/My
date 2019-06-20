using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Extensions.StringExtensions;

namespace Extensions.Testing.StringExtensions
{
    [TestClass]
    public class RemovingTesting
    {
        [TestMethod]
        public void RemoveDigits()
        {
            var source = "123qwe!@#";
            var actual = source.RemoveDigits();
            Assert.AreEqual("qwe!@#", actual);
        }

        [TestMethod]
        public void RemoveLetters()
        {
            var source = "123qwe!@#";
            var actual = source.RemoveLetters();
            Assert.AreEqual("123!@#", actual);
        }
        
    }
}
