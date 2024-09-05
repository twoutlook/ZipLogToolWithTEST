using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipLogToolNet8
{
    public class LogMsgHelper
    {
        public LogMsgHelper()
        {
        }
        string tag;
        public LogMsgHelper(string tag)
        {
            this.tag = tag;
        }
        public string msg(string tag, string message)
        {
            return $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFFFF")}|{tag}|{message}";
        }
        public string msg( string message)
        {
            return $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}|{tag}|{message}";
        }
    }
}
