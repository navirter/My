using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Waiting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Testing
{
    [TestClass()]
    public class WaitTesting
    {
        [TestMethod()]
        public void Decimal_DoTesting()
        {
            try
            {                
                Wait.Do(1.2);
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}