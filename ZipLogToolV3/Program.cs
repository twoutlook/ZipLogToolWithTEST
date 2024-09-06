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
          static void Main_XXX_GOOD(string[] args)
        {
            string ZipLogToolVer = "0.18.0"; // Bowman checked TODO 1234
            // Capture the start time
            DateTime startTime = DateTime.Now;

            // Display start time in the desired format
            Console.WriteLine($"ver: {ZipLogToolVer} ");
            Console.WriteLine($"[Start: {startTime:yyyy-MM-dd HH:mm:ss}] 程序開始執行...");

            // Process current running instance
            Process currentProcess = Process.GetCurrentProcess();

            // Get the running processes by name
            Process[] runningProcesses = Process.GetProcessesByName(currentProcess.ProcessName);

            // Check if there are other processes running with the same name (excluding the current one)
            if (runningProcesses.Length > 1)
            {
                Console.WriteLine("已有另一個程序正在執行，本次執行將被終止。");
                return; // Exit the program
            }
     


            if (args.Length == 0)
            {
                // Display system specs
                SystemInfo.DisplaySystemSpecs();
                var zipLogCore = new ZipLogCore(2);
                var zipLogUtil = new ZipLogUtil(2);
                zipLogCore.RunRule(ZipLogToolVer, "by parameters NMQ", zipLogCore.Rule003ProcessPaths);
            }
            else if (args.Length == 1 && args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                DisplayHelp();
            }
            else if (args.Length == 1 && args[0].Equals("init", StringComparison.OrdinalIgnoreCase))
            {
                // Display system specs
                SystemInfo.DisplaySystemSpecs();
                var testCase = new ZipLogTestCase(2, 40);// 40=> 50MB

                testCase.InitTestCase001();  // Create folders in TESTCASE001
                testCase.InitTestCase002();  // Create files in TESTCASE002
            }
            else if (args.Length == 1 && args[0].Equals("reset", StringComparison.OrdinalIgnoreCase))
            {
                var testCase = new ZipLogTestCase(2);
                testCase.DeleteTestCaseDirs();
            }
            else
            {
                Console.WriteLine($"不支援所使用參數 請使用 help 查看 ");
            }

            // Capture the end time
            DateTime endTime = DateTime.Now;

            // Calculate the total duration in seconds
            TimeSpan duration = endTime - startTime;
            //int totalSeconds = (int)duration.TotalSeconds;
            double totalSeconds = duration.TotalSeconds;
            // Display end time and the duration in seconds
            Console.WriteLine($"[End: {endTime:yyyy-MM-dd HH:mm:ss}] 程序結束! 總耗時: {totalSeconds:F3} 秒");
        }
        static void Main(string[] args)
        {
            string ZipLogToolVer = "0.18.0";// init2, init3 show cpu/ram/hd and testcase folder size well
            // Capture the start time
            DateTime startTime = DateTime.Now;

            // Display start time in the desired format
            Console.WriteLine($"ver: {ZipLogToolVer} ");
            Console.WriteLine($"[Start: {startTime:yyyy-MM-dd HH:mm:ss}] 程序開始執行...");

            // Process current running instance
            Process currentProcess = Process.GetCurrentProcess();

            // Get the running processes by name
            Process[] runningProcesses = Process.GetProcessesByName(currentProcess.ProcessName);

            // Check if there are other processes running with the same name (excluding the current one)
            if (runningProcesses.Length > 1)
            {
                Console.WriteLine("已有另一個程序正在執行，本次執行將被終止。");
                return; // Exit the program
            }

            if (args.Length == 0)
            {
                // Display system specs
                SystemInfo.DisplaySystemSpecs();
                var zipLogCore = new ZipLogCore(2);
                var zipLogUtil = new ZipLogUtil(2);
                zipLogCore.RunRule(ZipLogToolVer, "by parameters NMQ", zipLogCore.Rule003ProcessPaths);
            }
            //if (args.Length == 1 && args[0].Equals("info", StringComparison.OrdinalIgnoreCase))
            //{
            //    // Show working environment and paths size
            //    DisplaySystemInfo();
            //}
            else if (args.Length == 1 && args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                DisplayHelp();
            }
            else if (args.Length == 1 && args[0].Equals("init", StringComparison.OrdinalIgnoreCase))
            {
                // 40 => 50MB environment
                SystemInfo.DisplaySystemSpecs();
                var testCase = new ZipLogTestCase(2, 40);
                testCase.InitTestCase001();  // Create folders in TESTCASE001
                testCase.InitTestCase002();  // Create files in TESTCASE002




            }
            else if (args.Length == 1 && args[0].Equals("init2", StringComparison.OrdinalIgnoreCase))
            {
                // 45MB environment
                SystemInfo.DisplaySystemSpecs();
                var testCase = new ZipLogTestCase(2, 80);
                testCase.InitTestCase001();  // Create folders in TESTCASE001
                testCase.InitTestCase002();  // Create files in TESTCASE002
            }
            else if (args.Length == 1 && args[0].Equals("init3", StringComparison.OrdinalIgnoreCase))
            {
                // 50MB environment
                SystemInfo.DisplaySystemSpecs();
                var testCase = new ZipLogTestCase(2, 160);
                testCase.InitTestCase001();  // Create folders in TESTCASE001
                testCase.InitTestCase002();  // Create files in TESTCASE002
            }
            else if (args.Length == 1 && args[0].Equals("reset", StringComparison.OrdinalIgnoreCase))
            {
                var testCase = new ZipLogTestCase(2);
                testCase.DeleteTestCaseDirs();
            }
            //else if (args.Length == 1 && args[0].Equals("unzip", StringComparison.OrdinalIgnoreCase))
            //{
            //    var testCase = new ZipLogTestCase(2, 80);
            //    var zip = new ZipLogUtil(2);
            //    zip.UnzipFiles(testCase.TESTCASE_DIR001);
            //    zip.UnzipFiles(testCase.testCaseDir002);

            //}

            else
            {
                Console.WriteLine($"不支援所使用參數 請使用 help 查看 ");
            }

            // Capture the end time
            DateTime endTime = DateTime.Now;

            // Calculate the total duration in seconds
            TimeSpan duration = endTime - startTime;
            double totalSeconds = duration.TotalSeconds;
            // Display end time and the duration in seconds
            Console.WriteLine($"[End: {endTime:yyyy-MM-dd HH:mm:ss}] 程序結束! 總耗時: {totalSeconds:F3} 秒");
        }
        static void DisplaySystemInfo()
        {
            // Display system specs (CPU, RAM, Disk)
            SystemInfo.DisplaySystemSpecs();

            // Define paths
            string testCaseDir001 = @"D:\LAB\TESTCASE001";
            string testCaseDir002 = @"D:\LAB\TESTCASE002";

            // Calculate sizes of directories
            long sizeDir001 = GetDirectorySize(testCaseDir001);
            long sizeDir002 = GetDirectorySize(testCaseDir002);

            // Display the size of the directories
            Console.WriteLine($"Directory {testCaseDir001} size: {sizeDir001 / 1024 / 1024} MB ({sizeDir001:N0} bytes)");
            Console.WriteLine($"Directory {testCaseDir002} size: {sizeDir002 / 1024 / 1024} MB ({sizeDir002:N0} bytes)");
        }

        static long GetDirectorySize(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return 0;
            }

            // Get the size of all files in the directory, including subdirectories (exact size)
            return Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories)
                            .Sum(file => new FileInfo(file).Length);
        }

        static void DisplayHelp()
        {
            Console.WriteLine("Usage: ZipLogTool [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  help    Display help.");
            //Console.WriteLine("  info    Show working environment and paths size.");  BUG

            Console.WriteLine("  reset   Reset TESTCASE 2 folders. ");
            Console.WriteLine("  init    Initialize test cases  (~44MB total), ref:   6.2 sec");
            Console.WriteLine("  init2   Initialize test cases  (~88MB total), ref:  41.7 sec.");
            Console.WriteLine("  init3   Initialize test cases (~187MB total), ref: 188.4 sec.");

            //Console.WriteLine("  run               Run main functions.");
            //Console.WriteLine("  unzip             Unzip files as per config.");   BUG
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
