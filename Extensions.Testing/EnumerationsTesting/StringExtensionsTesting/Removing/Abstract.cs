using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions.Testing.EnumerationsTesting.StringExtensionsTesting.Removing
{
    public abstract class Abstract 
    {
        public readonly List<string> All = new List<string>() { "123qwe!@# ", "456rty$%^ ", "789uio&*( " };
        public readonly List<string> AllWithoutLetters = new List<string>() { "123!@# ", "456$%^ ", "789&*( " };
        public readonly List<string> AllWithoutDigits = new List<string>() { "qwe!@# ", "rty$%^ ", "uio&*( " };
        public readonly List<string> AllWithoutPunctuation = new List<string>() { "123qwe ", "456rty ", "789uio " };
    }
}
