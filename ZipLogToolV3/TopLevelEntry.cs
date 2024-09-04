using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipLogTool
{
    public class TopLevelEntry
    {
        public string EntryPath { get; set; }  // 檔案或資料夾的路徑
        public bool IsDirectory { get; set; }  // 是否是資料夾
        public string ZipFileName { get; set; }  // 對應的壓縮檔案名稱
    }
}
