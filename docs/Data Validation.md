# Generating Data Validation TypeScript for React Hook From

## Required

To specify that a property of a class or a struct is required, you can use the attribute `System.ComponentModel.DataAnnotations.RequiredAttribute`. If you would like to specify an error message, you may use its `ErrorMessage` property.

## String Length

To specify the length of a string property of a class or a struct must be within certain range, you can use the attribute `System.ComponentModel.DataAnnotations.StringLengthAttribute`. If you would like to specify an error message, you may use its `ErrorMessage` property.

## Range

To specify that a property of a class or a struct must be within a certain range, you can use the attribute `System.ComponentModel.DataAnnotations.RangeAttribute`.
* The constructor `RangeAttribute(int minimum, int maximum)` specifies the minimum and maximum as integers.
* The constructor `RangeAttribute(double minimum, double maximum)` specifies the minimum and maximum as doubles.
* For other types, the constructor `RangeAttribute(Type, string minimum, string maximum)` specifies minimum and maximum as strings. These strings are converted to the Type specified. If the conversion is not allowed, an exception is thrown.
* If you would like to specify an error message, you may use its `ErrorMessage` property.

## Regular Expression

To specify that a property of a class or a struct must match a specific regular expression, you can use the attribute `System.ComponentModel.DataAnnotations.RegularExpressionAttribute`. If you would like to specify an error message, you may use its `ErrorMessage` property.

## Email Address

To specify that a property of a class or a struct must match a specific regular expression, you can use the attribute `System.ComponentModel.DataAnnotations.EmailAddressAttribute`. If you would like to specify an error message, you may use its `ErrorMessage` property.

## Phone Number

To specify that a property of a class or a struct must match a specific regular expression, you can use the attribute `System.ComponentModel.DataAnnotations.PhoneAttribute`. If you would like to specify an error message, you may use its `ErrorMessage` property.

## Credit Card

To specify that a property of a class or a struct must have the check sum of a credit card number, you can use the attribute `System.ComponentModel.DataAnnotations.CreditCardAttribute`. If you would like to specify an error message, you may use its `ErrorMessage` property.

## Url

To specify that a property of a class or a struct must start with "http://", "https://", or "ftp://", you can use the attribute `System.ComponentModel.DataAnnotations.UrlAttribute`. If you would like to specify an error message, you may use its `ErrorMessage` property.

## Compare

To specify that a property of a class or a struct must match another property of the same class, you can use the attribute `System.ComponentModel.DataAnnotations.CompareAttribute`. If you would like to specify an error message, you may use its `ErrorMessage` property.

## Custom Validation

To specify a custom validation function for a property of a class or a struct, you can use the attribute `System.ComponentModel.DataAnnotations.CustomValidationAttribute`. 
`System.ComponentModel.DataAnnotations.CustomValidationAttribute` supports two different validation function signatures as shown below:
```
public class CustomerAccount
{
    public int Id { get; set; }

    [CustomValidation(typeof(CustomerAccount), nameof(ValidateBalance))]
    [CustomValidation(typeof(CustomerAccount), nameof(ValidateBalance2))]    
    public decimal? Balance { get; set; }

    public virtual ValidationResult? ValidateBalance(object? value)
    {
        if (value is decimal balance)
        {
            if (balance > 0)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Balance cannot be less than 0.");
        }
        return new ValidationResult("Balance data type is incorrect.");
    }

    public virtual ValidationResult? ValidateBalance2(object? value, ValidationContext context)
    {
        if (context.ObjectInstance is CustomerAccount values)
        {
            #region CSharpToTypeScript
            if ((values.Balance != null && values.Balance > 0) || values.Id == 0)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Balance must be greater than 0 when Id is not 0.");
            #endregion // CSharpToTypeScript
        }
        return new ValidationResult("Balance data type is incorrect.");
    }
}
```
To automatically generate TypeScript validation function contents, the second validation function signature is required. Inside the validation function, `context.ObjectInstance` must be converted to the typed `values`:
```
public ValidationResult? ValidateBalance2(object? value, ValidationContext context)
{
    if (context.ObjectInstance is CustomerAccount values)
    {
        #region CSharpToTypeScript
        ...
        #endregion // CSharpToTypeScript
    }
    return new ValidationResult("Balance data type is incorrect.");
}
```
In addition, a region `CSharpToTypeScript` is needed in the scope where `values` is available.

If you would like to specify an error message, you may use its `ErrorMessage` property.
