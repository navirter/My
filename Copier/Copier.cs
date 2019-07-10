using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace My.Copier
{

    public static class Copier
    {
        public static T CopyUsingSerializer<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                var res = (T)formatter.Deserialize(ms);
                return res;
            }
        }

        public static void CopyUsingSerializer(object from, object to, bool ignoreEnumerations = true)
        {            
            var typeFrom = from.GetType();
            var typeTo = to.GetType();
            if (isEnumRelated(typeFrom) || isEnumRelated(typeTo))
                if (!ignoreEnumerations)
                    throw new NotSupportedException("Cannot copy any kind of Enumeration! Try use this function for each element in enumeration!");

            var fieldsFrom = typeFrom.GetFields();
            var fieldsTo = typeTo.GetFields();
            var similar = fieldsTo.Where(s => fieldsFrom.Any(a => a.Name == s.Name && a.FieldType == s.FieldType)).ToList();
            foreach (var v in similar)
            {
                if (isEnumRelated(v.GetType()))
                {
                    if (ignoreEnumerations)
                        continue;
                    else
                        throw new NotSupportedException("Cannot copy any kind of Enumeration! Try use this function for each element in enumeration!");
                }
                var value = v.GetValue(from);
                using (var ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, value);
                    ms.Position = 0;

                    var res = formatter.Deserialize(ms);                    
                    v.SetValue(to, res);
                }
            }
        }
        static bool isEnumRelated(Type t)
        {
            if (t.IsArray || t.IsEnum || t.IsGenericType)
                return true;
            return false;
        }
    }
}
