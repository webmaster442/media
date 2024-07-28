using FFCmd.Infrastructure.Validation;

namespace FFCmd.Infrastructure;

internal abstract class ValidatedCommandSettings : CommandSettings
{
    public override Spectre.Console.ValidationResult Validate()
    {
        if (ObjectValidator.TryValidate(this, out string errors))
        {
            return Spectre.Console.ValidationResult.Success();
        }
        return Spectre.Console.ValidationResult.Error(errors);
    }
}
