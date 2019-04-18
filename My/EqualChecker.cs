using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    public static class EqualChecker
    {
        /// <summary>
        /// Return true if classes and  all fields match. Except for IEnumerables
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool NonIEnumerableValuesEquals(object a, object b)
        {
            if (a == null || b == null)
                return false;
            var aType = a.GetType();
            var bType = b.GetType();
            if (aType != bType)
                return false;
            var aFields = aType.GetFields();
            var bFields = bType.GetFields();
            if (aFields.Length != bFields.Length)
                throw new Exception("Types match, but fields amount vary.");
            for (int i = 0; i < aFields.Length; i++)
            {
                if (aFields[i].Name != bFields[i].Name)
                    throw new Exception("Objects don't match in fields names");
                var isEnum = aFields[i].GetType()
                    .GetInterfaces()
                    .Any(q => q.Name.Contains("IEnumerable"));
                if (isEnum)
                    continue;
                var aValue = aFields[i].GetValue(a);
                var bValue = bFields[i].GetValue(b);
                if (!aValue.Equals(bValue))
                    return false;
            }
            return true;
        }
    }
}
