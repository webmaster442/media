namespace FFCmd.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class SameExtensionAsAttribute : ValidationAttribute
{
    public SameExtensionAsAttribute(string otherPropertyName)
    {
        OtherPropertyName = otherPropertyName;
    }

    public string OtherPropertyName { get; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string filePath)
        {
            if (validationContext.Items.TryGetValue(OtherPropertyName, out object? otherValue)
                && otherValue is string otherFilePath)
            {
                if (Path.GetExtension(filePath) == Path.GetExtension(otherFilePath))
                    return ValidationResult.Success;
                else
                    return new ValidationResult($"File extension must be the same as {OtherPropertyName}");
            }
            throw new InvalidOperationException($"Property {OtherPropertyName} not found");
        }
        throw new InvalidOperationException("Property must be string");
    }

}