using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSharpToTypeScript.Models
{
    public interface ITsModuleService
    {
        TsModule GetModule(string name);

        IEnumerable<TsModule> GetModules();

        TsClass GetOrAddTsClass(Type clrType);

        TsInterface GetOrAddTsInterface(Type clrType);

        TsEnum GetOrAddTsEnum(Type clrType);
    }
}
