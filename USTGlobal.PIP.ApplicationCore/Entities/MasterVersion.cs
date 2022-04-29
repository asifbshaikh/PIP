using System;

namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public partial class MasterVersion : BaseEntity
    {
        public int MasterVersionId { get; set; }
        public int MasterId { get; set; }
        public string VersionName { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
