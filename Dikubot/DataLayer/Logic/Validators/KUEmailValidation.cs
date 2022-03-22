using System;
using System.Collections.Generic;
using System.Linq;
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
    
    private IEnumerable<string> ValidateValue(string arg)
    {
        var result = Validate(arg);
        if (result.IsValid)
            return new string[0];
        return result.Errors.Select(e => e.ErrorMessage);
    }

    public Func<string, IEnumerable<string>> Validation => ValidateValue;
}