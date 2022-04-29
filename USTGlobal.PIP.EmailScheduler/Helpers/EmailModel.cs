using System.Collections.Generic;
using System.Net.Mail;

namespace USTGlobal.PIP.EmailScheduler.Helpers
{
    public class EmailModel
    {
        /// <summary>Initializes a new instance of the <see cref="EmailModel"/> class</summary>
        public EmailModel()
        {
            Cc = new List<string>();
            Bcc = new List<string>();
            Attachments = new List<Attachment>();
        }

        /// <summary>Gets or sets From</summary>
        /// <value>From</value>
        public string From { get; set; }

        /// <summary>Gets or sets the Display Name</summary>
        /// <value>Display Name</value>
        public string DisplayName { get; set; }

        /// <summary>Gets or sets To</summary>
        /// <value>To</value>
        public string To { get; set; }

        /// <summary>Gets or sets the Cc</summary>
        /// <value>Cc</value>
        public List<string> Cc { get; set; }

        /// <summary>Gets or sets the Bcc</summary>
        /// <value>Bcc</value>
        public List<string> Bcc { get; set; }

        /// <summary>Gets or sets the Subject</summary>
        /// <value>Subject</value>
        public string Subject { get; set; }

        /// <summary>Gets or sets the Body</summary>
        /// <value>Body</value>
        public string Body { get; set; }

        /// <summary>Gets or sets the Attachments</summary>
        /// <value>Attachments</value>
        public List<Attachment> Attachments { get; set; }

        /// <summary>Gets or sets a value indicating whether this email body is HTML</summary>
        /// <value> <c>true</c> if email body is HTML; otherwise, <c>false</c> </value>
        public bool IsBodyHTML { get; set; } = true;

        /// <summary>Gets or sets the Mail Priority</summary>
        /// <value> <c>Low</c> <c>Normal</c> <c>High</c> </value>
        public MailPriority MailPriority { get; set; } = MailPriority.Normal;
    }
}
