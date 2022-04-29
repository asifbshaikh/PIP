using System;

namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public partial class Holiday : BaseEntity
    {
        public int HolidayId { get; set; }
        public string HolidayName { get; set; }
        public DateTime Date { get; set; }
        public int LocationId { get; set; }
        public int MasterVersionId { get; set; }
    }
}
