using System.Diagnostics;

namespace CSharpToTypeScript.Models
{
    [DebuggerDisplay("TsInterface - Name: {Name}")]
    public class TsInterface : TsModuleMemberWithHierarchy
    {
        public TsInterface(ITsModuleService tsModuleService, Type type)
          : base(tsModuleService, type)
        {
        }

        public bool IsExcludedFromExport()
        {
            return TsType.ExcludedNamespacePrefixes.Any(p => this.Name.StartsWith(p) || p.StartsWith(this.Name)
                || this.NamespaceName.StartsWith(p) || p.StartsWith(this.NamespaceName));
        }
    }
}
