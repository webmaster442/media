// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class FileExistsAttribute : ValidationAttribute
{
    public bool IsOptional { get; set; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) && IsOptional)
                return ValidationResult.Success;

            if (File.Exists(filePath))
                return ValidationResult.Success;
            else
                return new ValidationResult($"File does not exist: {filePath}");
        }
        throw new InvalidOperationException("Property must be string");
    }
}
