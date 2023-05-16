using System.Collections.Immutable;
using CSharpToTypeScript.Models;
using System;
using System.Collections.Generic;

namespace CSharpToTypeScript.AlternateGenerators
{
    public class TsGeneratorWithResolver : TsGenerator
    {
        public TsGeneratorWithResolver(bool enableNamespace)
            : base(enableNamespace)
        {
        }

        protected override IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> AppendImports(
            TsNamespace @namespace,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>> dependencies)
        {
            if (HasMemeberInfoForOutput(@namespace, generatorOptions))
            {
                sb.AppendLine("import { " + string.Join(", ", GetReactHookFormComponentNames(@namespace, generatorOptions))
                    + " } from 'react-hook-form';");

                if (dependencies.Count == 0 && !HasAdditionalImports(@namespace, generatorOptions))
                {
                    sb.AppendLine();
                }
            }

            return base.AppendImports(@namespace, sb, generatorOptions, dependencies);
        }

        protected bool HasMemeberInfoForOutput(TsNamespace @namespace, TsGeneratorOptions generatorOptions)
        {
            return @namespace.Classes.Any(c => !IsIgnored(c) && c.HasMemeberInfoForOutput(generatorOptions)) 
                || @namespace.TypeDefinitions.Any(d => !IsIgnored(d) && d.HasMemeberInfoForOutput(generatorOptions));
        }

        protected override void AppendAdditionalImports(
            TsNamespace @namespace,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>> dependencies,
            Dictionary<string, IReadOnlyDictionary<string, Int32>> importIndices)
        {
            if (!generatorOptions.HasFlag(TsGeneratorOptions.Properties) && !generatorOptions.HasFlag(TsGeneratorOptions.Fields))
            {
                return;
            }

            var baseClassesByNamespace = @namespace.Classes.Where(c => !IsIgnored(c))
                .Where(c => c.BaseType != null && c.BaseType.Namespace != @namespace)
                .Select(c => c.BaseType!)
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
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            var propertiesToExport = base.AppendClassDefinition(classModel, sb, generatorOptions, importNames);
            sb.AppendLine();

            var propertyList = propertiesToExport.ToImmutableSortedDictionary(FormatPropertyName, a => a);

            if (classModel.ImplementedGenericTypes.Count > 0)
            {
                foreach (var genericType in classModel.ImplementedGenericTypes.Values)
                {
                    if (genericType.Any(t => t.Type.IsGenericParameter))
                    {
                        throw new NotSupportedException("Partially specified generic argument list is not supported.");
                    }

                    AppendGenericClassDefinition(classModel, genericType, sb, importNames, propertyList);
                }
            }
            else
            {
                AppendClassDefinition(classModel, sb, importNames, propertyList);
            }

            return propertiesToExport;
        }

        protected override List<TsProperty> AppendProperties(ScriptBuilder sb, TsModuleMemberWithHierarchy tsModuleMemberWithHierarchy,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames,
            List<TsProperty> propertiesToExport, string namespaceName, TsGeneratorOptions generatorOptions)
        {
            foreach (var customValidationRule in tsModuleMemberWithHierarchy.ImplementedCustomValidationRules)
            {
                customValidationRule.ValidatorTypeName = FormatTypeName(namespaceName, tsModuleMemberWithHierarchy, importNames);

                foreach (var targetType in customValidationRule.TargetTypes)
                {
                    customValidationRule.AddValidationFunction(sb, FormatTypeName(namespaceName, targetType, importNames),
                        targetType, generatorOptions);
                }
            }
            return base.AppendProperties(sb, tsModuleMemberWithHierarchy, importNames, propertiesToExport, namespaceName, generatorOptions);
        }

        protected override IReadOnlyList<TsProperty> AppendTypeDefinition(
            TsTypeDefinition typeDefinitionModel,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            var propertiesToExport = base.AppendTypeDefinition(typeDefinitionModel, sb, generatorOptions, importNames);
            sb.AppendLine();

            var propertyList = propertiesToExport.ToImmutableSortedDictionary(a => this.FormatPropertyName(a), a => a);

            if (typeDefinitionModel.ImplementedGenericTypes.Count > 0)
            {
                foreach (var genericType in typeDefinitionModel.ImplementedGenericTypes.Values)
                {
                    AppendGenericTypeDefinition(typeDefinitionModel, genericType, sb, importNames, propertyList);
                }
            }
            else
            {
                AppendTypeDefinition(typeDefinitionModel, sb, importNames, propertyList);
            }

            return propertiesToExport;
        }

        protected static string BuildVariableNameWithGenericArguments(string typeName)
        {
            return typeName.Replace("<", "").Replace(", ", "").TrimEnd('>');
        }

        protected virtual IReadOnlyList<string> GetReactHookFormComponentNames(TsNamespace @namespace, TsGeneratorOptions generatorOptions)
        {
            List<string> reactHookFormComponentNames = new() { "Resolver", "FieldErrors" };

            if (generatorOptions.HasFlag(TsGeneratorOptions.Properties)
                && @namespace.Classes.Any(c => !IsIgnored(c) && c.BaseType != null))
            {
                reactHookFormComponentNames.Add("ResolverOptions");
            }

            if (@namespace.Classes.Any(c => !IsIgnored(c) && c.GetCustomValidations(generatorOptions).Count > 0)
                || @namespace.Interfaces.Any(d => !IsIgnored(d) && d.GetCustomValidations(generatorOptions).Count > 0)
                || @namespace.TypeDefinitions.Any(d => !IsIgnored(d) && d.GetCustomValidations(generatorOptions).Count > 0))
            {
                reactHookFormComponentNames.Add("FieldError");
            }

            return reactHookFormComponentNames;
        }

        protected string SubstituteTypeParameters(string? typeName, IReadOnlyList<TsType> genericArguments,
            string namespaceName, IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            var argumentIndex = typeName.IndexOf('<');

            return string.Concat(typeName.AsSpan(0, argumentIndex), "<", string.Join(", ", genericArguments
                .Select(a => this.FormatTypeName(namespaceName, a, importNames))), ">");
        }

        #region Private Methods
        private void AppendClassDefinition(TsClass classModel, ScriptBuilder sb, IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames, ImmutableSortedDictionary<string, TsProperty> propertyList)
        {
            string typeName = this.GetTypeName(classModel) ?? throw new ArgumentException("Class type name cannot be blank.", nameof(classModel));
            AppendResolverDefinition(classModel, sb, importNames, propertyList, typeName, typeName);
        }

        private void AppendGenericClassDefinition(TsClass classModel, IReadOnlyList<TsType> genericArguments, ScriptBuilder sb,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames,
            ImmutableSortedDictionary<string, TsProperty> propertyList)
        {
            string typeName = SubstituteTypeParameters(this.GetTypeName(classModel), genericArguments,
                classModel.NamespaceName, importNames);
            string resolverName = BuildVariableNameWithGenericArguments(typeName);
            AppendResolverDefinition(classModel, sb, importNames, propertyList, typeName, resolverName);
        }

        private void AppendResolverDefinition(TsModuleMemberWithHierarchy memberModel, ScriptBuilder sb,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames,
            ImmutableSortedDictionary<string, TsProperty> propertyList, string typeName, string resolverName)
        {
            string str = this.GetTypeVisibility(memberModel, typeName) ? "export " : "";
            sb.AppendLineIndented(str + "const " + resolverName + "Resolver: Resolver<" + typeName + "> = async (values) => {");

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
                if (memberModel is TsClass tsClass && tsClass.BaseType != null)
                {
                    var baseTypeName = FormatTypeName(memberModel.NamespaceName, tsClass.BaseType, importNames);
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

        private void AppendTypeDefinition(TsTypeDefinition typeDefinitionModel, ScriptBuilder sb, IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames, ImmutableSortedDictionary<string, TsProperty> propertyList)
        {
            string typeName = this.GetTypeName(typeDefinitionModel) ?? throw new ArgumentException("Struct type name cannot be blank.", nameof(typeDefinitionModel));
            AppendResolverDefinition(typeDefinitionModel, sb, importNames, propertyList, typeName, typeName);
        }

        private void AppendGenericTypeDefinition(TsTypeDefinition typeDefinitionModel, IReadOnlyList<TsType> genericArguments, ScriptBuilder sb,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames,
            ImmutableSortedDictionary<string, TsProperty> propertyList)
        {
            string typeName = SubstituteTypeParameters(this.GetTypeName(typeDefinitionModel), genericArguments,
                typeDefinitionModel.NamespaceName, importNames);
            string resolverName = BuildVariableNameWithGenericArguments(typeName);
            AppendResolverDefinition(typeDefinitionModel, sb, importNames, propertyList, typeName, resolverName);
        }
        #endregion // Private Methods
    }
}
