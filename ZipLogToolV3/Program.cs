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

    }

}
