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
    public class WaitSmootlyTesting
    {
        [TestMethod()]
        public void decimal_DoTesting()
        {
            try
            {
                WaitSmoothly.Do(1.2);
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}