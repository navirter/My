using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Extensions.Enumerations.StringExtensions;

namespace Extensions.Testing.EnumerationsTesting.StringExtensionsTesting.Removing
{
    [TestClass]
    public class RemoveDigits: Abstract
    {
        [TestMethod]
        public void List()
        {
            var res = All.RemoveDigits();
            var differnt = res.Union(AllWithoutDigits).Distinct().ToList();
            Assert.AreEqual(differnt.Count, AllWithoutDigits.Count);
        }

        [TestMethod]
        public void IEnum()
        {
            var res = (IEnumerable<string>)All.RemoveDigits();
            var differnt = res.Union(AllWithoutDigits).Distinct().ToList();
            Assert.AreEqual(differnt.Count, AllWithoutDigits.Count);
        }
        
        [TestMethod]
        public void Array()
        {
            var res = All.ToArray().RemoveDigits();
            var differnt = res.Union(AllWithoutDigits).Distinct().ToList();
            Assert.AreEqual(differnt.Count, AllWithoutDigits.Count);
        }
    }
}
