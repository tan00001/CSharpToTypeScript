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

                _Module?.Remove(this);

                _Module = value;

                _Module?.Add(this);
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

        public virtual HashSet<TsModuleMember> GetDependentTypes(TsNamespace tsNamespace)
        {
            var dependentTypes = new HashSet<TsModuleMember>();

            for (var parent = this.Type.BaseType; parent != null; parent = parent.BaseType)
            {
                var dependentMember = tsNamespace.Members.FirstOrDefault(m => m.Type == parent);
                if (dependentMember != null)
                {
                    dependentTypes.Add(dependentMember);
                }
            }

            GetDependentTypes(dependentTypes, this.Type.GetInterfaces(), tsNamespace);

            return dependentTypes;
        }

        private static void GetDependentTypes(HashSet<TsModuleMember> dependentTypes, IList<Type> interfaces, TsNamespace tsNamespace)
        {
            foreach (var @interface in interfaces)
            {
                var dependentMember = tsNamespace.Members.FirstOrDefault(m => m.Type == @interface);
                if (dependentMember != null)
                {
                    dependentTypes.Add(dependentMember);
                }
            }
        }

        public static string GetModuleName(Type type)
        {
            return Path.GetFileNameWithoutExtension(type.Module.Name);
        }

        public static string GetNamespaceName(Type type)
        {
            var dataContract = type.GetCustomAttribute<DataContractAttribute>(false);

            string str = string.Empty;

            for (Type? declaringType = type.DeclaringType; declaringType != null; declaringType = declaringType.DeclaringType)
                str = "." + declaringType.Name + str;

            return (dataContract?.Namespace ?? type.Namespace) + str;
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

            ModuleName = GetModuleName(type);

            string str = string.Empty;

            for (Type? declaringType = Type.DeclaringType; declaringType != null; declaringType = declaringType.DeclaringType)
                str = "." + declaringType.Name + str;

            NamespaceName = (DataContract?.Namespace ?? base.Type.Namespace) + str;
        }
    }
}
