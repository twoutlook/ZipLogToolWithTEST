using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ZipLogTool
{
    public class ZipLogTestCase
    {
        private const int numberOfDays = 81; // Number of days to create folders or files for
        private string testCaseDir001 = "D:\\LAB\\TESTCASE001";
        private string testCaseDir002 = "D:\\LAB\\TESTCASE002";
        private const int fileSizeInKB = 3; // Desired file size in KB

        // Method for TESTCASE001: Folder creation basis with log files every 2 hours
        public void InitTestCase001()
        {
            // 如果 TESTCASE001 目錄已經存在，則不執行任何操作
            if (Directory.Exists(testCaseDir001))
            {
                Console.WriteLine($"Directory '{testCaseDir001}' already exists. Skipping initialization.");
                return;
            }

            // Create the TESTCASE001 directory
            Console.WriteLine($"Creating directory '{testCaseDir001}'...");
            Directory.CreateDirectory(testCaseDir001);

            // Create subfolders from today to numberOfDays ago
            for (int i = 0; i <= numberOfDays; i++)
            {
                DateTime date = DateTime.Now.AddDays(-i);
                string folderName = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                string folderPath = Path.Combine(testCaseDir001, folderName);
                Directory.CreateDirectory(folderPath);
                Console.WriteLine($"Created folder: {folderPath}");

                // Create log files every 2 hours
                for (int hour = 0; hour < 24; hour += 2)
                {
                    string fileName = $"{folderName}-{hour.ToString("D2")}00.log";
                    CreateLogFile(folderPath, fileName);
                }
            }
        }

        // Method for TESTCASE002: File creation basis with two sample log files per day
        public void InitTestCase002()
        {
            // 如果 TESTCASE002 目錄已經存在，則不執行任何操作
            if (Directory.Exists(testCaseDir002))
            {
                Console.WriteLine($"Directory '{testCaseDir002}' already exists. Skipping initialization.");
                return;
            }

            // Create the TESTCASE002 directory
            Console.WriteLine($"Creating directory '{testCaseDir002}'...");
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
        }

        private void CreateLogFile(string folderPath, string fileName)
        {
            string filePath = Path.Combine(folderPath, fileName);
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                byte[] data = GenerateContent(fileSizeInKB);
                fs.Write(data, 0, data.Length);
                Console.WriteLine($"Created file: {filePath}");
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
