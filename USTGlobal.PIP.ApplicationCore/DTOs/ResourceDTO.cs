namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ResourceDTO
    {
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public int ResourceGroupId { get; set; }
        public string Grade { get; set; }
        public string OldName { get; set; }
        public int ResourceServiceLineId { get; set; }
        public int MasterVersionId { get; set; }
    }
}
