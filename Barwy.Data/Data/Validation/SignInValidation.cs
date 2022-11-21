using Barwy.Data.Data.ViewModels.Account;
using FluentValidation;

namespace Barwy.Data.Data.Validation
{
    public class SignInValidation : AbstractValidator<SignInVM>
    {
        public SignInValidation()
        {
            RuleFor(r => r.Login).NotEmpty();
            RuleFor(r => r.Password).NotEmpty().MinimumLength(6)
                .Matches("[A-Z]").WithMessage("Password must contain one or more capital letters.")
                .Matches("[a-z]").WithMessage("Password must contain one or more lowercase letters.")
                .Matches(@"\d").WithMessage("Password must contain one or more digits.")
                .Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("Password must contain one or more special characters.")
                .Matches("^[^£# “”]*$").WithMessage("Password must not contain the following characters £ # “” or spaces.")
                .WithMessage("Password contains a word that is not allowed.");
        }
    }
}
