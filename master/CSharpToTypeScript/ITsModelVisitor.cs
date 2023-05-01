using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public interface ITsModelVisitor
    {
        void VisitModel(TsModel model);

        void VisitModule(TsModule module);

        void VisitClass(TsClass classModel);

        void VisitInterface(TsInterface interfaceModel);

        void VisitProperty(TsProperty property);

        void VisitEnum(TsEnum enumModel);
    }
}
