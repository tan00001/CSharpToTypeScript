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

        public abstract IList<TsType> GenericArguments { get; protected set; }

        protected TsModuleMemberWithPropertiesAndGenericArguments(Type type)
          : base(type)
        {
        }
    }
}
