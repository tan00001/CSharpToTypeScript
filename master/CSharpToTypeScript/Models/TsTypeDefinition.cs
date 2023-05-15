using System.Diagnostics;
using System.Reflection;
using System.Security.AccessControl;
using System.Text.Json.Serialization;

namespace CSharpToTypeScript.Models
{
    [DebuggerDisplay("TsTypeDefinition - Name: {Name}")]
    public class TsTypeDefinition : TsModuleMemberWithHierarchy
    {
        public ICollection<TsProperty> Fields { get; private set; }

        public ICollection<TsProperty> Constants { get; private set; }

        public TsTypeDefinition(ITsModuleService tsModuleService, Type type)
          : base(tsModuleService, type)
        {
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

        public override IReadOnlyList<CustomValidationRule> GetCustomValidations(TsGeneratorOptions generatorOptions)
        {
            var customValidations = base.GetCustomValidations(generatorOptions);

            if (!generatorOptions.HasFlag(TsGeneratorOptions.Fields))
            {
                return customValidations;
            }

            return Properties.SelectMany(p => p.ValidationRules.Where(r => r is CustomValidationRule))
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

        public override bool HasMemeberInfoForOutput(TsGeneratorOptions generatorOptions)
        {
            if (base.HasMemeberInfoForOutput(generatorOptions))
            {
                return true;
            }

            return generatorOptions.HasFlag(TsGeneratorOptions.Fields)
                && Fields.Any(f => !f.HasIgnoreAttribute);
        }

        public override bool IsExportable(TsGeneratorOptions generatorOptions)
        {
            if (Type.IsGenericTypeParameter)
            {
                return false;
            }

            if (Properties.Count != 0 && generatorOptions.HasFlag(TsGeneratorOptions.Properties))
            {
                return true;
            }

            if (Fields.Count != 0 && generatorOptions.HasFlag(TsGeneratorOptions.Fields))
            {
                return true;
            }

            if (Constants.Count != 0 && generatorOptions.HasFlag(TsGeneratorOptions.Constants))
            {
                return true;
            }

            return false;
        }
    }
}
