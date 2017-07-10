using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace IW2S.Helpers
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
                logerror.Error(error);
            }
        }
    }
}