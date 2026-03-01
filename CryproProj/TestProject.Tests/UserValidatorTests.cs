using CryptoProj.API.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Tests
{
    public class UserValidatorTests
    {

        // -------- ValidateName ------

        [Fact]
        public void ValidateName_ValidName_ReturnsTrue()
        {
            var result = UserRequestValidator.ValidateName("User_123");

            Assert.True(result);
        }

        [Theory]
        [InlineData("John")]
        [InlineData("user_1")]
        [InlineData("A1B2C3")]
        [InlineData("Test_User")]
        public void ValidateName_ValidNames_ReturnsTrue(string name)
        {
            var result = UserRequestValidator.ValidateName(name);

            Assert.True(result);
        }

        [Theory]
        [InlineData("John Doe")]
        [InlineData("User-123")]
        [InlineData("User@123")]
        [InlineData("!User")]
        [InlineData("")]
        public void ValidateName_InvalidNames_ReturnsFalse(string name)
        {
            var result = UserRequestValidator.ValidateName(name);

            Assert.False(result);
        }




        // ------ ValidatePassword ---------

        [Fact]
        public void ValidatePassword_ValidPassword_ReturnsTrue()
        {
            var password = "StrongPass@XI♈";

            var result = UserRequestValidator.ValidatePassword(password);

            Assert.True(result);
        }

        [Theory]
        [InlineData("Aa12345!XI♈")]
        [InlineData("Password@IV♉")]
        [InlineData("GoodPass%M♓")]
        public void ValidatePassword_ValidPasswords_ReturnsTrue(string password)
        {
            var result = UserRequestValidator.ValidatePassword(password);

            Assert.True(result);
        }

        [Theory]
        [InlineData("Short1!I♈")]          
        [InlineData("nouppercase@XI♈")]    
        [InlineData("NOLOWERCASE@XI♈")]    
        [InlineData("NoSpecialXI♈")]     
        [InlineData("NoZodiac@XI")]      
        [InlineData("NoRoman@♈")]          
        public void ValidatePassword_InvalidPasswords_ReturnsFalse(string password)
        {
            var result = UserRequestValidator.ValidatePassword(password);

            Assert.False(result);
        }


    }
}
