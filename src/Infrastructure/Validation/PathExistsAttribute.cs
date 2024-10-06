// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class PathExistsAttribute : ValidationAttribute
{
    public bool AllowEmpty { get; set; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string filePath)
        {
            if (Directory.Exists(filePath)
                || File.Exists(filePath) 
                || (AllowEmpty && string.IsNullOrEmpty(filePath)))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"Directory does not exist: {filePath}");
            }
        }
        throw new InvalidOperationException("Property must be string");
    }
}
