using CSharpToTypeScript.Models;

using System.Collections.Immutable;
using System.Linq;

namespace CSharpToTypeScript.AlternateGenerators
{
    public class TsGeneratorWithVuelidate : TsGenerator
    {
        public TsGeneratorWithVuelidate(bool enableNamespace)
            : base(enableNamespace)
        {
        }

        protected override IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> AppendImports(
            TsNamespace @namespace,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions)
        {
            if ((generatorOptions.HasFlag(TsGeneratorOptions.Properties) || generatorOptions.HasFlag(TsGeneratorOptions.Fields))
                && (@namespace.Classes.Any(c => !IsIgnored(c)) || @namespace.TypeDefinitions.Any(d => !IsIgnored(d))))
            {
                sb.AppendLine("import { useVuelidate, type ValidationArgs } from '@vuelidate/core';");
                sb.AppendLine("import { required, email, minValue, maxValue, minLength, maxLength, helpers } from '@vuelidate/validators';");
            }

            int currentLength = sb.Length;
            var results = base.AppendImports(@namespace, sb, generatorOptions);

            if (sb.Length == currentLength)
            {
                sb.AppendLine();
            }

            return results;
        }

        protected override (IReadOnlyList<TsProperty> Properties, bool HasOutput) AppendClassDefinition(
            TsClass classModel,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames)
        {
            (var propertiesToExport, var hasOutput) = base.AppendClassDefinition(classModel, sb, generatorOptions, importNames);

            if (classModel.Name.EndsWith("Validator") && !classModel.RequiresAllExtensions)
            {
                return (propertiesToExport, hasOutput);
            }

            if (classModel.GenericArguments.Count > 0 && classModel.GenericArguments.Any(a => a.Type.IsGenericParameter))
            {
                return (propertiesToExport, hasOutput);
            }

            var propertyList = propertiesToExport.ToImmutableSortedDictionary(FormatPropertyName, a => a);

            if (classModel.GenericArguments.Count > 0)
            {
                AppendGenericClassDefinition(classModel, sb, importNames, propertyList);
            }
            else
            {
                // Add a new line only when the class definition was actually exported. For the case where the class
                // is generic with actual arguments, such as "MyClass<DateTime>", the definition is not exported,
                // so the new line is not needed.
                sb.AppendLine();

                AppendClassDefinition(classModel, sb, importNames, propertyList);
            }

            return (propertiesToExport, true);
        }

        protected override (IReadOnlyList<TsProperty> Properties, bool HasOutput) AppendTypeDefinition(
            TsTypeDefinition typeDefinitionModel,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            (var propertiesToExport, var hasOutput) = base.AppendTypeDefinition(typeDefinitionModel, sb, generatorOptions, importNames);

            if (typeDefinitionModel.GenericArguments.Count > 0 && typeDefinitionModel.GenericArguments.Any(a => a.Type.IsGenericParameter))
            {
                return (propertiesToExport, hasOutput);
            }

            var propertyList = propertiesToExport.ToImmutableSortedDictionary(a => this.FormatPropertyName(a), a => a);

            if (typeDefinitionModel.GenericArguments.Count > 0)
            {
                AppendGenericTypeDefinition(typeDefinitionModel, sb, importNames, propertyList);
            }
            else
            {
                sb.AppendLine();

                AppendTypeDefinition(typeDefinitionModel, sb, importNames, propertyList);
            }

            return (propertiesToExport, true);
        }

        protected override bool HasAdditionalImports(TsNamespace @namespace, TsGeneratorOptions generatorOptions)
        {
            return (generatorOptions.HasFlag(TsGeneratorOptions.Properties) || generatorOptions.HasFlag(TsGeneratorOptions.Fields))
                && @namespace.Classes.Any(c => !IsIgnored(c));
        }

        protected override void OnPropertiesAppended(ScriptBuilder sb, TsModuleMemberWithHierarchy tsModuleMemberWithHierarchy,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames,
            List<TsProperty> propertiesAppended, string namespaceName, TsGeneratorOptions generatorOptions)
        {
            var customValidationRules = propertiesAppended.SelectMany(p => p.ValidationRules.Where(v => v is CustomValidationRule)
                .Cast<CustomValidationRule>())
                .ToHashSet();

            foreach (var customValidationRule in customValidationRules)
            {
                System.Diagnostics.Debug.Assert(customValidationRule.TargetTypes.Contains(tsModuleMemberWithHierarchy));

                // Initialize the validator type name so that it can be used in our validation function. Validators in other namespaces or classes
                // may not be built yet. But we need the name to build our validation function, where we may call these external validators.
                // Furthermore, the imported name can be an alias that changes from one file to another.
                customValidationRule.ValidatorTypeName = FormatTypeName(namespaceName, customValidationRule.ValidatorType, importNames);
            }

            var localValidators = tsModuleMemberWithHierarchy.ImplementedCustomValidationRules.Where(r => r.ValidatorType == tsModuleMemberWithHierarchy).ToList();

            if (localValidators.Count > 0)
            {
                sb.AppendLine();

                foreach (var customValidationRule in localValidators)
                {
                    var property = tsModuleMemberWithHierarchy.Properties
                        .First(p => p.MemberInfo.CustomAttributes.Any(a => a.AttributeType == customValidationRule._CustomValidation.GetType()));

                    customValidationRule.AddVuelidationFunction(sb,
                        FormatTypeName(tsModuleMemberWithHierarchy.NamespaceName, tsModuleMemberWithHierarchy, importNames),
                        tsModuleMemberWithHierarchy,
                        FormatTypeName(property.PropertyType.Type.Namespace ?? tsModuleMemberWithHierarchy.NamespaceName, property.PropertyType, importNames),
                        generatorOptions);
                }
            }
        }

        #region Private Methods
        private void AppendClassDefinition(TsClass classModel, ScriptBuilder sb, IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames, ImmutableSortedDictionary<string, TsProperty> propertyList)
        {
            string typeName = GetTypeName(classModel) ?? throw new ArgumentException("Class type name cannot be blank.", nameof(classModel));

            AppendValidationRuleDefinition(classModel, sb, importNames, propertyList, typeName);
        }

        private void AppendGenericClassDefinition(TsClass classModel, ScriptBuilder sb,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames,
            ImmutableSortedDictionary<string, TsProperty> propertyList)
        {
            string typeName = SubstituteTypeParameters(this.GetTypeName(classModel), classModel.GenericArguments,
                classModel.NamespaceName, importNames);

            AppendValidationRuleDefinition(classModel, sb, importNames, propertyList, typeName);
        }

        private void AppendTypeDefinition(TsTypeDefinition typeDefinitionModel, ScriptBuilder sb, IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames, ImmutableSortedDictionary<string, TsProperty> propertyList)
        {
            string typeName = this.GetTypeName(typeDefinitionModel) ?? throw new ArgumentException("Struct type name cannot be blank.", nameof(typeDefinitionModel));
            AppendValidationRuleDefinition(typeDefinitionModel, sb, importNames, propertyList, typeName);
        }

        private void AppendGenericTypeDefinition(TsTypeDefinition typeDefinitionModel, ScriptBuilder sb,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames,
            ImmutableSortedDictionary<string, TsProperty> propertyList)
        {
            string typeName = SubstituteTypeParameters(this.GetTypeName(typeDefinitionModel), typeDefinitionModel.GenericArguments,
                typeDefinitionModel.NamespaceName, importNames);

            AppendValidationRuleDefinition(typeDefinitionModel, sb, importNames, propertyList, typeName);
        }

        private void AppendValidationRuleDefinition(TsModuleMemberWithHierarchy memberModel, ScriptBuilder sb,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames,
            ImmutableSortedDictionary<string, TsProperty> propertyList, string typeName)
        {
            string str = this.GetTypeVisibility(memberModel, typeName) ? "export " : "";

            string validationRulesName = BuildVariableNameWithGenericArguments(typeName) + "ValidationRules";

            sb.AppendLineIndented(str + "const " + validationRulesName + ": ValidationArgs<" + typeName + "> = {");

            using (sb.IncreaseIndentation())
            {
                HashSet<string> constNamesInUse = new();
                int propertyIndex = 0;
                foreach (var property in propertyList)
                {
                    sb.AppendLineIndented(property.Key + ": {");

                    int ruleIndex = 0;

                    using (sb.IncreaseIndentation())
                    {
                        foreach (var validationRule in property.Value.ValidationRules)
                        {
                            validationRule.BuildVuelidateRule(sb, property.Key, property.Value, propertyList, constNamesInUse);
                            if (++ruleIndex < property.Value.ValidationRules.Count)
                            {
                                sb.AppendLine(", ");
                            }
                            else
                            {
                                sb.AppendLine();
                            }
                        }
                    }

                    if (++propertyIndex >= propertyList.Count)
                    {
                        sb.AppendLineIndented("}");
                    }
                    else
                    {
                        sb.AppendLineIndented("},");
                    }
                }
            }

            sb.AppendLineIndented("};");
        }
        #endregion // Private Methods
    }
}
