namespace CSharpToTypeScript.Models
{
    public interface ITsModuleService
    {
        TsModule GetModule(string name);

        IEnumerable<TsModule> GetModules();

        IEnumerable<TsNamespace> GetNamespaces();

        IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>> GetDependentNamespaces(TsNamespace tsNamespace);

        TsClass GetOrAddTsClass(Type clrType);

        TsInterface GetOrAddTsInterface(Type clrType);

        TsEnum GetOrAddTsEnum(Type clrType);
    }
}
