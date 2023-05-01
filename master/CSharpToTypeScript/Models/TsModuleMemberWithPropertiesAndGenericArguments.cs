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

        public Int32 GetDerivationDepth()
        {
            int depth = 0;
            for (var parent = this.Type.BaseType; parent != null; parent = parent.BaseType)
            {
                ++depth;
            }
            return depth;
        }
    }
}
