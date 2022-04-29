using System.Collections;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class MasterDTO
    {
        public int MasterId { get; set; }
        public string MasterName { get; set; }
        public IList Data { get; set; }
    }
}
