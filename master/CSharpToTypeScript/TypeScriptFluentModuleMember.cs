using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public class TypeScriptFluentModuleMember : TypeScriptFluent
    {
        public TsModuleMember Member { get; protected set; }

        internal TypeScriptFluentModuleMember(
          TypeScriptFluent fluentConfigurator,
          TsModuleMember member)
          : base(fluentConfigurator)
        {
            this.Member = member;
        }

        public TypeScriptFluentModuleMember Named(string name)
        {
            this.Member.Name = name;
            return this;
        }

        public TypeScriptFluentModuleMember ToModule(string moduleName)
        {
            this.Member.Module = new TsModule(moduleName);
            return this;
        }

        public TypeScriptFluentModuleMember Ignore()
        {
            if (this.Member is TsClass tsClass)
                tsClass.IsIgnored = true;
            return this;
        }
    }
}
