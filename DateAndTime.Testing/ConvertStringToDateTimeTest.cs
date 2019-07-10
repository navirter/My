using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.DateAndTime;

namespace DateAndTime.Testing
{
    [TestClass]
    public class ConvertStringToDateTimeTest: Abstract
    {
        #region withTime = true

        [TestMethod]
        public void MinimumValueWithTime()
        {
            var res = ConvertStringToDateTime.Do(minDTWithTime, true);
            Assert.AreEqual(res, minDateTime);
        }

        [TestMethod]
        public void MaximumValueWithTime()
        {
            var res = ConvertStringToDateTime.Do(maxDTWithTime, true);
            bool equal = res.Date == maxDateTime.Date && res.Hour == maxDateTime.Hour && res.Minute == maxDateTime.Minute && res.Second == maxDateTime.Second;
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void MediumValueWithTime()
        {
            var res = ConvertStringToDateTime.Do(medDTWithTime, true);
            Assert.AreEqual(res, medDateTime);
        }

        #endregion

        #region withTime = false

        [TestMethod]
        public void MinimumValue()
        {
            var res = ConvertStringToDateTime.Do(minDTWithNone, false);
            Assert.AreEqual(res, minDateTime.Date);
        }

        [TestMethod]
        public void MaximumValue()
        {
            var res = ConvertStringToDateTime.Do(maxDTWithNone, false);
            Assert.AreEqual(res, maxDateTime.Date);
        }

        [TestMethod]
        public void MediumValue()
        {
            var res = ConvertStringToDateTime.Do(medDTWithNone, false);
            Assert.AreEqual(res, medDateTimeWithNoTime.Date);
        }

        #endregion
    }
}
