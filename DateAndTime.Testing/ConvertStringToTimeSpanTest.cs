using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.DateAndTime;

namespace DateAndTime.Testing
{
    [TestClass]
    public class ConvertStringToTimeSpanTest: Abstract
    {
        #region with words

        [TestMethod]
        public void MinimumValueWithWords()
        {
            var res = ConvertStringToTimeSpan.Do(minTSWithWords);
            Assert.AreEqual(res, minTimeSpan);
        }

        [TestMethod]
        public void MaximumValueWithWords()
        {
            var res = ConvertStringToTimeSpan.Do(maxTSWithWords);
            Assert.AreEqual(res, maxTimeSpan);
        }

        [TestMethod]
        public void MediumValueWithWords()
        {
            var res = ConvertStringToTimeSpan.Do(medTSWithWords);
            Assert.AreEqual(res, medTimeSpan);
        }

        #endregion


        #region without words

        [TestMethod]
        public void MinimumValueWithNone()
        {
            var res = ConvertStringToTimeSpan.Do(minTSWithNone);
            Assert.AreEqual(res, minTimeSpan);
        }

        [TestMethod]
        public void MaximumValueWithNone()
        {
            var res = ConvertStringToTimeSpan.Do(maxTSWithNone);
            Assert.AreEqual(res, maxTimeSpan);
        }

        [TestMethod]
        public void MediumValueWithNone()
        {
            var res = ConvertStringToTimeSpan.Do(medTSWithNone);
            Assert.AreEqual(res, medTimeSpan);
        }

        #endregion
    }
}
