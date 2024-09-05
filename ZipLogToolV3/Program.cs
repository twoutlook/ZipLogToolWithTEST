using System;
using System.IO;
using System.IO.Compression;
using IniParser;
using IniParser.Model;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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

            Console.WriteLine($"[ver{ZipLogToolVer}] 程序開始執行...");





            var zipLogCore = new ZipLogCore(2);
            var zipLogUtil = new ZipLogUtil(2);

            if (args.Length == 0)
            {
                // Display help when no arguments are provided
                //DisplayHelp();

                zipLogCore.RunRule(ZipLogToolVer, "by parameters NMQ", zipLogCore.Rule003ProcessPaths);
                Console.WriteLine($"[ver{ZipLogToolVer}] 程序結束!");
            }
            else if (args.Length ==1 && (args[0].Equals("help", StringComparison.OrdinalIgnoreCase) ))
            {
                // Display help if -h or --help is provided
                DisplayHelp();
            }
            else if (args.Length ==1 && args[0].Equals("init", StringComparison.OrdinalIgnoreCase))
            {
                // Initialize the ZipLogTestCase and run InitTestCase001 and InitTestCase002 methods
                var testCase = new ZipLogTestCase(2);
                testCase.InitTestCase001();  // Create folders in TESTCASE001
                testCase.InitTestCase002();  // Create files in TESTCASE002
                Console.WriteLine($"[ver{ZipLogToolVer}] 程序結束!");
            }
            else if (args.Length ==1 && args[0].Equals("unzip", StringComparison.OrdinalIgnoreCase))
            {
                // Load and process the INI file for unzipping
                var data = zipLogCore.ParseIniFile();
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string logFileName = $"{currentDate}_ZipLogTool_Unzip.log";
                string logFilePath = Path.Combine("logs", logFileName);
                Console.WriteLine($"[ver{ZipLogToolVer}] 程序結束!");
                //zipLogUtil.UnzipFiles(ZipLogToolVer, data, logFilePath);
            }
            //else if (args.Length ==1 && args[0].Equals("run", StringComparison.OrdinalIgnoreCase))
            //{
            //    // Now run the tool with default config.ini
            //    //Console.WriteLine("Running with default config.ini...");
            //    zipLogCore.RunRule(ZipLogToolVer, "by parameters NMQ", zipLogCore.Rule003ProcessPaths);
            //}
            else if (args.Length == 1 && args[0].Equals("reset", StringComparison.OrdinalIgnoreCase))
            {
                var testCase = new ZipLogTestCase(2);
                testCase.DeleteTestCaseDirs();
                Console.WriteLine($"[ver{ZipLogToolVer}] 程序結束!");
            }
            else
            {
                // Run Rule 003 with provided parameters
                Console.WriteLine($"不支援所使用參數 請使用 help 查看 ");
                Console.WriteLine($"[ver{ZipLogToolVer}] 程序結束!");
            }
        }

        static void DisplayHelp()
        {
            Console.WriteLine("Usage: ZipLogTool [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  help    Display help.");
            Console.WriteLine("  reset   Reset TESTCASE 2 folders. ");
            Console.WriteLine("  init    Initialize test cases.");
            //Console.WriteLine("  run               Run main functions.");
            //Console.WriteLine("  unzip             Unzip files as per config.");
            //Console.WriteLine("  spec              Display the requirement.");
            Console.WriteLine();
            //Console.WriteLine("If no options are provided, the tool runs using the default config.ini.");
        }


        static void DisplaySpec()
        {
            Console.WriteLine("Reading default config.ini [Options]");
            Console.WriteLine("For example:");
            Console.WriteLine(" N=3");
            Console.WriteLine(" M=5");
            Console.WriteLine(" Q=2");
            Console.WriteLine("");

            Console.WriteLine("1. Compress logs older than {N} days, meaning the most recent 3 days of logs will not be compressed.");
            Console.WriteLine("2. The content to be compressed must span {M} days. If there are fewer than M days of data (e.g., 5 days), do not compress; start from the earliest date.");
            Console.WriteLine("  2-1. The compressed file name should include the date range {from}_{to}.zip, where both are in the format yyyy-MM-dd, e.g., 2004-09-04.");
            Console.WriteLine("  2-2. If a file name conflict occurs, append a sequence number to the file name {from}_{to}_{x}.zip, where x is 01 to 99.");
            Console.WriteLine("       This could happen if new folders or files are generated after 2-1; this is an unusual situation but the feature is still available.");
            Console.WriteLine("3. Delete zip files older than {Q} months, using Q*30 days as the reference.");
            Console.WriteLine("   This is based on comparing the {to} date. Do not delete files exactly 60 days old (for Q=2), only those older.");



            //Console.WriteLine("讀取預設 config.ini [Options]");
            //Console.WriteLine("例如:");
            //Console.WriteLine(" N=3");
            //Console.WriteLine(" M=5");
            //Console.WriteLine(" Q=2");
            //Console.WriteLine("");



            //Console.WriteLine("1. 要壓縮 {N} 天前的 Log，也就是會看到最基本 3 天的 log 不要壓縮。");
            //Console.WriteLine("2. 壓縮內容的天數需要是 {M} 天的資料。如果不足M (以5個為例) 則不必壓縮，要由最早的起算。");
            //Console.WriteLine("  2-1. 壓縮檔名壓縮日期 {from}_{to}.zip, 兩者格式均為 yyyy-MM-dd, 例如 2004-09-04。");
            //Console.WriteLine("  2-2. 檔名若重複則加上流水號 {from}_{to}_{x}.zip, x: 01 to 99。");
            //Console.WriteLine("       這是在 2-1 之後，再產生出新的資料夾或檔案的情況, 正常操作不應出現, 仍備有此功能。");
            //Console.WriteLine("3. 可以刪除 {Q} 個月 以前的 zip 檔案，以 2*30=60天,為例 ");
            //Console.WriteLine("   是比較 {to} 才刪除, 不包含剛好 60天, 而是在此之前。");
        }

    }
}
