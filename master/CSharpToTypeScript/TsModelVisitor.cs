using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
  public abstract class TsModelVisitor : ITsModelVisitor
  {
    public virtual void VisitModel(TsModel model)
    {
    }

    public virtual void VisitModule(TsModule module)
    {
    }

    public virtual void VisitClass(TsClass classModel)
    {
    }

    public virtual void VisitInterface(TsInterface interfaceModel)
    {
    }

    public virtual void VisitProperty(TsProperty property)
    {
    }

    public virtual void VisitEnum(TsEnum enumModel)
    {
    }
  }
}
