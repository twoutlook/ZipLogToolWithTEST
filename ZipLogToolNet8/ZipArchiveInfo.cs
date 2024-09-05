using System;
using System.Collections.Generic;
using System.IO;

namespace ZipLogTool
{
  

    public class ZipArchiveInfo
    {
        public string ZipFileName { get; private set; }
        public List<string> ZippedItems { get; private set; }
        public string BaseDir { get; private set; }
        public string TempZipBaseDir { get; private set; }

        public ZipArchiveInfo(string baseDir, DateTime fromDate, DateTime toDate, int suffix = 0)
        {
            BaseDir = baseDir;
            ZipFileName = GenerateZipFileName(fromDate, toDate, suffix);
            ZippedItems = new List<string>();
        }
        public ZipArchiveInfo(string baseDir, string tempZipBaseDir, DateTime fromDate, DateTime toDate, int suffix = 0)
        {
            BaseDir = baseDir;
            TempZipBaseDir = tempZipBaseDir;
            ZipFileName = GenerateZipFileName(fromDate, toDate, suffix);
            ZippedItems = new List<string>();
        }

        // Method to add a file or folder to the list of zipped items
        public void AddZippedItem(string item)
        {
            ZippedItems.Add(item);
        }

        // Method to generate the zip file name based on the given date range
        private string GenerateZipFileName(DateTime fromDate, DateTime toDate, int suffix)
        {
            string fromDateString = fromDate.ToString("yyyy-MM-dd");
            string toDateString = toDate.ToString("yyyy-MM-dd");
            string zipFileName = suffix > 0 ? $"{fromDateString}_{toDateString}_{suffix:D2}.zip" : $"{fromDateString}_{toDateString}.zip";
            return Path.Combine(BaseDir, zipFileName);
        }

        // Method to check if a ZIP file with the same name already exists and adjust the name if necessary
        public void EnsureUniqueFileName()
        {
            int suffix = 1;
            while (File.Exists(ZipFileName))
            {
                ZipFileName = GenerateZipFileName(ExtractDateFromFileName(ZipFileName, true), ExtractDateFromFileName(ZipFileName, false), suffix);
                suffix++;
                if (suffix > 99)
                {
                    throw new InvalidOperationException("Too many ZIP files with the same date range.");
                }
            }
        }

        // Method to extract the date from the filename (helper method)
        private DateTime ExtractDateFromFileName(string zipFileName, bool isFromDate)
        {
            string fileName = Path.GetFileNameWithoutExtension(zipFileName);
            string[] dateParts = fileName.Split('_');
            return DateTime.ParseExact(isFromDate ? dateParts[0] : dateParts[1], "yyyy-MM-dd", null);
        }
    }

}
