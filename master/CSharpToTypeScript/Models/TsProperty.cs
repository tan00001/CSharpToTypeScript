using System.Diagnostics;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.Extensions;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSharpToTypeScript.Models
{
    [DebuggerDisplay("Name: {Name}")]
    public class TsProperty
    {
        public const string UiHintHidden = "hidden";
        public const string UiHintSelect = "select";
        public const string UiHintColSpan = "colSpan";
        public const string UiHintTypeContainingOptions = "typeContainingOptions";
        public const string UiHintNameOfOptions = "nameOfOptions";
        public const string UiHintReadOnly = "readOnly";

        public string Name { get; set; }

        public TsType PropertyType { get; set; }

        public MemberInfo MemberInfo { get; set; }

        public IList<TsType> GenericArguments { get; private set; }

        public object? ConstantValue { get; set; }

        public bool IsNullable { get; set; }
        public bool IsRequired { get; set; }

        public bool IsHidden
        {
            get
            {
                return UiHint?.UIHint == UiHintHidden;
            }
        }

        public bool IsValueType
        {
            get
            {
                return PropertyType.Type.IsValueType;
            }
        }

        public List<ITsValidationRule> ValidationRules { get; private set; }

        public DataMemberAttribute? DataMember { get; set; }
        public DisplayAttribute? Display { get; set; }
        public UIHintAttribute? UiHint { get; set; }
        public DataTypeAttribute? DataType { get; private set; }
        protected NotMappedAttribute? NotMapped { get; set; }
        protected JsonIgnoreAttribute? JsonIgnore { get; set; }        
        public JsonPropertyNameAttribute? JsonPropertyName { get; set; }

        public bool HasIgnoreAttribute
        {
            get
            {
                return JsonIgnore != null || NotMapped != null;
            }
        }

        private TsProperty(ITsModuleService tsModuleService, MemberInfo memberInfo, Type type, bool isNullableReferenceType,
            TsModuleMemberWithHierarchy parent)
        {
            this.MemberInfo = memberInfo;
            this.Name = memberInfo.Name;
            this.ValidationRules = new List<ITsValidationRule>();

            if (type.IsNullableValueType())
            {
                type = type.GetNullableValueType();
                IsNullable = true;
            }
            else
            {
                IsNullable = isNullableReferenceType;
            }

            this.GenericArguments = type.IsGenericType ? type.GetGenericArguments().Select(o => TsType.Create(tsModuleService, o)).ToArray() 
                : Array.Empty<TsType>();
            this.PropertyType = TsType.Create(tsModuleService, type);

            DataMember = memberInfo.SafeGetCustomAttribute<DataMemberAttribute>(false);
            Display = memberInfo.SafeGetCustomAttribute<DisplayAttribute>(false);
            JsonIgnore = memberInfo.SafeGetCustomAttribute<JsonIgnoreAttribute>(false);
            JsonPropertyName = memberInfo.SafeGetCustomAttribute<JsonPropertyNameAttribute>(false);
            UiHint = memberInfo.SafeGetCustomAttribute<UIHintAttribute>(false);
            NotMapped = memberInfo.SafeGetCustomAttribute<NotMappedAttribute>(false);
            DataType = memberInfo.SafeGetCustomAttribute<DataTypeAttribute>(false);

            AddValidationRules(tsModuleService, memberInfo, parent);
        }

        public TsProperty(ITsModuleService tsModuleService, PropertyInfo propertyInfo, TsModuleMemberWithHierarchy parent)
            : this(tsModuleService, propertyInfo, propertyInfo.PropertyType, propertyInfo.IsNullableReferenceType(), parent)
        {
            this.ConstantValue = null;
        }

        public TsProperty(ITsModuleService tsModuleService, FieldInfo fieldInfo, TsModuleMemberWithHierarchy parent)
            : this(tsModuleService, fieldInfo, fieldInfo.FieldType, fieldInfo.IsNullableReferenceType(), parent)
        {
            if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
                this.ConstantValue = fieldInfo.GetValue(null);
            else
                this.ConstantValue = null;
        }

        private void AddValidationRules(ITsModuleService tsModuleService, MemberInfo memberInfo, TsModuleMemberWithHierarchy parent)
        {
            var compare = memberInfo.SafeGetCustomAttribute<CompareAttribute>(false);
            if (compare != null)
            {
                ValidationRules.Add(new CompareRule(compare));
            }

            var creditCard = memberInfo.SafeGetCustomAttribute<CreditCardAttribute>(false);
            if (creditCard != null)
            {
                ValidationRules.Add(new CreditCardRule(creditCard));
            }

            var emailAddress = memberInfo.SafeGetCustomAttribute<EmailAddressAttribute>(false);
            if (emailAddress != null)
            {
                ValidationRules.Add(new EmailAddressRule(emailAddress));
            }

            var phone = memberInfo.SafeGetCustomAttribute<PhoneAttribute>(false);
            if (phone != null)
            {
                ValidationRules.Add(new PhoneNumberRule(phone));
            }

            var range = memberInfo.SafeGetCustomAttribute<RangeAttribute>(false);
            if (range != null)
            {
                ValidationRules.Add(new RangeRule(range, DataType));
            }

            var regularExpression = memberInfo.SafeGetCustomAttribute<RegularExpressionAttribute>(false);
            if (regularExpression != null)
            {
                ValidationRules.Add(new RegularExpressionRule(regularExpression));
            }

            var required = memberInfo.SafeGetCustomAttribute<RequiredAttribute>(false);
            if (required != null || DataMember?.IsRequired == true)
            {
                IsRequired = true;
                ValidationRules.Add(new RequiredRule(required, DataMember));
            }

            var stringLength = memberInfo.SafeGetCustomAttribute<StringLengthAttribute>(false);
            if (stringLength != null)
            {
                ValidationRules.Add(new StringLengthRule(stringLength));
            }

            var url = memberInfo.SafeGetCustomAttribute<UrlAttribute>(false);
            if (url != null)
            {
                ValidationRules.Add(new UrlRule(url));
            }

            foreach(var customValidation in memberInfo.SafeGetCustomAttributes<CustomValidationAttribute>(false))
            {
                var validatorType = customValidation.ValidatorType != parent.Type ? TsType.Create(tsModuleService, customValidation.ValidatorType) : parent;
                var customValidationRule = new CustomValidationRule(customValidation, validatorType);
                ValidationRules.Add(customValidationRule);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Null if it uses wildcard</returns>
        public Int32? GetColSpanHint(Int32 defaultColSpan, Int32 maxColSpan)
        {
            if (UiHint == null)
            {
                return defaultColSpan;
            }

            if (UiHint.ControlParameters.TryGetValue(UiHintColSpan, out object? colSpanSetting)
                && colSpanSetting is string colSpanString)
            {
                if (colSpanString == "*")
                {
                    return null;
                }

                if (!Int32.TryParse(colSpanString, out var colSpan))
                {
                    return defaultColSpan;
                }

                if (colSpan <= 0)
                {
                    return defaultColSpan;
                }

                if (colSpan > maxColSpan)
                {
                    return maxColSpan;
                }

                return colSpan * defaultColSpan;
            }

            return defaultColSpan;
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
                _ when this.PropertyType is TsCollection tsCollection => tsCollection.KeyType != null ? "{}" : "[]",
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

        public string? GetDisplayPrompt()
        {
            return Display?.Prompt;
        }
    }
}
