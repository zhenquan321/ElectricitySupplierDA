using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSBot2.Utility
{
    public class BotMngBase
    {
        public delegate void UpdateStatus();

        public event UpdateStatus SetBusy;
        public event UpdateStatus SetReady;
    }
}
