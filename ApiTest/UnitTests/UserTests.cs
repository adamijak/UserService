using Api;
using Api.Validators;

namespace ApiTest.UnitTests;

[TestClass]
public class UserTests
{
    [TestMethod]
    [DataRow(true, "david@email.com")]
    [DataRow(true,"david.david@email.com")]
    [DataRow(true,"a@a.a")]
    [DataRow(false, null)]
    [DataRow(false, "@email.com")]
    public async Task NegativeEmailValidation(bool valid, string email)
    {
        var user = new Api.UserDto { Email = email };
        
        var validator = new UserDtoValidator();
        var result = await validator.ValidateAsync(user);
        
        Assert.AreEqual(!valid, result.Errors.Any(e => e.PropertyName == "Email"));
    }
}