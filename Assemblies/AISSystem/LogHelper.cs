using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AISSystem
{
    public class LogerHelper
    {
        static log4net.ILog loginfo;   //选择<logger name="loginfo">的配置 

        static log4net.ILog logerror;   //选择<logger name="logerror">的配置 

        public static void SetConfig()
        {
            log4net.Config.XmlConfigurator.Configure();
            loginfo = log4net.LogManager.GetLogger("loginfo");
            logerror = log4net.LogManager.GetLogger("logerror");
        }

        public static void SetConfig(FileInfo configFile)
        {
            log4net.Config.XmlConfigurator.Configure(configFile);
            loginfo = log4net.LogManager.GetLogger("loginfo");
            logerror = log4net.LogManager.GetLogger("logerror");
        }

        public static void WriteLog(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
        }

        public static void WriteLog(string info, Exception se)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(info, se);
            }
        }

        public static void WriteErrorLog(string error)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(error );
            }
        }

        public static void Log(string content, string fileName, bool isAppend = true, string codeType = "utf-8")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "output.log";
            }
            WriteLog(content, fileName, isAppend, Encoding.GetEncoding(codeType));
        }


        public static void Log(string content)
        {
            var fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            WriteLog(content, fileName, true, Encoding.UTF8);
        }

        private static void WriteLog(string content, string fileName, bool isAppend, Encoding code)
        {
            var wr = new StreamWriter(fileName, isAppend, code);
            wr.WriteLine(DateTime.Now.ToString("o") + "   " + content);
            wr.Close();
        }
    }
}
