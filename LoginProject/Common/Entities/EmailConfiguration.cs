namespace LoginProject.Common.Entities;

public class EmailConfiguration
{
    public string From { get; set; }
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}