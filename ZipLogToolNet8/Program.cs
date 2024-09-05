using IniParser;
using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using INIParser;
//using INIParser.Model;

namespace ZipLogTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            string ZipLogToolVer = "0.16.0";

            var zipLogCore = new ZipLogCore(2);
            var zipLogUtil = new ZipLogUtil(2);

            if (args.Length == 0)
            {
                //zipLogCore.RunRule(ZipLogToolVer, "by parameters NMQ", zipLogCore.Rule003ProcessPaths);
                var log=zipLogCore.Rule003Action();
              
                foreach (var x in log)
                {
                    var info = x.Split("|");
                    if (info.Length >= 2 && info[1] != "1")
                    {
                        Console.WriteLine(x);
                    }

                }
            }
            else if (args[0].Equals("-h", StringComparison.OrdinalIgnoreCase) || args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
            {
                DisplayHelp();
            }
            else if (args[0].Equals("reset", StringComparison.OrdinalIgnoreCase))
            {
                var testCase = new ZipLogTestCase(2);
                testCase.DeleteTestCaseDirs();
            }
            else if (args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                DisplayHelp();
            }

            else if (args[0].Equals("init", StringComparison.OrdinalIgnoreCase))
            {
                var testCase = new ZipLogTestCase(2);
                testCase.InitTestCase001();
                testCase.InitTestCase002();
            }
            else if (args[0].Equals("unzip", StringComparison.OrdinalIgnoreCase))
            {
                var data = zipLogCore.ParseIniFile();
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string logFileName = $"{currentDate}_ZipLogTool_Unzip.log";
                string logFilePath = Path.Combine("logs", logFileName);
            }
            else if (args[0].Equals("run", StringComparison.OrdinalIgnoreCase))
            {
                zipLogCore.RunRule(ZipLogToolVer, "by parameters NMQ", zipLogCore.Rule003ProcessPaths);
            }
            else if (args[0].Equals("spec", StringComparison.OrdinalIgnoreCase))
            {
                DisplaySpec();
            }
        }

        static void DisplayHelp()
        {
            Console.WriteLine("Usage: ZipLogTool [options]");
            Console.WriteLine("       without [options] is to run");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  help    Display help.");
            Console.WriteLine("  reset   Reset TESTCASE 2 folders. ");
            Console.WriteLine("  init    Initialize TESTCASE 2 folders.");
            Console.WriteLine();
        }

        static void DisplaySpec()
        {
            Console.WriteLine("Reading default config.ini [Options]");
            Console.WriteLine("For example:");
            Console.WriteLine(" N=3");
            Console.WriteLine(" M=5");
            Console.WriteLine(" Q=2");
            Console.WriteLine("");

            Console.WriteLine("1. Compress logs older than {N} days.");
            Console.WriteLine("2. Compress data for {M} days.");
            Console.WriteLine("3. Delete zip files older than {Q} months.");
        }
    }
}
