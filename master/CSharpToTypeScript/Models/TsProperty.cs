#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.Extensions;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;

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

        public CompareAttribute? Compare { get; set; }
        public CreditCardAttribute? CreditCard { get; set; }
        public DisplayAttribute? Display { get; set; }
        public EmailAddressAttribute? EmailAddress { get; set; }
        public JsonIgnoreAttribute? JsonIgnore { get; set; }
        public JsonPropertyNameAttribute? JsonPropertyName { get; set; }
        public PhoneAttribute? Phone { get; set; }
        public RangeAttribute? Range { get; set; }
        public RegularExpressionAttribute? RegularExpression { get; set; }
        public RequiredAttribute? Required { get; set; }
        public StringLengthAttribute? StringLength { get; set; }
        public UrlAttribute? Url { get; set; }

        public TsProperty(PropertyInfo propertyInfo)
        {
            this.MemberInfo = propertyInfo;
            this.Name = propertyInfo.Name;
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

            this.GenericArguments = type.IsGenericType ? type.GetGenericArguments().Select(o => new TsType(o)).ToArray() : new TsType[0];
            this.PropertyType = type.IsEnum ? new TsEnum(type) : new TsType(type);

            Compare = propertyInfo.GetCustomAttribute<CompareAttribute>(false);
            CreditCard = propertyInfo.GetCustomAttribute<CreditCardAttribute>(false);
            Display = propertyInfo.GetCustomAttribute<DisplayAttribute>(false);
            EmailAddress = propertyInfo.GetCustomAttribute<EmailAddressAttribute>(false);
            JsonIgnore = propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>(false);
            JsonPropertyName = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>(false);
            Phone = propertyInfo.GetCustomAttribute<PhoneAttribute>(false);
            Range = propertyInfo.GetCustomAttribute<RangeAttribute>(false);
            RegularExpression = propertyInfo.GetCustomAttribute<RegularExpressionAttribute>(false);
            Required = propertyInfo.GetCustomAttribute<RequiredAttribute>(false);
            StringLength = propertyInfo.GetCustomAttribute<StringLengthAttribute>(false);
            Url = propertyInfo.GetCustomAttribute<UrlAttribute>(false);

            this.ConstantValue = null;
        }

        public TsProperty(FieldInfo fieldInfo)
        {
            this.MemberInfo = fieldInfo;
            this.Name = fieldInfo.Name;
            if (fieldInfo.ReflectedType?.IsGenericType == true)
            {
                this.PropertyType = !fieldInfo.ReflectedType?.GetGenericTypeDefinition()?.GetProperty(fieldInfo.Name)?.PropertyType.IsGenericParameter == true ?
                    (fieldInfo.FieldType.IsEnum ? new TsEnum(fieldInfo.FieldType) : new TsType(fieldInfo.FieldType)) : TsType.Any;
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
                this.PropertyType = type.IsEnum ? new TsEnum(type) : new TsType(type);
            }
            this.GenericArguments = new TsType[0];

            Compare = fieldInfo.GetCustomAttribute<CompareAttribute>(false);
            CreditCard = fieldInfo.GetCustomAttribute<CreditCardAttribute>(false);
            Display = fieldInfo.GetCustomAttribute<DisplayAttribute>(false);
            EmailAddress = fieldInfo.GetCustomAttribute<EmailAddressAttribute>(false);
            JsonIgnore = fieldInfo.GetCustomAttribute<JsonIgnoreAttribute>(false);
            JsonPropertyName = fieldInfo.GetCustomAttribute<JsonPropertyNameAttribute>(false);
            Phone = fieldInfo.GetCustomAttribute<PhoneAttribute>(false);
            Range = fieldInfo.GetCustomAttribute<RangeAttribute>(false);
            RegularExpression = fieldInfo.GetCustomAttribute<RegularExpressionAttribute>(false);
            Required = fieldInfo.GetCustomAttribute<RequiredAttribute>(false);
            StringLength = fieldInfo.GetCustomAttribute<StringLengthAttribute>(false);
            Url = fieldInfo.GetCustomAttribute<UrlAttribute>(false);

            if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
                this.ConstantValue = fieldInfo.GetValue(null);
            else
                this.ConstantValue = null;
        }
    }
}
