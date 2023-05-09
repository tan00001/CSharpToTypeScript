using System.Reflection;
using System.Text.Json.Serialization;

namespace CSharpToTypeScript.Models
{
    // This can be either an interface or a class
    public abstract class TsModuleMemberWithHierarchy : TsModuleMember
    {
        public virtual ICollection<TsProperty> Properties { get; protected set; }

        public virtual IReadOnlyList<TsType> GenericArguments { get; protected set; }

        public TsType? BaseType { get; internal set; }

        public IList<TsInterface> Interfaces { get; internal set; }

        protected TsModuleMemberWithHierarchy(ITsModuleService tsModuleService, Type type)
          : base(type)
        {
            this.Properties = this.Type.GetProperties().Where(pi => pi.DeclaringType == this.Type)
                .Select(pi => new TsProperty(tsModuleService, pi)).ToList();

            if (type.IsGenericType)
            {
                this.GenericArguments = type.GetGenericArguments()
                    .Select(t => TsType.Create(tsModuleService, t))
                    .ToList();
                if (DataContract == null || string.IsNullOrEmpty(DataContract.Name))
                {
                    this.Name = type.Name.Remove(type.Name.IndexOf('`'));
                }
            }
            else
            {
                this.GenericArguments = Array.Empty<TsType>();
            }

            if (this.Type.BaseType != null && this.Type.BaseType != typeof(object) && this.Type.BaseType != typeof(ValueType))
                this.BaseType = new TsType(this.Type.BaseType);

            this.Interfaces = this.Type.GetInterfaces().Where(@interface => @interface.GetCustomAttribute<JsonIgnoreAttribute>(false) == null)
                .Select(t => tsModuleService.GetOrAddTsInterface(t)).ToList();
        }

        public override HashSet<TsModuleMember> GetDependentTypes(TsNamespace tsNamespace)
        {
            var dependentTypes = base.GetDependentTypes(tsNamespace);

            foreach (var porperty in Properties)
            {
                var dependentMember = tsNamespace.Members.FirstOrDefault(m => m.Type == porperty.PropertyType.Type);
                if (dependentMember != null)
                {
                    dependentTypes.Add(dependentMember);
                }
            }

            foreach (var argument in GenericArguments)
            {
                var dependentMember = tsNamespace.Members.FirstOrDefault(m => m.Type == argument.Type);
                if (dependentMember != null)
                {
                    dependentTypes.Add(dependentMember);
                }
            }

            return dependentTypes;
        }
    }
}
