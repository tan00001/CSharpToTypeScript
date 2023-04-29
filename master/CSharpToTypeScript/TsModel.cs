using System.Collections.Generic;
using System.Linq;
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public class TsModel
    {
        public ISet<TsClass> Classes { get; private set; }

        public ISet<TsEnum> Enums { get; private set; }

        public ISet<string> References { get; private set; }

        public ISet<TsModule> Modules { get; private set; }

        public TsModel()
          : this(Array.Empty<TsClass>())
        {
        }

        public TsModel(IEnumerable<TsClass> classes)
        {
            this.Classes = new HashSet<TsClass>(classes);
            this.References = new HashSet<string>();
            this.Modules = new HashSet<TsModule>();
            this.Enums = new HashSet<TsEnum>();
        }

        public TsModel(IEnumerable<TsClass> classes, IEnumerable<TsEnum> enums)
        {
            this.Classes = new HashSet<TsClass>(classes);
            this.References = new HashSet<string>();
            this.Modules = new HashSet<TsModule>();
            this.Enums = new HashSet<TsEnum>(enums);
        }

        public void RunVisitor(ITsModelVisitor visitor)
        {
            visitor.VisitModel(this);

            foreach (TsModule module in this.Modules)
                visitor.VisitModule(module);

            foreach (TsClass classModel in this.Classes)
            {
                visitor.VisitClass(classModel);
                foreach (TsProperty property in classModel.Properties.Union(classModel.Fields).Union(classModel.Constants))
                    visitor.VisitProperty(property);
            }
            foreach (TsEnum enumModel in this.Enums)
                visitor.VisitEnum(enumModel);
        }
    }
}
