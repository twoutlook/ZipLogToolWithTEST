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

            if (args.Length == 0)
            {
                // Display help when no arguments are provided
                DisplayHelp();

              
            }
            else if (args.Length > 0 && (args[0].Equals("-h", StringComparison.OrdinalIgnoreCase) || args[0].Equals("--help", StringComparison.OrdinalIgnoreCase)))
            {
                // Display help if -h or --help is provided
                DisplayHelp();
            }
            else if (args.Length > 0 && args[0].Equals("init", StringComparison.OrdinalIgnoreCase))
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
            else if (args.Length > 0 && args[0].Equals("run", StringComparison.OrdinalIgnoreCase))
            {
                // Now run the tool with default config.ini
                Console.WriteLine("Running with default config.ini...");
                zipLogCore.RunRule(ZipLogToolVer, "by parameters NMQ", zipLogCore.Rule003ProcessPaths);
            }
            else
            {
                // Run Rule 003 with provided parameters
              
            }
        }

        static void DisplayHelp()
        {
            Console.WriteLine("Usage: ZipLogTool [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -h|--help         Display help.");
            Console.WriteLine("  run               Run main functions.");
            Console.WriteLine("  init              Initialize test cases.");
            Console.WriteLine("  unzip             Unzip files as per config.");
            Console.WriteLine();
            Console.WriteLine("If no options are provided, the tool runs using the default config.ini.");
        }

        static void RunWithDefaultConfig(ZipLogCore zipLogCore, string version)
        {
            string defaultConfig = "config.ini";
            zipLogCore.RunRule(version, defaultConfig, zipLogCore.Rule003ProcessPaths);
        }
    }
}
