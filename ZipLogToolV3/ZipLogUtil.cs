using System;
using System.IO;
using System.IO.Compression;
using IniParser.Model;
using System.Text;

namespace ZipLogTool
{
    public class ZipLogUtil
    {
        public void UnzipFiles(string ver, IniData data, string logFilePath)
        {
            using (StreamWriter logWriter = new StreamWriter(logFilePath, true, Encoding.UTF8))
            {
                var pathsSection = data["Paths"];
                logWriter.WriteLine($"\n\n\n=== ZipLogTool(ver:{ver}): {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Start Unzipping] ===");

                foreach (var path in pathsSection)
                {
                    string baseDir = path.Value;

                    if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
                    {
                        logWriter.WriteLine($"Invalid or non-existent directory: {baseDir}. Skipping processing!");
                        continue;
                    }

                    logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Processing: {path.KeyName} => {path.Value} [Start Unzipping]");

                    var zipFiles = Directory.GetFiles(baseDir, "*.zip", SearchOption.TopDirectoryOnly);

                    foreach (var zipFile in zipFiles)
                    {
                        try
                        {
                            string extractPath = Path.Combine(baseDir, Path.GetFileNameWithoutExtension(zipFile));
                            if (Directory.Exists(extractPath))
                            {
                                logWriter.WriteLine($"Directory {extractPath} already exists. Skipping unzip for {zipFile}.");
                                continue;
                            }

                            ZipFile.ExtractToDirectory(zipFile, extractPath);
                            logWriter.WriteLine($"Successfully unzipped {zipFile} to {extractPath}");

                            // Delete the ZIP file after successful extraction
                            File.Delete(zipFile);
                            logWriter.WriteLine($"Deleted ZIP file: {zipFile}");
                        }
                        catch (Exception ex)
                        {
                            logWriter.WriteLine($"Failed to unzip {zipFile}: {ex.Message}");
                        }
                    }

                    logWriter.WriteLine($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Processing: {path.KeyName} => {path.Value} [End Unzipping]\n");
                }

                logWriter.WriteLine($"=== ZipLogTool(ver:{ver})  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [End Unzipping] ===\n\n");
            }
        }

    }
}
