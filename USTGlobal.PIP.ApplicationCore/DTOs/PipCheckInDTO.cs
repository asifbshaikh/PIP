namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PipCheckInDTO
    {
        public int PIPSheetId { get; set; }
        public bool IsCheckedOut { get; set; }
        public string CheckedInOutBy { get; set; }
    }
}
