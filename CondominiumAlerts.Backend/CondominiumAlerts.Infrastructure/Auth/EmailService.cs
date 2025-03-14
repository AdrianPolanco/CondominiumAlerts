
using CondominiumAlerts.Infrastructure.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Polly;
using Polly.Retry;


namespace CondominiumAlerts.Infrastructure.Auth.Interfaces;

public class EmailService: IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly IAsyncPolicy _retryPolicy;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> smtpSettings, IAsyncPolicy retryPolicy, ILogger<EmailService> logger)
    {
        _smtpSettings = smtpSettings.Value;
        _retryPolicy = retryPolicy;
        _logger = logger;
    }
    
    public async Task SendConfirmationEmailAsync(string name, string lastname, string email, string confirmationLink, CancellationToken cancellationToken)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.Sender));
        mimeMessage.To.Add(new MailboxAddress($"{name} {lastname}", email));
        mimeMessage.Subject = "Confirma tu cuenta de Condominiums Alerts.";

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = string.Format(
            await GetEmailTemplateAsync(), 
            name, 
            lastname,
            confirmationLink
        );

        mimeMessage.Body = bodyBuilder.ToMessageBody();

        // Usar la política de reintentos para el envío del correo
        await _retryPolicy.ExecuteAsync(async () =>
        {
            using SmtpClient client = new();

            await client.ConnectAsync(_smtpSettings.SmtpServer, _smtpSettings.Port, true);
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await client.SendAsync(mimeMessage, cancellationToken);
            await client.DisconnectAsync(true);
        });
        
    }

    private async Task<string> GetEmailTemplateAsync()
    {
        return @"
            <!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Confirmación de Cuenta</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;"">
    <table width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"">
        <tr>
            <td align=""center"" style=""padding: 20px;"">
                <table width=""600"" style=""background-color: #ffffff; padding: 20px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);"">
                    <tr>
                        <td align=""center"" style=""padding-bottom: 20px;"">
                            <img src=""https://res.cloudinary.com/dmeixgsxo/image/upload/v1738627118/CondominiumAlertsLogo_kqozqe.jpg"" alt=""Condominiums Alerts"" style=""max-width: 150px; margin-bottom: 20px;"">
                            <h1 style=""color: #333;"">Bienvenido a Condominiums Alerts</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding: 10px 20px; font-size: 16px; color: #555;"">
                            <p>Hola <strong>{0} {1}</strong>,</p>
                            <p>Gracias por registrarte en <strong>Condominiums Alerts</strong>. Para completar tu registro, por favor confirma tu cuenta haciendo clic en el botón a continuación:</p>
                        </td>
                    </tr>
                    <tr>
                        <td align=""center"" style=""padding: 20px;"">
                            <a href=""{2}"" style=""background-color: #007bff; color: #ffffff; padding: 12px 24px; text-decoration: none; font-size: 18px; border-radius: 5px;"">Confirmar Cuenta</a>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding: 10px 20px; font-size: 14px; color: #777; text-align: center;"">
                            <p>Si no solicitaste esta cuenta, simplemente ignora este mensaje.</p>
                            <p>&copy; 2024 Condominiums Alerts. Todos los derechos reservados.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>

";
    }
}