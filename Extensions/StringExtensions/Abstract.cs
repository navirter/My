using System;
using System.Collections.Generic;
using System.Text;

namespace My.Extensions.StringExtensions
{
    public abstract class Abstract
    {
        public static char[] RussianLowerCase => new[] { 'а','б','в','г','д', 'е', 'ё', 'ж', 'з', 'и'
            , 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т'
            , 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь'
            , 'э', 'ю', 'я' };
        public static char[] EnglishLowerCase => new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j'
            , 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't'
            , 'u', 'v', 'w', 'x', 'y', 'z' };
    }
}
