using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Helpers
{
    public class ExceptionHelper
    {
        public static void LogExceptionErr(Exception e)
        {
            while (e != null)
            {
                LogerHelper.WriteErrorLog(string.Format("异常，Message：{0},SatackTrace：{1}", e.Message, e.StackTrace));
                e = e.InnerException;
            }
        }
    }
}