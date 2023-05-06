#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CSharpToTypeScript.Models
{
    public class TsEnumValue
    {
        public string Name { get; private set; }

        public string? Value { get; private set; }

        public FieldInfo Field { get; private set; }

        public DisplayAttribute? Display { get; private set; }

        public TsEnumValue(FieldInfo field)
        {
            this.Field = field;
            this.Name = field.Name;
            object? obj = field.GetValue(null);
            if (obj == null)
            {
                return;
            }

            switch (Enum.GetUnderlyingType(field.FieldType))
            {
                case Type t when t == typeof(byte):
                    this.Value = ((byte)obj).ToString();
                    break;

                case Type t when t == typeof(sbyte):
                    this.Value = ((sbyte)obj).ToString();
                    break;

                case Type t when t == typeof(short):
                    this.Value = ((short)obj).ToString();
                    break;

                case Type t when t == typeof(ushort):
                    this.Value = ((ushort)obj).ToString();
                    break;

                case Type t when t == typeof(int):
                    this.Value = ((int)obj).ToString();
                    break;

                case Type t when t == typeof(uint):
                    this.Value = ((uint)obj).ToString();
                    break;

                case Type t when t == typeof(long):
                    this.Value = ((long)obj).ToString();
                    break;

                case Type t when t == typeof(ulong):
                    this.Value = ((ulong)obj).ToString();
                    break;
            }
        }

        public string GetDisplayName()
        {
            return Display?.Name ?? this.Name;
        }
    }
}
