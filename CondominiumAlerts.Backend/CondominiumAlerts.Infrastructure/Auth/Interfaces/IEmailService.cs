namespace CondominiumAlerts.Infrastructure.Auth.Interfaces;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string name, string lastname, string email, string confirmationLink, CancellationToken cancallCancellationToken);
}