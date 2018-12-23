using System.Diagnostics;
using System.Management;
using System.Text;

namespace PoroQueue
{
    public static class ProcessExtensions
    {
        public static string GetCommandLine(this Process Instance)
        {
            var CommandLineString = new StringBuilder(Instance.MainModule.FileName);

            CommandLineString.Append(" ");
            using (var Searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + Instance.Id))
            {
                foreach (var Object in Searcher.Get())
                {
                    CommandLineString.Append(Object["CommandLine"]);
                    CommandLineString.Append(" ");
                }
            }

            return CommandLineString.ToString();
        }
    }
}