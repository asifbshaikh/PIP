using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class Project:BaseEntity
    {
        public int ProjectId { get; set; }
        public string SFProjectId { get; set; }
        public string ProjectName { get; set; }
        public int AccountId { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public List<PipSheet> PipSheet { get; set; }
    }
}
