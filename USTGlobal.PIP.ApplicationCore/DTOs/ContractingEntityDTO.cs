namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ContractingEntityDTO : BaseDTO
    {
        public int ContractingEntityId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int MasterVersionId { get; set; }
    }
}
