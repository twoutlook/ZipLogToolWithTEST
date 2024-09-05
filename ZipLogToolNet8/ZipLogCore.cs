//using IniParser.Model;
using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ZipLogTool
{

    public class ZipLogCore
    {
        public string iniFilePath = "config.ini";
        //private string tempIniFilePath = "temp_config.ini";
        public string logPath = "logs";
        public int N = 3;
        public int M = 5;
        //private int P;
        public int Q = 2;
        public bool IS_Q = false;
        public IniData data;
        // Declare the cmdOutput field correctly
        private CmdOutput cmdOutput;

        //private INIParser iNIParser;
        // Constructor to initialize cmdOutput
        public ZipLogCore(int verbosityLevel)
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
                cmdOutput?.WriteLine(1, $"Log directory created at {logPath}");
            }

            // NOTE by Mark, 09/05, 兩個用法有差異
            data = ParseIniFile();
            var pathsSection = data["Paths"];
            //var logSettings = data["LogSettings"];

            var optionsSection = data["Options"];
            //N = int.Parse(optionsSection["N"]);
            //M = int.Parse(optionsSection["M"]);
            //P = int.Parse(optionsSection["P"]);
            //try
            //{
            //    Q = int.Parse(optionsSection["Q"]);
            //    IS_Q = true;
            //}
            //catch (Exception ex)
            //{
            //    IS_Q = false;
            //    //沒設視為0
            //}
            //ConfigureLogSettings(data);

            // Initialize cmdOutput with the verbosity level
            cmdOutput = new CmdOutput(verbosityLevel);
        }
        private void Rule003DeleteOldZipFiles(string baseDir, int qMonths, StreamWriter logWriter)
        {
            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003: Deleting old ZIP files in: {baseDir} [Start]");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003: Deleting old ZIP files in: {baseDir} [Start]");

            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logWriter.WriteLine($"Rule003: Invalid or non-existent directory: {baseDir}. Skipping deletion!");
                return;
            }

            int totalDays = qMonths * 30;
            DateTime deleteBeforeDate = DateTime.Now.AddDays(-totalDays);

            //logWriter.WriteLine($"Rule003: Threshold date for ZIP file deletion is {deleteBeforeDate.ToString("yyyy-MM-dd")} ({totalDays} days ago)");
            cmdOutput.WriteLine(1, $"Rule003: Threshold date for ZIP file deletion is {deleteBeforeDate.ToString("yyyy-MM-dd")} ({totalDays} days ago)");

            var zipFiles = Directory.GetFiles(baseDir, "*.zip", SearchOption.TopDirectoryOnly).ToList();

            if (zipFiles.Count > 0)
            {
                //logWriter.WriteLine("Rule003: Checking the following ZIP files for deletion:");
                cmdOutput.WriteLine(1, "Rule003: Checking the following ZIP files for deletion:");
                foreach (var zipFile in zipFiles)
                {
                    string zipFileName = Path.GetFileName(zipFile);
                    DateTime zipFileDate;

                    // Assuming the ZIP file names are in the format {from}_{to}.zip or {from}_{to}_{x}.zip
                    // Extract the 'to' date from the file name (before any underscore or .zip)
                    string toDateString = zipFileName.Split('_').Last().Split('.')[0];

                    if (DateTime.TryParseExact(toDateString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out zipFileDate))
                    {
                        int fileAgeInDays = (DateTime.Now - zipFileDate).Days;


                        if (zipFileDate < deleteBeforeDate)
                        {
                            File.Delete(zipFile);
                            //logWriter.WriteLine($"  - [ZIP File] {zipFile} deleted (Age: {fileAgeInDays} days)");
                            cmdOutput.WriteLine(1, $"  - [ZIP File] {zipFile} deleted (Age: {fileAgeInDays} days)");
                            logWriter.WriteLine($"  - {zipFile} (Age: {fileAgeInDays} days) deleted");
                        }
                        else
                        {
                            logWriter.WriteLine($"  - {zipFile} (Age: {fileAgeInDays} days)");
                        }
                    }
                    else
                    {
                        logWriter.WriteLine($"  - {zipFile} (Skipped: Could not parse 'to' date)");
                        cmdOutput.WriteLine(1, $"  - {zipFile} (Skipped: Could not parse 'to' date)");
                    }
                }
            }
            else
            {
                logWriter.WriteLine($"Rule003: No old ZIP files found for deletion in ({baseDir}).");
                cmdOutput.WriteLine(1, $"Rule003: No old ZIP files found for deletion in ({baseDir}).");
            }

            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003: Deleting old ZIP files in: {baseDir} [End]\n");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003: Deleting old ZIP files in: {baseDir} [End]\n");
        }

        private void Rule003CompressLogFiles_Plan(string pathKey, string baseDir, StreamWriter logWriter)
        {
            //logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003CompressLogFiles_Plan");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003 Plan: Listing top-level folders and files in: {pathKey} => {baseDir}");

            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logWriter.WriteLine($" [Rule003CompressLogFiles_Plan]: Invalid or non-existent directory: {baseDir}. Cannot list contents.");
                return;
            }

            var entries = Directory.GetFileSystemEntries(baseDir, "*", SearchOption.TopDirectoryOnly);

            if (entries.Length == 0)
            {
                logWriter.WriteLine($" [Rule003CompressLogFiles_Plan]: No folders or files found in the directory: {baseDir}");
                cmdOutput.WriteLine(1, $"Rule003 Plan: No folders or files found in the directory: {baseDir}");
            }
            else
            {
                logWriter.WriteLine($"  [Rule003CompressLogFiles_Plan]:  DIR {baseDir}");
                cmdOutput.WriteLine(1, $"Rule003 Plan: Found the following top-level folders and files in: {baseDir}");

                foreach (var entry in entries)
                {
                    string entryType = Directory.Exists(entry) ? "Folder" : "File";
                    logWriter.WriteLine($"  - [{entryType}] {entry}");
                    cmdOutput.WriteLine(1, $"  - [{entryType}] {entry}");
                }



                // Process each entry and determine if it should be compressed
                var localLogs = ZipDirLogBy2Parameters(baseDir, N, M);
                foreach (var x in localLogs)
                {
                    var info = x.Split("|");
                    if (info.Length >= 2 && info[1] == "1")
                    {
                        Console.WriteLine(x);
                    }

                }
                Console.WriteLine("-------------------------");

                throw new Exception("by Mark, TESTING ZipDirLogBy2Parameters");

                ProcessEntriesForCompression(baseDir, entries, logWriter);


                // NOTE by Mark,需要處理同一天有多個 log
                // 
                //ProcessEntriesForCompressionV2(baseDir, entries, logWriter);

            }

            //logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003 Plan: Listing complete for {pathKey} => {baseDir}\n");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003 Plan: Listing complete for {pathKey} => {baseDir}\n");
        }
        private void ZipDirByRule003CompressLogFiles_Plan(string pathKey, string baseDir, StreamWriter logWriter)
        {
            //logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003CompressLogFiles_Plan");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003 Plan: Listing top-level folders and files in: {pathKey} => {baseDir}");

            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logWriter.WriteLine($" [Rule003CompressLogFiles_Plan]: Invalid or non-existent directory: {baseDir}. Cannot list contents.");
                return;
            }

            var entries = Directory.GetFileSystemEntries(baseDir, "*", SearchOption.TopDirectoryOnly);

            if (entries.Length == 0)
            {
                logWriter.WriteLine($" [Rule003CompressLogFiles_Plan]: No folders or files found in the directory: {baseDir}");
                cmdOutput.WriteLine(1, $"Rule003 Plan: No folders or files found in the directory: {baseDir}");
            }
            else
            {
                logWriter.WriteLine($"  [Rule003CompressLogFiles_Plan]:  DIR {baseDir}");
                cmdOutput.WriteLine(1, $"Rule003 Plan: Found the following top-level folders and files in: {baseDir}");

                foreach (var entry in entries)
                {
                    string entryType = Directory.Exists(entry) ? "Folder" : "File";
                    logWriter.WriteLine($"  - [{entryType}] {entry}");
                    cmdOutput.WriteLine(1, $"  - [{entryType}] {entry}");
                }



                // Process each entry and determine if it should be compressed
                var localLogs = ZipDirLogBy2Parameters(baseDir, N, M);
                foreach (var x in localLogs)
                {
                    var info = x.Split("|");
                    if (info.Length >= 2 && info[1] == "1")
                    {
                        Console.WriteLine(x);
                    }

                }
                Console.WriteLine("-------------------------");

                throw new Exception("by Mark, TESTING ZipDirLogBy2Parameters");

                ProcessEntriesForCompression(baseDir, entries, logWriter);


                // NOTE by Mark,需要處理同一天有多個 log
                // 
                //ProcessEntriesForCompressionV2(baseDir, entries, logWriter);

            }

            //logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003 Plan: Listing complete for {pathKey} => {baseDir}\n");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003 Plan: Listing complete for {pathKey} => {baseDir}\n");
        }
        private void Rule003CopyZipBack(string baseDir, StreamWriter logWriter)
        {
            // Define the _ZIP directory path
            string zipOutputDir = $"{baseDir}_ZIP";
            //logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003CopyZipBack: Starting to copy ZIP files back to the original folder from: {zipOutputDir}");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003CopyZipBack: Starting to copy ZIP files back to the original folder from: {zipOutputDir}");

            if (!Directory.Exists(zipOutputDir))
            {
                //logWriter.WriteLine($"Rule003CopyZipBack: The ZIP output directory does not exist: {zipOutputDir}");
                cmdOutput.WriteLine(1, $"Rule003CopyZipBack: The ZIP output directory does not exist: {zipOutputDir}");
                return;
            }

            // Get all ZIP files in the _ZIP directory
            var zipFiles = Directory.GetFiles(zipOutputDir, "*.zip", SearchOption.TopDirectoryOnly);

            if (zipFiles.Length == 0)
            {
                logWriter.WriteLine($"Rule003CopyZipBack: No ZIP files found in the directory: {zipOutputDir}");
                cmdOutput.WriteLine(1, $"Rule003CopyZipBack: No ZIP files found in the directory: {zipOutputDir}");
                return;
            }

            //logWriter.WriteLine($"Rule003CopyZipBack: Found the following ZIP files in {zipOutputDir} to copy back:");
            cmdOutput.WriteLine(1, $"Rule003CopyZipBack: Found the following ZIP files in {zipOutputDir} to copy back:");
            // NOTE: need to clean up after use
            foreach (var zipFile in zipFiles)
            {
                //logWriter.WriteLine($"  - {zipFile}");
                cmdOutput.WriteLine(1, $"  - {zipFile}");
            }

            // Copy each ZIP file back to the original base directory
            foreach (var zipFile in zipFiles)
            {
                string fileName = Path.GetFileName(zipFile);
                string destinationPath = Path.Combine(baseDir, fileName);

                //logWriter.WriteLine($"Rule003CopyZipBack: Copying {fileName} back to {baseDir}");
                cmdOutput.WriteLine(1, $"Rule003CopyZipBack: Copying {fileName} back to {baseDir}");

                try
                {
                    File.Copy(zipFile, destinationPath, overwrite: true);
                    //logWriter.WriteLine($"Rule003CopyZipBack: Successfully copied {fileName} to {destinationPath}");
                    cmdOutput.WriteLine(1, $"Rule003CopyZipBack: Successfully copied {fileName} to {destinationPath}");
                }
                catch (Exception ex)
                {
                    logWriter.WriteLine($"Rule003CopyZipBack: Error copying {fileName} to {destinationPath}: {ex.Message}");
                    cmdOutput.WriteLine(1, $"Rule003CopyZipBack: Error copying {fileName} to {destinationPath}: {ex.Message}");
                }
            }

            // Delete the _ZIP directory
            try
            {
                Directory.Delete(zipOutputDir, true);
                //logWriter.WriteLine($"Rule003CopyZipBack: Successfully deleted the _ZIP directory: {zipOutputDir}");
                cmdOutput.WriteLine(1, $"Rule003CopyZipBack: Successfully deleted the _ZIP directory: {zipOutputDir}");
            }
            catch (Exception ex)
            {
                logWriter.WriteLine($"Rule003CopyZipBack: Error deleting the _ZIP directory: {ex.Message}");
                cmdOutput.WriteLine(1, $"Rule003CopyZipBack: Error deleting the _ZIP directory: {ex.Message}");
            }

            //logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003CopyZipBack: Copy operation complete for {baseDir}");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003CopyZipBack: Copy operation complete for {baseDir}");
        }

        // NOTE by Mark, 09/05, 
        // 在指定目錄, 根據 N M 兩參數, 只看 2024-09-05{any suffix}.log 做 ZIP
        // 在 4.7 在原目錄, 有遇到困難,
        // 在 .Net8 try advance, 包括 lock if possible and necessary
        private List<string> ZipDirLogBy2Parameters(string baseDir, int DaysNotToZip, int ZipEveryDays)
        {
            try
            {
                var localLog = new List<string>();
                var entries = Directory.GetFileSystemEntries(baseDir, "*", SearchOption.TopDirectoryOnly);

                localLog.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}|1|ZipDirLogBy2Parameters");
                localLog.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}|1|baseDir={baseDir}");
                localLog.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}|1|DaysNotToZip={DaysNotToZip}");
                localLog.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}|1|ZipEveryDays={ZipEveryDays}");


                //var topLevel = new List<TopLevelEntry>();
                var entriesNoZip = entries
                    .Where(entry => !entry.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                //foreach (var entry in entriesNoZip)
                //{
                //    string entryType = Directory.Exists(entry) ? "Folder" : "File";
                //    logWriter.WriteLine($"  - [{entryType}] {entry}");
                //    cmdOutput.WriteLine(1, $"  - [{entryType}] {entry}");
                //}





                // 過濾並排序項目
                var filteredEntries = entriesNoZip
                    .Where(entry =>
                    {
                        string entryName = Path.GetFileName(entry);
                        if (DateTime.TryParseExact(entryName.Substring(0, 10), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime entryDate))
                        {
                            return entryDate <= DateTime.Now.AddDays(-N);
                        }
                        return false;
                    })
                    .OrderBy(entry => Path.GetFileName(entry).Substring(0, 10))  // 根據檔案或資料夾名稱中的日期進行排序
                    .ToList();

                var entriesToZip = new List<string>();
                DateTime? firstDate = null;
                DateTime? lastDate = null;
                DateTime? flagDate = null;

                bool flagSameDate = true;

                // 當累積了 M 天後，進行壓縮操作
                bool flag1 = false;


                int dayCount = 0;

                for (int i = 0; i < filteredEntries.Count; i++)
                {
                    var entry = filteredEntries[i];
                    string entryName = Path.GetFileName(entry);
                    DateTime entryDate = DateTime.ParseExact(entryName.Substring(0, 10), "yyyy-MM-dd", null);

                    if (firstDate == null)
                    {
                        firstDate = entryDate;
                    }

                    // 累積天數
                    if (lastDate == null || entryDate != lastDate)
                    {
                        dayCount++;
                        lastDate = entryDate;

                        flagDate = entryDate;
                    }

                    // 
                    entriesToZip.Add(entry);

                    // 當累積了 M 天後，進行壓縮操作
                    if (dayCount == M)
                    {

                        // TODO to i++ if the same date

                        // 如果不是同一天，則進行壓縮操作
                        //CreateZipFileExtV2(baseDir, entriesToZip, firstDate.Value, lastDate.Value, logWriter);
                        //CreateZipFileExtV3(baseDir, firstDate.Value, lastDate.Value, logWriter);
                        var subLog = CreateZipFileExtrReturnLog(baseDir, firstDate.Value, lastDate.Value);
                        localLog.AddRange(subLog);


                        entriesToZip.Clear();
                        dayCount = 0;
                        firstDate = null;
                        lastDate = null;


                        // NOTE by Mark, 一個快轉OK!
                        //由於  CreateZipFileExtV2
                        for (int k = 0; k < 100; k++)
                        {

                            var j = 1 + i;
                            if (j < filteredEntries.Count)
                            {
                                var entryPeek = filteredEntries[j];
                                string entryNamePeek = Path.GetFileName(entryPeek);
                                DateTime entryDatePeek = DateTime.ParseExact(entryNamePeek.Substring(0, 10), "yyyy-MM-dd", null);
                                if (entryDatePeek == flagDate)
                                {
                                    i++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                return localLog;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void ProcessEntriesForCompression(string baseDir, string[] entries, StreamWriter logWriter)
        {
            try
            {


                //var topLevel = new List<TopLevelEntry>();
                var entriesNoZip = entries
                    .Where(entry => !entry.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                //foreach (var entry in entriesNoZip)
                //{
                //    string entryType = Directory.Exists(entry) ? "Folder" : "File";
                //    logWriter.WriteLine($"  - [{entryType}] {entry}");
                //    cmdOutput.WriteLine(1, $"  - [{entryType}] {entry}");
                //}





                // 過濾並排序項目
                var filteredEntries = entriesNoZip
                    .Where(entry =>
                    {
                        string entryName = Path.GetFileName(entry);
                        if (DateTime.TryParseExact(entryName.Substring(0, 10), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime entryDate))
                        {
                            return entryDate <= DateTime.Now.AddDays(-N);
                        }
                        return false;
                    })
                    .OrderBy(entry => Path.GetFileName(entry).Substring(0, 10))  // 根據檔案或資料夾名稱中的日期進行排序
                    .ToList();

                var entriesToZip = new List<string>();
                DateTime? firstDate = null;
                DateTime? lastDate = null;
                DateTime? flagDate = null;

                bool flagSameDate = true;

                // 當累積了 M 天後，進行壓縮操作
                bool flag1 = false;


                int dayCount = 0;

                for (int i = 0; i < filteredEntries.Count; i++)
                {
                    var entry = filteredEntries[i];
                    string entryName = Path.GetFileName(entry);
                    DateTime entryDate = DateTime.ParseExact(entryName.Substring(0, 10), "yyyy-MM-dd", null);

                    if (firstDate == null)
                    {
                        firstDate = entryDate;
                    }

                    // 累積天數
                    if (lastDate == null || entryDate != lastDate)
                    {
                        dayCount++;
                        lastDate = entryDate;

                        flagDate = entryDate;
                    }

                    // 
                    entriesToZip.Add(entry);

                    // 當累積了 M 天後，進行壓縮操作
                    if (dayCount == M)
                    {

                        // TODO to i++ if the same date

                        // 如果不是同一天，則進行壓縮操作
                        //CreateZipFileExtV2(baseDir, entriesToZip, firstDate.Value, lastDate.Value, logWriter);
                        CreateZipFileExtV3(baseDir, firstDate.Value, lastDate.Value, logWriter);



                        entriesToZip.Clear();
                        dayCount = 0;
                        firstDate = null;
                        lastDate = null;


                        // NOTE by Mark, 一個快轉OK!
                        //由於  CreateZipFileExtV2
                        for (int k = 0; k < 100; k++)
                        {

                            var j = 1 + i;
                            if (j < filteredEntries.Count)
                            {
                                var entryPeek = filteredEntries[j];
                                string entryNamePeek = Path.GetFileName(entryPeek);
                                DateTime entryDatePeek = DateTime.ParseExact(entryNamePeek.Substring(0, 10), "yyyy-MM-dd", null);
                                if (entryDatePeek == flagDate)
                                {
                                    i++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void CreateZipFileExtV3(string baseDir, DateTime fromDate, DateTime toDate, StreamWriter logWriter)
        {
            // Set the target directory for storing the ZIP file
            string zipOutputDir = $"{baseDir}_ZIP";
            if (!Directory.Exists(zipOutputDir))
            {
                Directory.CreateDirectory(zipOutputDir);
            }

            // Create ZipArchiveInfo, and store the ZIP file in the specified target directory
            var zipInfo = new ZipArchiveInfo(zipOutputDir, fromDate, toDate);
            zipInfo.EnsureUniqueFileName();

            // Need to check 
            // creating..., not actually created
            //logWriter.WriteLine($"\nCreating ZIP file: {zipInfo.ZipFileName}");

            // Retrieve all directories and files in the baseDir that match the date range
            var entriesToZip = Directory.GetFileSystemEntries(baseDir, "*", SearchOption.TopDirectoryOnly)
                .Where(entry =>
                {
                    string entryName = Path.GetFileName(entry);
                    if (DateTime.TryParseExact(entryName.Substring(0, 10), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime entryDate))
                    {
                        return entryDate >= fromDate && entryDate <= toDate;
                    }
                    return false;
                })
                .ToList();

            using (var zipArchive = ZipFile.Open(zipInfo.ZipFileName, ZipArchiveMode.Create))
            {
                foreach (var entry in entriesToZip)
                {
                    if (Directory.Exists(entry))
                    {
                        foreach (var file in Directory.GetFiles(entry, "*", SearchOption.AllDirectories))
                        {
                            if (!file.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)) // Exclude .zip files
                            {
                                string relativePath = GetRelativePath(baseDir, file);
                                zipArchive.CreateEntryFromFile(file, relativePath);
                                zipInfo.AddZippedItem(relativePath);

                                // Delete the compressed file
                                File.Delete(file);
                            }
                        }

                        // If all files have been compressed, delete the directory
                        if (Directory.GetFiles(entry, "*", SearchOption.AllDirectories).Length == 0)
                        {
                            Directory.Delete(entry, true);
                        }
                    }
                    else
                    {
                        if (!entry.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)) // Exclude .zip files
                        {
                            string relativePath = GetRelativePath(baseDir, entry);
                            zipArchive.CreateEntryFromFile(entry, relativePath);
                            zipInfo.AddZippedItem(relativePath);

                            // Delete the compressed file
                            File.Delete(entry);
                        }
                    }
                }
            }

            // 
            if (zipInfo.ZippedItems.Count > 0)
            {


                logWriter.WriteLine($"ZIP file created: {zipInfo.ZipFileName}");
                logWriter.WriteLine("Zipped and deleted items:");
                foreach (var item in zipInfo.ZippedItems)
                {
                    logWriter.WriteLine($"  - {zipInfo.BaseDir}\\{item}");
                }
            }
        }


        private List<string> CreateZipFileExtrReturnLog(string baseDir, DateTime fromDate, DateTime toDate)
        {
            List<string> localLog = new List<string>();
            // Set the target directory for storing the ZIP file
            string zipOutputDir = $"{baseDir}_ZIP";
            if (!Directory.Exists(zipOutputDir))
            {
                Directory.CreateDirectory(zipOutputDir);
            }

            // Create ZipArchiveInfo, and store the ZIP file in the specified target directory
            //var zipInfo = new ZipArchiveInfo(zipOutputDir, fromDate, toDate);
            var zipInfo = new ZipArchiveInfo(baseDir, $"{baseDir}_ZIP", fromDate, toDate);

            zipInfo.EnsureUniqueFileName();

            // Need to check 
            // creating..., not actually created
            //logWriter.WriteLine($"\nCreating ZIP file: {zipInfo.ZipFileName}");

            // Retrieve all directories and files in the baseDir that match the date range
            var entriesToZip = Directory.GetFileSystemEntries(baseDir, "*", SearchOption.TopDirectoryOnly)
                .Where(entry =>
                {
                    string entryName = Path.GetFileName(entry);
                    if (DateTime.TryParseExact(entryName.Substring(0, 10), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime entryDate))
                    {
                        return entryDate >= fromDate && entryDate <= toDate;
                    }
                    return false;
                })
                .ToList();

            using (var zipArchive = ZipFile.Open(zipInfo.ZipFileName, ZipArchiveMode.Create))
            {
                foreach (var entry in entriesToZip)
                {
                    if (Directory.Exists(entry))
                    {
                        foreach (var file in Directory.GetFiles(entry, "*", SearchOption.AllDirectories))
                        {
                            if (!file.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)) // Exclude .zip files
                            {
                                string relativePath = GetRelativePath(baseDir, file);
                                zipArchive.CreateEntryFromFile(file, relativePath);
                                zipInfo.AddZippedItem(relativePath);

                                //localLog.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}|1|{zipInfo.ZipFileName} added {relativePath}!");

                                // Delete the compressed file
                                File.Delete(file);
                                //localLog.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}|1|{relativePath} deleted !");

                            }
                        }

                        // If all files have been compressed, delete the directory
                        if (Directory.GetFiles(entry, "*", SearchOption.AllDirectories).Length == 0)
                        {
                            Directory.Delete(entry, true);
                        }
                    }
                    else
                    {
                        if (!entry.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)) // Exclude .zip files
                        {
                            string relativePath = GetRelativePath(baseDir, entry);
                            zipArchive.CreateEntryFromFile(entry, relativePath);
                            zipInfo.AddZippedItem(relativePath);

                            // Delete the compressed file
                            File.Delete(entry);
                        }
                    }
                }
            }

            // 這是先把所有 zip 生成及其內容 暫存 in memory, then to make log 
            if (zipInfo.ZippedItems.Count > 0)
            {
                localLog.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}|2|{zipInfo.ZipFileName} zip file created!");
                foreach (var item in zipInfo.ZippedItems)
                {
                    localLog.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}|2| - {zipInfo.BaseDir}\\{item} log file deleted!");
                    //      logWriter.WriteLine($"  - {zipInfo.BaseDir}\\{item}");
                }
            }
            return localLog;
        }






        private void Rule003CompressLogFiles(string pathKey, string baseDir, StreamWriter logWriter)
        {
            //Rule003CompressLogFiles_Plan(pathKey, baseDir, logWriter);


            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003: Compressing log files in: {pathKey} => {baseDir} [Start]");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003: Compressing log files in: {pathKey} => {baseDir} [Start]");

            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logWriter.WriteLine($"Rule003: Invalid or non-existent directory: {baseDir}. Skipping compression!");
                return;
            }

            var (zipFiles, deletedItems) = Rule003CompressAndDeleteLogs(baseDir, logWriter);

            if (zipFiles.Count > 0)
            {
                logWriter.WriteLine($"Rule003: Successfully created the following ZIP files: {string.Join(", ", zipFiles)}");
            }

            if (deletedItems.Count > 0)
            {
                logWriter.WriteLine("Rule003: Deleted the following files:");
                foreach (var item in deletedItems)
                {
                    logWriter.WriteLine($"  - {item}");
                }
            }
            else if (Directory.Exists(baseDir))
            {
                logWriter.WriteLine($"Rule003: No action required for ({pathKey}).");
            }
            else
            {
                logWriter.WriteLine($"Rule003: Invalid directory path: {baseDir}");
            }

            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003: Compressing log files in: {pathKey} => {baseDir} [End]\n");
        }
        public (List<string> zipFiles, List<string> deletedItems) Rule003CompressAndDeleteLogs(string baseDir, StreamWriter logWriter)
        {
            List<string> generatedZipFiles = new List<string>();
            List<string> deletedItems = new List<string>();

            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logWriter.WriteLine($"Invalid or non-existent directory: {baseDir}. Skipping processing!");
                return (generatedZipFiles, deletedItems);
            }

            DateTime currentDate = DateTime.Now;
            DateTime nDaysAgo = currentDate.AddDays(-N);

            var entries = Directory.GetFileSystemEntries(baseDir, "*", SearchOption.AllDirectories)
                .Where(entry =>
                {
                    string entryName = Path.GetFileName(entry);
                    if (DateTime.TryParseExact(entryName.Substring(0, 10), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime entryDate))
                    {
                        return entryDate <= nDaysAgo;
                    }
                    return false;
                })
                .OrderByDescending(entry => Path.GetFileName(entry))
                .ToList();

            for (int i = 0; i < entries.Count; i += M)
            {
                var entriesToZip = entries.Skip(i).Take(M).ToList();
                if (entriesToZip.Count < M)
                {
                    logWriter.WriteLine($"Insufficient number of days for compression. Skipping...");
                    break;
                }

                string fromDate = Path.GetFileName(entriesToZip.First()).Substring(0, 10);
                string toDate = Path.GetFileName(entriesToZip.Last()).Substring(0, 10);
                string zipFileName = $"{fromDate}_{toDate}.zip";
                string zipFilePath = Path.Combine(baseDir, zipFileName);

                int suffix = 1;
                while (File.Exists(zipFilePath))
                {
                    zipFileName = $"{fromDate}_{toDate}_{suffix:D2}.zip";
                    zipFilePath = Path.Combine(baseDir, zipFileName);
                    suffix++;
                }

                using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    foreach (var entry in entriesToZip)
                    {
                        if (Directory.Exists(entry))
                        {
                            foreach (var file in Directory.GetFiles(entry, "*", SearchOption.AllDirectories))
                            {
                                //string relativePath = GetRelativePath(baseDir, file);
                                string tempZipFileFullPath = GetRelativePath(baseDir+"_ZIP", file);

                                zipArchive.CreateEntryFromFile(file, tempZipFileFullPath);
                            }
                        }
                        else
                        {
                            zipArchive.CreateEntryFromFile(entry, Path.GetFileName(entry));
                            File.Delete(entry);
                            deletedItems.Add(entry);
                        }
                    }
                }

                logWriter.WriteLine($"Created ZIP file: {zipFilePath}");
                generatedZipFiles.Add(zipFileName);
            }

            return (generatedZipFiles, deletedItems);
        }


        // Define a delegate that matches the signature of ProcessPaths methods
        public delegate void ProcessPathsDelegate(string ver, IniData data, string logFilePath);
        public void RunRule(string ver, string ruleName, ProcessPathsDelegate processPathsMethod)
        {
            //if (File.Exists(iniFilePath))
            //{
            //    cmdOutput.WriteLine(1, $"config.ini is found!");

            //}
            //else
            //{
            //    cmdOutput.WriteLine(99, $"config.ini not found! ");
            //    return;
            //    //CreateDefaultIniFile();
            //    //cmdOutput.WriteLine(1, $"A default one has been created!");
            //}

            //LoadAndFilterIniFile();
            //var data = ParseIniFile();
            //ConfigureLogSettings(data);

            // Generate log file path
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd_HHmm");
            string logFileName = $"{currentDate}_ZipLogTool.log";
            string logFilePath = Path.Combine(logPath, logFileName);

            // Use the delegate to call the specific ProcessPaths method
            cmdOutput.WriteLine(1, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule ({ruleName}) is going to process...");

            processPathsMethod(ver, data, logFilePath);

            cmdOutput.WriteLine(1, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule ({ruleName}) has been processed completely, and log file is {logFilePath}");
        }

        private void CreateDefaultIniFile()
        {
            string defaultContent = @"
[Paths]
Path1=d:\Lab\TESTCASE001
Path2=d:\Lab\TESTCASE002

[Options]
N=3
M=5
Q=2
";
            File.WriteAllText(iniFilePath, defaultContent, Encoding.UTF8);
        }


        public IniData ParseIniFile()
        {
            try
            {
                // Read the entire file into a string
                string iniContent = File.ReadAllText(iniFilePath);

                // Create an instance of the parser
                var parser = new IniParser.IniDataParser();

                // Parse the content of the file
                var obj = parser.Parse(iniContent);

                return obj;
            }
            catch (Exception e)
            {
                // Log the error or handle it as needed
                throw new Exception($"Error parsing INI file: {e.Message}", e);
            }
        }

        //public IniData ParseIniFile()
        //{
        //    try
        //    {

        //        var parser = new IniParser.IniDataParser();
        //        //   var parser = new FileIniDataParser();
        //        //   var parser=new INIPaser.Parser();
        //      //  parser.Configuration.SkipInvalidLines = true; // Skip invalid lines

        //        var obj = parser.Parse(iniFilePath);
        //        return obj;
        //        //return parser.ReadFile(iniFilePath);
        //    }
        //    catch (Exception e)
        //    {
        //        throw;
        //    }


        //}




        // Method to get the local IP address
        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }





        private string CreateUniqueZipFilePath(string baseDir, string dateString)
        {
            int zipSuffix = 0;
            string zipFilePath = Path.Combine(baseDir, $"{dateString}.zip");

            while (File.Exists(zipFilePath))
            {
                zipSuffix++;
                zipFilePath = Path.Combine(baseDir, $"{dateString}_{zipSuffix}.zip");
            }

            return zipFilePath;
        }

        private (List<string> zipFiles, List<string> deletedItems) ProcessPath_BUG(string pathKey, string baseDir, StreamWriter logWriter)
        {
            List<string> generatedZipFiles = new List<string>();
            List<string> deletedItems = new List<string>();

            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logWriter.WriteLine($"無效或不存在的目錄({pathKey}): {baseDir} 不處理!");
                return (generatedZipFiles, deletedItems);
            }

            logWriter.WriteLine($"開始處理目錄: {baseDir}");
            LogDirectoryContents(baseDir, logWriter);

            int startYear = DateTime.Now.Year - 9;
            for (int year = startYear; year <= DateTime.Now.Year; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    string monthString = new DateTime(year, month, 1).ToString("yyyy-MM");

                    if (string.Compare(monthString, DateTime.Now.ToString("yyyy-MM")) >= 0)
                        break;

                    if (!Directory.GetDirectories(baseDir).Any(d => Path.GetFileName(d).StartsWith(monthString)) &&
                        !Directory.GetFiles(baseDir, "*.log").Any(f => Path.GetFileName(f).StartsWith(monthString)) &&
                        !Directory.GetFiles(baseDir, "*.txt").Any(f => Path.GetFileName(f).StartsWith(monthString)))
                    {
                        continue;
                    }

                    string tempDir = Path.Combine(baseDir, $"TempForZipping_{monthString}");
                    Directory.CreateDirectory(tempDir);

                    bool hasContentToZip = false;
                    List<string> originalItems = new List<string>();

                    hasContentToZip |= MoveMatchingDirectories(baseDir, monthString, tempDir, logWriter, originalItems);
                    hasContentToZip |= MoveMatchingFiles(baseDir, monthString, "*.log", tempDir, logWriter, originalItems);
                    hasContentToZip |= MoveMatchingFiles(baseDir, monthString, "*.txt", tempDir, logWriter, originalItems);

                    if (hasContentToZip)
                    {
                        string zipFilePath = CreateUniqueZipFilePath(baseDir, monthString);
                        ZipFile.CreateFromDirectory(tempDir, zipFilePath);
                        logWriter.WriteLine($"生成 ZIP 文件：{zipFilePath}");
                        generatedZipFiles.Add(Path.GetFileName(zipFilePath));

                        deletedItems.AddRange(originalItems);
                    }

                    Directory.Delete(tempDir, true);
                }
            }

            return (generatedZipFiles, deletedItems);
        }


        private (List<string> zipFiles, List<string> deletedItems) ProcessPath(string pathKey, string baseDir, StreamWriter logWriter)
        {
            List<string> generatedZipFiles = new List<string>();
            List<string> deletedItems = new List<string>();

            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logWriter.WriteLine($"無效或不存在的目錄({pathKey}): {baseDir} 不處理!");
                return (generatedZipFiles, deletedItems);
            }

            logWriter.WriteLine($"開始處理目錄: {baseDir}");
            LogDirectoryContents(baseDir, logWriter);

            int startYear = DateTime.Now.Year - 9;
            for (int year = startYear; year <= DateTime.Now.Year; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    string monthString = new DateTime(year, month, 1).ToString("yyyy-MM");

                    if (string.Compare(monthString, DateTime.Now.ToString("yyyy-MM")) >= 0)
                        break;

                    if (!Directory.GetDirectories(baseDir).Any(d => Path.GetFileName(d).StartsWith(monthString)) &&
                        !Directory.GetFiles(baseDir, "*.log").Any(f => Path.GetFileName(f).StartsWith(monthString)) &&
                        !Directory.GetFiles(baseDir, "*.txt").Any(f => Path.GetFileName(f).StartsWith(monthString)))
                    {
                        continue;
                    }

                    string tempDir = Path.Combine(baseDir, $"TempForZipping_{monthString}");
                    Directory.CreateDirectory(tempDir);

                    bool hasContentToZip = false;
                    List<string> originalItems = new List<string>();

                    hasContentToZip |= MoveMatchingDirectories(baseDir, monthString, tempDir, logWriter, originalItems, keepFolder: true);
                    hasContentToZip |= MoveMatchingFiles(baseDir, monthString, "*.log", tempDir, logWriter, originalItems);
                    hasContentToZip |= MoveMatchingFiles(baseDir, monthString, "*.txt", tempDir, logWriter, originalItems);

                    if (hasContentToZip)
                    {
                        string zipFilePath = CreateUniqueZipFilePath(baseDir, monthString);
                        ZipFile.CreateFromDirectory(tempDir, zipFilePath);
                        logWriter.WriteLine($"生成 ZIP 文件：{zipFilePath}");
                        generatedZipFiles.Add(Path.GetFileName(zipFilePath));

                        deletedItems.AddRange(originalItems);
                    }

                    Directory.Delete(tempDir, true);
                }
            }

            return (generatedZipFiles, deletedItems);
        }

        private bool MoveMatchingDirectories(string baseDir, string monthString, string tempDir, StreamWriter logWriter, List<string> originalItems, bool keepFolder = false)
        {
            bool hasContentToZip = false;
            foreach (string dir in Directory.GetDirectories(baseDir))
            {
                string dirName = Path.GetFileName(dir);
                if (dirName.StartsWith(monthString))
                {
                    hasContentToZip = true;
                    string destDir = Path.Combine(tempDir, dirName);
                    Directory.CreateDirectory(destDir);

                    foreach (var file in Directory.GetFiles(dir))
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = Path.Combine(destDir, fileName);
                        File.Move(file, destFile);
                        logWriter.WriteLine($"移動文件: {fileName} 到臨時目錄。");
                        originalItems.Add(file);
                    }

                    if (!keepFolder)
                    {
                        Directory.Delete(dir, true);
                        logWriter.WriteLine($"移動目錄: {dirName} 到臨時目錄。");
                        originalItems.Add(dir);
                    }
                }
            }
            return hasContentToZip;
        }



        private bool MoveMatchingDirectories(string baseDir, string monthString, string tempDir, StreamWriter logWriter, List<string> originalItems)
        {
            bool hasContentToZip = false;
            foreach (string dir in Directory.GetDirectories(baseDir))
            {
                string dirName = Path.GetFileName(dir);
                if (dirName.StartsWith(monthString))
                {
                    hasContentToZip = true;
                    string destDir = Path.Combine(tempDir, dirName);
                    Directory.Move(dir, destDir);
                    logWriter.WriteLine($"移動目錄: {dirName} 到臨時目錄。");
                    originalItems.Add(dir);
                }
            }
            return hasContentToZip;
        }

        private bool MoveMatchingFiles(string baseDir, string monthString, string searchPattern, string tempDir, StreamWriter logWriter, List<string> originalItems)
        {
            bool hasContentToZip = false;
            foreach (string file in Directory.GetFiles(baseDir, searchPattern))
            {
                string fileName = Path.GetFileName(file);
                if (fileName.StartsWith(monthString))
                {
                    hasContentToZip = true;
                    string destFile = Path.Combine(tempDir, fileName);
                    File.Move(file, destFile);
                    logWriter.WriteLine($"移動文件: {fileName} 到臨時目錄。");
                    originalItems.Add(file);
                }
            }
            return hasContentToZip;
        }


        public void Rule003ProcessPaths(string ver, IniData data, string logFilePath)
        {
            using (StreamWriter logWriter = new StreamWriter(logFilePath, true, Encoding.UTF8))
            {
                int numDashes = 120; // Set this to the number of dashes you want

                var pathsSection = data["Paths"];
                //Dictionary<string, string> pathsSection = new Dictionary<string, string>
                //{
                //    { "Path1", @"D:\LAB\TESTCASE001" },
                //    { "Path2", @"D:\LAB\TESTCASE002" }
                //};

                logWriter.WriteLine($"******* ZipLogTool(ver:{ver}): {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Start] *******");
                logWriter.WriteLine(new string('-', numDashes));

                //logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Working environment [Start] ");
                cmdOutput.WriteLine(1, $"\n\n\n=== ZipLogTool(ver:{ver}): {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Start] ===");

                // Log system and environment information
                string currentDirectory = Directory.GetCurrentDirectory();
                logWriter.WriteLine($"  DIR={currentDirectory}");
                cmdOutput.WriteLine(1, $"  DIR={currentDirectory}");

                string osInfo = Environment.OSVersion.ToString();
                logWriter.WriteLine($"  OS={osInfo}");
                cmdOutput.WriteLine(1, $"  OS={osInfo}");

                string ipAddress = GetLocalIPAddress();
                logWriter.WriteLine($"  IP={ipAddress}");
                cmdOutput.WriteLine(1, $"  IP={ipAddress}");

                string computerName = Environment.MachineName;
                logWriter.WriteLine($"  Name={computerName}");
                cmdOutput.WriteLine(1, $"  Name={computerName}");
                //logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Working environment [End] ");

                logWriter.WriteLine(new string('-', numDashes));
                cmdOutput.WriteLine(1, $"------------------------------------------");

                // Log the reading of the config file
                //logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Read config.ini [Start] ");
                cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Read config.ini [Start] ");
                logWriter.WriteLine($"[Paths]");
                foreach (var path in pathsSection)
                {
                    logWriter.WriteLine($" {path.Key}={path.Value}");
                }
                logWriter.WriteLine($"[Options]");
                logWriter.WriteLine($"  N={N} : Number of days before which log files will be compressed.");
                logWriter.WriteLine($"  M={M} : Number of days of log data to include in each ZIP file.");

                if (IS_Q)
                {

                    if (Q <= 0)
                    {
                        logWriter.WriteLine($"  Q={Q} : No need to perform deletion.");

                    }
                    else
                    {
                        logWriter.WriteLine($"  Q={Q} : Number of {Q}*30={30 * Q} days before which folders or files will be deleted.");

                    }
                }



                //logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Read config.ini [End]");
                logWriter.WriteLine(new string('-', numDashes));


                // Process each path: compress and delete logs, then delete old ZIP files
                foreach (var path in pathsSection)
                {
                    logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Processing path [{path.Key}]{path.Value} [Start]");

                    // First part: Compress log files based on N and M, and delete original logs
                    //Rule003CompressLogFiles_Plan(path.Key, path.Value, logWriter);
                    var baseDir = path.Value;
                    var localLogs = ZipDirLogBy2Parameters(baseDir, N, M);
                    foreach (var x in localLogs)
                    {
                        var info = x.Split("|");
                        if (info.Length >= 2 && info[1] == "1")
                        {
                            Console.WriteLine(x);
                        }

                    }
                    Console.WriteLine("-------------------------");

                    throw new Exception("by Mark, TESTING ZipDirLogBy2Parameters");




                    Rule003CopyZipBack(path.Value, logWriter);

                    // Second part: Delete old ZIP files based on Q

                    //logWriter.WriteLine($"\n 先不刪 zip");

                    // 另外月份的參數不給或是為零應該就是視為不刪除
                    if (Q > 0)
                    {
                        Rule003DeleteOldZipFiles(path.Value, Q, logWriter);
                    }

                    logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Processing path [{path.Key}]{path.Value} [End]");
                    logWriter.WriteLine(new string('-', numDashes));
                }

                logWriter.WriteLine($"******* ZipLogTool(ver:{ver})  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [End] *******\n\n\n\n\n");
                cmdOutput.WriteLine(1, $"=== ZipLogTool(ver:{ver})  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [End] ===\n\n");
            }
        }


        private string GetRelativePath(string baseDir, string fullPath)
        {
            Uri baseUri = new Uri(baseDir.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
            Uri fullUri = new Uri(fullPath);
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }




        private void LogDirectoryContents(string dirPath, StreamWriter logWriter)
        {
            logWriter.WriteLine($"目錄內容列舉: {dirPath}");
            var entries = Directory.GetFileSystemEntries(dirPath);

            if (entries.Length == 0)
            {
                logWriter.WriteLine("  [目錄為空]");
            }

            foreach (var entry in entries)
            {
                if (Directory.Exists(entry))
                {
                    logWriter.WriteLine($"  [目錄] {entry}");
                    var subEntries = Directory.GetFileSystemEntries(entry);
                    foreach (var subEntry in subEntries)
                    {
                        logWriter.WriteLine($"    - {subEntry}");
                    }
                }
                else
                {
                    logWriter.WriteLine($"  [文件] {entry}");
                }
            }
        }
    }
}
