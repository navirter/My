using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace My.Extensions.StringExtensions
{
    public static class Removing
    {
        public static string RemoveDigits(this string source)
        {
            return new string(source.ToCharArray().Where(s => !Char.IsDigit(s)).ToArray());
        }

        public static string RemoveLetters(this string source)
        {
            return new string(source.ToCharArray().Where(s => !Char.IsLetter(s)).ToArray());
        }
        
    }
}
