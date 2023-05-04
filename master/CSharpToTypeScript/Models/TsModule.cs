using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CSharpToTypeScript.Models
{
    public class TsModule
    {
        public string Name { get; set; }

        public IDictionary<string, TsNamespace> Namespaces { get; private set; }

        public TsNamespace DefaultNamespace { get; private set; }

        public TsModule(string name)
        {
            DefaultNamespace = new TsNamespace(name);

            this.Namespaces = new SortedList<string, TsNamespace>()
            {
                { name, DefaultNamespace }
            };

            this.Name = name;
        }

        internal void Add(TsModuleMember toAdd)
        {
            toAdd.Module = this;

            var memberNamespace = toAdd.NamespaceName;
            if (string.IsNullOrEmpty(memberNamespace))
            {
                DefaultNamespace.Add(toAdd);
                return;
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
                DefaultNamespace.Remove(toRemove);
                return;
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
