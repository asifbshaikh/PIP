using System;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class MasterVersionDTO
    {
        public int MasterVersionId { get; set; }
        public int MasterId { get; set; }
        public string VersionName { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
