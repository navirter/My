using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Extensions.Enumerations.StringExtensions;

namespace Extensions.Testing.EnumerationsTesting.StringExtensionsTesting.Removing
{
    [TestClass]
    public class RemoveLetters: Abstract
    {
        [TestMethod]
        public void List()
        {
            var res = All.RemoveLetters();
            var differnt = res.Union(AllWithoutLetters).Distinct().ToList();
            Assert.AreEqual(differnt.Count, AllWithoutLetters.Count);
        }

        [TestMethod]
        public void IEnum()
        {
            var res = (IEnumerable<string>)All.RemoveLetters();
            var differnt = res.Union(AllWithoutLetters).Distinct().ToList();
            Assert.AreEqual(differnt.Count, AllWithoutLetters.Count);
        }

        [TestMethod]
        public void Array()
        {
            var res = All.ToArray().RemoveLetters();
            var differnt = res.Union(AllWithoutLetters).Distinct().ToList();
            Assert.AreEqual(differnt.Count, AllWithoutLetters.Count);
        }
    }
}
