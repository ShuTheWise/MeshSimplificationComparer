using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshSimplificationComparer
{
    public class Cli
    {
        public static bool verbose;

        public static void Run(string args, string filename)
        {
            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.FileName = filename;
            //startInfo.Arguments = args;
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //startInfo.RedirectStandardOutput = verbose;
            //startInfo.CreateNoWindow = !verbose;
            //startInfo.UseShellExecute = false;
            //startInfo.RedirectStandardInput = true;
            //var cmd = Process.Start(startInfo);
            //cmd.WaitForExit();

            //if (verbose)
            //    Logger.WriteLine(cmd.StandardOutput.ReadToEnd());
            RunWithOutput2(args, filename);
        }

        public static void RunWithOutput2(string args, string filename)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filename;
            startInfo.Arguments = args;
            if (!verbose)
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            // Define variables to track the peak
            // memory usage of the process.
            long peakPagedMem = 0,
                 peakWorkingSet = 0,
                 peakVirtualMem = 0;

            using (Process p = Process.Start(startInfo))
            {
                // Display the process statistics until
                // the user closes the program.
                do
                {
                    if (!p.HasExited)
                    {
                        // Refresh the current process property values.
                        p.Refresh();

                        //Console.WriteLine();

                        //// Display current process statistics.

                        //Console.WriteLine($"{p} -");
                        //Console.WriteLine("-------------------------------------");

                        //Console.WriteLine($"  Physical memory usage     : {p.WorkingSet64}");
                        //Console.WriteLine($"  Base priority             : {p.BasePriority}");
                        //Console.WriteLine($"  Priority class            : {p.PriorityClass}");
                        //Console.WriteLine($"  User processor time       : {p.UserProcessorTime}");
                        //Console.WriteLine($"  Privileged processor time : {p.PrivilegedProcessorTime}");
                        //Console.WriteLine($"  Total processor time      : {p.TotalProcessorTime}");
                        //Console.WriteLine($"  Paged system memory size  : {p.PagedSystemMemorySize64}");
                        //Console.WriteLine($"  Paged memory size         : {p.PagedMemorySize64}");

                        // Update the values for the overall peak memory statistics.
                        peakPagedMem = p.PeakPagedMemorySize64;
                        peakVirtualMem = p.PeakVirtualMemorySize64;
                        peakWorkingSet = p.PeakWorkingSet64;

                        //if (p.Responding)
                        //{
                        //    Console.WriteLine("Status = Running");
                        //}
                        //else
                        //{
                        //    Console.WriteLine("Status = Not Responding");
                        //}
                    }
                }
                while (!p.WaitForExit(int.MaxValue));

                //Logger.WriteLine("");
                //Logger.WriteLine($"  Process exit code          : {p.ExitCode}");

                //float n = 1024 * 1024;
                //// Display peak memory statistics for the process.
                //Logger.WriteLine($"  Peak physical memory usage : {peakWorkingSet / n} mb");
                //Logger.WriteLine($"  Peak paged memory usage    : {peakPagedMem / n} mb");
                //Logger.WriteLine($"  Peak virtual memory usage  : {peakVirtualMem / n} mb");
            }
        }

        public static string RunWithOutput(string args, string filename)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filename;
            startInfo.Arguments = args;

            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = false;

            Process p = new Process();
            p.StartInfo = startInfo;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            return output;
        }

        //public static void RunWithoutOutput(string args, string filename, string _batDir)
        //{
        //    Process proc = null;
        //    proc = new Process();
        //    proc.StartInfo.WorkingDirectory = _batDir;
        //    proc.StartInfo.FileName = "cmd.exe";
        //    proc.StartInfo.Arguments = args;
        //    proc.StartInfo.CreateNoWindow = false;
        //    proc.StartInfo.RedirectStandardError = true;
        //    proc.StartInfo.UseShellExecute = false;

        //    proc.Start();
        //    proc.WaitForExit();
        //    Console.WriteLine(proc.ExitCode);
        //    Console.WriteLine(proc.StandardError);
        //    proc.Close();
        //    // MessageBox.Show("Bat file executed...");
        //}
    }
}
