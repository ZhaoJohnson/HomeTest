using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeTest
{
    public class LogUtility
    {
        /// <summary>
        /// Log the stirng to application runing folder.
        /// </summary>
        /// <param name="logFileName"></param>
        /// <param name="log"></param>
        public static void LogLine(string logFileName, string log)
        {
            try
            {
                string d = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
                if (!System.IO.Directory.Exists(d))
                {
                    System.IO.Directory.CreateDirectory(d);
                }
                string path = System.IO.Path.Combine(d, logFileName);
                using (StreamWriter file = new StreamWriter(path, true, Encoding.UTF8))
                {
                    file.WriteLine(log);
                }
            }
            catch { }
        }
    }
}
