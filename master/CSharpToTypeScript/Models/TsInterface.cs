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
    public class TsInterface : TsModuleMemberWithPropertiesAndGenericArguments
    {
        public override ICollection<TsProperty> Properties { get; protected set; }

        public override IList<TsType> GenericArguments { get; protected set; }

        public TsType? BaseType { get; internal set; }

        public IList<TsType> Interfaces { get; internal set; }

        public TsInterface(Type type)
          : base(type)
        {
            this.Properties = this.Type.GetProperties().Where(pi => pi.DeclaringType == this.Type)
                .Select(pi => new TsProperty(pi)).ToList();

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

            if (this.Type.BaseType != null && this.Type.BaseType.IsInterface)
                this.BaseType = new TsType(this.Type.BaseType);

            Type[] interfaces = this.Type.GetInterfaces();

            this.Interfaces = interfaces.Where(@interface => @interface.GetCustomAttribute<JsonIgnoreAttribute>(false) == null)
                .Select(t => TsType.Create(t)).ToList();
        }
    }
}
