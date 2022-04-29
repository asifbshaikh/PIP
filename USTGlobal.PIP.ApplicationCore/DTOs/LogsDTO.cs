using System;
using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class LogsDTO
    {
        public string Message { get; set; }
        public List<dynamic> Additional { get; set; }
        public int Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string FileName { get; set; }
        public string LineNummber { get; set; }

    }
}
