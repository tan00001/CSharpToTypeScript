using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSharpToTypeScript.Models
{
    // This can be either an interface or a class
    public abstract class TsModuleMemberWithHierarchy : TsModuleMember
    {
        public virtual ICollection<TsProperty> Properties { get; protected set; }

        protected TsModuleMemberWithHierarchy(ITsModuleService tsModuleService, Type type)
          : base(tsModuleService, type)
        {
            this.Properties = this.Type.GetProperties().Where(pi => pi.DeclaringType == this.Type)
                .Select(pi => new TsProperty(tsModuleService, pi, this)).ToList();
        }

        public virtual IReadOnlyList<TsProperty> GetMemeberInfoForOutput(TsGeneratorOptions generatorOptions)
        {
            if (!generatorOptions.HasFlag(TsGeneratorOptions.Properties))
            {
                return Array.Empty<TsProperty>();
            }

            return Properties.Where(p => !p.HasIgnoreAttribute).ToArray();
        }

        /// <summary>
        /// Get members of tsNamespace that depend on this current module member.
        /// tsNamespace is different from the namespace of the this current module member.
        /// </summary>
        /// <param name="tsNamespace"></param>
        /// <param name="generatorOptions"></param>
        /// <returns>Members of tsNamespace that depend on this current module member</returns>
        public override HashSet<TsModuleMember> GetDependentTypes(TsNamespace tsNamespace, TsGeneratorOptions generatorOptions)
        {
            var dependentTypes = base.GetDependentTypes(tsNamespace, generatorOptions);

            foreach (var property in Properties.Where(p => !p.HasIgnoreAttribute))
            {
                dependentTypes.UnionWith(tsNamespace.Members.Where(m => (m.Type == property.PropertyType.Type)
                    && !m.Type.IsGenericParameter));

                foreach (CustomValidationRule validationRule in property.ValidationRules.Where(r => r is CustomValidationRule))
                {
                    dependentTypes.UnionWith(tsNamespace.Members.Where(m => (m.Type == validationRule._CustomValidation.ValidatorType)
                                           && !m.Type.IsGenericParameter));
                }
            }

            foreach (var argument in GenericArguments)
            {
                dependentTypes.UnionWith(tsNamespace.Members.Where(m => m.Type == argument.Type && !m.Type.IsGenericParameter));
            }

            return dependentTypes;
        }

        /// <summary>
        /// Get dependent types from the interfaces here because we are only interested in interfaces on members
        /// that have properties.
        /// </summary>
        /// <param name="dependentTypes"></param>
        /// <param name="interfaces"></param>
        /// <param name="tsNamespace"></param>
        /// <param name="generatorOptions"></param>
        protected override void GetDependentTypes(HashSet<TsModuleMember> dependentTypes, IList<TsInterface> interfaces,
            TsNamespace tsNamespace, TsGeneratorOptions generatorOptions)
        {
            if (!generatorOptions.HasFlag(TsGeneratorOptions.Properties))
            {
                return;
            }

            foreach (var @interface in interfaces)
            {
                dependentTypes.UnionWith(tsNamespace.Members.Where(m => m.Type == @interface.Type
                    && @interface.Properties.Any(p => !p.HasIgnoreAttribute)));

                GetDependentTypes(dependentTypes, @interface.Interfaces, tsNamespace, generatorOptions);
            }
        }

        public virtual IReadOnlyList<CustomValidationRule> GetCustomValidations(TsGeneratorOptions generatorOptions)
        {
            if (!generatorOptions.HasFlag(TsGeneratorOptions.Properties))
            {
                return Array.Empty<CustomValidationRule>();
            }

            return Properties.Where(p => !p.HasIgnoreAttribute).SelectMany(p => p.ValidationRules.Where(r => r is CustomValidationRule))
                .Cast<CustomValidationRule>()
                .ToList();
        }

        public virtual bool HasMemeberInfoForOutput(TsGeneratorOptions generatorOptions)
        {
            if (!generatorOptions.HasFlag(TsGeneratorOptions.Properties))
            {
                return false;
            }

            return Properties.Any(p => !p.HasIgnoreAttribute);
        }

        public override bool IsExportable(TsGeneratorOptions generatorOptions)
        {
            if (!generatorOptions.HasFlag(TsGeneratorOptions.Properties) || !Properties.Any(p => !p.HasIgnoreAttribute))
            {
                return false;
            }

            return base.IsExportable(generatorOptions);
        }

        public virtual void UpdateCustomValidatorTypes(ITsModuleService tsModuleService)
        {
            foreach (CustomValidationRule customValidationRule in Properties.Where(p => !p.HasIgnoreAttribute).SelectMany(p =>
                p.ValidationRules.Where(r => r is CustomValidationRule)))
            {
                var validatorType = TsType.Create(tsModuleService, customValidationRule._CustomValidation.ValidatorType);
                validatorType.AddImplementedCustomValidationRule(customValidationRule);
                customValidationRule.TargetTypes.Add(this);
            }
        }
    }
}
