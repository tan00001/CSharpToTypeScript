using System.Collections.Generic;
using System.Linq;

namespace CSharpToTypeScript.Models
{
    public class TsModule
    {
        private readonly ISet<TsModuleMember> _members;

        public string Name { get; set; }

        public IEnumerable<TsClass> Classes => this._members.OfType<TsClass>();

        public IEnumerable<TsEnum> Enums => this._members.OfType<TsEnum>();

        public IEnumerable<TsModuleMember> Members => this._members;

        public TsModule(string name)
        {
            this._members = new HashSet<TsModuleMember>();
            this.Name = name;
        }

        internal void Add(TsModuleMember toAdd) => this._members.Add(toAdd);

        internal void Remove(TsModuleMember toRemove) => this._members.Remove(toRemove);
    }
}
