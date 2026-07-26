namespace Pizzeria.Tests.Integration.Infrastructure;

internal static class UserHelper
{
    public static (string Username, string Password, string Email) GenerateTestUserCredentials()
    {
        var username = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";
        var email = $"{username}@example.com";

        return (username, password, email);
    }
}
