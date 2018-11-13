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
        public static void copyFields<T>(T source, T destination, bool onlyPublic = false)
        {
            if (source.GetType() != destination.GetType())
                throw new InvalidCastException("Types of objects are not the same!");
            if (!onlyPublic)
            {
                var sourceProperties = source.GetType().GetProperties();
                var destinationProperties = destination.GetType().GetProperties();
                foreach (var sp in sourceProperties)
                    foreach (var dp in destinationProperties)
                        if (sp.Name == dp.Name && sp.PropertyType == dp.PropertyType)
                        {
                            dp.SetValue(destination, sp.GetValue(source));
                            break;
                        }
            }
            else
            {
                var sourceProperties = source.GetType().GetProperties(BindingFlags.Public);
                var destinationProperties = destination.GetType().GetProperties(BindingFlags.Public);
                foreach (var sp in sourceProperties)
                    foreach (var dp in destinationProperties)
                        if (sp.Name == dp.Name && sp.PropertyType == dp.PropertyType)
                        {
                            dp.SetValue(destination, sp.GetValue(source));
                            break;
                        }
            }
        }        
    }
}
