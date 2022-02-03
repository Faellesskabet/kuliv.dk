using Dikubot.DataLayer.Static;
using FluentValidation;

namespace Dikubot.DataLayer.Logic.Validators;

public class KUEmailValidation : AbstractValidator<string>
{
    public KUEmailValidation()
    {
        RuleFor(s => s).EmailAddress().WithMessage("Ikke en gyldig email");
        RuleFor(s => s).Must(Util.IsKUEmail).WithMessage("Ikke en gyldig KU email");
    }
}