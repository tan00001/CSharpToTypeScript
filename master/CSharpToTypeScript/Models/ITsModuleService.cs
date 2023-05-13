using Microsoft.VisualBasic;

namespace CSharpToTypeScript.Models
{
    public interface ITsModuleService
    {
        IReadOnlyList<TsType> BuildGenericArgumentList(Type clrType);

        TsModule GetModule(string name);

        IEnumerable<TsModule> GetModules();

        IEnumerable<TsNamespace> GetNamespaces();

        IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>> GetDependentNamespaces(TsNamespace tsNamespace,
            TsGeneratorOptions generatorOptions);

        TsClass GetOrAddTsClass(Type clrType);

        TsTypeDefinition GetOrAddTsTypeDefinition(Type clrType);

        TsInterface GetOrAddTsInterface(Type clrType);

        TsEnum GetOrAddTsEnum(Type clrType);

        bool IsProcessing(Type type);
    }
}
