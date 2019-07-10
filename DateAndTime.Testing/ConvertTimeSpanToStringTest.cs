using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.DateAndTime;

namespace DateAndTime.Testing
{
    [TestClass]
    public class ConvertTimeSpanToStringTest: Abstract
    {

        #region with words

        [TestMethod]
        public void MinimumValueWithWords()
        {
            var res = ConvertTimeSpanToString.Do(minTimeSpan, true);
            Assert.AreEqual(res, minTSWithWords);
        }

        [TestMethod]
        public void MaximumValueWithWords()
        {
            var res = ConvertTimeSpanToString.Do(maxTimeSpan, true);
            Assert.AreEqual(res, maxTSWithWords);
        }

        [TestMethod]
        public void MediumValueWithWords()
        {
            var res = ConvertTimeSpanToString.Do(medTimeSpan, true);
            Assert.AreEqual(res, medTSWithWords);
        }

        #endregion


        #region without words

        [TestMethod]
        public void MinimumValueWithNone()
        {
            var res = ConvertTimeSpanToString.Do(minTimeSpan, false);
            Assert.AreEqual(res, minTSWithNone);
        }

        [TestMethod]
        public void MaximumValueWithNone()
        {
            var res = ConvertTimeSpanToString.Do(maxTimeSpan, false);
            Assert.AreEqual(res, maxTSWithNone);
        }

        [TestMethod]
        public void MediumValueWithNone()
        {
            var res = ConvertTimeSpanToString.Do(medTimeSpan, false);
            Assert.AreEqual(res, medTSWithNone);
        }

        #endregion
    }
}
