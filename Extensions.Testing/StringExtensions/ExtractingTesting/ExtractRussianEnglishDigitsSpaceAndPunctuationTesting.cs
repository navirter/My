using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Extensions.StringExtensions;

namespace Extensions.Testing.StringExtensions.ExtractingTesting
{
    [TestClass]
    public class ExtractRussianEnglishDigitsSpaceAndPunctuation: Abstract
    {
        [TestMethod]
        public void Main()
        {
            var res = Extracting.ExtractRussianEnglishDigitsSpaceAndPunctuation(Source);
            Assert.AreEqual(RusEngLetNumSpaceAndPunct, res);
        }
    }
}
