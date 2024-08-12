// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class DriveLetterAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string driveLetter)
        {
            var drives = Environment.GetLogicalDrives().ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (drives.Contains(driveLetter))
                return ValidationResult.Success;
            else
                return new ValidationResult($"Drive letter {driveLetter} is not a valid drive");
        }
        throw new InvalidOperationException("Property must be string");
    }
}
