#nullable enable
using System;

namespace CSharpToTypeScript.Models
{
    public class TsSystemType : TsType
    {
        public SystemTypeKind Kind { get; private set; }

        public TsSystemType(Type type)
          : base(type)
        {
            if (!TryGet(type, out SystemTypeKind? tsSystemType))
            {
                throw new ArgumentException(string.Format("The type '{0}' is not supported system type.", type.FullName));
            }

            this.Kind = tsSystemType!.Value;
        }

        public static bool TryGet(Type type, out SystemTypeKind? tsSystemType)
        {
            switch (type)
            {
                case Type t when t == typeof(bool):
                    tsSystemType = SystemTypeKind.Bool;
                    return true;
                case Type t when t == typeof(byte):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(decimal):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(double):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(short):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(int):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(long):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(IntPtr):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(sbyte):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(float):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(ushort):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(uint):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(ulong):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(UIntPtr):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(char):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(string):
                    tsSystemType = SystemTypeKind.String;
                    return true;
                case Type t when t == typeof(DateTime):
                    tsSystemType = SystemTypeKind.Number;
                    return true;
                case Type t when t == typeof(DateTimeOffset):
                    tsSystemType = SystemTypeKind.Date;
                    return true;
                default:
                    tsSystemType = null;
                    return false;
            }
        }
    }
}
