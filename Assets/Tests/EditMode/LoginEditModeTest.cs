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
            Assert.IsTrue(AuthService.IsValid(Username,Password));
        }

        [Test]
        public void AuthService_WrongUsername()
        {
            Assert.IsFalse(AuthService.IsValid(WrongUsername,Password));
        }
        
        [Test]
        public void AuthService_WrongPassword()
        {
            Assert.IsFalse(AuthService.IsValid(Username,WrongPassword));
        }
        
        [Test]
        public void AuthService_WrongCredentials()
        {    
            Assert.IsFalse(AuthService.IsValid(WrongUsername,WrongPassword));
        }
    }