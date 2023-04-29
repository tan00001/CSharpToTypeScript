#nullable enable
using System;

namespace CSharpToTypeScript.Models
{
    public abstract class TsModuleMember : TsType
    {
        private TsModule _module;

        public TsModule Module
        {
            get => this._module;
            set
            {
                this._module.Remove(this);
                this._module = value;
                this._module.Add(this);
            }
        }

        public string Name { get; set; }

        protected TsModuleMember(Type type)
          : base(type)
        {
            string str = string.Empty;
            for (Type? declaringType = type.DeclaringType; declaringType != null; declaringType = declaringType.DeclaringType)
                str = "." + declaringType.Name + str;
            _module = new TsModule(type.Namespace + str);
            this.Name = this.Type.Name;
        }
    }
}
