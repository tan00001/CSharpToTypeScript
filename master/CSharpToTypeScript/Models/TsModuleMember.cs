﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using CSharpToTypeScript.Extensions;

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
            var dataContract = type.SafeGetCustomAttribute<DataContractAttribute>(false);

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
            Interfaces = Type.GetInterfaces().Where(@interface =>
                    @interface.SafeGetCustomAttribute<JsonIgnoreAttribute>(false) == null
                    && @interface.SafeGetCustomAttribute<NotMappedAttribute>(false) == null)
                .Where(i =>
                {
                    var nameSpaceName = i.Namespace ?? i.FullName;
                    if (nameSpaceName == null)
                    {
                        return true;
                    }
                    // Enumerables and Dictionaries get special treatments 
                    return !nameSpaceName.StartsWith("System.Collections");
                })
                .Select(t => tsModuleService.GetOrAddTsInterface(t)).ToList();

            ImplementedGenericTypes = new Dictionary<Type, IReadOnlyList<TsType>>();

            DataContract = this.Type.SafeGetCustomAttribute<DataContractAttribute>(false);

            this.Name = TsModuleService.GetTsTypeName(this.Type);

            if (type.IsGenericType)
            {
                this.GenericArguments = type.GetGenericArguments()
                    .Select(t => TsType.Create(tsModuleService, t))
                    .ToList();
            }
            else
            {
                this.GenericArguments = Array.Empty<TsType>();
            }

            Display = this.Type.SafeGetCustomAttribute<DisplayAttribute>(false);

            Ignore = this.Type.SafeGetCustomAttribute<JsonIgnoreAttribute>(false);

            NotMapped = this.Type.SafeGetCustomAttribute<NotMappedAttribute>(false);

            IsIgnored = Ignore != null || NotMapped != null;

            ModuleName = GetModuleName(type);

            string str = string.Empty;

            for (Type? declaringType = Type.DeclaringType; declaringType != null; declaringType = declaringType.DeclaringType)
                str = "." + declaringType.Name + str;

            NamespaceName = (DataContract?.Namespace ?? base.Type.Namespace) + str;
        }
    }
}
