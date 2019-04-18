using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace My
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

        //public static Y DeepCopy<T, Y>(T source, bool onlyPublic = false, bool ignoreAssignibility = false) where Y:new()
        //{
        //    if (source == null)
        //        throw new ArgumentNullException("Object cannot be null");
        //    Type type = source.GetType();
        //    Y res = new Y();
        //    Type destinationType = res.GetType();
        //    if (!ignoreAssignibility)
        //    {
        //        bool asssignable = destinationType.IsSubclassOf(type) || type.IsSubclassOf(destinationType);
        //        if (type != destinationType && !asssignable)
        //            throw new InvalidCastException("Types of objects are not the same and even not inherinants!");
        //    }
        //    var result = (Y)Process(source, destinationType);
        //    return result;
        //}

        //static object Process(object source, Type destinationType, bool onlyPublic = false)
        //{
        //    if (source == null)
        //        return null;
        //    Type type = source.GetType();
        //    if (type.IsValueType || type == typeof(string))
        //    {
        //        return source;
        //    }
        //    else if (type.IsArray)
        //    {
        //        Type elementType = Type.GetType(
        //             type.FullName.Replace("[]", string.Empty));
        //        var array = source as Array;
        //        Array copied = Array.CreateInstance(elementType, array.Length);
        //        for (int i = 0; i < array.Length; i++)
        //        {
        //            var localValue = array.GetValue(i);
        //            Type fieldType = localValue.GetType();
        //            copied.SetValue(Process(localValue, fieldType), i);
        //        }
        //        return Convert.ChangeType(copied, source.GetType());
        //    }
        //    else if (type.GetInterfaces()
        //            .FirstOrDefault(i => i.Name == "IEnumerable") != null)
        //    {                
        //        Type elementType = Type.GetType(
        //             type.FullName.Replace("[]", string.Empty));
        //        dynamic sourceIenum = source;
        //        for (int i = 0; i < sourceIenum.Count; i++)
        //        {
        //            sourceIenum[i] = Process(sourceIenum[i], sourceIenum[i].GetType());
        //        }
        //        return sourceIenum;
        //        //var sourceTypeIenum = sourceIenum.Select(s =>
        //        //{
        //        //    var sType = s.GetType();
        //        //    var res = Convert.ChangeType(s, sType);
        //        //    return res;
        //        //}).AsEnumerable();
        //        //return sourceTypeIenum;
        //        //var sourceArray = sourceIenum.ToArray();
        //        //var newArray = new object[sourceArray.Length];
        //        //for (int i = 0; i < sourceArray.Length; i++)
        //        //    newArray[i] = Process(sourceArray[i], sourceArray[i].GetType());
        //        //var list = newArray.Select(s =>
        //        //{
        //        //    var sType = s.GetType();
        //        //    var res = Convert.ChangeType(s, sType);
        //        //    return res;
        //        //}).ToList();
        //        //var final = Convert.ChangeType(list, destinationType);
        //        //Array copied = Array.CreateInstance(elementType, sourceArray.Length);
        //        //for (int i = 0; i < copied.Length; i++)
        //        //    try
        //        //    {
        //        //        var localValue = newArray[i];
        //        //        Type fieldType = localValue.GetType();
        //        //        copied.SetValue(localValue, i);
        //        //    }
        //        //    catch(Exception e)
        //        //    { }
        //        //return Convert.ChangeType(copied, destinationType);
        //    }
        //    else if (type.IsClass)
        //    {
        //        object destinationObject = Activator.CreateInstance(destinationType);
        //        BindingFlags flags = BindingFlags.Public;
        //        if (onlyPublic)
        //            flags = BindingFlags.Public | BindingFlags.Instance;
        //        else
        //            flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        //        FieldInfo[] fields = type.GetFields(flags);
        //        foreach (FieldInfo field in fields)
        //            try
        //            {
        //                object fieldValue = field.GetValue(source);
        //                if (fieldValue == null)
        //                    continue;
        //                Type fieldType = fieldValue.GetType();
        //                field.SetValue(destinationObject, Process(fieldValue, fieldType));
        //            }
        //            catch { }
        //        return destinationObject;
        //    }
        //    else
        //        throw new ArgumentException("Unknown type: " + type.FullName);
        //}




        //[Obsolete("Use DeepCopy")]
        ///// <summary>
        ///// Supports copying fields with the same name form one object to another
        ///// a parent class to its inheritants and vice versa. In this case the present in both classes fields are copied only
        ///// Both objects must have a parameterless constructor
        ///// </summary>
        ///// <param name="source">Object to get fields' values from</param>
        ///// <param name="destination">Object to set fields' values to if they are present</param>
        ///// <param name="onlyPublic">Set true to copy only public fields</param>    
        ///// <param name="ignoreAssignibility">If true, source and destination can be of any classes.\nOtherwise they must be of the same class or one must inherit from another</param>        
        //public static void CopyFields<T, Y>(T source, ref Y destination, bool onlyPublic = false, bool ignoreAssignibility = false) 
        //    where Y: new()
        //{
        //    destination = CopyUsingSerializer(source);
        //    //destination = DeepCopy<T, Y>(source, onlyPublic, ignoreAssignibility);

        //    //Type type = source.GetType();
        //    //Type destinationType = destination.GetType();

        //    //if (!ignoreAssignibility)
        //    //{
        //    //    bool asssignable = destinationType.IsSubclassOf(type) || type.IsSubclassOf(destinationType);
        //    //    if (type != destinationType && !asssignable)
        //    //        throw new InvalidCastException("Types of objects are not the same and can't be inherited right!");
        //    //}

        //    //var typeFields = new List<FieldInfo>();
        //    //var destinationTypeFields = new List<FieldInfo>();
        //    //if (onlyPublic)
        //    //{
        //    //    typeFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
        //    //    destinationTypeFields = destinationType.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
        //    //}
        //    //else
        //    //{
        //    //    typeFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
        //    //    destinationTypeFields = destinationType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
        //    //}
            

        //    //if (type.IsValueType || type == typeof(string))
        //    //{
        //    //    object thing = source;
        //    //    object thing2 = destination;
        //    //    thing2 = thing;
        //    //    destination = (Y)thing2;
        //    //}
        //    //else if (type.IsArray)
        //    //{
        //    //    Type elementType = Type.GetType(
        //    //         type.FullName.Replace("[]", string.Empty));
        //    //    var array = source as Array;
        //    //    Array copied = Array.CreateInstance(elementType, array.Length);
        //    //    for (int i = 0; i < array.Length; i++)
        //    //    {
        //    //        copied.SetValue(Process(array.GetValue(i)), i);
        //    //    }
        //    //    object thing = Convert.ChangeType(copied, destination.GetType());
        //    //    destination = (Y)thing;
        //    //}
        //    //else if (type.IsClass)
        //    //{
        //    //    object toret = Activator.CreateInstance(destination.GetType());
        //    //    foreach (FieldInfo field in destinationTypeFields)
        //    //        try
        //    //        {
        //    //            object fieldValue = field.GetValue(source);
        //    //            if (fieldValue == null)
        //    //                continue;
        //    //            field.SetValue(toret, Process(fieldValue));
        //    //        }
        //    //        catch (ArgumentException ae)
        //    //        {

        //    //        }
        //    //        catch(Exception e)
        //    //        {
        //    //            throw new Exception(e.Message);
        //    //        }
        //    //    destination = (Y)toret;
        //    //}
        //    //else
        //    //    throw new ArgumentException("Unknown type: " + type.FullName);

        //    //Y Ydest = new Y();
        //    //foreach (var sourceField in typeFields)
        //    //{
        //    //    var match = destinationTypeFields.FirstOrDefault(s => s.Name == sourceField.Name);
        //    //    if (match == null)
        //    //        continue;
        //    //    var value = match.GetValue(source);
        //    //    var valueType = value.GetType();
        //    //    //if value is some kind if IEnumerable
        //    //    if (valueType.GetInterfaces()
        //    //        .FirstOrDefault(i => i.Name == "IEnumerable") != null)
        //    //    {
        //    //        if (value is string)// it is IEnumerable as well
        //    //        {
        //    //            match.SetValue(Ydest, new string(value.ToString().ToCharArray()));//string.ToString() must work!
        //    //        }
        //    //        else
        //    //        {
        //    //            var littleCopy = value;
        //    //            CopyFields(value, ref littleCopy, true);//recursive assignation of objects in IEnumerables
        //    //            match.SetValue(Ydest, littleCopy);
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        //if (valueType.IsValueType)
        //    //        //    Ydest = (Y)value;
        //    //        //else
        //    //            match.SetValue(Ydest, value);
        //    //    }              
        //    //}
        //    //destination = Ydest;
        //}

        ////public static T CopyFields<T>(T source, bool onlyPublic = false)
        //{
        //    List<T> lsource = new List<T>();            
        //    lsource.Add(source);
        //    var ldestination = lsource.Select(s => s).ToList();
        //    return ldestination.First();
        //}        
    }
}
