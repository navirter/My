using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.DateAndTime;

namespace DateAndTime.Testing
{
    [TestClass]
    public class ConvertDateTimeToStringTest : Abstract
    {
     
        #region withTime = false, withWords = false
        [TestMethod]
        public void MinimumValue()
        {
            var res = ConvertDateTimeToString.Do(minDateTime, false, false);
            Assert.AreEqual(res, minDTWithNone);
        }

        [TestMethod]
        public void MaximumValue()
        {
            var res = ConvertDateTimeToString.Do(maxDateTime, false, false);
            Assert.AreEqual(res, maxDTWithNone);
        }

        [TestMethod]
        public void MediumValue()
        {
            var res = ConvertDateTimeToString.Do(medDateTime, false, false);
            Assert.AreEqual(res, medDTWithNone);
        }
        #endregion

        #region withTime = false, withWords = true
        [TestMethod]
        public void MinimumValueWithWords()
        {
            var res = ConvertDateTimeToString.Do(minDateTime, false, true);
            Assert.AreEqual(res, minDTWithWords);
        }

        [TestMethod]
        public void MaximumValueWithWords()
        {
            var res = ConvertDateTimeToString.Do(maxDateTime, false, true);
            Assert.AreEqual(res, maxDTWithWords);
        }

        [TestMethod]
        public void MediumValueWithWords()
        {
            var res = ConvertDateTimeToString.Do(medDateTime, false, true);
            Assert.AreEqual(res, medDTWithWords);
        }
        #endregion

        #region withTime = true, withWords = false
        [TestMethod]
        public void MinimumValueWithTime()
        {
            var res = ConvertDateTimeToString.Do(minDateTime, true, false);
            Assert.AreEqual(res, minDTWithTime);
        }

        [TestMethod]
        public void MaximumValueWithTime()
        {
            var res = ConvertDateTimeToString.Do(maxDateTime, true, false);
            Assert.AreEqual(res, maxDTWithTime);
        }

        [TestMethod]
        public void MediumValueWithTime()
        {
            var res = ConvertDateTimeToString.Do(medDateTime, true, false);
            Assert.AreEqual(res, medDTWithTime);
        }
        #endregion

        #region withTime = true, withWords = true
        [TestMethod]
        public void MinimumValueWithAll()
        {
            var res = ConvertDateTimeToString.Do(minDateTime, true, true);
            Assert.AreEqual(res, minDTWithAll);
        }

        [TestMethod]
        public void MaximumValueWithAll()
        {
            var res = ConvertDateTimeToString.Do(maxDateTime, true, true);
            Assert.AreEqual(res, maxDTWithAll);
        }

        [TestMethod]
        public void MediumValueWithAll()
        {
            var res = ConvertDateTimeToString.Do(medDateTime, true, true);
            Assert.AreEqual(res, medDTWithAll);
        }
        #endregion
    }
}
