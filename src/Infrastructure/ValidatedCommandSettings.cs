// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure.Validation;

namespace Media.Infrastructure;

internal abstract class ValidatedCommandSettings : CommandSettings
{
    public override Spectre.Console.ValidationResult Validate()
    {
        if (ObjectValidator.TryValidate(this, out string errors))
        {
            return Spectre.Console.ValidationResult.Success();
        }
        return Spectre.Console.ValidationResult.Error(errors);
    }
}
