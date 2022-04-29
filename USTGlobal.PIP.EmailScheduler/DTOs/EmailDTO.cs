using System.Collections.Generic;

namespace USTGlobal.PIP.EmailScheduler.DTOs
{
    public class EmailDTO
    {
        public int EmailNotificationId { get; set; }
        public PlaceHolderDTO TemplateData { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Link { get; set; }
        public List<int> Versions { get; set; }
        public List<string> Roles { get; set; }
        public string htmlString { get; set; }
        public List<PipSheetCommentDTO> pipSheetCommentDTO { get; set; }
    }
}
