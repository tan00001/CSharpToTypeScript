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

        public IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>>? Dependencies { get; set; }

        internal void Add(TsModuleMember toAdd)
        {
            this._members.Add(toAdd);
            toAdd.Namespace = this;
        }

        /// <summary>
        /// Get members of tsNamespace that depend on this current namespace.
        /// tsNamespace is different from this current namespace.
        /// </summary>
        /// <param name="tsNamespace"></param>
        /// <param name="generatorOptions"></param>
        /// <returns></returns>
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
            if (TsType.ExcludedNamespacePrefixes.Any(p => p.StartsWith(this.Name) || this.Name.StartsWith(p)))
            {
                return false;
            }

            return Members.Any(m => m.IsExportable(generatorOptions));
        }

        internal void Remove(TsModuleMember toRemove) => this._members.Remove(toRemove);
    }
}
