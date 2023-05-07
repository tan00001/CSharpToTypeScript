using System.Diagnostics;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.Extensions;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace CSharpToTypeScript.Models
{
    [DebuggerDisplay("Name: {Name}")]
    public class TsProperty
    {
        public string Name { get; set; }

        public TsType PropertyType { get; set; }

        public MemberInfo MemberInfo { get; set; }

        public IList<TsType> GenericArguments { get; private set; }

        public object? ConstantValue { get; set; }

        public bool IsNullable { get; set; }
        public bool IsRequired { get; set; }

        public List<ITsValidationRule> ValidationRules { get; private set; }
        public DataMemberAttribute? DataMember { get; set; }
        public DisplayAttribute? Display { get; set; }
        public JsonIgnoreAttribute? JsonIgnore { get; set; }
        public JsonPropertyNameAttribute? JsonPropertyName { get; set; }

        public TsProperty(ITsModuleService tsModuleService, PropertyInfo propertyInfo)
        {
            this.MemberInfo = propertyInfo;
            this.Name = propertyInfo.Name;
            this.ValidationRules = new List<ITsValidationRule>();

            Type type = propertyInfo.PropertyType;
            if (type.IsNullableValueType())
            {
                type = type.GetNullableValueType();
                IsNullable = true;
            }
            else
            {
                IsNullable = propertyInfo.IsNullableReferenceType();
            }

            this.GenericArguments = type.IsGenericType ? type.GetGenericArguments().Select(o => new TsType(o)).ToArray() : Array.Empty<TsType>();
            this.PropertyType = TsType.Create(tsModuleService, type);

            DataMember = propertyInfo.GetCustomAttribute<DataMemberAttribute>(false);
            Display = propertyInfo.GetCustomAttribute<DisplayAttribute>(false);
            JsonIgnore = propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>(false);
            JsonPropertyName = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>(false);

            var compare = propertyInfo.GetCustomAttribute<CompareAttribute>(false);
            if (compare != null)
            {
                ValidationRules.Add(new CompareRule(compare));
            }

            var creditCard = propertyInfo.GetCustomAttribute<CreditCardAttribute>(false);
            if (creditCard != null)
            {
                ValidationRules.Add(new CreditCardRule(creditCard));
            }

            var emailAddress = propertyInfo.GetCustomAttribute<EmailAddressAttribute>(false);
            if (emailAddress != null)
            {
                ValidationRules.Add(new EmailAddressRule(emailAddress));
            }

            var phone = propertyInfo.GetCustomAttribute<PhoneAttribute>(false);
            if (phone != null)
            {
                ValidationRules.Add(new PhoneNumberRule(phone));
            }

            var range = propertyInfo.GetCustomAttribute<RangeAttribute>(false);
            if (range != null)
            {
                ValidationRules.Add(new RangeRule(range));
            }

            var regularExpression = propertyInfo.GetCustomAttribute<RegularExpressionAttribute>(false);
            if (regularExpression != null)
            {
                ValidationRules.Add(new RegularExpressionRule(regularExpression));
            }

            var required = propertyInfo.GetCustomAttribute<RequiredAttribute>(false);
            if (required != null)
            {
                IsRequired = true;
                ValidationRules.Add(new RequiredRule(required));
            }

            var stringLength = propertyInfo.GetCustomAttribute<StringLengthAttribute>(false);
            if (stringLength != null)
            {
                ValidationRules.Add(new StringLengthRule(stringLength));
            }

            var url = propertyInfo.GetCustomAttribute<UrlAttribute>(false);
            if (url != null)
            {
                ValidationRules.Add(new UrlRule(url));
            }

            this.ConstantValue = null;
        }

        public TsProperty(ITsModuleService tsModuleService, FieldInfo fieldInfo)
        {
            this.MemberInfo = fieldInfo;
            this.Name = fieldInfo.Name;
            this.ValidationRules = new List<ITsValidationRule>();

            if (fieldInfo.ReflectedType?.IsGenericType == true)
            {
                this.PropertyType = !fieldInfo.ReflectedType?.GetGenericTypeDefinition()?.GetProperty(fieldInfo.Name)?.PropertyType.IsGenericParameter == true ?
                    TsType.Create(tsModuleService, fieldInfo.FieldType) : TsType.Any;
            }
            else
            {
                Type type = fieldInfo.FieldType;
                if (type.IsNullableValueType())
                {
                    type = type.GetNullableValueType();
                }
                else
                {
                    IsNullable = fieldInfo.IsNullableReferenceType();
                }
                this.PropertyType = TsType.Create(tsModuleService, type);
            }
            this.GenericArguments = Array.Empty<TsType>();

            DataMember = fieldInfo.GetCustomAttribute<DataMemberAttribute>(false);
            Display = fieldInfo.GetCustomAttribute<DisplayAttribute>(false);
            JsonIgnore = fieldInfo.GetCustomAttribute<JsonIgnoreAttribute>(false);
            JsonPropertyName = fieldInfo.GetCustomAttribute<JsonPropertyNameAttribute>(false);

            var compare = fieldInfo.GetCustomAttribute<CompareAttribute>(false);
            if (compare != null)
            {
                ValidationRules.Add(new CompareRule(compare));
            }

            var creditCard = fieldInfo.GetCustomAttribute<CreditCardAttribute>(false);
            if (creditCard != null)
            {
                ValidationRules.Add(new CreditCardRule(creditCard));
            }

            var emailAddress = fieldInfo.GetCustomAttribute<EmailAddressAttribute>(false);
            if (emailAddress != null)
            {
                ValidationRules.Add(new EmailAddressRule(emailAddress));
            }

            var phone = fieldInfo.GetCustomAttribute<PhoneAttribute>(false);
            if (phone != null)
            {
                ValidationRules.Add(new PhoneNumberRule(phone));
            }

            var range = fieldInfo.GetCustomAttribute<RangeAttribute>(false);
            if (range != null)
            {
                ValidationRules.Add(new RangeRule(range));
            }

            var regularExpression = fieldInfo.GetCustomAttribute<RegularExpressionAttribute>(false);
            if (regularExpression != null)
            {
                ValidationRules.Add(new RegularExpressionRule(regularExpression));
            }

            var required = fieldInfo.GetCustomAttribute<RequiredAttribute>(false);
            if (required != null)
            {
                ValidationRules.Add(new RequiredRule(required));
            }

            var stringLength = fieldInfo.GetCustomAttribute<StringLengthAttribute>(false);
            if (stringLength != null)
            {
                ValidationRules.Add(new StringLengthRule(stringLength));
            }

            var url = fieldInfo.GetCustomAttribute<UrlAttribute>(false);
            if (url != null)
            {
                ValidationRules.Add(new UrlRule(url));
            }

            if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
                this.ConstantValue = fieldInfo.GetValue(null);
            else
                this.ConstantValue = null;
        }

        public string GetDisplayName()
        {
            return Display?.Name ?? this.Name;
        }

        public string? GetDefaultValue()
        {
            if (this.IsNullable)
            {
                return "null";
            }

            return this.PropertyType switch
            {
                _ when this.PropertyType is TsEnum => "0",
                _ when this.PropertyType is TsCollection => "[]",
                _ when this.PropertyType is TsSystemType tsSystemType =>
                tsSystemType.Kind switch {
                    _ when tsSystemType.Kind == SystemTypeKind.String => "\"\"",
                    _ when tsSystemType.Kind == SystemTypeKind.Number => "0",
                    _ when tsSystemType.Kind == SystemTypeKind.Bool => "false",
                    _ => null
                },
                _ => null
            };
        }
    }
}
