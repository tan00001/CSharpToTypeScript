#nullable enable
using System.Collections.Generic;
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript.AlternateGenerators
{
    public class TsBackboneModelGenerator : TsGenerator
    {
        protected override void AppendClassDefinition(
          TsClass classModel,
          ScriptBuilder sb,
          TsGeneratorOutput generatorOutput)
        {
            string? typeName = this.GetTypeName(classModel);
            string str = this.GetTypeVisibility(classModel, typeName) ? "export " : "";
            sb.AppendFormatIndented("{0}class {1} extends {2}", str, typeName, classModel.BaseType != null ? this.GetFullyQualifiedTypeName(classModel.BaseType) : "Backbone.Model");
            sb.AppendLine(" {");
            List<TsProperty> tsPropertyList = new ();
            if ((generatorOutput & TsGeneratorOutput.Properties) == TsGeneratorOutput.Properties)
                tsPropertyList.AddRange(classModel.Properties);
            if ((generatorOutput & TsGeneratorOutput.Fields) == TsGeneratorOutput.Fields)
                tsPropertyList.AddRange(classModel.Fields);
            using (sb.IncreaseIndentation())
            {
                foreach (TsProperty property in tsPropertyList)
                {
                    if (property.JsonIgnore == null)
                    {
                        sb.AppendLineIndented(string.Format("get {0}(): {1} {{ return this.get(\"{0}\"); }}", this.GetPropertyName(property), this.GetPropertyType(property)));
                        sb.AppendLineIndented(string.Format("set {0}(v: {1}) {{ this.set(\"{0}\", v); }}", this.GetPropertyName(property), this.GetPropertyType(property)));
                    }
                }
            }
            sb.AppendLineIndented("}");
            this._generatedClasses.Add(classModel);
        }
    }
}
