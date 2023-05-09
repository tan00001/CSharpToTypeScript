using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CSharpToTypeScript.Models
{
    [DebuggerDisplay("TsInterface - Name: {Name}")]
    public class TsInterface : TsModuleMemberWithHierarchy
    {
        public TsInterface(ITsModuleService tsModuleService, Type type)
          : base(tsModuleService, type)
        {
        }
    }
}
