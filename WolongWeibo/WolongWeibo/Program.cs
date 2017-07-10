using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WolongWeibo
{
    class Program
    {
        static void Main(string[] args)
        {
            var th_1 = new Thread(() =>
            {

                var tm = new TaskManager();
                tm.Start();
            });

            var th_2 = new Thread(() =>
            {
                var tm = new TaskManager();
                tm.CommintTask();
            });

            th_1.Start();
            th_2.Start();



            //var th_3 = new Thread(() =>
            //{

            //    var tm = new IW2SBotMng();
            //    tm.Run();
            //});

            //th_3.Start();



        }
    }
}
