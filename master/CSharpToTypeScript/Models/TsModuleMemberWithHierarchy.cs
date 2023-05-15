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

        public override HashSet<TsModuleMember> GetDependentTypes(TsNamespace tsNamespace, TsGeneratorOptions generatorOptions)
        {
            var dependentTypes = base.GetDependentTypes(tsNamespace, generatorOptions);

            foreach (var porperty in Properties)
            {
                var dependentMember = tsNamespace.Members.FirstOrDefault(m => m.Type == porperty.PropertyType.Type && !m.Type.IsGenericParameter);
                if (dependentMember != null)
                {
                    dependentTypes.Add(dependentMember);
                }
            }

            foreach (var argument in GenericArguments)
            {
                var dependentMember = tsNamespace.Members.FirstOrDefault(m => m.Type == argument.Type && !m.Type.IsGenericParameter);
                if (dependentMember != null)
                {
                    dependentTypes.Add(dependentMember);
                }
            }

            return dependentTypes;
        }

        protected override void GetDependentTypes(HashSet<TsModuleMember> dependentTypes, IList<TsInterface> interfaces,
            TsNamespace tsNamespace, TsGeneratorOptions generatorOptions)
        {
            if (!generatorOptions.HasFlag(TsGeneratorOptions.Properties))
            {
                return;
            }

            foreach (var @interface in interfaces)
            {
                var dependentMember = tsNamespace.Members.FirstOrDefault(m => m.Type == @interface.Type
                    && @interface.Properties.Count > 0);
                if (dependentMember != null)
                {
                    dependentTypes.Add(dependentMember);
                }

                GetDependentTypes(dependentTypes, @interface.Interfaces, tsNamespace, generatorOptions);
            }
        }

        public override bool IsExportable(TsGeneratorOptions generatorOptions)
        {
            if (Properties.Count == 0 || !generatorOptions.HasFlag(TsGeneratorOptions.Properties))
            {
                return false;
            }

            return base.IsExportable(generatorOptions);
        }
    }
}
