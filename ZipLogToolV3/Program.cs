using System;
using System.IO;
using System.IO.Compression;
using IniParser;
using IniParser.Model;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace ZipLogTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string ZipLogToolVer = "0.9.3"; // Incremented version

            var zipLogCore = new ZipLogCore();
            var zipLogUtil = new ZipLogUtil();

            // Check if --help or -h is passed in the arguments
            if (args.Length > 0 && (args[0].Equals("--help", StringComparison.OrdinalIgnoreCase) || args[0].Equals("-h", StringComparison.OrdinalIgnoreCase)))
            {
                DisplayHelp();
                return;  // Exit after displaying help
            }

            if (args.Length > 0 && args[0].Equals("init", StringComparison.OrdinalIgnoreCase))
            {
                // Initialize the ZipLogTestCase and run InitTestCase001 and InitTestCase002 methods
                var testCase = new ZipLogTestCase();
                testCase.InitTestCase001();  // Create folders in TESTCASE001
                testCase.InitTestCase002();  // Create files in TESTCASE002
            }
            else if (args.Length > 0 && args[0].Equals("unzip", StringComparison.OrdinalIgnoreCase))
            {
                // Load and process the INI file for unzipping
                var data = zipLogCore.ParseIniFile();
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string logFileName = $"{currentDate}_ZipLogTool_Unzip.log";
                string logFilePath = Path.Combine("logs", logFileName);

                zipLogUtil.UnzipFiles(ZipLogToolVer, data, logFilePath);
            }
            else
            {
                // Run Rule 003
                zipLogCore.RunRule(ZipLogToolVer, "by parameters NMQ", zipLogCore.Rule003ProcessPaths);
            }
        }

        // Method to display help message
        static void DisplayHelp()
        {
            Console.WriteLine("Usage: ZipLogTool [options]");
            Console.WriteLine("Usage: ZipLogTool [path-to-application]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -h|--help         Display help.");
            Console.WriteLine("  init              Initialize test cases (creates folders and files).");
            Console.WriteLine("  unzip             Unzip the files as per configuration in the INI file.");
            Console.WriteLine();
            Console.WriteLine("path-to-application:");
            Console.WriteLine("  The path to an application .dll file to execute.");
            Console.WriteLine();
            //Console.WriteLine($"ZipLogTool Version: {ZipLogToolVer}");
        }
    }
}
