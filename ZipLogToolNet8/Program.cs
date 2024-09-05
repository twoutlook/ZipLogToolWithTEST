using IniParser;
using System;
using System.Diagnostics;
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
            string ZipLogToolVer = "0.17.0"; // Bowman checked TODO 1234

            // 取得目前的進程
            Process currentProcess = Process.GetCurrentProcess();

            // 根據目前執行檔案名稱取得同名的所有進程
            Process[] runningProcesses = Process.GetProcessesByName(currentProcess.ProcessName);

            // 檢查是否有其他相同名稱的進程正在執行 (排除當前的進程)
            if (runningProcesses.Length > 1)
            {
                Console.WriteLine("已有另一個程序正在執行，本次執行將被終止。");
                return; // 結束應用程式
            }

            Console.WriteLine($".NET8[ver{ZipLogToolVer}] 程序開始執行...");



            var zipLogCore = new ZipLogCore(2);
            var zipLogUtil = new ZipLogUtil(2);

            if (args.Length == 0)
            {
                //zipLogCore.RunRule(ZipLogToolVer, "by parameters NMQ", zipLogCore.Rule003ProcessPaths);
                var log=zipLogCore.R3Action();
              
                foreach (var x in log)
                {
                    var info = x.Split("|");
                    if (info.Length >= 2 && info[1].Contains( "r3"))
                    {
                        Console.WriteLine(info[1] + "|" + info[2]);
                    }

                }
                Console.WriteLine($".NET8[ver{ZipLogToolVer}] 程序執行完成");
            }
            else if (args[0].Equals("-h", StringComparison.OrdinalIgnoreCase) || args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
            {
                DisplayHelp();
            }
            else if (args[0].Equals("reset", StringComparison.OrdinalIgnoreCase))
            {
                var testCase = new ZipLogTestCase(2);
                testCase.DeleteTestCaseDirs();
                Console.WriteLine($".NET8[ver{ZipLogToolVer}] 程序執行完成");
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
                Console.WriteLine($".NET8[ver{ZipLogToolVer}] 程序執行完成");
            }
            else if (args[0].Equals("unzip", StringComparison.OrdinalIgnoreCase))
            {
                var data = zipLogCore.ParseIniFile();
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string logFileName = $"{currentDate}_ZipLogTool_Unzip.log";
                string logFilePath = Path.Combine("logs", logFileName);
                Console.WriteLine($".NET8[ver{ZipLogToolVer}] 程序執行完成");
            }
            else if (args[0].Equals("run", StringComparison.OrdinalIgnoreCase))
            {
              //  zipLogCore.RunRule(ZipLogToolVer, "by parameters NMQ", zipLogCore.Rule003ProcessPaths);
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
