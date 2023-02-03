using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Oracle.ManagedDataAccess.Client;
using System.Threading;
using System.Diagnostics;

namespace ReadMccsLog
{
    class Program
    {
        static private string _mccsFileName = null;
        static private string _lastLog = null;
        static private string _currentServer = string.Empty;
      

        static void Main(string[] args)
        {

            if(StartWatchDog() == true) return; //이미 실행되고 있으면 아웃
            GetMccsFileName();
            Database.CreateDatabase();
                   

            while (true)
            {
                try
                {
                    ReadWriteMccsLogFile();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Thread.Sleep(1000 * 60);
            }
        }

        static private bool StartWatchDog()
        {
            // Process.Start("WatchDog.exe");

            Process[] processes = Process.GetProcessesByName("WatchDog");
            int count = processes.Count();

            if (count < 1)
            {
                ProcessStartInfo psi = new ProcessStartInfo("WatchDog.exe")
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                Process process = Process.Start(psi);

                return false;
            }
            else
            {
                return true;
            }
        }


        static private void GetMccsFileName()
        {
            if (File.Exists(@"path.txt") == false)
            {
                Console.WriteLine("There is no file path.txt");
                Console.Read();
                return;
            }
            else
            {
                string[] lines = File.ReadAllLines(@"path.txt");
                if (File.Exists(lines[0]))
                {
                    _mccsFileName = lines[0];
                }
            }
        }

        static private void ReadWriteMccsLogFile()
        {
            if (File.Exists(_mccsFileName) == false)
            {
                Console.WriteLine("Error, File not exists..");
                return;
            }

            if (_mccsFileName != null)
            {
                List<string> log = new List<string>();
                string[] lines = null;
                try
                {
                    lines = File.ReadAllLines(_mccsFileName);
                }
                catch (Exception ex)
                {
                    throw new IOException("MCCS LOG File Error");
                }

                if (lines.Count() < 1) return;

                foreach (var line in lines)
                {
                    if (line.Contains("[PARTIAL] -> [ONLINE]"))
                    {
                        log.Add(line);
                    }
                }

                _lastLog = log.Last();
                if (_lastLog.Contains("[DB-C2-HVAC-P1]"))
                {
                    if (_currentServer != "PRI")
                    {
                        DBUpdate("PRI");
                        _currentServer = "PRI";
                        ShowChange();
                    }
                }
                else if (log.Last().Contains("[DB-C2-HVAC-S1]"))
                {
                    if (_currentServer != "SEC")
                    {
                        DBUpdate("SEC");
                        _currentServer = "SEC";
                        ShowChange();
                    }
                }
            }

        }
        private static void ShowChange()
        {
            string[] split = _lastLog.Split('|');
            Console.WriteLine($"{split[0]} {split.Last()}");
        }

        private static bool DBUpdate(string server)
        {
            if (Database.Open() == false)
            {
                Console.WriteLine("Error, DB connection is fail..");
                return false;
            }
            else
            {
                string query = "UPDATE C2_MCCS_STATUS SET STATUS = :1";

                try
                {
                    using (OracleCommand cmd = new OracleCommand(query, Database.OracleConn))
                    {
                        cmd.Parameters.Add(":1", OracleDbType.NVarchar2).Value = server;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    return false;
                    throw new ApplicationException("Database Error");
                }
                finally
                {
                    Database.Close();
                }
                return true;
            }
        }
    }
}
