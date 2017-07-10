using GoogleS.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleS
{
    class Program
    {
        static void Main(string[] args)
        {


            Thread t = new Thread(new ThreadStart(() =>
            {
                BotSearch.Instance.Run();

            }));
            t.Start();

            Console.ReadLine();



        }
    }
}
