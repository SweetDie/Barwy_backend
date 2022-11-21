using Barwy.Data.Data.ViewModels.Account;
using FluentValidation;

namespace Barwy.Data.Data.Validation
{
    public class TokenRequestValidation : AbstractValidator<TokenRequestVM>
    {
        public TokenRequestValidation()
        {
            RuleFor(r => r.Token).NotEmpty();
            RuleFor(r => r.RefreshToken).NotEmpty();
        }
    }
}
