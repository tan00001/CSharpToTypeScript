using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Models
{
    // This can be either an interface or a class
    public abstract class TsModuleMemberWithPropertiesAndGenericArguments : TsModuleMember
    {
        public abstract ICollection<TsProperty> Properties { get; protected set; }

        public abstract IReadOnlyList<TsType> GenericArguments { get; protected set; }

        protected TsModuleMemberWithPropertiesAndGenericArguments(Type type)
          : base(type)
        {
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
