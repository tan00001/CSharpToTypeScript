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
    public class TsClass : TsModuleMemberWithPropertiesAndGenericArguments
    {
        public override ICollection<TsProperty> Properties { get; protected set; }

        public ICollection<TsProperty> Fields { get; private set; }

        public override IList<TsType> GenericArguments { get; protected set; }

        public ICollection<TsProperty> Constants { get; private set; }

        public TsType? BaseType { get; internal set; }

        public IList<TsInterface> Interfaces { get; internal set; }

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

            this.Interfaces = this.Type.GetInterfaces().Where(@interface => @interface.GetCustomAttribute<JsonIgnoreAttribute>(false) == null)
                .Select(t => new TsInterface(t)).ToList();
        }

        public Int32 GetDerivationDepth()
        {
            int depth = 0;
            for (var parent = this.Type.BaseType; parent != null; parent = parent.BaseType)
            {
                ++depth;
            }
            return depth;
        }
    }
}
