﻿using System;
namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class LocationDTO
    {
        public int SerialId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public decimal? HoursPerDay { get; set; }
        public decimal? HoursPerMonth { get; set; }
        public int MasterVersionId { get; set; }
        public int? CountryId { get; set; }
        public decimal? RefUSD { get; set; }
        public decimal? EbitdaSeatCost { get; set; }
        public decimal? InflationRate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Comments { get; set; }
        public int Status { get; set; }
    }
}
