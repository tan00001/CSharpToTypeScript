#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using CSharpToTypeScript.Models;

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
                if (@namespace.Classes.Any(c => !this._typeConvertors.IsConvertorRegistered(c.Type) && !c.IsIgnored && c.BaseType != null))
                {
                    sb.AppendLine("import { Resolver, FieldErrors, ResolverOptions } from 'react-hook-form';");
                }
                else
                {
                    sb.AppendLine("import { Resolver, FieldErrors } from 'react-hook-form';");
                }

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
            sb.AppendLineIndented(str + "const " + typeName + "Resolver: Resolver<" + typeName  + "> = async (values) => {");

            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("const errors: FieldErrors<" + typeName + "> = {};");
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
                    var baseTypeName = this.GetTypeName(baseClass);
                    sb.AppendLineIndented("const baseResults = await " + baseTypeName + "Resolver(values, undefined, {} as ResolverOptions<" + baseTypeName + ">);");
                    sb.AppendLineIndented("return {");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("values,");
                        sb.AppendLineIndented("errors: { ...baseResults.errors, ...errors }");
                    }
                    sb.AppendLineIndented("};");
                }
                else
                {
                    sb.AppendLineIndented("return {");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("values,");
                        sb.AppendLineIndented("errors");
                    }
                    sb.AppendLineIndented("};");
                }
            }

            sb.AppendLineIndented("};");
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
