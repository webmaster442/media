using System.Reflection;

namespace Media.Infrastructure.Validation;

internal static class ObjectValidator
{
    public static bool TryValidate(object obj, out string errors)
    {
        var validationResults = new List<ValidationResult>();

        var validationContext = new ValidationContext(instance: obj,
                                                      serviceProvider: null,
                                                      items: GetPropertyValues(obj));

        bool isValid = Validator.TryValidateObject(instance: obj,
                                                   validationContext: validationContext,
                                                   validationResults: validationResults,
                                                   validateAllProperties: true);

        if (!isValid)
        {
            var sb = new StringBuilder();
            foreach (var validationResult in validationResults)
            {
                sb.AppendLine(validationResult.ErrorMessage);
            }

            errors = sb.ToString();
            return false;
        }

        errors = string.Empty;
        return true;
    }

    private static Dictionary<object, object?>? GetPropertyValues(object obj)
    {
        Dictionary<object, object?> propertyValues = new();
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            propertyValues[property.Name] = property.GetValue(obj);
        }
        return propertyValues;
    }
}
