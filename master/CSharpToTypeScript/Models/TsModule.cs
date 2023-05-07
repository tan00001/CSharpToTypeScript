using System.Diagnostics.CodeAnalysis;

namespace CSharpToTypeScript.Models
{
    public class TsModule
    {
        public string Name { get; set; }

        private readonly IDictionary<string, TsNamespace> Namespaces;

        public TsModule(string name, IDictionary<string, TsNamespace> namespaces)
        {
            this.Namespaces = namespaces;

            this.Name = name;
        }

        internal void Add(TsModuleMember toAdd)
        {
            toAdd.Module = this;

            var memberNamespace = toAdd.NamespaceName;
            if (string.IsNullOrEmpty(memberNamespace))
            {
                memberNamespace = this.Name;
            }

            if (Namespaces.TryGetValue(memberNamespace, out var @namespace))
            {
                @namespace.Add(toAdd);
                return;
            }

            @namespace = new TsNamespace(memberNamespace);
            Namespaces.Add(memberNamespace, @namespace);
            @namespace.Add(toAdd);
        }

        internal void Remove(TsModuleMember toRemove)
        {
            var memberNamespace = toRemove.NamespaceName;
            if (string.IsNullOrEmpty(memberNamespace))
            {
                memberNamespace = this.Name;
            }

            if (Namespaces.TryGetValue(memberNamespace, out var @namespace))
            {
                @namespace.Remove(toRemove);
            }
        }

        public bool TryGetMember(string namespaceName, string name, [MaybeNullWhen(false)] out TsModuleMember? member)
        {
            if (!Namespaces.TryGetValue(namespaceName, out var @namespace))
            {
                member = null;
                return false;
            }

            member = @namespace.Members.FirstOrDefault(m => m.Name == name);
            return member != null;
        }
    }
}
