using System;
using System.IO;
using System.Linq;
using System.Management;

namespace ZipLogTool
{
    public class SystemInfo
    {
        public static void DisplaySystemSpecs()
        {
            try
            {
                // Get CPU Information
                var cpuName = GetCPUName();
                Console.WriteLine($"CPU: {cpuName}");

                // Get RAM Information
                var totalMemory = GetTotalRAM();
                Console.WriteLine($"Total RAM: {totalMemory / 1024 / 1024} MB");

                // Get Disk Information
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    if (drive.IsReady)
                    {
                        Console.WriteLine($"Drive {drive.Name} - Available space: {drive.AvailableFreeSpace / 1024 / 1024} MB, Total space: {drive.TotalSize / 1024 / 1024} MB");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving system specs: {ex.Message}");
            }
        }

        private static string GetCPUName()
        {
            string cpuName = string.Empty;
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_Processor");
                foreach (ManagementObject mo in mos.Get())
                {
                    cpuName = mo["Name"].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CPU details: {ex.Message}");
            }
            return cpuName;
        }

        private static ulong GetTotalRAM()
        {
            ulong totalMemory = 0;
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
                foreach (ManagementObject mo in mos.Get())
                {
                    totalMemory = (ulong)mo["TotalPhysicalMemory"];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching RAM details: {ex.Message}");
            }
            return totalMemory;
        }
    }
}
