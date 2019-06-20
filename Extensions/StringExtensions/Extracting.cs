using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace My.Extensions.StringExtensions
{
    public static class Extracting
    {
        public static int ExtractDigits(this string source)
        {
            string str = new string(source.Where(s => Char.IsDigit(s)).ToArray());
            return int.Parse(str);
        }
        
        /// <summary>
        /// Russian, English symbols and digits as long as space and punctuation remain only 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>             
        public static string ExtractRussianEnglishDigitsSpaceAndPunctuation(this string source)
        {
            string res = "";
            char[] chars = source.ToCharArray();
            string lowers = source.ToLower();
            for (int i = 0; i < lowers.Length; i++)
            {
                char lower = lowers[i];
                if (Abstract.RussianLowerCase.Contains(lower) || Abstract.EnglishLowerCase.Contains(lower) || Char.IsDigit(lower)
                    || lower == ' ' || Char.IsPunctuation(lower))
                    res += chars[i];
            }
            return res;
        }
    }
}
