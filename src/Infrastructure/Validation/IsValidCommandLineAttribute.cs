// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class IsValidCommandLineAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string commandLine)
        {
            if (!commandLine.Contains("{input}"))
                return new ValidationResult("Command line must contain {input} placeholder");

            if (!commandLine.Contains("{output}"))
                return new ValidationResult("Command line must contain {output} placeholder");

            return ValidationResult.Success;
        }
        throw new InvalidOperationException("Property must be string");
    }
}
