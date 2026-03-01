using CryptoProj.Domain.Models.Requests;
using FluentValidation;

namespace CryptoProj.API.Validators;

public class CryptocurrencyRequestValidator : AbstractValidator<CryptocurrencyRequest>
{
    public CryptocurrencyRequestValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Limit must be between 1 and 100");
        
        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Offset must be greater than or equal to 1");
        
        RuleFor(x => x.Symbol)
            .Matches(@"^[A-Z]{3}$")
            .WithMessage("");
    }
}