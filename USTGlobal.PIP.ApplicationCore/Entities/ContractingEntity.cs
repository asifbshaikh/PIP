namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class ContractingEntity : BaseEntity
    {
        public int ContractingEntityId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int MasterVersionId { get; set; }
    }
}
