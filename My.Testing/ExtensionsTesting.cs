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
    public class ExtensionsTesting
    {
        #region string

        #region removing

        #region remove digits
        [TestMethod()]
        public void RemoveDigitsTesting()
        {
            try
            {
                string input = "ValeraN1JenyaN2NellyN3";
                string output = input.RemoveDigits();
                string expected = "ValeraNJenyaNNellyN";
                Assert.AreEqual(expected, output);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        [TestMethod()]
        public void RemoveDigitsTesting_long()
        {
            try
            {
                string input = "ValeraN1JenyaN2NellyN3";
                for (int i = 0; i < 100; i++)
                    input += "1r";
                string output = input.RemoveDigits();
                string expected = "ValeraNJenyaNNellyN";
                for (int i = 0; i < 100; i++)
                    expected += "r";
                Assert.AreEqual(expected, output);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        #endregion

        [TestMethod()]
        public void RemovePunctuationTesting()
        {
            try
            {
                string input = "Valera.Jenya,Nelly!";
                string output = input.RemovePunctuation();
                string expected = "ValeraJenyaNelly";
                Assert.AreEqual(expected, output);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod()]
        public void RemoveLettersTesting()
        {
            try
            {
                string input = "ValeraN1JenyaN2NellyN3";
                string output = input.RemoveLetters();
                string expected = "123";
                Assert.AreEqual(expected, output);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        #region FilterForRussianEnglishDigitsSpaceAndPunctuationTesting             

        [TestMethod()]
        public void check_for_rus_eng_jap_numbers_spaces_punctuation()
        {
            try
            {
                string input = "Valera JenyaN2Nelly.,!?:;-()<>$ Валераサック";
                string output = input.FilterForRussianEnglishDigitsSpaceAndPunctuation();
                string expected = "Valera JenyaN2Nelly.,!?:;-() Валера";
                Assert.AreEqual(expected, output);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}