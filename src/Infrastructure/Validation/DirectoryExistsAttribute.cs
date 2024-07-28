namespace FFCmd.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class DirectoryExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string filePath)
        {
            if (Directory.Exists(filePath))
                return ValidationResult.Success;
            else
                return new ValidationResult($"Directory does not exist: {filePath}");
        }
        throw new InvalidOperationException("Property must be string");
    }
}
