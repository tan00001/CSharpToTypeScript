namespace CSharpToTypeScript.Models
{
    internal class TsModuleService : ITsModuleService
    {
        private readonly Dictionary<string, TsModule> Modules = new();

        public IDictionary<string, TsNamespace> Namespaces { get; private set; }

        public TsModuleService(string? defaultNamespaceName = null) 
        {
            this.Namespaces = new SortedList<string, TsNamespace>();

            if (!string.IsNullOrEmpty(defaultNamespaceName))
            {
                this.Namespaces.Add(defaultNamespaceName, new TsNamespace(defaultNamespaceName));
            };
        }

        #region IModuleService
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

        public IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>> GetDependentNamespaces(TsNamespace tsNamespace)
        {
            return Namespaces.Where(n => n.Key != tsNamespace.Name).ToDictionary(n => n.Key, n => tsNamespace.GetDependentMembers(n.Value))
                .Where(n => n.Value.Count > 0).ToDictionary(n => n.Key, n => n.Value);
        }

        public TsClass GetOrAddTsClass(Type clrType)
        {
            var moduleName = TsModuleMember.GetModuleName(clrType);
            TsModule module = GetModule(moduleName);

            if (!module.TryGetMember(TsModuleMember.GetNamespaceName(clrType),
                clrType.Name, out TsModuleMember? tsModuleMember))
            {
                tsModuleMember = new TsClass(this, clrType);
                module.Add(tsModuleMember);
                return (TsClass)tsModuleMember;
            }

            if (tsModuleMember is TsClass classType)
            {
                return classType;
            }

            throw new Exception("Name conflict. \"" + tsModuleMember!.Name + "\" is defined more than once.");
        }

        public TsInterface GetOrAddTsInterface(Type clrType)
        {
            var moduleName = TsModuleMember.GetModuleName(clrType);
            TsModule module = GetModule(moduleName);

            if (!module.TryGetMember(TsModuleMember.GetNamespaceName(clrType),
                clrType.Name, out TsModuleMember? tsModuleMember))
            {
                tsModuleMember = new TsInterface(this, clrType);
                module.Add(tsModuleMember);
                return (TsInterface)tsModuleMember;
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
                clrType.Name, out TsModuleMember? tsModuleMember))
            {
                tsModuleMember = new TsEnum(clrType);
                module.Add(tsModuleMember);
                return (TsEnum)tsModuleMember;
            }

            if (tsModuleMember is TsEnum enumType)
            {
                return enumType;
            }

            throw new Exception("Name conflict. \"" + tsModuleMember!.Name + "\" is defined more than once.");
        }
        #endregion // IModuleService
    }
}
