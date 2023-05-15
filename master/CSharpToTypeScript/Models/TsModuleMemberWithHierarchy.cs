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
                .Select(pi => new TsProperty(tsModuleService, pi)).ToList();
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

            foreach (var property in Properties)
            {
                dependentTypes.UnionWith(tsNamespace.Members.Where(m => (m.Type == property.PropertyType.Type)
                    && !m.Type.IsGenericParameter));
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
                    && @interface.Properties.Count > 0));

                GetDependentTypes(dependentTypes, @interface.Interfaces, tsNamespace, generatorOptions);
            }
        }

        public virtual IReadOnlyList<CustomValidationRule> GetCustomValidations(TsGeneratorOptions generatorOptions)
        {
            if (!generatorOptions.HasFlag(TsGeneratorOptions.Properties))
            {
                return Array.Empty<CustomValidationRule>();
            }

            return Properties.SelectMany(p => p.ValidationRules.Where(r => r is CustomValidationRule))
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
            if (Properties.Count == 0 || !generatorOptions.HasFlag(TsGeneratorOptions.Properties))
            {
                return false;
            }

            return base.IsExportable(generatorOptions);
        }

        public virtual void UpdateCustomValidatorTypes(ITsModuleService tsModuleService)
        {
            foreach (CustomValidationRule customValidationRule in Properties.SelectMany(p =>
                p.ValidationRules.Where(r => r is CustomValidationRule)))
            {
                var validatorType = TsType.Create(tsModuleService, customValidationRule._CustomValidation.ValidatorType);
                validatorType.AddImplementedCustomValidationRule(customValidationRule);
                customValidationRule.TargetTypes.Add(this);
            }
        }
    }
}
