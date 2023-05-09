using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CSharpToTypeScript.Models
{
    [DebuggerDisplay("TsClass - Name: {Name}")]
    public class TsClass : TsModuleMemberWithHierarchy
    {
        public ICollection<TsProperty> Fields { get; private set; }

        public ICollection<TsProperty> Constants { get; private set; }

        public TsClass(ITsModuleService tsModuleService, Type type)
          : base(tsModuleService, type)
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
        }

        public virtual List<TsProperty> GetBaseProperties(bool includeProperties, bool includeFields)
        {
            if (BaseType is not TsClass baseClassType)
            {
                return new List<TsProperty>();
            }

            var baseProperties = baseClassType.GetBaseProperties(includeProperties, includeFields);

            if (includeProperties)
            {
                baseProperties.AddRange(baseClassType.Properties);
            }

            if (includeFields)
            {
                baseProperties.AddRange(baseClassType.Fields);
            }

            return baseProperties;
        }

        public virtual List<TsProperty> GetBaseRequiredProperties(bool includeProperties, bool includeFields)
        {
            if (BaseType is not TsClass baseClassType)
            {
                return new List<TsProperty>();
            }

            var baseRequiredProperties = baseClassType.GetBaseRequiredProperties(includeProperties, includeFields);

            if (includeProperties)
            {
                baseRequiredProperties.AddRange(baseClassType.Properties.Where(p => p.IsRequired));
            }

            if (includeFields)
            {
                baseRequiredProperties.AddRange(baseClassType.Fields.Where(p => p.IsRequired));
            }

            return baseRequiredProperties;
        }

        public override HashSet<TsModuleMember> GetDependentTypes(TsNamespace tsNamespace)
        {
            var dependentTypes = base.GetDependentTypes(tsNamespace);

            foreach (var field in Fields)
            {
                var dependentMember = tsNamespace.Members.FirstOrDefault(m => m.Type == field.PropertyType.Type);
                if (dependentMember != null)
                {
                    dependentTypes.Add(dependentMember);
                }
            }

            foreach (var constant in Constants)
            {
                var dependentMember = tsNamespace.Members.FirstOrDefault(m => m.Type == constant.PropertyType.Type);
                if (dependentMember != null)
                {
                    dependentTypes.Add(dependentMember);
                }
            }

            return dependentTypes;
        }
    }
}
