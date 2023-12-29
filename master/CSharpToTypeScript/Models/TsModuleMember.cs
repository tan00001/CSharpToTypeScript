using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public IList<TsInterface> Interfaces { get; internal set; }

        public virtual IReadOnlyList<TsType> GenericArguments { get; protected set; }

        public IDictionary<Type, IReadOnlyList<TsType>> ImplementedGenericTypes { get; private set; }

        public DisplayAttribute? Display { get; private set; }
        public DataContractAttribute? DataContract { get; private set; }
        protected JsonIgnoreAttribute? Ignore { get; private set; }
        protected NotMappedAttribute? NotMapped { get; private set; }
        protected bool HasBindNeverAttribute { get; set; }

        public bool IsIgnored { get; set; }

        public bool RequiresAllExtensions { get; set; }

        public string NamespaceName
        {
            get;
            private set;
        }

        /// <summary>
        /// Get members of tsNamespace that depend on this current module member.
        /// tsNamespace is different from the namespace of the this current module member.
        /// </summary>
        /// <param name="tsNamespace"></param>
        /// <param name="generatorOptions"></param>
        /// <returns></returns>
        public virtual HashSet<TsModuleMember> GetDependentTypes(TsNamespace tsNamespace, TsGeneratorOptions generatorOptions)
        {
            var dependentTypes = new HashSet<TsModuleMember>();

            for (var parent = this.Type.BaseType; parent != null; parent = parent.BaseType)
            {
                dependentTypes.UnionWith(tsNamespace.Members.Where(m => m.Type == parent && !m.Type.IsGenericParameter));
            }

            GetDependentTypes(dependentTypes, this.Interfaces, tsNamespace, generatorOptions);

            return dependentTypes;
        }

        protected virtual void GetDependentTypes(HashSet<TsModuleMember> dependentTypes, IList<TsInterface> interfaces,
            TsNamespace tsNamespace, TsGeneratorOptions generatorOptions)
        {
            // We are only interested in interfaces with actual properties to export. The default behavior is do nothing.
            // Let the overriding function do the work.
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

        public virtual bool IsExportable(TsGeneratorOptions generatorOptions)
        {
            return !Type.IsGenericParameter;
        }

        protected TsModuleMember(ITsModuleService tsModuleService, Type type)
          : base(type)
        {
            this.Name = this.Type.Name;

            Interfaces = Type.GetInterfaces().Where(@interface =>
                    @interface.GetCustomAttribute<JsonIgnoreAttribute>(false) == null
                    && @interface.GetCustomAttribute<NotMappedAttribute>(false) == null)
                .Select(t => tsModuleService.GetOrAddTsInterface(t)).ToList();

            ImplementedGenericTypes = new Dictionary<Type, IReadOnlyList<TsType>>();

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

            Display = this.Type.GetCustomAttribute<DisplayAttribute>(false);

            DataContract = this.Type.GetCustomAttribute<DataContractAttribute>(false);

            if (DataContract != null)
            {
                if (!string.IsNullOrEmpty(DataContract.Name))
                    this.Name = DataContract.Name;
            }

            Ignore = this.Type.GetCustomAttribute<JsonIgnoreAttribute>(false);

            NotMapped = this.Type.GetCustomAttribute<NotMappedAttribute>(false);

            HasBindNeverAttribute = this.Type.GetCustomAttributes(false).Any(a => a.GetType().FullName == "Microsoft.AspNetCore.Mvc.ModelBinding.BindNeverAttribute");

            IsIgnored = Ignore != null || NotMapped != null || HasBindNeverAttribute;

            ModuleName = GetModuleName(type);

            string str = string.Empty;

            for (Type? declaringType = Type.DeclaringType; declaringType != null; declaringType = declaringType.DeclaringType)
                str = "." + declaringType.Name + str;

            NamespaceName = (DataContract?.Namespace ?? base.Type.Namespace) + str;
        }
    }
}
