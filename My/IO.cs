﻿using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;

namespace My
{
    public static class IO
    {
        class CharacterPair
        {
            public string invalid = "";
            public string valid = "";
            public CharacterPair() { }
            public CharacterPair(string invalid, string valid)
            {
                this.invalid = invalid;
                this.valid = valid;
            }
        }
        static CharacterPair[] charactersToReplace = {
            new CharacterPair("&", "&amp;"),
            new CharacterPair("<", "&lt;"),
            new CharacterPair(">", "&gt;"), 
            new CharacterPair("＆", "&amp;")
        };
        /// <summary>
        /// Must be called before and reversily after the serialization for each element of an string enumeration.
        /// If they contain '&', '<' or '>'
        /// </summary>
        /// <param name="source"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        public static string ReplaceWeirdCharactersForSerialization(this string source, bool reverse = false)
        {
            string res = source;
            foreach (var v in charactersToReplace)
                if (!reverse)
                    res = res.Replace(v.invalid, v.valid);
                else
                    res = res.Replace(v.valid, v.invalid);
            return res;
        }
        /// <summary>
        /// Writes the given object instance to an XML file creating all necessary folders.
        /// It FAILS at writing a TimeSpan instance. Even when they are a field.    
        /// It returns a read instance of what's been written        
        /// If a DataStructure is saved, all it's strings must NOT contain chars '&', '<', '>'
        /// A string extension method ReplaceWeirdCharactersForSerialization can be called then.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>            /// 
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>        
        public static T WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {//cant filter list's strings
            /*
The ampersand character (&) and the left angle bracket (<) must not appear in their literal form, 
except when used as markup delimiters, or within a comment, a processing instruction, or a CDATA section.
If they are needed elsewhere, they must be escaped using either numeric character references or the strings 
" &amp; " and " &lt; " respectively.
The right angle bracket (>) may be represented using the string " &gt; ", 
and must, for compatibility, be escaped using either " &gt;
" or a character reference when it appears in the string " ]]> " in content,
when that string is not marking the end of a CDATA section.

In the content of elements, character data is any string of characters 
which does not contain the start-delimiter of any markup and does not include the CDATA-section-close delimiter
, " ]]> ". In a CDATA section, character data is any string of characters 
not including the CDATA-section-close delimiter, " ]]> ".

To allow attribute values to contain both single and double quotes,
the apostrophe or single-quote character (') may be represented as " &apos; "
, and the double-quote character (") as " &quot; ".
             */
            TextWriter writer = null;
            try
            {
                string[] parts = filePath.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                string dir = parts[0];
                for (int i = 1; i < parts.Length - 1; i++)//from second to the one before last                
                    dir += "\\" + parts[i];
                Directory.CreateDirectory(dir);
                
                objectToWrite.formateAllStringFieldsForSerialisation();

                string serializedObject = serializeObject(objectToWrite);
                Encoding encoding = Encoding.GetEncoding(1251);//i cannot detect this as i am working with classes, not strings                
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append, encoding);
                serializer.Serialize(writer, objectToWrite);
                
                objectToWrite.formateAllStringFieldsForSerialisation(true);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
            return ReadFromXmlFile<T>(filePath);//this is needed to ensure the file is readable
        }
        
        static void formateAllStringFieldsForSerialisation<T>(this T original, bool reverse = false)
        {           
            Type type = original.GetType();
            FieldInfo[] fields = type.GetFields().Where(s => s.IsPublic).ToArray();
            FieldInfo[] strings = fields.Where(s => s.FieldType.Name == "String").ToArray();
            for (int i = 0; i < strings.Length; i++)
            {
                string value = (string)strings[i].GetValue(original);
                string modified = value;
                foreach (var v in charactersToReplace)
                    if (!reverse)
                        modified = modified.Replace(v.invalid, v.valid);
                    else
                        modified = modified.Replace(v.valid, v.invalid);
                if (modified != value)
                    strings[i].SetValue(original, modified);
            }
        }

        static string serializeObject<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            XmlReader reader = null;
            try
            {
                Encoding encoding = Encoding.GetEncoding(1251);
                string xml_text = File.ReadAllText(filePath, encoding);
                var xmlReaderSettings = new XmlReaderSettings() { CheckCharacters = false };
                reader = XmlReader.Create(new StringReader(xml_text), xmlReaderSettings);
                var serializer = new XmlSerializer(typeof(T));
                var res = (T)serializer.Deserialize(reader);
                res.formateAllStringFieldsForSerialisation(true);
                return res;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public static string CurrentDirectory { get; private set; } = Directory.GetCurrentDirectory();
        public static string ApplicationData { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);        
    }
}
