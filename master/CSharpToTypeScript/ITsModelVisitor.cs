using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public interface ITsModelVisitor
    {
        void VisitModel(ITsModuleService tsModuleService, TsModel model);

        void VisitModule(ITsModuleService tsModuleService, TsModule module);

        void VisitClass(ITsModuleService tsModuleService, TsClass classModel);

        void VisitInterface(ITsModuleService tsModuleService, TsInterface interfaceModel);

        void VisitProperty(ITsModuleService tsModuleService, TsProperty property);

        void VisitEnum(ITsModuleService tsModuleService, TsEnum enumModel);
    }
}
