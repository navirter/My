using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace My.Extensions.StringExtensions
{
    public static class GetFirst
    {
        public static int GetFirstDigits(this string str)
        {
            if (str == null)
                throw new NullReferenceException("how can a string be null?");
            var digits = str.TakeWhile(q => Char.IsDigit(q)).ToList();
            string concat = String.Concat(digits);
            int res = int.Parse(concat);
            return res;
        }
    }
}
