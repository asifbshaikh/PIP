using System;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class HolidayDTO : BaseDTO
    {
        public int Id { get; set; }
        public string HolidayName { get; set; }
        public DateTime Date { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int MasterVersionId { get; set; }
    }
}
