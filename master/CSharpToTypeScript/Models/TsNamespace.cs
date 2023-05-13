namespace CSharpToTypeScript.Models
{
    public class TsNamespace
    {
        private readonly ISet<TsModuleMember> _members;

        public string Name { get; set; }

        public TsNamespace(string name)
        {
            this._members = new HashSet<TsModuleMember>();
            this.Name = name;
        }

        public IEnumerable<TsClass> Classes => this._members.OfType<TsClass>();

        public IEnumerable<TsTypeDefinition> TypeDefinitions => this._members.OfType<TsTypeDefinition>();

        public IEnumerable<TsInterface> Interfaces => this._members.OfType<TsInterface>();

        public IEnumerable<TsEnum> Enums => this._members.OfType<TsEnum>();

        public IEnumerable<TsModuleMember> Members => this._members;

        internal void Add(TsModuleMember toAdd)
        {
            this._members.Add(toAdd);
            toAdd.Namespace = this;
        }

        internal IReadOnlyCollection<TsModuleMember> GetDependentMembers(TsNamespace tsNamespace, TsGeneratorOptions generatorOptions)
        {
            HashSet<TsModuleMember>? dependentTypes = null;

            foreach (var member in this._members)
            {
                var memberDependentTypes = member.GetDependentTypes(tsNamespace, generatorOptions);
                if (memberDependentTypes.Count == 0)
                {
                    continue;
                }

                if (dependentTypes == null)
                {
                    dependentTypes = memberDependentTypes;
                }
                else
                {
                    dependentTypes.UnionWith(memberDependentTypes);
                }
            }

            return dependentTypes ?? (IReadOnlyCollection<TsModuleMember>)Array.Empty<TsModuleMember>();
        }

        public bool HasExportableMembers(TsGeneratorOptions generatorOptions)
        {
            return Members.Any(m => m.IsExportable(generatorOptions));
        }

        internal void Remove(TsModuleMember toRemove) => this._members.Remove(toRemove);
    }
}
