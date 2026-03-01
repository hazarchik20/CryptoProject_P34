using CryptoProj.Domain.Services.Users;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CryptoProj.API.Validators
{
    public class UserRequestValidator{
        public static bool ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return Regex.IsMatch(name, @"^[a-zA-Z0-9_]+$");
        }

        public static bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;


            if (password.Length <= 8)
                return false;

            if (!password.Any(char.IsUpper))
                return false;

            if (!password.Any(char.IsLower))
                return false;

            if (!password.Any(c => "!;%@*".Contains(c)))
                return false;

            if (!password.Any(c => c >= '\u2648' && c <= '\u2653'))
                return false;

            var romanPattern = @"\b(M{0,4}(CM|CD|D?C{0,3})
                              (XC|XL|L?X{0,3})
                              (IX|IV|V?I{0,3}))\b";

            if (!Regex.IsMatch(password, romanPattern, RegexOptions.IgnorePatternWhitespace))
                return false;

            return true;
        }



    }
}
