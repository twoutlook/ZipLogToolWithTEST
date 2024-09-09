using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ZipLogTool
{
    public class ZipLogTestCase
    {
        private CmdOutput cmdOutput;
        private int fileSizeInKB = 40; // 在我電腦5 sec
        public ZipLogTestCase(int verbosityLevel)
        {
            // Initialize cmdOutput with the verbosity level
            cmdOutput = new CmdOutput(verbosityLevel);
        }
        public ZipLogTestCase(int verbosityLevel, int _fileSizeInKB)
        {
            // Initialize cmdOutput with the verbosity level
            cmdOutput = new CmdOutput(verbosityLevel);
            fileSizeInKB = _fileSizeInKB;
        }
        private const int numberOfDays = 81; // Number of days to create folders or files for
        public string TESTCASE_DIR001 = "D:\\LAB\\TESTCASE001";
        public string testCaseDir002 = "D:\\LAB\\TESTCASE002";
        //private string testCaseDir001 = "LAB\\TESTCASE001";
        //private string testCaseDir002 = "LAB\\TESTCASE002";


     
        static void DeleteDirectory(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, true); // 'true' to delete the directory and its contents
                //Console.WriteLine($"Deleted directory: {dirPath}");
            }
            else
            {
                //Console.WriteLine($"Directory does not exist: {dirPath}");
            }
        }
        public void DeleteTestCaseDirs()
        {
            try
            {
                DeleteDirectory(TESTCASE_DIR001);
                DeleteDirectory(testCaseDir002);
                DeleteDirectory(TESTCASE_DIR001 + "_ZIP");
                DeleteDirectory(testCaseDir002 + "_ZIP");

                Console.WriteLine("Reset successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        // Method for TESTCASE001: Folder creation basis with log files every 2 hours
        public void InitTestCase001()
        {
            // 如果 TESTCASE001 目錄已經存在，則不執行任何操作
            if (Directory.Exists(TESTCASE_DIR001))
            {
                cmdOutput.WriteLine(99,$"Directory '{TESTCASE_DIR001}' already exists. Skipping initialization.");
                return;
            }

            // Create the TESTCASE001 directory
            cmdOutput.WriteLine(99,$"Creating directory '{TESTCASE_DIR001}'...");
            Directory.CreateDirectory(TESTCASE_DIR001);

            // Create subfolders from today to numberOfDays ago
            for (int i = 0; i <= numberOfDays; i++)
            {
                DateTime date = DateTime.Now.AddDays(-i);
                string folderName = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                string folderPath = Path.Combine(TESTCASE_DIR001, folderName);
                Directory.CreateDirectory(folderPath);
                cmdOutput.WriteLine(1,$"Created folder: {folderPath}");

                // Create log files every 2 hours
                for (int hour = 0; hour < 24; hour += 2)
                {
                    string fileName = $"{folderName}-{hour.ToString("D2")}00.log";
                    CreateLogFile(folderPath, fileName);
                }
            }

            long sizeDir = GetDirectorySize(TESTCASE_DIR001);

            // NOTE by Mark, 09/09, 先同 ver: 0.17.0 不顯示
            //Console.WriteLine($"  Size: {sizeDir / 1024.0 / 1024.0 :F1} MB ({sizeDir:N0} bytes)");

        }

        long GetDirectorySize(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return 0;
            }

            // Get the size of all files in the directory, including subdirectories
            return Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories)
                            .Sum(file => new FileInfo(file).Length);
        }
        // Method for TESTCASE002: File creation basis with two sample log files per day
        public void InitTestCase002()
        {
            // 如果 TESTCASE002 目錄已經存在，則不執行任何操作
            if (Directory.Exists(testCaseDir002))
            {
                cmdOutput.WriteLine(99,$"Directory '{testCaseDir002}' already exists. Skipping initialization.");
                return;
            }

            // Create the TESTCASE002 directory
            cmdOutput.WriteLine(99,$"Creating directory '{testCaseDir002}'...");
            Directory.CreateDirectory(testCaseDir002);

            // Create files for each day from today to numberOfDays ago
            for (int i = 0; i <= numberOfDays; i++)
            {
                DateTime date = DateTime.Now.AddDays(-i);
                string folderName = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                string file1Name = $"{folderName}_abc.log";
                string file2Name = $"{folderName}_xyz.log";
                CreateLogFile(testCaseDir002, file1Name);
                CreateLogFile(testCaseDir002, file2Name);
            }
            long sizeDir = GetDirectorySize(testCaseDir002);

            // NOTE by Mark, 09/09, 先同 ver: 0.17.0 不顯示
            //Console.WriteLine($"  Size: {sizeDir / 1024.0 / 1024.0 :F1} MB ({sizeDir:N0} bytes)");
        }

        private void CreateLogFile(string folderPath, string fileName)
        {
            string filePath = Path.Combine(folderPath, fileName);
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                byte[] data = GenerateContent(fileSizeInKB);
                fs.Write(data, 0, data.Length);
                cmdOutput.WriteLine(1,$"Created file: {filePath}");
            }
        }

        private byte[] GenerateContent(int sizeInKB)
        {
            // Generate a string of random text to fill up the desired size
            StringBuilder content = new StringBuilder();
            Random random = new Random();
            while (Encoding.UTF8.GetByteCount(content.ToString()) < sizeInKB * 1024)
            {
                content.Append("This is some test content for the log file. ");
                content.Append("Random number: ").Append(random.Next(0, 1000)).Append(". ");
            }

            return Encoding.UTF8.GetBytes(content.ToString());
        }
    }
}
