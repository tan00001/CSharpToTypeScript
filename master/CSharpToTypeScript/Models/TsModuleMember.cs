#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CSharpToTypeScript.Models
{
    public abstract class TsModuleMember : TsType
    {
        public string ModuleName
        {
            get;
            set;
        }

        private TsModule? _Module = null;
        public TsModule? Module
        {
            get
            {
                return _Module;
            }
            set
            {
                if(_Module == value )
                {
                    return;
                }
                if (_Module != null)
                {
                    _Module.Remove(this);
                }

                _Module = value;
                if (_Module != null)
                {
                    _Module.Add(this);
                }
            }
        }

        public TsNamespace? Namespace
        {
            get; set;
        }

        public string Name { get; set; }

        public DisplayAttribute? Display { get; private set; }
        public DataContractAttribute? DataContract { get; private set; }
        public JsonIgnoreAttribute? Ignore { get; private set; }

        public bool IsIgnored { get; set; }

        public string NamespaceName
        {
            get;
            private set;
        }

        protected TsModuleMember(Type type)
          : base(type)
        {
            this.Name = this.Type.Name;

            Display = this.Type.GetCustomAttribute<DisplayAttribute>(false);

            if (Display != null)
            {
                if (!string.IsNullOrEmpty(Display.Name))
                    this.Name = Display.Name!;
            }

            DataContract = this.Type.GetCustomAttribute<DataContractAttribute>(false);

            Ignore = this.Type.GetCustomAttribute<JsonIgnoreAttribute>(false);

            IsIgnored = Ignore != null;

            ModuleName = Path.GetFileNameWithoutExtension(type.Module.Name);

            string str = string.Empty;

            for (Type? declaringType = Type.DeclaringType; declaringType != null; declaringType = declaringType.DeclaringType)
                str = "." + declaringType.Name + str;

            NamespaceName = (DataContract?.Namespace ?? base.Type.Namespace) + str;
        }
    }
}
