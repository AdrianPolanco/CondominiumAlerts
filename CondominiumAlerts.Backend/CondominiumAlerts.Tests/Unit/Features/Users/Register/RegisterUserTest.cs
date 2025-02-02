using CondominiumAlerts.Features.Commands;

namespace CondominiumAlerts.Tests.Unit.Features.Users.Register;

public class RegisterUserTest
{
    [Fact]
    public async Task Test_RegisterUser_Validation()
    {
        var validator = new RegisterUserValidator();
        //var user = new RegisterUserCommand("pr", "prueba21.com", "Prueba123");
        //var result = await validator.ValidateAsync(user);
  
    }
}