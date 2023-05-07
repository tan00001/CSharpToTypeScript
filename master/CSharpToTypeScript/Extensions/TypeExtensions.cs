using System.Collections.ObjectModel;
using System.Reflection;

namespace CSharpToTypeScript.Extensions
{
    public static class TypeExtensions
    {
        const string NullableAttributeTypeName = "System.Runtime.CompilerServices.NullableAttribute";
        const string NullableContextAttributeTypeName = "System.Runtime.CompilerServices.NullableContextAttribute";

        public static bool IsNullableValueType(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static Type GetNullableValueType(this Type type) => type.GetGenericArguments().Single<Type>();

        public static bool IsNullableReferenceType(this PropertyInfo propertyInfo) =>
            IsNullableReferenceType(propertyInfo.PropertyType, propertyInfo.DeclaringType, propertyInfo.CustomAttributes);

        public static bool IsNullableReferenceType(this FieldInfo fieldInfo) =>
            IsNullableReferenceType(fieldInfo.FieldType, fieldInfo.DeclaringType, fieldInfo.CustomAttributes);

        public static bool IsNullableReferenceType(this ParameterInfo parameterInfo) =>
            IsNullableReferenceType(parameterInfo.ParameterType, null, parameterInfo.CustomAttributes);

        private static bool IsNullableReferenceType(Type memberType, MemberInfo? declaringType, IEnumerable<CustomAttributeData> customAttributes)
        {
            if (memberType.IsValueType)
                return false;

            var nullable = customAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == NullableAttributeTypeName);
            if (nullable != null && nullable.ConstructorArguments.Count == 1)
            {
                var attributeArgument = nullable.ConstructorArguments[0];
                if (attributeArgument.ArgumentType == typeof(byte[]))
                {
                    var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;
                    if (args.Count > 0 && args[0].ArgumentType == typeof(byte))
                    {
                        return (byte)args[0].Value! == 2;
                    }
                }
                else if (attributeArgument.ArgumentType == typeof(byte))
                {
                    return (byte)attributeArgument.Value! == 2;
                }
            }

            for (var type = declaringType; type != null; type = type.DeclaringType)
            {
                var context = type.CustomAttributes
                    .FirstOrDefault(x => x.AttributeType.FullName == NullableContextAttributeTypeName);
                if (context != null &&
                    context.ConstructorArguments.Count == 1 &&
                    context.ConstructorArguments[0].ArgumentType == typeof(byte))
                {
                    return (byte)context.ConstructorArguments[0].Value! == 2;
                }
            }

            return false;
        }
    }
}
