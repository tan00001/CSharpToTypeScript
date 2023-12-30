using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace CSharpToTypeScript.Models
{
    [DebuggerDisplay("TsClass - Name: {Name}")]
    public class TsClass : TsModuleMemberWithHierarchy
    {
        public ICollection<TsProperty> Fields { get; private set; }

        public ICollection<TsProperty> Constants { get; private set; }

        public TsClass? BaseType { get; internal set; }

        public TsClass(ITsModuleService tsModuleService, Type type)
          : base(tsModuleService, type)
        {
            if (this.Type.BaseType != null && TsType.GetTypeFamily(this.Type.BaseType) == TsTypeFamily.Class)
                this.BaseType = tsModuleService.GetOrAddTsClass(this.Type.BaseType);

            Fields = Type.GetFields().Where(fi =>
                {
                    if (fi.DeclaringType != Type)
                        return false;
                    return !fi.IsLiteral || fi.IsInitOnly;
                })
                .Select(fi => new TsProperty(tsModuleService, fi, this))
                .ToList();

            Constants = Type.GetFields().Where(fi => fi.DeclaringType == Type && fi.IsLiteral && !fi.IsInitOnly)
                .Select(fi => new TsProperty(tsModuleService, fi, this))
                .ToList();
        }

        public virtual List<TsProperty> GetBaseProperties(bool includeProperties, bool includeFields)
        {
            if (BaseType == null)
            {
                return new List<TsProperty>();
            }

            var baseProperties = BaseType.GetBaseProperties(includeProperties, includeFields);

            if (includeProperties)
            {
                baseProperties.AddRange(BaseType.Properties);
            }

            if (includeFields)
            {
                baseProperties.AddRange(BaseType.Fields);
            }

            return baseProperties;
        }

        public virtual List<TsProperty> GetBaseRequiredProperties(bool includeProperties, bool includeFields)
        {
            if (BaseType == null)
            {
                return new List<TsProperty>();
            }

            var baseRequiredProperties = BaseType.GetBaseRequiredProperties(includeProperties, includeFields);

            if (includeProperties)
            {
                baseRequiredProperties.AddRange(BaseType.Properties.Where(p => p.IsRequired));
            }

            if (includeFields)
            {
                baseRequiredProperties.AddRange(BaseType.Fields.Where(p => p.IsRequired));
            }

            return baseRequiredProperties;
        }

        public override IReadOnlyList<CustomValidationRule> GetCustomValidations(TsGeneratorOptions generatorOptions)
        {
            var customValidations = base.GetCustomValidations(generatorOptions);

            if (!generatorOptions.HasFlag(TsGeneratorOptions.Fields))
            {
                return customValidations;
            }

            return Fields.SelectMany(p => p.ValidationRules.Where(r => r is CustomValidationRule))
                .Cast<CustomValidationRule>()
                .Union(customValidations, CustomValidationRule.Comparer)
                .ToList();
        }

        public override HashSet<TsModuleMember> GetDependentTypes(TsNamespace tsNamespace, TsGeneratorOptions generatorOptions)
        {
            var dependentTypes = base.GetDependentTypes(tsNamespace, generatorOptions);

            foreach (var field in Fields)
            {
                dependentTypes.UnionWith(tsNamespace.Members.Where(m => (m.Type == field.PropertyType.Type)
                    && !m.Type.IsGenericParameter));
            }

            foreach (var constant in Constants)
            {
                dependentTypes.UnionWith(tsNamespace.Members.Where(m => m.Type == constant.PropertyType.Type
                    && !m.Type.IsGenericParameter));
            }

            return dependentTypes;
        }

        public override IReadOnlyList<TsProperty> GetMemeberInfoForOutput(TsGeneratorOptions generatorOptions)
        {
            List<TsProperty> properties;
            if (generatorOptions.HasFlag(TsGeneratorOptions.Constants))
            {
                properties = Constants.Where(c => !c.HasIgnoreAttribute).ToList();
            }
            else
            {
                properties = new();
            }

            if (generatorOptions.HasFlag(TsGeneratorOptions.Properties))
            {
                properties.AddRange(base.GetMemeberInfoForOutput(generatorOptions));
            }

            if (generatorOptions.HasFlag(TsGeneratorOptions.Fields))
            {
                properties.AddRange(Fields.Where(c => !c.HasIgnoreAttribute));
            }

            return properties;
        }

        public override bool HasMemeberInfoForOutput(TsGeneratorOptions generatorOptions)
        {
            if (base.HasMemeberInfoForOutput(generatorOptions))
            {
                return true;
            }

            if (generatorOptions.HasFlag(TsGeneratorOptions.Fields)
                && Fields.Any(f => !f.HasIgnoreAttribute))
            {
                return true;
            }

            if (BaseType == null)
            {
                return false;
            }

            return BaseType.HasMemeberInfoForOutput(generatorOptions);
        }

        public override bool IsExportable(TsGeneratorOptions generatorOptions)
        {
            if (Type.IsGenericTypeParameter)
            {
                return false;
            }

            if (generatorOptions.HasFlag(TsGeneratorOptions.Properties) && Properties.Any(p => !p.HasIgnoreAttribute))
            {
                return true;
            }

            if (generatorOptions.HasFlag(TsGeneratorOptions.Fields) && Fields.Any(f => !f.HasIgnoreAttribute))
            {
                return true;
            }

            if (generatorOptions.HasFlag(TsGeneratorOptions.Constants) && Constants.Any(c => !c.HasIgnoreAttribute))
            {
                return true;
            }

            if (BaseType is not TsClass baseClass)
            {
                return false;
            }

            return baseClass.IsExportable(generatorOptions);
        }
    }
}
