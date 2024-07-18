using System.ComponentModel.DataAnnotations;

namespace FFCmd.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal class FileExistsValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string filePath)
        {
            if (File.Exists(filePath))
                return ValidationResult.Success;
            else
                return new ValidationResult($"File does not exist: {filePath}");
        }
        throw new InvalidOperationException("Property must be string");
    }
}
