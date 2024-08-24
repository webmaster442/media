// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class NotEmptyOrWiteSpaceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return new ValidationResult("Value cannot be empty or whitespace");
            else
                return ValidationResult.Success;
        }
        throw new InvalidOperationException("Property must be string");
    }
}
