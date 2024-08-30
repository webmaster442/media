// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class FileHasExtensionAttribute : ValidationAttribute
{
    private readonly HashSet<string> _extensions;

    public FileHasExtensionAttribute(params string[] extensions)
    {
        _extensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (_extensions.Contains(extension))
                return ValidationResult.Success;
            else
                return new ValidationResult($"File has invalid extension: {filePath}");
        }
        throw new InvalidOperationException("Property must be string");
    }
}