using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Extensions.StringExtensions;

namespace Extensions.Testing.StringExtensions.ExtractingTesting
{
    [TestClass]
    public class ExtractDigitsTesting: Abstract
    {
        [TestMethod]
        public void Main()
        {
            var res = Extracting.ExtractDigits(Source);
            Assert.AreEqual(Numbers, res);
        }
    }
}
