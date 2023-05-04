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

        public override IReadOnlyList<TsType> GenericArguments { get; protected set; }

        public ICollection<TsProperty> Constants { get; private set; }

        public TsType? BaseType { get; internal set; }

        public IList<TsInterface> Interfaces { get; internal set; }

        public TsClass(ITsModuleService tsModuleService, Type type)
          : base(type)
        {
            this.Properties = this.Type.GetProperties().Where(pi => pi.DeclaringType == this.Type)
                .Select(pi => new TsProperty(tsModuleService, pi)).ToList();
            this.Fields = this.Type.GetFields().Where(fi =>
                {
                    if (fi.DeclaringType != this.Type)
                        return false;
                    return !fi.IsLiteral || fi.IsInitOnly;
                })
                .Select(fi => new TsProperty(tsModuleService, fi))
                .ToList();

            this.Constants = this.Type.GetFields().Where(fi => fi.DeclaringType == this.Type && fi.IsLiteral && !fi.IsInitOnly)
                .Select(fi => new TsProperty(tsModuleService, fi))
                .ToList();

            if (type.IsGenericType)
            {
                this.Name = type.Name.Remove(type.Name.IndexOf('`'));
                this.GenericArguments = type.GetGenericArguments()
                    .Select(t => TsType.Create(tsModuleService, t))
                    .ToList();
            }
            else
            {
                this.Name = type.Name;
                this.GenericArguments = Array.Empty<TsType>();
            }

            if (this.Type.BaseType != null && this.Type.BaseType != typeof(object) && this.Type.BaseType != typeof(ValueType))
                this.BaseType = new TsType(this.Type.BaseType);

            this.Interfaces = this.Type.GetInterfaces().Where(@interface => @interface.GetCustomAttribute<JsonIgnoreAttribute>(false) == null)
                .Select(t => tsModuleService.GetOrAddTsInterface(t)).ToList();
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
