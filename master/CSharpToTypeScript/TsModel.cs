using System.Collections.Generic;
using System.Linq;
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public class TsModel
    {
        public ISet<TsClass> Classes { get; private set; }

        public ISet<TsInterface> Interfaces { get; private set; }

        public ISet<TsEnum> Enums { get; private set; }

        public ISet<string> References { get; private set; }

        public TsModelBuilder Builder { get; private set; }

        public TsModel()
          : this(new TsModelBuilder(), Array.Empty<TsClass>())
        {
        }

        public TsModel(TsModelBuilder builder, IEnumerable<TsClass> classes)
            : this(builder, classes, Array.Empty<TsInterface>())
        {
        }

        public TsModel(TsModelBuilder builder, IEnumerable<TsClass> classes, IEnumerable<TsInterface> interfaces)
            : this(builder, classes, interfaces, Array.Empty<TsEnum>())
        {
        }

        public TsModel(TsModelBuilder builder, IEnumerable<TsClass> classes, IEnumerable<TsEnum> enums)
            : this(builder, classes, Array.Empty<TsInterface>(), enums)
        {
        }

        public TsModel(TsModelBuilder builder, IEnumerable<TsClass> classes, IEnumerable<TsInterface> interfaces, IEnumerable<TsEnum> enums)
        {
            this.Classes = new HashSet<TsClass>(classes);
            this.Interfaces = new HashSet<TsInterface>(interfaces);
            this.References = new HashSet<string>();
            this.Builder = builder;
            this.Enums = new HashSet<TsEnum>(enums);
        }

        public void RunVisitor(ITsModuleService tsModuleService, ITsModelVisitor visitor)
        {
            visitor.VisitModel(tsModuleService, this);

            foreach (TsModule module in this.Builder.TsModuleService.GetModules())
                visitor.VisitModule(tsModuleService, module);

            foreach (TsInterface interfaceModel in this.Interfaces)
            {
                visitor.VisitInterface(tsModuleService, interfaceModel);
                foreach (TsProperty property in interfaceModel.Properties)
                    visitor.VisitProperty(tsModuleService, property);
            }

            foreach (TsClass classModel in this.Classes)
            {
                visitor.VisitClass(tsModuleService, classModel);
                foreach (TsProperty property in classModel.Properties.Union(classModel.Fields).Union(classModel.Constants))
                    visitor.VisitProperty(tsModuleService, property);
            }

            foreach (TsEnum enumModel in this.Enums)
                visitor.VisitEnum(tsModuleService, enumModel);
        }
    }
}
