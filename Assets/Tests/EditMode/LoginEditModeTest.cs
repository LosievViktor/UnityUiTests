using NUnit.Framework;

public class LoginEditModeTests
{
    private const string Username = "Viktor";
    private const string Password = "password";

    private const string WrongUsername = "John";
    private const string WrongPassword = "bla-bla-bla";

    [Test]
    public void AuthService_CorrectCredentials()
    {
        Assert.IsTrue(AuthService.IsValid(Username, Password),
            $"Correct credentials '{Username}' / '{Password}' must be accepted.");
    }

    [Test]
    public void AuthService_WrongUsername()
    {
        Assert.IsFalse(AuthService.IsValid(WrongUsername, Password),
            $"Unknown username '{WrongUsername}' must be rejected even with correct password '{Password}'.");
    }

    [Test]
    public void AuthService_WrongPassword()
    {
        Assert.IsFalse(AuthService.IsValid(Username, WrongPassword),
            $"Wrong password '{WrongPassword}' must be rejected even for correct username '{Username}'.");
    }

    [Test]
    public void AuthService_WrongCredentials()
    {
        Assert.IsFalse(AuthService.IsValid(WrongUsername, WrongPassword),
            $"Incorrect credentials '{WrongUsername}' / '{WrongPassword}' must be rejected.");
    }
}
