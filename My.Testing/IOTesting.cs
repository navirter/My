using Microsoft.VisualStudio.TestTools.UnitTesting;
using My;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Testing
{
    [TestClass()]
    public class IOTesting
    {
        #region XML

        [TestClass()]
        public class Wrapper1
        {
            public string s = "";

            public override bool Equals(object obj)
            {
                var other = obj as Wrapper1;
                if (other.s != s)
                    return false;
                return true;
            }

        }
        [TestMethod()]
        public void WriteAndRead()
        {
            try
            {
                Wrapper1 wp = new Wrapper1() { s = "wat&^*()^%$#@" };
                Wrapper1 new_wp = IO.WriteToXmlFile(IO.CurrentDirectoryFolder + "\\tests\\WriteToXmlFileTesting.xml", wp);
                Assert.AreEqual(wp.s, new_wp.s);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        [TestMethod]
        public void ReadDollar()
        {
            try
            {
                Wrapper1 wp = new Wrapper1() { s = "&" };
                Wrapper1 res = IO.WriteToXmlFile(IO.CurrentDirectoryFolder + "\\tests\\ReadDollar.xml", wp);
                Assert.AreEqual(wp, res);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestClass()]
        public class Wrapper2
        {
            public int x = 1;
            public string y = "wat",
                z = DateAndTime.convertDateTimeToString(DateTime.Now),
                q = "12123340",
                w = "1Q2w#$%^&*()",
                e = "q1w2e3r4t5y6u7i8o9p0[-]=zaxscdvfbgnhmj,k.l/;'\\!@#$%^&*()_+";
            public DateTime dt = DateTime.Now;

            public Wrapper2()
            {
                dt = DateTime.Now;
            }

            public override bool Equals(object obj)
            {
                try
                {
                    var other = obj as Wrapper2;
                    if (x != other.x)
                        return false;
                    if (y != other.y)
                        return false;
                    if (q != other.q)
                        return false;
                    if (z != other.z)
                        return false;
                    if (w != other.w)
                        return false;
                    if (e != other.e)
                        return false;
                    if (dt != other.dt)
                        return false;
                    return true;
                }
                catch { }
                return false;
            }
        }
        [TestMethod()]
        public void WriteAndReadDifferent()
        {
            try
            {
                Wrapper2 wp = new Wrapper2();
                Wrapper2 new_wp = IO.WriteToXmlFile(IO.CurrentDirectoryFolder + "\\tests\\WriteToXmlFileTesting.xml", wp);
                Assert.IsTrue(wp.Equals(new_wp));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }



        #endregion
    }
}