using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AISSystem
{
    public interface IQuery
    {
        object Query(params object[] args);
    }

    public interface ILog
    {
        void Info(string msg);
        void Error(string msg);
    }

    public interface IBotMng
    {
        void Start();
    }
}
