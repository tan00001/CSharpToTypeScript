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
    [DebuggerDisplay("TsInterface - Name: {Name}")]
    public class TsInterface : TsModuleMemberWithPropertiesAndGenericArguments
    {
        public override ICollection<TsProperty> Properties { get; protected set; }

        public override IReadOnlyList<TsType> GenericArguments { get; protected set; }

        public TsType? BaseType { get; internal set; }

        public IList<TsType> Interfaces { get; internal set; }

        public TsInterface(ITsModuleService tsModuleService, Type type)
          : base(type)
        {
            this.Properties = this.Type.GetProperties().Where(pi => pi.DeclaringType == this.Type)
                .Select(pi => new TsProperty(tsModuleService, pi)).ToList();

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

            if (this.Type.BaseType != null && this.Type.BaseType.IsInterface)
                this.BaseType = new TsType(this.Type.BaseType);

            Type[] interfaces = this.Type.GetInterfaces();

            this.Interfaces = interfaces.Where(@interface => @interface.GetCustomAttribute<JsonIgnoreAttribute>(false) == null)
                .Select(t => TsType.Create(tsModuleService, t)).ToList();
        }

        public Int32 GetDerivationDepth()
        {
            return GetDerivationDepth(this.Interfaces);
        }

        public Int32 GetDerivationDepth(IList<TsType> interfaces)
        {
            if (interfaces.Count == 0)
            {
                return 0;
            }

            int depth = 0;

            foreach (var parent in this.Interfaces)
            {
                if (parent is TsInterface tsInterface)
                {
                    var parentDepth = GetDerivationDepth(tsInterface.Interfaces);
                    if (depth < parentDepth)
                    {
                        depth = parentDepth;
                    }
                }
            }

            return depth + 1; // +1 for the current depth
        }
    }
}
