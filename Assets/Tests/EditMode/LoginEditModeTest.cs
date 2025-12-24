using NUnit.Framework;
    public class LoginEditModeTests
    {
        [Test]
        public void AuthService_ValidCredentials_ReturnsTrue()
        {
            Assert.IsTrue(AuthService.IsValid("Viktor", "password"));
        }

        [Test]
        public void AuthService_InvalidCredentials_ReturnsFalse()
        {
            Assert.IsFalse(AuthService.IsValid("Viktor", "123"));
            Assert.IsFalse(AuthService.IsValid("John", "password"));
        }
    }