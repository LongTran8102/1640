namespace Project_1640.Models
{
    public class Email
    {
        public string From { get; set; }
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string Password { get; set; }
    }

    public class MailSettings
    {
        public string Mail { get; set; }
        public string Password { get; set; }
    }
}
