#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CSharpToTypeScript.Models
{
    [DebuggerDisplay("TsClass - Name: {Name}")]
    public class TsClass : TsModuleMember
    {
        public ICollection<TsProperty> Properties { get; private set; }

        public ICollection<TsProperty> Fields { get; private set; }

        public IList<TsType> GenericArguments { get; private set; }

        public ICollection<TsProperty> Constants { get; private set; }

        public TsType? BaseType { get; internal set; }

        public IList<TsType> Interfaces { get; internal set; }

        public bool IsIgnored { get; set; }

        public TsClass(Type type)
          : base(type)
        {
            this.Properties = this.Type.GetProperties().Where(pi => pi.DeclaringType == this.Type)
                .Select(pi => new TsProperty(pi)).ToList();
            this.Fields = this.Type.GetFields().Where(fi =>
                {
                    if (fi.DeclaringType != this.Type)
                        return false;
                    return !fi.IsLiteral || fi.IsInitOnly;
                })
                .Select(fi => new TsProperty(fi))
                .ToList();

            this.Constants = this.Type.GetFields().Where(fi => fi.DeclaringType == this.Type && fi.IsLiteral && !fi.IsInitOnly)
                .Select(fi => new TsProperty(fi))
                .ToList();

            if (type.IsGenericType)
            {
                this.Name = type.Name.Remove(type.Name.IndexOf('`'));
                this.GenericArguments = type.GetGenericArguments()
                    .Select(t => TsType.Create(t))
                    .ToList();
            }
            else
            {
                this.Name = type.Name;
                this.GenericArguments = new TsType[0];
            }

            if (this.Type.BaseType != null && this.Type.BaseType != typeof(object) && this.Type.BaseType != typeof(ValueType))
                this.BaseType = new TsType(this.Type.BaseType);

            Type[] interfaces = this.Type.GetInterfaces();

            this.Interfaces = interfaces.Where(@interface => @interface.GetCustomAttribute<JsonIgnoreAttribute>(false) == null)
                .Select(t => TsType.Create(t)).ToList();

            var displayAttribute = this.Type.GetCustomAttribute<DisplayAttribute>(false);
            if (displayAttribute != null)
            {
                if (!string.IsNullOrEmpty(displayAttribute.Name))
                    this.Name = displayAttribute.Name!;
            }

            if (this.Type.GetCustomAttribute<JsonIgnoreAttribute>(false) == null)
                return;

            this.IsIgnored = true;
        }
    }
}
