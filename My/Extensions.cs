using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    public static class Extensions
    {
        static char[] russianLower = new[] { 'а','б','в','г','д', 'е', 'ё', 'ж', 'з', 'и'
            , 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т'
            , 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь'
            , 'э', 'ю', 'я' };
        static char[] englishLower = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j'
            , 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't'
            , 'u', 'v', 'w', 'x', 'y', 'z' };

        #region string

        #region removing

        public static string RemoveDigits(this string source)
        {
            return new string(source.ToCharArray().Where(s => !Char.IsDigit(s)).ToArray());
        }
        public static int ExtractDigits(this string source)
        {
            string str = new string(source.Where(s => Char.IsDigit(s)).ToArray());
            return int.Parse(str);
        }

        public static string RemoveLetters(this string source)
        {
            return new string(source.ToCharArray().Where(s => !Char.IsLetter(s)).ToArray());
        }

        public static string RemovePunctuation(this string source)
        {
            return new string(source.ToCharArray().Where(s => !Char.IsPunctuation(s)).ToArray());
        }

        /// <summary>
        /// Russian, English symbols and digits as long as space and punctuation remain only 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>             
        public static string FilterForRussianEnglishDigitsSpaceAndPunctuation(this string source)
        {
            string res = "";
            char[] chars = source.ToCharArray();
            string lowers = source.ToLower();
            for (int i = 0; i < chars.Length; i++)
            {
                char lower = lowers[i];
                if (russianLower.Contains(lower) || englishLower.Contains(lower) || Char.IsDigit(lower)
                    || lower == ' ' || Char.IsPunctuation(lower))
                    res += chars[i];
            }
            return res;
        }

        #endregion

        #region firsting

        public static int getFirstDigits(this string str)
        {
            if (str == null)
                throw new NullReferenceException("how can a string be null?");
            var digits = str.TakeWhile(q => Char.IsDigit(q)).ToList();
            string concat = String.Concat(digits);
            int res = int.Parse(concat);
            return res;
        }
        #endregion

        #endregion
        

        #region IEnumerable<string>

        #region removing

        #region RemoveDigits

        public static List<string> RemoveDigits(this List<string> source)
        {
            for (int i = 0; i < source.Count; i++)
                source[i] = source[i].RemoveDigits();
            return source;
        }

        public static string[] RemoveDigits(this string[] source)
        {
            var list = source.ToList().RemoveDigits();
            return list.ToArray();
        }

        public static IEnumerable<string> RemoveDigits(this IEnumerable<string> source)
        {
            var list = source.ToList().RemoveDigits();
            return list;
        }

        #endregion

        #region RemoveLetters

        public static List<string> RemoveLetters(this List<string> source)
        {
            for (int i = 0; i < source.Count; i++)
                source[i] = source[i].RemoveLetters();
            return source;
        }

        public static string[] RemoveLetters(this string[] source)
        {
            var list = source.ToList().RemoveLetters();
            return list.ToArray();
        }

        public static IEnumerable<string> RemoveLetters(this IEnumerable<string> source)
        {
            var list = source.ToList().RemoveLetters();
            return list;
        }

        #endregion

        #region RemovePunctuation

        public static List<string> RemovePunctuation(this List<string> source)
        {
            for (int i = 0; i < source.Count; i++)
                source[i] = source[i].RemovePunctuation();
            return source;
        }

        public static string[] RemovePunctuation(this string[] source)
        {
            var list = source.ToList().RemovePunctuation();
            return list.ToArray();
        }

        public static IEnumerable<string> RemovePunctuation(this IEnumerable<string> source)
        {
            var list = source.ToList().RemovePunctuation();
            return list;
        }

        #endregion

        #endregion

        #endregion
    }
}
