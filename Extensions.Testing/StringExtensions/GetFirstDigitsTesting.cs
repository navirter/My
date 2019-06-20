using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Extensions.StringExtensions;

namespace Extensions.Testing.StringExtensions.GetFirstTesting
{
    [TestClass]
    public class GetFirstDigitsTesting
    {
        [TestMethod]
        public void Main()
        {
            string source = "123qwe456";
            var expected = 123;
            var actual = source.GetFirstDigits();
            Assert.AreEqual(expected, actual);
        }
    }
}
