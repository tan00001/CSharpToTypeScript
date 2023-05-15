using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using CSharpToTypeScript.Extensions;

namespace CSharpToTypeScript.Models
{
    [DebuggerDisplay("TsType - Type: {ClrType}")]
    public class TsType
    {
        public static readonly IReadOnlyList<string> ExcludedNamespacePrefixes = new List<string>()
        {
            "System.",
            "Microsoft.",
            "Newtonsoft.",
        };

        private readonly HashSet<CustomValidationRule> _ImplementedCustomValidationRules = new(CustomValidationRule.Comparer);

        /// <summary>
        /// Custom validation rules this type implements
        /// </summary>
        public virtual IReadOnlySet<CustomValidationRule> ImplementedCustomValidationRules
        {
            get
            {
                return _ImplementedCustomValidationRules;
            }
        }

        public static readonly TsType Any = new(typeof(object));

        public Type Type { get; private set; }

        public TsType(Type type)
        {
            if (type.IsNullableValueType())
                type = type.GetNullableValueType();
            this.Type = type;
        }

        public bool IsCollection() => TsType.GetTypeFamily(this.Type) == TsTypeFamily.Collection;

        internal static TsTypeFamily GetTypeFamily(Type type)
        {
            if (type.IsInterface)
                return TsTypeFamily.Interface;

            if (type.IsNullableValueType())
                return TsType.GetTypeFamily(type.GetNullableValueType());

            if (type == typeof(string) || type.IsPrimitive || TsSystemType.TryGet(type, out _))
                return TsTypeFamily.System;

            if (typeof(IEnumerable).IsAssignableFrom(type))
                return TsTypeFamily.Collection;

            if (type.IsEnum)
                return TsTypeFamily.Enum;

            if (type.IsValueType)
            {
                if ((type.IsTypeDefinition | type.IsGenericType)
                    && (type.GetProperties().Length > 0 || type.GetFields().Length > 0) )
                {
                    return TsTypeFamily.TypeDefinition;
                }

                return TsTypeFamily.Type;
            }

            if (!string.IsNullOrEmpty(type.FullName) 
                && ExcludedNamespacePrefixes.Any(p => type.FullName.StartsWith(p)))
                return TsTypeFamily.Type;

            return type.IsClass ? TsTypeFamily.Class : TsTypeFamily.Type;
        }

        public void AddImplementedCustomValidationRule(CustomValidationRule rule)
        {
            _ImplementedCustomValidationRules.Add(rule);
        }

        internal static TsType Create(ITsModuleService tsModuleService, Type type)
        {
            if (tsModuleService.IsProcessing(type) || type.IsGenericParameter)
            {
                return new TsType(type);
            }

            switch (TsType.GetTypeFamily(type))
            {
                case TsTypeFamily.System:
                    return new TsSystemType(type);
                case TsTypeFamily.Collection:
                    return new TsCollection(tsModuleService, type);
                case TsTypeFamily.Class:
                    return tsModuleService.GetOrAddTsClass(type);
                case TsTypeFamily.TypeDefinition:
                    return tsModuleService.GetOrAddTsTypeDefinition(type);
                case TsTypeFamily.Interface:
                    return tsModuleService.GetOrAddTsInterface(type);
                case TsTypeFamily.Enum:
                    return tsModuleService.GetOrAddTsEnum(type);
                default:
                    return new TsType(type);
            }
        }

        internal static Type? GetEnumeratedType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            foreach (Type @interface in type.GetInterfaces())
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return @interface.GetGenericArguments()[0];
            }

            return null;
        }
    }
}
