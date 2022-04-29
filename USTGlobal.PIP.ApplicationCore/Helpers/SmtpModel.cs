namespace USTGlobal.PIP.ApplicationCore.Helpers
{
    public class SmtpModel
    {
        /// <summary>Initializes a new instance of the <see cref="SmtpModel"/> class</summary>
        public SmtpModel() { }

        /// <summary>Initializes a new instance of the <see cref="SmtpModel"/> class</summary>
        /// <param name="smtpAddress">SMTP Address</param>
        /// <param name="portNumber">Port Number</param>
        public SmtpModel(string smtpAddress, int portNumber)
        {
            SmtpAddress = smtpAddress;
            PortNumber = portNumber;
        }

        /// <summary>Initializes a new instance of the <see cref="SmtpModel"/> class</summary>
        /// <param name="smtpAddress">SMTP Address</param>
        /// <param name="portNumber">Port Number</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public SmtpModel(string smtpAddress, int portNumber, string username, string password)
        {
            SmtpAddress = smtpAddress;
            PortNumber = portNumber;
            Username = username;
            Password = password;
        }

        /// <summary>Gets or sets the SMTP Address</summary>
        /// <value>SMTP Address</value>
        public string SmtpAddress { get; set; }

        /// <summary>Gets or sets the Port Number</summary>
        /// <value>Port Number</value>
        public int PortNumber { get; set; }

        /// <summary>Gets or sets the Username</summary>
        /// <value>Username</value>
        public string Username { get; set; }

        /// <summary>Gets or sets the Password</summary>
        /// <value>Password</value>
        public string Password { get; set; }

        /// <summary>Gets or sets a value indicating whether [enable SSL]</summary>
        /// <value> <c>true</c> if [enable SSL]; otherwise, <c>false</c> </value>
        public bool EnableSSL { get; set; } = false;
    }
}
