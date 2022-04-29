namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public partial class Markup
    {
        public int MarkupId { get; set; }
        public string Name { get; set; }
        public decimal Percent { get; set; }
        public int MasterVersionId { get; set; }
    }
}
