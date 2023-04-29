#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CSharpToTypeScript.Extensions;

namespace CSharpToTypeScript.Models
{
    [DebuggerDisplay("TsType - Type: {ClrType}")]
    public class TsType
    {
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
            if (type.IsNullableValueType())
                return TsType.GetTypeFamily(type.GetNullableValueType());
            int num = type == typeof(string) ? 1 : 0;
            bool flag = typeof(IEnumerable).IsAssignableFrom(type);
            if (num != 0 || type.IsPrimitive || TsSystemType.TryGet(type, out _))
                return TsTypeFamily.System;
            if (flag)
                return TsTypeFamily.Collection;
            if (type.IsEnum)
                return TsTypeFamily.Enum;
            return type.IsClass && type.FullName != "System.Object" || type.IsValueType || type.IsInterface ? TsTypeFamily.Class : TsTypeFamily.Type;
        }

        internal static TsType Create(Type type)
        {
            switch (TsType.GetTypeFamily(type))
            {
                case TsTypeFamily.System:
                    return new TsSystemType(type);
                case TsTypeFamily.Collection:
                    return new TsCollection(type);
                case TsTypeFamily.Class:
                    return new TsClass(type);
                case TsTypeFamily.Enum:
                    return new TsEnum(type);
                default:
                    return new TsType(type);
            }
        }

        internal static Type? GetEnumerableType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];
            foreach (Type type1 in type.GetInterfaces())
            {
                if (type1.IsGenericType && type1.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return type1.GetGenericArguments()[0];
            }
            return null;
        }
    }
}
