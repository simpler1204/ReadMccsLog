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
                //Process[] processes = Process.GetProcessesByName("ReadMccsLog");

                //int count = processes.Count();

                //if (count < 1)
                //{
                //    Process.Start("ReadMccsLog.exe");
                //}


                Process[] processes = Process.GetProcessesByName("ReadMccsLog");
                int count = processes.Count();

                if (count < 1)
                {
                    ProcessStartInfo psi = new ProcessStartInfo("ReadMccsLog.exe")
                    {
                        CreateNoWindow = false,
                        WindowStyle = ProcessWindowStyle.Normal,
                        UseShellExecute = true,
                        RedirectStandardOutput = false
                    };

                    Process.Start(psi);
                }              

                Thread.Sleep(1000*10);
            }
        }
    }
}
