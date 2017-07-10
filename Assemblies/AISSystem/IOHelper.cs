using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AISSystem
{
    public static class IOHelper
    {
        public static void WriteOutPutFile(StreamWriter outputFile, char separator, string NullString, List<string> contents)
        {
            foreach (string content in contents)
                Write(outputFile, separator, NullString, content);
            outputFile.Flush();
        }

        public static void Write(StreamWriter f, char separator, string NullString, string value)
        {
            if (string.IsNullOrEmpty(value))
                f.Write(NullString);
            else
            {
                value = value.Replace("\t", "");
                f.Write(value);

            }
            f.Write(separator);
        }

        public static StreamWriter CreatStreamWriter(string path, Encoding toEncoding)
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            return new StreamWriter(fs, toEncoding);
        }

        public static void AppendStr(string path, string str)
        {
            using (FileStream fs = new FileStream(path, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(str);
            }
        }
        public static void AppendLineStr(string path, string str)
        {
            using (FileStream fs = new FileStream(path, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine(str);
            }
        }

        public static string GetString(string path)
        {
            string result = null;
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }
        public static byte[] GetBytes(string path)
        {
            byte[] result = null;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                result = new byte[(int)fs.Length];
                fs.Read(result, 0, (int)fs.Length);
            }
            return result;
        }

        public static byte[] ConvertToBytes(string text)
        {
            return System.Text.Encoding.Default.GetBytes(text);
        }

        public static string ConvertToString(byte[] buff)
        {
            return System.Text.Encoding.Default.GetString(buff );
        }


        public static void SaveString(string path, string content)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(content);
            }
        }

        public static void AppenString(string path, string content)
        {
            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine(content);
            }
        }


        public static T GetObject<T>(string path)
        {
            string xml = IOHelper.GetString(path);
            if (!string.IsNullOrEmpty(xml))
                return DcsHelper.GetObject<T>(xml);
            return default(T);
        }

        public static void SaveObject<T>(T o, string path)
        {
            string xml = DcsHelper.GetXml<T>(o);
            SaveString(path, xml);
        }



        public static bool IsFileExist(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        public static string[] GetFiles(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            var fs = di.GetFiles();
            if (fs == null || fs.Length == 0)
                return null;
            return fs.Select(x => x.Name).ToArray();
        }
    }
}
