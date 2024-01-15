using System.Formats.Asn1;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Serialization;

using CSharpToTypeScript.Extensions;

namespace CSharpToTypeScript.Models
{
    internal class TsModuleService : ITsModuleService
    {
        private readonly Dictionary<string, TsModule> Modules = new();

        public IDictionary<string, TsNamespace> Namespaces { get; private set; }

        private static readonly Stack<Type> _TypesBeingProcessed = new();

        public TsModuleService(string? defaultNamespaceName = null) 
        {
            this.Namespaces = new SortedList<string, TsNamespace>();

            if (!string.IsNullOrEmpty(defaultNamespaceName))
            {
                this.Namespaces.Add(defaultNamespaceName, new TsNamespace(defaultNamespaceName));
            };
        }

        #region IModuleService
        public IReadOnlyList<TsType> BuildGenericArgumentList(Type clrType)
        {
            List<TsType> tsTypes = new(clrType.GetGenericArguments().Length);

            foreach (var type in clrType.GetGenericArguments())
            {
                tsTypes.Add(TsType.Create(this, type));
            }

            return tsTypes;
        }

        public TsModule GetModule(string name)
        {
            if (Modules.TryGetValue(name, out TsModule? value))
                return value;
            TsModule tsModule = new(name, Namespaces);
            Modules[name] = tsModule;
            return tsModule;
        }

        public IEnumerable<TsModule> GetModules()
        {
            return Modules.Values;
        }

        public IEnumerable<TsNamespace> GetNamespaces()
        {
            return Namespaces.Values;
        }

        public IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>> GetDependentNamespaces(TsNamespace tsNamespace,
            TsGeneratorOptions generatorOptions)
        {
            return Namespaces.Where(n => n.Key != tsNamespace.Name)
                .ToDictionary(n => n.Key, n => tsNamespace.GetDependentMembers(n.Value, generatorOptions))
                .Where(n => n.Value.Count > 0).ToDictionary(n => n.Key, n => n.Value);
        }

        public TsClass GetOrAddTsClass(Type clrType)
        {
            var moduleName = TsModuleMember.GetModuleName(clrType);
            TsModule module = GetModule(moduleName);

            if (!module.TryGetMember(TsModuleMember.GetNamespaceName(clrType),
                GetTsTypeName(clrType), out TsModuleMember? tsModuleMember))
            {
                _TypesBeingProcessed.Push(clrType);
                var tsClass = new TsClass(this, clrType);
                module.Add(tsClass);
                _TypesBeingProcessed.Pop();
                tsClass.UpdateCustomValidatorTypes(this);
                return tsClass;
            }

            if (tsModuleMember is TsClass classType)
            {
                return classType;
            }

            throw new Exception("Name conflict. \"" + tsModuleMember!.Name + "\" is defined more than once.");
        }

        public TsTypeDefinition GetOrAddTsTypeDefinition(Type clrType)
        {
            var moduleName = TsModuleMember.GetModuleName(clrType);
            TsModule module = GetModule(moduleName);

            if (!module.TryGetMember(TsModuleMember.GetNamespaceName(clrType),
                GetTsTypeName(clrType), out TsModuleMember? tsModuleMember))
            {
                _TypesBeingProcessed.Push(clrType);
                var tsTypeDefinition = new TsTypeDefinition(this, clrType);
                module.Add(tsTypeDefinition);
                _TypesBeingProcessed.Pop();
                tsTypeDefinition.UpdateCustomValidatorTypes(this);
                return tsTypeDefinition;
            }

            if (tsModuleMember is TsTypeDefinition typeDefinition)
            {
                return typeDefinition;
            }

            throw new Exception("Name conflict. \"" + tsModuleMember!.Name + "\" is defined more than once.");
        }

        public TsInterface GetOrAddTsInterface(Type clrType)
        {
            var moduleName = TsModuleMember.GetModuleName(clrType);
            TsModule module = GetModule(moduleName);

            if (!module.TryGetMember(TsModuleMember.GetNamespaceName(clrType),
                GetTsTypeName(clrType), out TsModuleMember? tsModuleMember))
            {
                var tsInterface = new TsInterface(this, clrType);
                module.Add(tsInterface);
                tsInterface.UpdateCustomValidatorTypes(this);
                return tsInterface;
            }

            if (tsModuleMember is TsInterface interfaceType)
            {
                return interfaceType;
            }

            throw new Exception("Name conflict. \"" + tsModuleMember!.Name + "\" is defined more than once.");
        }

        public TsEnum GetOrAddTsEnum(Type clrType)
        {
            var moduleName = TsModuleMember.GetModuleName(clrType);
            TsModule module = GetModule(moduleName);

            if (!module.TryGetMember(TsModuleMember.GetNamespaceName(clrType),
                GetTsTypeName(clrType), out TsModuleMember? tsModuleMember))
            {
                tsModuleMember = new TsEnum(this, clrType);
                module.Add(tsModuleMember);
                return (TsEnum)tsModuleMember;
            }

            if (tsModuleMember is TsEnum enumType)
            {
                return enumType;
            }

            throw new Exception("Name conflict. \"" + tsModuleMember!.Name + "\" is defined more than once.");
        }

        public bool IsProcessing(Type type)
        {
            return _TypesBeingProcessed.Contains(type);
        }
        #endregion // IModuleService

        #region Private Methods
        private static string GetTsTypeName(Type type)
        {
            var dataContract = type.SafeGetCustomAttribute<DataContractAttribute>(false);
            if (dataContract != null && !string.IsNullOrEmpty(dataContract.Name))
            {
                return dataContract.Name;
            }

            return type.Name;
        }
        #endregion
    }
}
