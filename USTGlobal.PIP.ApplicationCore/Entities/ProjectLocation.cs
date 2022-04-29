namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class ProjectLocation : BaseEntity
    {
        public int LocationId { get; set; }
        public int PIPSheetId { get; set; }
        public decimal? HoursPerDay { get; set; }
        public decimal? HoursPerMonth { get; set; }

        public decimal? OverheadAmount { get; set; }
        public decimal? SharedSeatsUsePercent { get; set; }

    }
}
