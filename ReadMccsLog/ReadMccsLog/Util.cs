using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ReadMccsLog
{
    class Util
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
                                                        int size, string filePath);
        private string path = string.Empty;

        public Util(string path)
        {
            this.path = path;
        }

        public void SetIni(string _Section, string _Key, string _Value)
        {
            WritePrivateProfileString(_Section, _Key, _Value, this.path);

        }

        public string GetIni(string _Section, string _Key)
        {
            StringBuilder STBD = new StringBuilder(1000);

            GetPrivateProfileString(_Section, _Key, null, STBD, 5000, this.path);

            return STBD.ToString().Trim();
        }
    }
}
