using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class TotalDealFinancialsDTO
    {
        public int DescriptionId { get; set; }
        public int RowSectionId { get; set; }
        public decimal TotalUSD { get; set; }
        public decimal TotalLocal { get; set; }
        public IList<TotalDealFinancialsYearDTO> TotalDealFinancialsYearList { get; set; }
    }
}
