using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace WatchDog
{
    
    class Program
    {       

        static void Main(string[] args)
        {
            while (true)
            {
                Process[] processes = Process.GetProcessesByName("ReadMccsLog");

                int count = processes.Count();

                if (count < 1)
                {
                    Process.Start("ReadMccsLog.exe");
                }

                Thread.Sleep(1000*30);
            }
        }
    }
}
