using System.ComponentModel.DataAnnotations;
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
        public static readonly CustomValidationRuleComparer Comparer = new ();

        public readonly CustomValidationAttribute _CustomValidation;

        public TsModuleMemberWithHierarchy? ValidatorType { get; set; }

        public string? ValidatorTypeName { get; set; }

        public HashSet<TsType> TargetTypes { get; private set; } = new();

        public CustomValidationRule(CustomValidationAttribute customValidation)
        {
            _CustomValidation = customValidation;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties)
        {
            sb.AppendLineIndented("errors." + propertyName + " ??= " 
                + (string.IsNullOrEmpty(ValidatorTypeName) ? "" : (ValidatorTypeName + '.'))
                + _CustomValidation.Method + "(values);");
        }

        public void AddValidationFunction(ScriptBuilder sb, string typeName)
        {
            sb.AppendLineIndented("static " + _CustomValidation.Method + "(values: " + typeName + "): FieldError | undefined {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("// TODO: Please implement this function.");

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
                            metaDataReader.GetString(metaDataReader.GetMethodDefinition(m).Name) == method.Name);

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

                            if (startLine > 0 && endLine >= startLine)
                            {
                                var sourceFileContents = File.ReadAllText(documentPath).Split("\r\n");
                                if (startLine < sourceFileContents.Length && endLine < sourceFileContents.Length)
                                {
                                    for (var i = startLine - 1; i <= endLine -1; ++i)
                                    {
                                        sb.AppendLineIndented("// " + sourceFileContents[i]);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) 
                {
                    sb.AppendLineIndented("// Unable to read symbol file. " + ex.Message);
                }

                sb.AppendLineIndented("return {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: \"custom\",");
                    sb.AppendLineIndented("message: \"" + _CustomValidation.Method + " is to be implemented\"");
                };
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("};");
        }
    }
}
