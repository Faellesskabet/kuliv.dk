using System;
using System.Collections.Generic;
using System.Linq;
using Dikubot.DataLayer.Static;
using FluentValidation;
using FluentValidation.Results;

namespace Dikubot.DataLayer.Logic.Validators;

public class KUEmailValidation : AbstractValidator<string>
{
    public KUEmailValidation()
    {
        RuleFor(s => s).EmailAddress().WithMessage("Ikke en gyldig email");
        RuleFor(s => s).Must(Util.IsKUEmail).WithMessage("Ikke en gyldig KU email");
    }

    public Func<string, IEnumerable<string>> Validation => ValidateValue;

    private IEnumerable<string> ValidateValue(string arg)
    {
        ValidationResult result = Validate(arg);
        return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
    }
}