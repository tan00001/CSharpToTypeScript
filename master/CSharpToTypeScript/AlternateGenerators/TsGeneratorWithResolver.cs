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

        protected virtual IReadOnlyList<string> GetReactHookFormComponentNames(IEnumerable<TsClass> classes)
        {
            if (classes.Any(c => !this._typeConvertors.IsConvertorRegistered(c.Type) && !c.IsIgnored && c.BaseType != null))
            {
                return new string[]{ "Resolver", "FieldErrors", "ResolverOptions" };
            }
            else
            {
                return new string[] { "Resolver", "FieldErrors" };
            }
        }


        protected override IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> AppendImports(
            TsNamespace @namespace,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>> dependencies)
        {
            if ((generatorOptions.HasFlag(TsGeneratorOptions.Properties) || generatorOptions.HasFlag(TsGeneratorOptions.Fields))
                && @namespace.Classes.Any(c => !this._typeConvertors.IsConvertorRegistered(c.Type) && !c.IsIgnored))
            {
                sb.AppendLine("import { " + string.Join(", ", GetReactHookFormComponentNames(@namespace.Classes))
                    + " } from 'react-hook-form';");

                if (dependencies.Count == 0 && !HasAdditionalImports(@namespace, generatorOptions))
                {
                    sb.AppendLine();
                }
            }

            return base.AppendImports(@namespace, sb, generatorOptions, dependencies);
        }

        protected override void AppendAdditionalImports(
            TsNamespace @namespace,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOutput,
            IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>> dependencies,
            Dictionary<string, IReadOnlyDictionary<string, Int32>> importIndices)
        {
            if (!generatorOutput.HasFlag(TsGeneratorOptions.Properties) && !generatorOutput.HasFlag(TsGeneratorOptions.Fields))
            {
                return;
            }

            var baseClassesByNamespace = @namespace.Classes.Where(c => !this._typeConvertors.IsConvertorRegistered(c.Type) && !c.IsIgnored)
                .Where(c => c.BaseType != null && c.BaseType is TsClass baseClass && baseClass.Namespace != @namespace)
                .Select(c => (TsClass)c.BaseType!)
                .GroupBy(c => c.NamespaceName);
            foreach (var baseClasses in baseClassesByNamespace)
            {
                var importSourceName = baseClasses.Key + "Form";
                var importIndicesForNamespace = new Dictionary<string, Int32>();
                sb.AppendLine("import { " + string.Join(", ", baseClasses.Select(c => GetImportName(c.Name + "Resolver", importIndices, importIndicesForNamespace))) + " } from './" + importSourceName + "';");
                importIndices[importSourceName] = importIndicesForNamespace;
            }
        }

        protected override IReadOnlyList<TsProperty> AppendClassDefinition(
            TsClass classModel,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOutput,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            var propertiesToExport = base.AppendClassDefinition(classModel, sb, generatorOutput, importNames);
            sb.AppendLine();

            var propertyList = propertiesToExport.ToImmutableSortedDictionary(a => this.FormatPropertyName(a), a => a);

            string? typeName = this.GetTypeName(classModel);
            string str = this.GetTypeVisibility(classModel, typeName) ? "export " : "";
            sb.AppendLineIndented(str + "const " + typeName + "Resolver: Resolver<" + typeName  + "> = async (values) => {");

            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("const errors: FieldErrors<" + typeName + "> = {};");
                sb.AppendLine();

                foreach (var property in propertyList)
                {
                    foreach (var validationRule in property.Value.ValidationRules)
                    {
                        validationRule.BuildRule(sb, property.Key, property.Value, propertyList);
                    }
                }

                sb.AppendLine();
                if (classModel.BaseType is TsClass baseClass)
                {
                    var baseTypeName = FormatTypeName(classModel.NamespaceName, baseClass, importNames);
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

            return propertiesToExport;
        }
    }
}
