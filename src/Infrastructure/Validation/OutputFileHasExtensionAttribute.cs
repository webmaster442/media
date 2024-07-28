namespace Media.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class OutputFileHasExtensionAttribute : ValidationAttribute
{
    public OutputFileHasExtensionAttribute(string propertyName)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string filePath
            && validationContext.Items.TryGetValue(PropertyName, out object? extensionObject)
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