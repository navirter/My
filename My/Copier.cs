using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace My
{

    public static class Copier
    {

        class TypeNameComparer : IEqualityComparer<Type>
        {
            // Products are equal if their names and product numbers are equal.
            public bool Equals(Type x, Type y)
            {
                //Check whether the compared objects reference the same data.
                if (Object.ReferenceEquals(x, y)) return true;

                //Check whether any of the compared objects is null.
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                //Check whether the products' properties are equal.
                return  x.Name == y.Name;
            }

            // If Equals() returns true for a pair of objects 
            // then GetHashCode() must return the same value for these objects.

            public int GetHashCode(Type t)
            {
                //Check whether the object is null
                if (Object.ReferenceEquals(t, null)) return 0;

                //Get hash code for the Name field if it is not null.
                int hashProductName = t.Name == null ? 0 : t.Name.GetHashCode();
                
                return hashProductName;
            }
        }
        /// <summary>
        /// Supports copying fields from a parent class to its inheritants and vice versa. In this case the present in both classes fields are copied only
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="onlyPublic"></param>
        public static void copyFields<T>(T source, T destination, bool onlyPublic = false)
        {
            Type type = source.GetType();
            Type destinationType = destination.GetType();
            bool asssignable = destinationType.IsSubclassOf(type) || type.IsSubclassOf(destinationType);
            if (type != destinationType && !asssignable)
                throw new InvalidCastException("Types of objects are not the same and can't be inherited right!");
            var typeFields = new List<FieldInfo>();
            var destinationTypeFields = new List<FieldInfo>();
            if (onlyPublic)
            {
                typeFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
                destinationTypeFields = destinationType.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
            }
            else
            {
                typeFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
                destinationTypeFields = destinationType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
            }

            foreach (var sourceField in typeFields)
            {
                var match = destinationTypeFields.FirstOrDefault(s => s.Name == sourceField.Name);
                if (match == null)
                    continue;
                var value = match.GetValue(source);
                match.SetValue(destination, value);
            }
        }        
    }
}
