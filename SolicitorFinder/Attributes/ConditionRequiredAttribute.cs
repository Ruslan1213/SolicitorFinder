using System.ComponentModel.DataAnnotations;

namespace SolicitorFinder.Attributes;

public sealed class ConditionRequiredAttribute : ValidationAttribute
{
    private readonly string _checkIfProperty;

    public ConditionRequiredAttribute(string checkIfProperty)
    {
        _checkIfProperty = checkIfProperty;
        ErrorMessage = "This field is required when {0} is empty";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var checkProperty = context.ObjectType.GetProperty(_checkIfProperty);
        if (checkProperty == null)
            return new ValidationResult($"Property '{_checkIfProperty}' not found");

        var conditionValue = checkProperty.GetValue(context.ObjectInstance);
        var isConditionNull = conditionValue == null && IsEmpty(conditionValue);

        if (isConditionNull)
        {
            if (value == null || IsEmpty(value))
            {
                return new ValidationResult($"'{context.DisplayName}' is required when '{_checkIfProperty}' is provided");
            }
        }

        return ValidationResult.Success;
    }

    private static bool IsEmpty(object? value)
    {
        if (value == null)
            return true;

        return value switch
        {
            string s => string.IsNullOrWhiteSpace(s),
            int i => i == 0,
            long l => l == 0,
            double d => d == 0,
            decimal d => d == 0,
            DateTime dt => dt == default,
            _ => false
        };
    }
}
