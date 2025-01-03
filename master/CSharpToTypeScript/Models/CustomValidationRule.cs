﻿using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Diagnostics.CodeAnalysis;

namespace CSharpToTypeScript.Models
{
    public class CustomValidationRuleComparer : IEqualityComparer<CustomValidationRule>
    {
        public bool Equals(CustomValidationRule? x, CustomValidationRule? y)
        {
            if (x != null && y != null)
            {
                return x._CustomValidation.ValidatorType == y._CustomValidation.ValidatorType
                    && x._CustomValidation.Method == y._CustomValidation.Method;
            }

            return x == null && y == null;
        }

        public int GetHashCode([DisallowNull] CustomValidationRule rule)
        {
            return HashCode.Combine(rule._CustomValidation.ValidatorType, rule._CustomValidation.Method);
        }
    }

    public class CustomValidationRule : ITsValidationRule
    {
        public static readonly CustomValidationRuleComparer Comparer = new();

        public static readonly IReadOnlyDictionary<string, string> OperatorReplacements = new Dictionary<string, string>()
        {
            { " != ", " !== "},
            { " == ", " === " }
        };

        public static readonly char[] SymbolSeparators = new char[]{ ' ', '\t', '(', '=', ':', '?', '\n'};

        public static readonly char[] SymbolTerminators = new char[] { ' ', '\t', '.', ';', ')', '?', '\r' };

        public readonly CustomValidationAttribute _CustomValidation;

        public TsType ValidatorType { get; set; }

        public string? ValidatorTypeName { get; set; }

        public HashSet<TsModuleMemberWithHierarchy> TargetTypes { get; private set; } = new();

        public CustomValidationRule(CustomValidationAttribute customValidation, TsType validatorType)
        {
            _CustomValidation = customValidation;
            ValidatorType = validatorType;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            var errorVariableName = propertyName + "Error";
            if (constNamesInUse.Contains(errorVariableName))
            {
                var replacementErrorVariableIndex = 1;
                var replacementErrorVariableName = errorVariableName + replacementErrorVariableIndex;
                while(constNamesInUse.Contains(replacementErrorVariableName))
                {
                    replacementErrorVariableName = errorVariableName + (++replacementErrorVariableIndex);
                }
                errorVariableName = replacementErrorVariableName;
            }

            constNamesInUse.Add(errorVariableName);

            sb.AppendLineIndented("const " + errorVariableName + " = "
                + (string.IsNullOrEmpty(ValidatorTypeName) ? "" : (ValidatorTypeName + '.'))
                + _CustomValidation.Method + "(values);");
            sb.AppendLineIndented("if (" + errorVariableName + ") {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " ??= " + errorVariableName + ';');
            }
            sb.AppendLineIndented("}");
        }

        public void BuildVuelidateRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            sb.AppendIndented($"helpers.withMessage('" + _CustomValidation.ErrorMessage + "', (value, siblings) => " + (string.IsNullOrEmpty(ValidatorTypeName) ? "" : (ValidatorTypeName + '.'))
                + _CustomValidation.Method + "(value, siblings))");
        }

        public void AddVuelidationFunction(ScriptBuilder sb, string typeName, TsModuleMemberWithHierarchy targetType, string propertyTypeName,
            TsGeneratorOptions generatorOptions)
        {
            sb.AppendLineIndented("static " + _CustomValidation.Method + "(value: " + propertyTypeName + ", values : " + typeName + "): boolean {");
            using (sb.IncreaseIndentation())
            {
                bool csharpToTypeScriptSectionFound = false;

                try
                {
                    MethodInfo? method = _CustomValidation.ValidatorType.GetMethod(_CustomValidation.Method);
                    if (method != null)
                    {
                        var assemblyPath = _CustomValidation.ValidatorType.Assembly.Location;

                        using var assemblyStream = File.OpenRead(assemblyPath);
                        using var peReader = new PEReader(assemblyStream);

                        MetadataReader metaDataReader = peReader.GetMetadataReader();
                        var methodDefinitionHandle = metaDataReader.MethodDefinitions.FirstOrDefault(m =>
                        {
                            var methodDefinition = metaDataReader.GetMethodDefinition(m);

                            if (metaDataReader.GetString(methodDefinition.Name) != method.Name)
                            {
                                return false;
                            }

                            var typeDefinition = metaDataReader.GetTypeDefinition(methodDefinition.GetDeclaringType());

                            string typeName = metaDataReader.GetString(typeDefinition.Name);
                            string namespaceName = metaDataReader.GetString(typeDefinition.Namespace);

                            if (method.DeclaringType == null)
                            {
                                return string.IsNullOrEmpty(typeName) && string.IsNullOrEmpty(namespaceName);
                            }

                            return typeName == method.DeclaringType.Name && namespaceName == method.DeclaringType.Namespace;
                        });

                        if (peReader.TryOpenAssociatedPortablePdb(assemblyPath, File.OpenRead,
                            out var pdbReaderProvider, out _) && pdbReaderProvider != null)
                        {
                            var pdbReader = pdbReaderProvider.GetMetadataReader();

                            // Get the method debug information
                            var methodDebugInfo = pdbReader.GetMethodDebugInformation(methodDefinitionHandle);

                            // Get the source code line information
                            var sequencePoints = methodDebugInfo.GetSequencePoints();
                            var document = pdbReader.GetDocument(sequencePoints.First().Document);

                            string documentPath = pdbReader.GetString(document.Name);
                            int startLine = sequencePoints.First().StartLine;
                            int endLine = sequencePoints.Last().EndLine;

                            List<string> validatorDefinition = new();

                            if (startLine > 0 && endLine >= startLine)
                            {
                                var sourceFileContents = File.ReadAllText(documentPath).Split("\n").Select(l => l.TrimEnd('\r')).ToArray();
                                if (startLine < sourceFileContents.Length && endLine < sourceFileContents.Length)
                                {
                                    for (var i = startLine - 1; i <= endLine - 1; ++i)
                                    {
                                        validatorDefinition.Add(sourceFileContents[i]);
                                    }
                                }
                            }

                            var csharpToTypeScriptSectionStartIndex = validatorDefinition.FindIndex(l => l.Trim() == "#region CSharpToTypeScript");
                            var csharpToTypeScriptSectionEndIndex = validatorDefinition.FindIndex(l => l.TrimStart().StartsWith("#endregion"));
                            csharpToTypeScriptSectionFound = csharpToTypeScriptSectionStartIndex >= 0
                                && csharpToTypeScriptSectionEndIndex > csharpToTypeScriptSectionStartIndex;

                            if (!csharpToTypeScriptSectionFound)
                            {
                                AdjustIndents(sb, validatorDefinition);

                                foreach (var validatorDefinitionLine in validatorDefinition)
                                {
                                    sb.AppendLineIndented("// " + validatorDefinitionLine);
                                }
                            }
                            else
                            {
                                validatorDefinition = validatorDefinition.GetRange(csharpToTypeScriptSectionStartIndex + 1,
                                    csharpToTypeScriptSectionEndIndex - csharpToTypeScriptSectionStartIndex - 1);

                                AdjustIndents(sb, validatorDefinition);

                                AddVuelidationFunction(sb, validatorDefinition, targetType, generatorOptions);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLineIndented("// Unable to read symbol file. " + ex.Message);
                }

                if (!csharpToTypeScriptSectionFound)
                {
                    sb.AppendLineIndented("// TODO: Please implement this function.");
                    sb.AppendLineIndented("return true;");
                }
            }
            sb.AppendLineIndented("}");
            sb.AppendLine();
        }

        public void AddValidationFunction(ScriptBuilder sb, string typeName, TsModuleMemberWithHierarchy targetType,
            TsGeneratorOptions generatorOptions)
        {
            sb.AppendLineIndented("static " + _CustomValidation.Method + "(values: " + typeName + "): FieldError | undefined {");
            using (sb.IncreaseIndentation())
            {
                bool csharpToTypeScriptSectionFound = false;

                try
                {
                    MethodInfo? method = _CustomValidation.ValidatorType.GetMethod(_CustomValidation.Method);
                    if (method != null)
                    {
                        var assemblyPath = _CustomValidation.ValidatorType.Assembly.Location;

                        using var assemblyStream = File.OpenRead(assemblyPath);
                        using var peReader = new PEReader(assemblyStream);

                        MetadataReader metaDataReader = peReader.GetMetadataReader();
                        var methodDefinitionHandle = metaDataReader.MethodDefinitions.FirstOrDefault(m =>
                        {
                            var methodDefinition = metaDataReader.GetMethodDefinition(m);

                            if (metaDataReader.GetString(methodDefinition.Name) != method.Name)
                            {
                                return false;
                            }

                            var typeDefinition = metaDataReader.GetTypeDefinition(methodDefinition.GetDeclaringType());

                            string typeName = metaDataReader.GetString(typeDefinition.Name);
                            string namespaceName = metaDataReader.GetString(typeDefinition.Namespace);

                            if (method.DeclaringType == null)
                            {
                                return string.IsNullOrEmpty(typeName) && string.IsNullOrEmpty(namespaceName);
                            }

                            return typeName == method.DeclaringType.Name && namespaceName == method.DeclaringType.Namespace;
                        });

                        if (peReader.TryOpenAssociatedPortablePdb(assemblyPath, File.OpenRead,
                            out var pdbReaderProvider, out _) && pdbReaderProvider != null)
                        {
                            var pdbReader = pdbReaderProvider.GetMetadataReader();

                            // Get the method debug information
                            var methodDebugInfo = pdbReader.GetMethodDebugInformation(methodDefinitionHandle);

                            // Get the source code line information
                            var sequencePoints = methodDebugInfo.GetSequencePoints();
                            var document = pdbReader.GetDocument(sequencePoints.First().Document);

                            string documentPath = pdbReader.GetString(document.Name);
                            int startLine = sequencePoints.First().StartLine;
                            int endLine = sequencePoints.Last().EndLine;

                            List<string> validatorDefinition = new();

                            if (startLine > 0 && endLine >= startLine)
                            {
                                var sourceFileContents = File.ReadAllText(documentPath).Split("\n").Select(l => l.TrimEnd('\r')).ToArray();
                                if (startLine < sourceFileContents.Length && endLine < sourceFileContents.Length)
                                {
                                    for (var i = startLine - 1; i <= endLine - 1; ++i)
                                    {
                                        validatorDefinition.Add(sourceFileContents[i]);
                                    }
                                }
                            }

                            var csharpToTypeScriptSectionStartIndex = validatorDefinition.FindIndex(l => l.Trim() == "#region CSharpToTypeScript");
                            var csharpToTypeScriptSectionEndIndex = validatorDefinition.FindIndex(l => l.TrimStart().StartsWith("#endregion"));
                            csharpToTypeScriptSectionFound = csharpToTypeScriptSectionStartIndex >= 0
                                && csharpToTypeScriptSectionEndIndex > csharpToTypeScriptSectionStartIndex;

                            if (!csharpToTypeScriptSectionFound)
                            {
                                AdjustIndents(sb, validatorDefinition);

                                foreach (var validatorDefinitionLine in validatorDefinition)
                                {
                                    sb.AppendLineIndented("// " + validatorDefinitionLine);
                                }
                            }
                            else
                            {
                                validatorDefinition = validatorDefinition.GetRange(csharpToTypeScriptSectionStartIndex + 1,
                                    csharpToTypeScriptSectionEndIndex - csharpToTypeScriptSectionStartIndex - 1);

                                AdjustIndents(sb, validatorDefinition);

                                AddValidationFunction(sb, validatorDefinition, targetType, generatorOptions);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLineIndented("// Unable to read symbol file. " + ex.Message);
                }

                if (!csharpToTypeScriptSectionFound)
                {
                    sb.AppendLineIndented("// TODO: Please implement this function.");
                    sb.AppendLineIndented("return {");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("type: \"custom\",");
                        sb.AppendLineIndented("message: \"" + _CustomValidation.Method + " is to be implemented\"");
                    };
                    sb.AppendLineIndented("};");
                }
            }
            sb.AppendLineIndented("}");
            sb.AppendLine();
        }

        private static void AdjustIndents(ScriptBuilder sb, List<string> validatorDefinitions)
        {
            var nonEmptyValidatorDefinitions = validatorDefinitions.Where(d => !string.IsNullOrWhiteSpace(d));
            if (!nonEmptyValidatorDefinitions.Any())
            {
                return;
            }

            int indent = nonEmptyValidatorDefinitions.Min(d => GetLeadingWhitespaceCount(sb, d));

            if (indent > 0)
            {
                for (var i = 0; i < validatorDefinitions.Count; ++i)
                {
                    TrimLeadingSpaces(sb, validatorDefinitions, indent, i);
                }
            }
        }

        private static void TrimLeadingSpaces(ScriptBuilder sb, List<string> validatorDefinitions, int indent, int i)
        {
            var validatorDefinitionLine = validatorDefinitions[i];
            int count = 0;
            foreach (var c in validatorDefinitionLine)
            {
                if (c == ' ')
                {
                    count++;
                }
                else if (c == '\t')
                {
                    count += sb.TabSize;
                }
                else
                {
                    validatorDefinitions[i] = validatorDefinitionLine.Substring(count);
                    return;
                }

                if (count >= indent)
                {
                    validatorDefinitions[i] = validatorDefinitionLine.Substring(count);
                    return;
                }
            }
        }

        private static int GetLeadingWhitespaceCount(ScriptBuilder sb, string d)
        {
            int count = 0;
            foreach (var c in d)
            {
                if (c == ' ')
                {
                    count++;
                }
                else if (c == '\t')
                {
                    count += sb.TabSize;
                }
            }

            return count;
        }

        private void AddValidationFunction(ScriptBuilder sb, List<string> validatorDefinition,
            TsModuleMemberWithHierarchy targetType, TsGeneratorOptions generatorOptions)
        {
            var propertiesForOutput = targetType.GetMemeberInfoForOutput(generatorOptions);
            var symbolReplacements = propertiesForOutput
                .ToDictionary(p => "values." + p.Name, p => "values." + TsGenerator.ToCamelCase(p.Name));

            foreach (var property in propertiesForOutput)
            {
                if (property.PropertyType is TsSystemType tsSystemType)
                {
                    switch (tsSystemType.Kind)
                    {
                        case SystemTypeKind.String:
                            SetupForStringAttributesReplacements(symbolReplacements, property);
                            break;

                        case SystemTypeKind.Date:
                            SetupForDateAttributesReplacements(symbolReplacements, property);
                            break;
                    }
                }

                if (property.ValidationRules.Any(r => r is CustomValidationRule customValidationRule
                    && Comparer.Equals(customValidationRule, this)))
                {
                    symbolReplacements.Add(TsGenerator.ToCamelCase(property.Name), "values." + TsGenerator.ToCamelCase(property.Name));
                }
            }


            foreach (var validatorDefinitionLine in validatorDefinition)
            {
                var outputLine = ReplaceSuccessResult(validatorDefinitionLine);
                outputLine = ReplaceFailResult(outputLine);
                outputLine = ReplaceSymbolNames(outputLine, symbolReplacements);
                sb.AppendLineIndented(outputLine);
            }
        }

        private void AddVuelidationFunction(ScriptBuilder sb, List<string> validatorDefinition,
            TsModuleMemberWithHierarchy targetType, TsGeneratorOptions generatorOptions)
        {
            var propertiesForOutput = targetType.GetMemeberInfoForOutput(generatorOptions);
            var symbolReplacements = propertiesForOutput
                .ToDictionary(p => "values." + p.Name, p => "values." + TsGenerator.ToCamelCase(p.Name));

            foreach (var property in propertiesForOutput)
            {
                if (property.PropertyType is TsSystemType tsSystemType)
                {
                    switch (tsSystemType.Kind)
                    {
                        case SystemTypeKind.String:
                            SetupForStringAttributesReplacements(symbolReplacements, property);
                            break;

                        case SystemTypeKind.Date:
                            SetupForDateAttributesReplacements(symbolReplacements, property);
                            break;
                    }
                }

                if (property.ValidationRules.Any(r => r is CustomValidationRule customValidationRule
                    && Comparer.Equals(customValidationRule, this)))
                {
                    symbolReplacements.Add(TsGenerator.ToCamelCase(property.Name), "values." + TsGenerator.ToCamelCase(property.Name));
                }
            }


            foreach (var validatorDefinitionLine in validatorDefinition)
            {
                var outputLine = ReplaceSuccessVuelidateResult(validatorDefinitionLine);
                outputLine = ReplaceFailVuelidateResult(outputLine);
                outputLine = ReplaceSymbolNames(outputLine, symbolReplacements);
                sb.AppendLineIndented(outputLine);
            }
        }

        private static void SetupForStringAttributesReplacements(Dictionary<string, string> propertNamesForExport, TsProperty property)
        {
            var camelCaseName = "values." + TsGenerator.ToCamelCase(property.Name);
            propertNamesForExport.Add(camelCaseName + ".Length", camelCaseName + ".length");
            propertNamesForExport.Add(camelCaseName + "?.Length", camelCaseName + "?.length");
        }

        private static void SetupForDateAttributesReplacements(Dictionary<string, string> propertNamesForExport, TsProperty property)
        {
            var camelCaseName = "values." + TsGenerator.ToCamelCase(property.Name);
            propertNamesForExport.Add(camelCaseName + ".Year", camelCaseName + ".getFullYear()");
            propertNamesForExport.Add(camelCaseName + ".Day", camelCaseName + ".getDate()");
            propertNamesForExport.Add(camelCaseName + ".Month", '(' + camelCaseName + ".getMonth() + 1)");
            propertNamesForExport.Add(camelCaseName + ".Hour", camelCaseName + ".getHours()");
            propertNamesForExport.Add(camelCaseName + ".Minutes", camelCaseName + ".getMinutes()");
            propertNamesForExport.Add(camelCaseName + ".Seconds", camelCaseName + ".getSeconds()");
            propertNamesForExport.Add(camelCaseName + "?.Year", camelCaseName + "?.getFullYear()");
            propertNamesForExport.Add(camelCaseName + "?.Day", camelCaseName + "?.getDate()");
            propertNamesForExport.Add(camelCaseName + "?.Month", '(' + camelCaseName + "?.getMonth() + 1)");
            propertNamesForExport.Add(camelCaseName + "?.Hour", camelCaseName + "?.getHours()");
            propertNamesForExport.Add(camelCaseName + "?.Minutes", camelCaseName + "?.getMinutes()");
            propertNamesForExport.Add(camelCaseName + "?.Seconds", camelCaseName + "?.getSeconds()");
        }

        private static string ReplaceSuccessResult(string validatorDefinitionLine)
        {
            return validatorDefinitionLine.Replace("ValidationResult.Success", "undefined");
        }

        private static string ReplaceSuccessVuelidateResult(string validatorDefinitionLine)
        {
            return validatorDefinitionLine.Replace("ValidationResult.Success", "true");
        }

        private static string ReplaceFailResult(string validatorDefinitionLine)
        {
            var resultIndex = validatorDefinitionLine.IndexOf("new ValidationResult(");
            if (resultIndex > 0)
            {
                resultIndex += "new ValidationResult(".Length;
                var resultEndIndex = validatorDefinitionLine.IndexOf(")", resultIndex);
                return validatorDefinitionLine.Substring(0, resultEndIndex)
                    .Replace("new ValidationResult(", "{ type: \"custom\", message: ")
                    + " }"
                    + validatorDefinitionLine.Substring(resultEndIndex + 1);
            }

            return validatorDefinitionLine;
        }

        private static string ReplaceFailVuelidateResult(string validatorDefinitionLine)
        {
            var resultIndex = validatorDefinitionLine.IndexOf("new ValidationResult(");
            if (resultIndex > 0)
            {
                var resultEndIndex = validatorDefinitionLine.IndexOf(")", "new ValidationResult(".Length);
                return validatorDefinitionLine.Substring(0, resultIndex)
                    + "false"
                    + validatorDefinitionLine.Substring(resultEndIndex + 1);
            }

            return validatorDefinitionLine;
        }

        private static string ReplaceSymbolNames(string validatorDefinitionLine, IReadOnlyDictionary<string, string> symbolReplacements)
        {
            foreach (var operatorReplancement in OperatorReplacements.OrderBy(n => n.Key.Length))
            {
                validatorDefinitionLine = validatorDefinitionLine.Replace(operatorReplancement.Key, operatorReplancement.Value);
            }

            foreach (var propertNameForExport in symbolReplacements.OrderBy(n => n.Key.Length))
            {
                if (propertNameForExport.Key.StartsWith("values."))
                {
                    foreach (var symbolSeparator in SymbolSeparators)
                    {
                        foreach (var symbolTerminator in SymbolTerminators)
                        {
                            validatorDefinitionLine = validatorDefinitionLine.Replace(symbolSeparator + propertNameForExport.Key + symbolTerminator,
                                symbolSeparator + propertNameForExport.Value + symbolTerminator);
                        }
                    }
                }
                else
                {
                    foreach (var symbolTerminator in SymbolTerminators)
                    {
                        validatorDefinitionLine = validatorDefinitionLine.Replace(propertNameForExport.Key + symbolTerminator,
                            propertNameForExport.Value + symbolTerminator);
                    }
                }
            }

            return validatorDefinitionLine;
        }
    }
}
