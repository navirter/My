using My.Extensions.StringExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace My.Extensions.Enumerations.StringExtensions
{
    public static class Removing
    {

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
    }
}
