using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Runtime.Serialization.Json;

namespace AISSystem
{
    public class DcsHelper
    {
        private static bool isInitialized = false;
        private static XmlWriterSettings writerSettings = new XmlWriterSettings();

        static DcsHelper()
        {
            if (!isInitialized)
            {
                writerSettings.OmitXmlDeclaration = true;
                writerSettings.Indent = true;
                writerSettings.IndentChars = "\t";
                writerSettings.CheckCharacters = false;
            }
        }

        #region GetXml<T>
        public static string GetXml<T>(T instance)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));

            StringBuilder sb = new StringBuilder(Environment.NewLine);

            using (XmlWriter writer = XmlWriter.Create(sb, writerSettings))
                dcs.WriteObject(writer, instance);

            return sb.ToString();
        }
        #endregion

        #region GetJson<T>
        public static string GetJson<T>(T instance)
        {
            string json = string.Empty;
            DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(T));
            
            using (MemoryStream stream = new MemoryStream())
            using (XmlWriter writer = JsonReaderWriterFactory.CreateJsonWriter(stream))
            {
                dcs.WriteObject(writer, instance);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream))
                    json = reader.ReadToEnd();
            }

            return json;
        }

        public static T GetObjFromJson<T>(Stream stream)
        { 
            DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(T));
            object o= dcs.ReadObject(stream);
            if (o == null)
                return default(T);
            return (T)o; 
        }

        public static List<Dictionary<string, string>> GetNameValuesFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;           
            string[] entities = json.Split(new char[] { '}' });
            if (entities == null || entities.Length == 0)
                return null;
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            foreach (var entity in entities)
            {
                if (string.IsNullOrEmpty(entity))
                    continue;
                string[] fields = entity.Split(new char[] { ',' });
                if (fields == null || fields.Length == 0)
                    continue;
                Dictionary<string,string> hus = new Dictionary<string,string>();
                foreach (var f in fields)
                {
                    string fn = f.SubstringBefore(":").GetTrimed();
                    string fv = f.SubstringAfter(":").GetTrimed();
                    if (string.IsNullOrEmpty(fn) || string.IsNullOrEmpty(fv))
                        continue;
                    fn = fn.ReplaceWith("\"", "").ReplaceWith("{","").ReplaceWith("[","");
                    fv = fv.ReplaceWith("\"", "").ReplaceWith("{", "").ReplaceWith("[", "");
                    if (!hus.ContainsKey(fn))
                        hus.Add(fn, fv);
                    else
                        hus[fn] = fv;
                }
                if (hus.Count > 0)
                    result.Add(hus);
            }
            return result;
        }
        #endregion


        #region GetObject<T>
        public static T GetObject<T>(string xml)
        {
            T instance;
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));

            using (StringReader sr = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(sr))
            {
                instance = (T)dcs.ReadObject(reader);
            }

            return instance;
        }

        public static T GetObject<T>(Stream xml)
        {
            T instance;
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));

            using (StreamReader sr = new StreamReader(xml))
            using (XmlReader reader = XmlReader.Create(sr))
            {
                instance = (T)dcs.ReadObject(reader);
            }

            return instance;
        }

        public static object GetObject(string objectType, Stream xml)
        {
            object instance;
            DataContractSerializer dcs = new DataContractSerializer(Type.GetType(objectType));

            using (StreamReader sr = new StreamReader(xml))
            using (XmlReader reader = XmlReader.Create(sr))
            {
                instance = dcs.ReadObject(reader);
            }

            return instance;
        }

        #endregion

        #region GetObject
        public static object GetObject(string xml, Type type)
        {
            object instance;
            DataContractSerializer dcs = new DataContractSerializer(type);

            using (StringReader sr = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(sr))
            {
                instance = dcs.ReadObject(reader);
            }

            return instance;
        }
        #endregion

        #region Byte
         public static MemoryStream GetStream<T>(T entity)
        {
            MemoryStream stream = null;
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            stream = new MemoryStream();
            dcs.WriteObject(stream, entity);
            stream.Seek(0, 0);
            return stream;
        }

        public static byte[] GetBytes<T>(T entity)
        {
            MemoryStream stream = GetStream<T>(entity);
            return stream == null ? null : stream.ToArray();
        }

        public static T GetObject<T>(byte[] bytes) where T : class
        {
            MemoryStream stream = new MemoryStream(bytes);
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            object o = dcs.ReadObject(stream);
            return o as T;
        } 
        #endregion 

        #region ToPublicDateTimeFormat
        public static string ToPublicDateTimeFormat(DateTime date)
        {
            string result = null;
            if (date != DateTime.MinValue)
            {
                result = date.ToString(PublicDateTimeFormat, CultureInfo.InvariantCulture);
            }
            return result;
        }

        public static string PublicDateTimeFormat
        {
            get { return "yyyy-MM-ddTHH:mm:ss.fffzzz"; }
        }
        #endregion
    }
}
