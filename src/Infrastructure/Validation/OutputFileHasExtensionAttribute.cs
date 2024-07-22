using System.ComponentModel.DataAnnotations;

namespace FFCmd.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class OutputFileHasExtensionAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string filePath
            && validationContext.Items.TryGetValue(nameof(BaseFFMpegSettings.OutputExtension), out object? extensionObject)
            && extensionObject is string extension)
        {
            var currentExtension = Path.GetExtension(filePath);
            if (currentExtension.Equals(extension, StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"Output file must have an extension: {extension}");
            }
        }
        throw new InvalidOperationException("Property must be string");
    }
}