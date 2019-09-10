using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshSimplificationComparer
{
    public static class Logger
    {
        private static string content;

        public static void Write(string msg)
        {
            Console.Write(msg);
            Append(msg + "\n");
        }

        public static void WriteLine(string msg)
        {
            Console.WriteLine(msg);
            Append(msg + "\n");
        }

        private static void Append(string msg)
        {
            content += msg;
        }

        public static void WriteToFile(string path)
        {
            File.WriteAllText(path, content);
            content = "";
        }

        public static void Flush()
        {
            content = "";
        }
    }
}
