using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class AccountDTO
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public decimal PaymentLag { get; set; }
    }
}
