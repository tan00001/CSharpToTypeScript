#nullable enable
using System.Collections.Generic;
using System.Collections.Immutable;
using CSharpToTypeScript.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSharpToTypeScript.AlternateGenerators
{
    public class TsGeneratorWithResolver : TsGenerator
    {
        public static bool DefaultTypeVisibilityFormatterForResolver(TsType tsType, string? typeName) => tsType is TsClass || tsType is TsInterface ;

        public TsGeneratorWithResolver(bool enableNamespace)
            : base(enableNamespace)
        {
            SetTypeVisibilityFormatter(DefaultTypeVisibilityFormatterForResolver);
        }

        protected override void AppendNamespace(
          TsNamespace @namespace,
          ScriptBuilder sb,
          TsGeneratorOutput generatorOutput,
          IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>> dependencies)
        {
            if (@namespace.Classes.Any(c => !this._typeConvertors.IsConvertorRegistered(c.Type) && !c.IsIgnored))
            {
                sb.AppendLine("import { Resolver, FieldError } from 'react-hook-form';");

                if (dependencies.Count == 0)
                {
                    sb.AppendLine();
                }
            }

            base.AppendNamespace(@namespace, sb, generatorOutput, dependencies);
        }

        protected override void AppendClassDefinition(
            TsClass classModel,
            ScriptBuilder sb,
            TsGeneratorOutput generatorOutput,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            base.AppendClassDefinition(classModel, sb, generatorOutput, importNames);
            sb.AppendLine();

            List<TsProperty> source = new();
            if ((generatorOutput & TsGeneratorOutput.Properties) == TsGeneratorOutput.Properties)
                source.AddRange(classModel.Properties);
            if ((generatorOutput & TsGeneratorOutput.Fields) == TsGeneratorOutput.Fields)
                source.AddRange(classModel.Fields);
            source.RemoveAll(p => p.JsonIgnore != null);
            var sortedSourceList = source.ToImmutableSortedDictionary(a => this.FormatPropertyName(a), a => a);

            string? typeName = this.GetTypeName(classModel);
            string str = this.GetTypeVisibility(classModel, typeName) ? "export " : "";
            sb.AppendLineIndented(str + "export const " + typeName + "Resolver: Resolver<" + typeName  + "> = async (values) => {");

            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("const errorBuffer = {");
                using (sb.IncreaseIndentation())
                {
                    foreach (var property in sortedSourceList.Where(p => p.Value.ValidationRules.Count > 0))
                    {
                        string propertyName = this._memberFormatter(property.Value);
                        sb.AppendLineIndented(propertyName + ": FieldError[]");
                    }
                }
                sb.AppendLineIndented("};");
                sb.AppendLine();

                foreach (var property in sortedSourceList)
                {
                    foreach (var validationRule in property.Value.ValidationRules)
                    {
                        validationRule.BuildRule(sb, property.Key, property.Value, sortedSourceList);
                    }
                }

                sb.AppendLine();
                if (classModel.BaseType is TsClass baseClass)
                {
                    sb.AppendLineIndented("const baseResults = await " + this.GetTypeName(baseClass) + "Resolver(values);");
                    sb.AppendLineIndented("let returnValues: " + typeName + " = { ...baseResults.Values };");
                    sb.AppendLineIndented("let returnErrors = { ...baseResults.Errors };");
                }
                else
                {
                    sb.AppendLineIndented("let returnValues: " + typeName + " = {};");
                    sb.AppendLineIndented("let returnErrors = {};");
                }
                sb.AppendLine();

                foreach (var property in sortedSourceList)
                {
                    string propertyName = this._memberFormatter(property.Value);
                    if (property.Value.ValidationRules.Count == 0)
                    {
                        sb.AppendLineIndented("returnValues." + propertyName + " = values." + propertyName + ';');
                    }
                    else
                    {
                        sb.AppendLineIndented("if (errorBuffer." + propertyName + ".length == 0) {");
                        using (sb.IncreaseIndentation())
                        {
                            sb.AppendLineIndented("returnValues." + propertyName + " = values." + propertyName + ';');
                        }
                        sb.AppendLineIndented("} else {");
                        using (sb.IncreaseIndentation())
                        {
                            sb.AppendLineIndented("returnErrors." + propertyName + " = errorBuffer." + propertyName + ';');
                        }
                        sb.AppendLineIndented("};");
                    }
                }
                sb.AppendLine();

                sb.AppendLineIndented("return {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("values: returnValues,");
                    sb.AppendLineIndented("errors: returnErrors");
                }
                sb.AppendLineIndented("};");
            }

            sb.AppendLineIndented("};");

            sb.AppendLine();
        }

        protected override void AppendInterfaceDefinition(
            TsInterface interfaceModel,
            ScriptBuilder sb,
            TsGeneratorOutput generatorOutput,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            base.AppendInterfaceDefinition(interfaceModel, sb, generatorOutput, importNames);
        }
    }
}
