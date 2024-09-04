using IniParser.Model;
using IniParser;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ZipLogTool
{

    public class ZipLogCore
    {
        private string iniFilePath = "config.ini";
        private string tempIniFilePath = "temp_config.ini";
        private string logPath = "logs";
        private int N;
        private int M;
        private int P;
        private int Q;

        // Declare the cmdOutput field correctly
        private CmdOutput cmdOutput;

        // Constructor to initialize cmdOutput
        public ZipLogCore(int verbosityLevel)
        {
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

            logWriter.WriteLine($"Rule003: Threshold date for ZIP file deletion is {deleteBeforeDate.ToString("yyyy-MM-dd")} ({totalDays} days ago)");
            cmdOutput.WriteLine(1, $"Rule003: Threshold date for ZIP file deletion is {deleteBeforeDate.ToString("yyyy-MM-dd")} ({totalDays} days ago)");

            var zipFiles = Directory.GetFiles(baseDir, "*.zip", SearchOption.TopDirectoryOnly).ToList();

            if (zipFiles.Count > 0)
            {
                logWriter.WriteLine("Rule003: Checking the following ZIP files for deletion:");
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
                        logWriter.WriteLine($"  - {zipFile}");

                        if (zipFileDate < deleteBeforeDate)
                        {
                            int daysOld = (DateTime.Now - zipFileDate).Days;

                            File.Delete(zipFile);
                            logWriter.WriteLine($"  - [ZIP File] {zipFile} deleted (Age: {daysOld} days)");
                            cmdOutput.WriteLine(1, $"  - [ZIP File] {zipFile} deleted (Age: {daysOld} days)");
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

        private void Rule003DeleteOldFoldersOrFiles_XXX(string pathKey, string baseDir, StreamWriter logWriter)
        {
            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003: Deleting old folders or files in: {pathKey} => {baseDir} [Start]");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003: Deleting old folders or files in: {pathKey} => {baseDir} [Start]");

            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logWriter.WriteLine($"Rule003: Invalid or non-existent directory: {baseDir}. Skipping deletion!");
                return;
            }

            int totalDays = P + (Q * 30);
            DateTime finalDeleteDate = DateTime.Now.AddDays(-totalDays);

            logWriter.WriteLine($"Rule003: Threshold date for deletion is {finalDeleteDate.ToString("yyyy-MM-dd")} ({P + 30 * Q} days ago)");
            cmdOutput.WriteLine(1, $"Rule003: Threshold date for deletion is {finalDeleteDate.ToString("yyyy-MM-dd")} ({P + 30 * Q} days ago)");

            var entries = Directory.GetFileSystemEntries(baseDir, "*", SearchOption.TopDirectoryOnly).ToList();

            if (entries.Count > 0)
            {
                logWriter.WriteLine("Rule003: Checking the following folders or files for deletion:");
                cmdOutput.WriteLine(1, "Rule003: Checking the following folders or files for deletion:");
                foreach (var entry in entries)
                {
                    string entryName = Path.GetFileName(entry);
                    DateTime entryDate;

                    if (DateTime.TryParseExact(entryName.Substring(0, 10), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out entryDate))
                    {
                        logWriter.WriteLine($"  - {entry}");

                        if (entryDate < finalDeleteDate)
                        {
                            int daysOld = (DateTime.Now - entryDate).Days;

                            if (Directory.Exists(entry))
                            {
                                Directory.Delete(entry, true);
                                logWriter.WriteLine($"  - [Folder] {entry} deleted (Age: {daysOld} days)");
                                cmdOutput.WriteLine(1, $"  - [Folder] {entry} deleted (Age: {daysOld} days)");
                            }
                            else
                            {
                                File.Delete(entry);
                                logWriter.WriteLine($"  - [File] {entry} deleted (Age: {daysOld} days)");
                                cmdOutput.WriteLine(1, $"  - [File] {entry} deleted (Age: {daysOld} days)");
                            }
                        }
                    }
                    else
                    {
                        logWriter.WriteLine($"  - {entry} (Skipped: Could not parse date)");
                        cmdOutput.WriteLine(1, $"  - {entry} (Skipped: Could not parse date)");
                    }
                }
            }
            else
            {
                logWriter.WriteLine($"Rule003: No old folders or files found for deletion in ({pathKey}).");
                cmdOutput.WriteLine(1, $"Rule003: No old folders or files found for deletion in ({pathKey}).");
            }

            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003: Deleting old folders or files in: {pathKey} => {baseDir} [End]\n");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003: Deleting old folders or files in: {pathKey} => {baseDir} [End]\n");
        }
        private void Rule003CompressLogFiles_Plan(string pathKey, string baseDir, StreamWriter logWriter)
        {
            logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003CompressLogFiles_Plan");
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
                logWriter.WriteLine($"  [Rule003CompressLogFiles_Plan]:  Found the following top-level folders and files in: {baseDir}");
                cmdOutput.WriteLine(1, $"Rule003 Plan: Found the following top-level folders and files in: {baseDir}");

                foreach (var entry in entries)
                {
                    string entryType = Directory.Exists(entry) ? "Folder" : "File";
                    logWriter.WriteLine($"  - [{entryType}] {entry}");
                    cmdOutput.WriteLine(1, $"  - [{entryType}] {entry}");
                }

                // Process each entry and determine if it should be compressed
                ProcessEntriesForCompression(baseDir, entries, logWriter);

                // NOTE by Mark,需要處理同一天有多個 log
                // 
                //ProcessEntriesForCompressionV2(baseDir, entries, logWriter);

            }

            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003 Plan: Listing complete for {pathKey} => {baseDir}\n");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003 Plan: Listing complete for {pathKey} => {baseDir}\n");
        }
        private void Rule003CopyZipBack(string baseDir, StreamWriter logWriter)
        {
            // Define the _ZIP directory path
            string zipOutputDir = $"{baseDir}_ZIP";
            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003CopyZipBack: Starting to copy ZIP files back to the original folder from: {zipOutputDir}");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003CopyZipBack: Starting to copy ZIP files back to the original folder from: {zipOutputDir}");

            if (!Directory.Exists(zipOutputDir))
            {
                logWriter.WriteLine($"Rule003CopyZipBack: The ZIP output directory does not exist: {zipOutputDir}");
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

            logWriter.WriteLine($"Rule003CopyZipBack: Found the following ZIP files in {zipOutputDir} to copy back:");
            cmdOutput.WriteLine(1, $"Rule003CopyZipBack: Found the following ZIP files in {zipOutputDir} to copy back:");

            foreach (var zipFile in zipFiles)
            {
                logWriter.WriteLine($"  - {zipFile}");
                cmdOutput.WriteLine(1, $"  - {zipFile}");
            }

            // Copy each ZIP file back to the original base directory
            foreach (var zipFile in zipFiles)
            {
                string fileName = Path.GetFileName(zipFile);
                string destinationPath = Path.Combine(baseDir, fileName);

                logWriter.WriteLine($"Rule003CopyZipBack: Copying {fileName} back to {baseDir}");
                cmdOutput.WriteLine(1, $"Rule003CopyZipBack: Copying {fileName} back to {baseDir}");

                try
                {
                    File.Copy(zipFile, destinationPath, overwrite: true);
                    logWriter.WriteLine($"Rule003CopyZipBack: Successfully copied {fileName} to {destinationPath}");
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
                logWriter.WriteLine($"Rule003CopyZipBack: Successfully deleted the _ZIP directory: {zipOutputDir}");
                cmdOutput.WriteLine(1, $"Rule003CopyZipBack: Successfully deleted the _ZIP directory: {zipOutputDir}");
            }
            catch (Exception ex)
            {
                logWriter.WriteLine($"Rule003CopyZipBack: Error deleting the _ZIP directory: {ex.Message}");
                cmdOutput.WriteLine(1, $"Rule003CopyZipBack: Error deleting the _ZIP directory: {ex.Message}");
            }

            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003CopyZipBack: Copy operation complete for {baseDir}");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule003CopyZipBack: Copy operation complete for {baseDir}");
        }


        private void ProcessEntriesForCompression(string baseDir, string[] entries, StreamWriter logWriter)
        {
            var topLevel = new List<TopLevelEntry>();

            // 過濾並排序項目
            var filteredEntries = entries
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

                    //while (true)
                    //{
                    //    var j = 1 + i;
                    //    if (j >= filteredEntries.Count)
                    //    {
                    //        break; // Exit the loop if there are no more entries to check
                    //    }

                    //    var entryPeek = filteredEntries[j];
                    //    string entryNamePeek = Path.GetFileName(entryPeek);
                    //    DateTime entryDatePeek = DateTime.ParseExact(entryNamePeek.Substring(0, 10), "yyyy-MM-dd", null);

                    //    if (entryDatePeek == flagDate)
                    //    {
                    //        i++;
                    //    }
                    //    else
                    //    {
                    //        break; // Exit the loop if the next entry is not on the same date
                    //    }
                    //}

                }
            }







            //foreach (var entry in filteredEntries)
            //{
            //    string entryName = Path.GetFileName(entry);
            //    DateTime entryDate = DateTime.ParseExact(entryName.Substring(0, 10), "yyyy-MM-dd", null);

            //    if (firstDate == null)
            //    {
            //        firstDate = entryDate;
            //    }

            //    // 累積天數
            //    if (lastDate == null || entryDate != lastDate)
            //    {
            //        dayCount++;
            //        lastDate = entryDate;
            //    }

            //    // 
            //    entriesToZip.Add(entry);

            //    // 當累積了 M 天後，進行壓縮操作
            //    if (dayCount == M)
            //    {
            //        flag1 = true; // 先不壓, 要看下一個是否同一天的


            //        //CreateZipFileExt(baseDir, firstDate.Value, lastDate.Value, logWriter);
            //        CreateZipFileExtV2(baseDir, entriesToZip, firstDate.Value, lastDate.Value, logWriter);

            //        entriesToZip.Clear();
            //        dayCount = 0;
            //        firstDate = null;
            //        lastDate = null;
            //    }
            //}

            // 處理剩餘未壓縮的檔案或資料夾
            //if (entriesToZip.Count > 0)
            //{
            //    CreateZipFile(baseDir, entriesToZip, firstDate.Value, lastDate.Value, logWriter);
            //}
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

            logWriter.WriteLine($"\nCreating ZIP file: {zipInfo.ZipFileName}");

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

            logWriter.WriteLine($"ZIP file created: {zipInfo.ZipFileName}");
            logWriter.WriteLine("Zipped and deleted items:");
            foreach (var item in zipInfo.ZippedItems)
            {
                logWriter.WriteLine($"  - {zipInfo.BaseDir}\\{item}");
            }
        }

        private void CreateZipFileExt(string baseDir, DateTime fromDate, DateTime toDate, StreamWriter logWriter)
        {
            // Set the target directory for storing the ZIP file
            string zipOutputDir = $"{baseDir}_ZIP";
            if (!Directory.Exists(zipOutputDir))
            {
                Directory.CreateDirectory(zipOutputDir);
            }

            // Create ZipArchiveInfo, and store the ZIP file in the specified target directory
            var zipInfo = new ZipArchiveInfo(zipOutputDir, fromDate, toDate);
            //  zipInfo.EnsureUniqueFileName();
            // NEED TO FIX FILENAME

            //  zipInfo.ZipFileName=

            logWriter.WriteLine($"\nCreating ZIP file: {zipInfo.ZipFileName}");

            // Get the list of entries to zip based on the date range
            var entriesToZip = Directory.GetFileSystemEntries(baseDir, "*", SearchOption.TopDirectoryOnly)
                .Where(entry =>
                {
                    string entryName = Path.GetFileName(entry);
                    if (DateTime.TryParseExact(entryName.Substring(0, 10), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime entryDate))
                    {
                        return entryDate >= fromDate && entryDate <= toDate;
                    }
                    return false;
                }).ToList();

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

            logWriter.WriteLine($"ZIP file created: {zipInfo.ZipFileName}");
            logWriter.WriteLine("Zipped and deleted items:");
            foreach (var item in zipInfo.ZippedItems)
            {
                logWriter.WriteLine($"  - {item}");
            }
        }


        private void CreateZipFile(string baseDir, List<string> entriesToZip, DateTime fromDate, DateTime toDate, StreamWriter logWriter)
        {
            var zipInfo = new ZipArchiveInfo(baseDir, fromDate, toDate);
            zipInfo.EnsureUniqueFileName();

            logWriter.WriteLine($"\nCreating ZIP file: {zipInfo.ZipFileName}");

            using (var zipArchive = ZipFile.Open(zipInfo.ZipFileName, ZipArchiveMode.Create))
            {
                foreach (var entry in entriesToZip)
                {
                    if (Directory.Exists(entry))
                    {
                        foreach (var file in Directory.GetFiles(entry, "*", SearchOption.AllDirectories))
                        {
                            string relativePath = GetRelativePath(baseDir, file);
                            zipArchive.CreateEntryFromFile(file, relativePath);
                            zipInfo.AddZippedItem(relativePath);

                            // Delete the file after adding it to the ZIP archive
                            File.Delete(file);
                        }

                        // Optionally, delete the directory itself if all files have been zipped
                        Directory.Delete(entry, true);
                    }
                    else
                    {
                        string relativePath = GetRelativePath(baseDir, entry);
                        zipArchive.CreateEntryFromFile(entry, relativePath);
                        zipInfo.AddZippedItem(relativePath);

                        // Delete the file after adding it to the ZIP archive
                        File.Delete(entry);
                    }
                }
            }

            logWriter.WriteLine($"ZIP file created: {zipInfo.ZipFileName}");
            logWriter.WriteLine("Zipped and deleted items:");
            foreach (var item in zipInfo.ZippedItems)
            {
                logWriter.WriteLine($"  - {item}");
            }
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
                                string relativePath = GetRelativePath(baseDir, file);
                                zipArchive.CreateEntryFromFile(file, relativePath);
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
            if (File.Exists(iniFilePath))
            {
                cmdOutput.WriteLine(1, $"config.ini is found!");
            }
            else
            {
                cmdOutput.WriteLine(1, $"config.ini not found! ");
                CreateDefaultIniFile();
                cmdOutput.WriteLine(1, $"A default one has been created!");
            }

            LoadAndFilterIniFile();
            var data = ParseIniFile();
            ConfigureLogSettings(data);

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


        private void LoadAndFilterIniFile()
        {
            var filteredLines = File.ReadLines(iniFilePath, Encoding.UTF8)
                .Where(line => !line.TrimStart().StartsWith("#"))
                .ToArray();
            File.WriteAllLines(tempIniFilePath, filteredLines, Encoding.UTF8);
        }

        public IniData ParseIniFile()
        {
            var parser = new FileIniDataParser();
            return parser.ReadFile(tempIniFilePath, Encoding.UTF8);
        }

        private void ConfigureLogSettings(IniData data)
        {
            var pathsSection = data["Paths"];
            var logSettings = data["LogSettings"];

            var optionsSection = data["Options"];
            N = int.Parse(optionsSection["N"]);
            M = int.Parse(optionsSection["M"]);
            //P = int.Parse(optionsSection["P"]);
            Q = int.Parse(optionsSection["Q"]);

            //  logPath = logSettings["OutputLogPath"];
            if (string.IsNullOrWhiteSpace(logPath) || !Directory.Exists(logPath))
            {
                cmdOutput.WriteLine(1, $"Processing log {logPath} not existing，going to create one...");
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                logPath = Path.Combine(exeDirectory, logPath);
                Directory.CreateDirectory(logPath);
                cmdOutput.WriteLine(1, $"Processing log  {logPath} has been created!");
            }
        }


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

        private void Rule002DeleteOldFoldersOrFiles(string pathKey, string baseDir, StreamWriter logWriter)
        {
            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule002: Deleting old folders or files in: {pathKey} => {baseDir} [Start]");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule002: Deleting old folders or files in: {pathKey} => {baseDir} [Start]");

            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logWriter.WriteLine($"Rule002: Invalid or non-existent directory: {baseDir}. Skipping deletion!");
                return;
            }

            // Calculate the total number of days to subtract (P days + Q months treated as Q * 30 days)
            int totalDays = P + (Q * 30);
            DateTime finalDeleteDate = DateTime.Now.AddDays(-totalDays);

            logWriter.WriteLine($"Rule002: Threshold date for deletion is {finalDeleteDate.ToString("yyyy-MM-dd")} ({P + 30 * Q} days ago)");
            cmdOutput.WriteLine(1, $"Rule002: Threshold date for deletion is {finalDeleteDate.ToString("yyyy-MM-dd")} ({P + 30 * Q} days ago)");

            var entries = Directory.GetFileSystemEntries(baseDir, "*", SearchOption.TopDirectoryOnly)
                .ToList();

            if (entries.Count > 0)
            {
                logWriter.WriteLine("Rule002: Checking the following folders or files for deletion:");
                cmdOutput.WriteLine(1, "Rule002: Checking the following folders or files for deletion:");
                foreach (var entry in entries)
                {
                    string entryName = Path.GetFileName(entry);
                    DateTime entryDate;

                    // Attempt to parse the date from the entry name (assuming the format is yyyy-MM-dd)
                    if (DateTime.TryParseExact(entryName.Substring(0, 10), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out entryDate))
                    {
                        logWriter.WriteLine($"  - {entry}");
                        //cmdOutput.WriteLine(1,$"  - {entry} (Parsed Date: {entryDate})");

                        if (entryDate < finalDeleteDate)
                        {
                            // Calculate how many days old the entry is
                            int daysOld = (DateTime.Now - entryDate).Days;

                            if (Directory.Exists(entry))
                            {
                                Directory.Delete(entry, true); // Delete directory and its contents
                                logWriter.WriteLine($"  - [Folder] {entry} deleted (Age: {daysOld} days)");
                                cmdOutput.WriteLine(1, $"  - [Folder] {entry} deleted (Age: {daysOld} days)");
                            }
                            else
                            {
                                File.Delete(entry); // Delete file
                                logWriter.WriteLine($"  - [File] {entry} deleted (Age: {daysOld} days)");
                                cmdOutput.WriteLine(1, $"  - [File] {entry} deleted (Age: {daysOld} days)");
                            }
                        }
                    }
                    else
                    {
                        logWriter.WriteLine($"  - {entry} (Skipped: Could not parse date)");
                        cmdOutput.WriteLine(1, $"  - {entry} (Skipped: Could not parse date)");
                    }
                }
            }
            else
            {
                logWriter.WriteLine($"Rule002: No old folders or files found for deletion in ({pathKey}).");
                cmdOutput.WriteLine(1, $"Rule002: No old folders or files found for deletion in ({pathKey}).");
            }

            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule002: Deleting old folders or files in: {pathKey} => {baseDir} [End]\n");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule002: Deleting old folders or files in: {pathKey} => {baseDir} [End]\n");
        }

        private void Rule002CompressLogFiles(string pathKey, string baseDir, StreamWriter logWriter)
        {
            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule002: Compressing log files in: {pathKey} => {baseDir} [Start]");
            cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule002: Compressing log files in: {pathKey} => {baseDir} [Start]");

            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logWriter.WriteLine($"Rule002: Invalid or non-existent directory: {baseDir}. Skipping compression!");
                return;
            }

            var (zipFiles, deletedItems) = Rule002CompressAndDeleteLogs(baseDir, logWriter);

            if (zipFiles.Count > 0)
            {
                logWriter.WriteLine($"Rule002: Successfully created the following ZIP files: {string.Join(", ", zipFiles)}");
                //cmdOutput.WriteLine(1,$"Rule002: Successfully created the following ZIP files: {string.Join(", ", zipFiles)}");
            }

            if (deletedItems.Count > 0)
            {
                logWriter.WriteLine("Rule002: Deleted the following files:");
                //cmdOutput.WriteLine(1,"Rule002: Deleted the following files:");
                foreach (var item in deletedItems)
                {
                    logWriter.WriteLine($"  - {item}");
                    //cmdOutput.WriteLine(1,$"  - {item}");
                }
            }
            else if (Directory.Exists(baseDir))
            {
                logWriter.WriteLine($"Rule002: No action required for ({pathKey}).");
                //cmdOutput.WriteLine(1,$"Rule002: No action required for ({pathKey}).");
            }
            else
            {
                logWriter.WriteLine($"Rule002: Invalid directory path: {baseDir}");
                //cmdOutput.WriteLine(1,$"Rule002: Invalid directory path: {baseDir}");
            }

            logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule002: Compressing log files in: {pathKey} => {baseDir} [End]\n");
            //cmdOutput.WriteLine(1,$"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Rule002: Compressing log files in: {pathKey} => {baseDir} [End]\n");
        }



        private string Rule002CreateUniqueZipFilePath(string baseDir, string dateString)
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

        public (List<string> zipFiles, List<string> deletedItems) Rule002CompressAndDeleteLogs(string baseDir, StreamWriter logWriter)
        {
            List<string> generatedZipFiles = new List<string>();
            List<string> deletedItems = new List<string>();

            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logWriter.WriteLine($"Invalid or non-existent directory: {baseDir}. Skipping processing!");
                return (generatedZipFiles, deletedItems);
            }

            // 設定日期變數
            DateTime currentDate = DateTime.Now;
            DateTime nDaysAgo = currentDate.AddDays(-N);
            DateTime pDaysAgo = currentDate.AddDays(-P);
            DateTime qMonthsAgo = currentDate.AddMonths(-Q);

            // 計算應刪除的日期
            DateTime deleteBeforeDate = pDaysAgo < qMonthsAgo ? pDaysAgo : qMonthsAgo;

            // 選取 N 天前的資料夾或檔案（命名規則為 yyyy-MM-dd）
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

            // 按 M 天的範圍進行壓縮
            for (int i = 0; i < entries.Count; i += M)
            {
                var entriesToZip = entries.Skip(i).Take(M).ToList();
                if (entriesToZip.Count > 0)
                {
                    string zipFilePath = CreateUniqueZipFilePath(baseDir, currentDate.ToString("yyyy-MM-dd"));

                    using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var entry in entriesToZip)
                        {
                            try
                            {
                                if (Directory.Exists(entry))
                                {
                                    // Add directory to ZIP
                                    foreach (var file in Directory.GetFiles(entry, "*", SearchOption.AllDirectories))
                                    {
                                        string relativePath = GetRelativePath(baseDir, file);
                                        zipArchive.CreateEntryFromFile(file, relativePath);
                                    }
                                    // Don't delete the folder itself
                                }
                                else
                                {
                                    zipArchive.CreateEntryFromFile(entry, Path.GetFileName(entry));
                                    File.Delete(entry); // Delete file after zipping
                                }
                                deletedItems.Add(entry);
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                cmdOutput.WriteLine(1, $"Error: Access to the path '{entry}' is denied.");
                                cmdOutput.WriteLine(1, ex.Message);
                                // Handle or log the exception as needed
                            }
                            catch (Exception ex)
                            {
                                cmdOutput.WriteLine(1, ex.Message);
                            }
                        }
                    }

                    logWriter.WriteLine($"Created ZIP file: {zipFilePath}");
                    generatedZipFiles.Add(Path.GetFileName(zipFilePath));
                }
            }

            // 刪除 35 天前的 ZIP 檔案
            var zipFiles = Directory.GetFiles(baseDir, "*.zip", SearchOption.TopDirectoryOnly)
                .Where(file => File.GetLastWriteTime(file) < deleteBeforeDate)
                .ToList();

            foreach (var zipFile in zipFiles)
            {
                File.Delete(zipFile);
                deletedItems.Add(zipFile);
                logWriter.WriteLine($"Deleted old ZIP file: {zipFile}");
            }

            return (generatedZipFiles, deletedItems);
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
                logWriter.WriteLine($"=== ZipLogTool(ver:{ver}): {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Start] ===");
                logWriter.WriteLine(new string('-', numDashes));

                logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Working environment [Start] ");
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
                logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Working environment [End] ");

                logWriter.WriteLine(new string('-', numDashes));
                cmdOutput.WriteLine(1, $"------------------------------------------");

                // Log the reading of the config file
                logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Read config.ini [Start] ");
                cmdOutput.WriteLine(1, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Read config.ini [Start] ");

                logWriter.WriteLine($"[Options]");
                logWriter.WriteLine($"  N={N} : Number of days before which log files will be compressed.");
                logWriter.WriteLine($"  M={M} : Number of days of log data to include in each ZIP file.");
                logWriter.WriteLine($"  Q={Q} : Number of {Q}*30={30 * Q} days before which folders or files will be deleted.");

                logWriter.WriteLine($"[Path]");
                foreach (var path in pathsSection)
                {
                    logWriter.WriteLine($"  {path.KeyName}={path.Value}");
                }

                logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Read config.ini [End]");
                logWriter.WriteLine(new string('-', numDashes));


                // Process each path: compress and delete logs, then delete old ZIP files
                foreach (var path in pathsSection)
                {
                    logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Processing path [{path.KeyName}]{path.Value} [Start]");

                    // First part: Compress log files based on N and M, and delete original logs
                    Rule003CompressLogFiles_Plan(path.KeyName, path.Value, logWriter);

                    Rule003CopyZipBack(path.Value, logWriter);

                    // Second part: Delete old ZIP files based on Q

                    //logWriter.WriteLine($"\n 先不刪 zip");

                    Rule003DeleteOldZipFiles(path.Value, Q, logWriter);

                    logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Processing path [{path.KeyName}]{path.Value} [End]\n");
                    logWriter.WriteLine(new string('-', numDashes));
                }

                logWriter.WriteLine($"=== ZipLogTool(ver:{ver})  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [End] ===\n\n");
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
