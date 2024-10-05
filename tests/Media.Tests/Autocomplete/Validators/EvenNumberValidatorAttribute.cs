using Spectre.Console;

namespace Media.Tests.Autocomplete.Validators;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class EvenNumberValidatorAttribute : ParameterValidationAttribute
{
    public EvenNumberValidatorAttribute(string errorMessage)
        : base(errorMessage)
    {
    }

    public override ValidationResult Validate(CommandParameterContext context)
    {
        if (context.Value is int integer)
        {
            if (integer % 2 == 0)
            {
                return ValidationResult.Success();
            }

            return ValidationResult.Error($"Number is not even ({context.Parameter.PropertyName}).");
        }

        throw new InvalidOperationException($"Parameter is not a number ({context.Parameter.PropertyName}).");
    }
}
