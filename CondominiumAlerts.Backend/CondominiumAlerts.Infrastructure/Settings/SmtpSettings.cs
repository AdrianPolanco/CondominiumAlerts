namespace CondominiumAlerts.Infrastructure.Settings;

public class SmtpSettings
{
    public string Sender { get; set; }
    public string SenderName { get; set; }
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}