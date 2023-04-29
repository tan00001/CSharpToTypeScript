#nullable enable
using System.Collections.Generic;
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript.AlternateGenerators
{
    public class TsKnockoutModelGenerator : TsGenerator
    {
        protected override void AppendClassDefinition(
          TsClass classModel,
          ScriptBuilder sb,
          TsGeneratorOutput generatorOutput)
        {
            string? typeName = this.GetTypeName(classModel);
            string str1 = this.GetTypeVisibility(classModel, typeName) ? "export " : "";
            sb.AppendFormatIndented("{0}interface {1}", str1, typeName);
            if (classModel.BaseType != null)
                sb.AppendFormat(" extends {0}", this.GetFullyQualifiedTypeName(classModel.BaseType));
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
                        string str2 = this.GetPropertyType(property);
                        string str3;
                        if (property.PropertyType.IsCollection())
                        {
                            if (str2.Length > 2 && str2.Substring(str2.Length - 2) == "[]")
                                str2 = str2.Substring(0, str2.Length - 2);
                            str3 = "KnockoutObservableArray<" + str2 + ">";
                        }
                        else
                            str3 = "KnockoutObservable<" + str2 + ">";
                        sb.AppendLineIndented(string.Format("{0}: {1};", this.GetPropertyName(property), str3));
                    }
                }
            }
            sb.AppendLineIndented("}");
            this._generatedClasses.Add(classModel);
        }
    }
}
