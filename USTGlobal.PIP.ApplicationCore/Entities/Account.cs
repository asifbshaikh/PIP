namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class Account : BaseEntity
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public decimal PaymentLag { get; set; }
    }
}
