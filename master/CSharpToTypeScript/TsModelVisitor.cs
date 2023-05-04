using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
  public abstract class TsModelVisitor : ITsModelVisitor
  {
    public virtual void VisitModel(ITsModuleService tsModuleService, TsModel model)
    {
    }

    public virtual void VisitModule(ITsModuleService tsModuleService, TsModule module)
    {
    }

    public virtual void VisitClass(ITsModuleService tsModuleService, TsClass classModel)
    {
    }

    public virtual void VisitInterface(ITsModuleService tsModuleService, TsInterface interfaceModel)
    {
    }

    public virtual void VisitProperty(ITsModuleService tsModuleService, TsProperty property)
    {
    }

    public virtual void VisitEnum(ITsModuleService tsModuleService, TsEnum enumModel)
    {
    }
  }
}
