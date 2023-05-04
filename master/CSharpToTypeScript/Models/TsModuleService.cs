using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Models
{
    internal class TsModuleService : ITsModuleService
    {
        private readonly Dictionary<string, TsModule> Modules = new();

        #region IModuleService
        public TsModule GetModule(string name)
        {
            if (Modules.TryGetValue(name, out TsModule? value))
                return value;
            TsModule tsModule = new(name);
            Modules[name] = tsModule;
            return tsModule;
        }

        public IEnumerable<TsModule> GetModules()
        {
            return Modules.Values;
        }

        public TsClass GetOrAddTsClass(Type clrType)
        {
            var moduleName = TsModuleMember.GetModuleName(clrType);
            TsModule module = GetModule(moduleName);

            if (!module.TryGetMember(TsModuleMember.GetNamespaceName(clrType),
                clrType.Name, out TsModuleMember? tsModuleMember))
            {
                tsModuleMember = new TsClass(clrType);
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
                tsModuleMember = new TsInterface(clrType);
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
