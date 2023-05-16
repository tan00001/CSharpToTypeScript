using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript.AlternateGenerators
{
    public class TsGeneratorWithForm : TsGeneratorWithResolver
    {
        // Bootstrap column count max. is 12
        public const Int32 MaxColCount = 12;
        public const string TypeScriptXmlFileType = ".tsx";
        public const string BootstrapUtilsNamespace = "BootstrapUtils";

        public bool GenerateBootstrapUtils { get; private set; }

        public Int32 ColCount { get; init; }

        public TsGeneratorWithForm(int colCount, bool enableNamespace)
            : base(enableNamespace)
        {
            if (colCount > MaxColCount)
            {
                throw new ArgumentOutOfRangeException(nameof(colCount), "Column count cannot exceed 12.");
            }

            ColCount = colCount;
        }

        protected override IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> AppendImports(
            TsNamespace @namespace,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>> dependencies)
        {
            if ((generatorOptions.HasFlag(TsGeneratorOptions.Properties) || generatorOptions.HasFlag(TsGeneratorOptions.Fields))
                && (@namespace.Classes.Any(c => !IsIgnored(c)) || @namespace.TypeDefinitions.Any(d => !IsIgnored(d))))
            {
                sb.AppendLine("import { useId } from 'react';");
            }

            return base.AppendImports(@namespace, sb, generatorOptions, dependencies);
        }

        protected override void AppendAdditionalImports(
            TsNamespace @namespace,
            ScriptBuilder sb, 
            TsGeneratorOptions tsGeneratorOptions,
            IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>> dependencies,
            Dictionary<string, IReadOnlyDictionary<string, Int32>> importIndices)
        {
            base.AppendAdditionalImports(@namespace, sb, tsGeneratorOptions, dependencies, importIndices);

            bool hasForms = HasMemeberInfoForOutput(@namespace, tsGeneratorOptions);

            if (hasForms)
            {
                bool hasCheckBoxes = (tsGeneratorOptions.HasFlag(TsGeneratorOptions.Properties) && HasBooleanProperties(@namespace))
                    || (tsGeneratorOptions.HasFlag(TsGeneratorOptions.Fields) && HasBooleanFields(@namespace));
                if (hasCheckBoxes)
                {
                    sb.AppendLineIndented("import { getClassName, getCheckBoxClassName, getErrorMessage } from './" + BootstrapUtilsNamespace + "';");
                    importIndices.Add(BootstrapUtilsNamespace, new Dictionary<string, Int32>()
                    {
                        { "getClassName", 0 },
                        { "getErrorMessage", 0 },
                        { "getCheckBoxClassName", 0 }
                    });
                }
                else
                {
                    sb.AppendLineIndented("import { getClassName, getErrorMessage } from './" + BootstrapUtilsNamespace + "';");
                    importIndices.Add(BootstrapUtilsNamespace, new Dictionary<string, Int32>() { { "getClassName", 0 }, { "getErrorMessage", 0 } });
                }

                GenerateBootstrapUtils = true;
            }
        }

        protected override void AppendAdditionalDependencies(Dictionary<string, TsGeneratorOutput> results)
        {
            base.AppendAdditionalDependencies(results);

            if (GenerateBootstrapUtils)
            {
                ScriptBuilder sb = new(this.IndentationString);

                sb.AppendLine("import { FieldError } from 'react-hook-form';");
                sb.AppendLine();

                sb.AppendLine("export const getClassName = (isValidated: boolean | undefined, error: FieldError | undefined): string =>");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("error ? \"form-control is-invalid\" : (isValidated ? \"form-control is-valid\" : \"form-control\");");
                }

                sb.AppendLine();

                sb.AppendLine("export const getCheckBoxClassName = (isValidated: boolean | undefined, error: FieldError | undefined): string =>");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("error ? \"form-check-input is-invalid\" : (isValidated ? \"form-check-input is-valid\" : \"form-check-input\");");
                }

                sb.AppendLine();

                sb.AppendLine("export const getErrorMessage = (error: FieldError | undefined) =>");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("error && <span className=\"invalid-feedback\">{error.message}</span>;");
                }

                results.Add(BootstrapUtilsNamespace, new TsGeneratorOutput(TypeScriptXmlFileType, sb.ToString()) { ExcludeFromResultToString = true });
            }
        }

        protected override string AppendNamespace(ScriptBuilder sb, TsGeneratorOptions generatorOptions,
            IReadOnlyList<TsClass> moduleClasses, 
            IReadOnlyList<TsTypeDefinition> moduleTypeDefinitions,
            IReadOnlyList<TsInterface> moduleInterfaces,
            IReadOnlyList<TsEnum> moduleEnums, IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            bool hasForms = (generatorOptions.HasFlag(TsGeneratorOptions.Properties) || generatorOptions.HasFlag(TsGeneratorOptions.Fields))
                && (moduleClasses.Any(c => !IsIgnored(c)) || moduleTypeDefinitions.Any(d => !IsIgnored(d)));

            var fileType = base.AppendNamespace(sb, generatorOptions, moduleClasses, moduleTypeDefinitions,
                moduleInterfaces, moduleEnums, importNames);

            if (hasForms)
            {
                return TypeScriptXmlFileType;
            }
            else
            {
                return fileType;
            }
        }

        protected override IReadOnlyList<TsProperty> AppendClassDefinition(
            TsClass classModel,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            var propertiesToExport = base.AppendClassDefinition(classModel, sb, generatorOptions, importNames);

            List<TsProperty> allProperties = classModel.GetBaseProperties(generatorOptions.HasFlag(TsGeneratorOptions.Properties),
                generatorOptions.HasFlag(TsGeneratorOptions.Fields));

            allProperties.AddRange(propertiesToExport);

            sb.AppendLine();

            var hiddenProperties = allProperties.Where(p => p.IsHidden).OrderBy(a => GetPropertyOrdinal(a))
                .ToList();
            var visiblePropertyList = allProperties.Where(p => !p.IsHidden).OrderBy(a => GetPropertyOrdinal(a))
                .ToList();

            if (classModel.ImplementedGenericTypes.Count > 0)
            {
                List<TsProperty> properties = new();
                foreach (var genericType in classModel.ImplementedGenericTypes.Values)
                {
                    AppendGenericClassDefinition(classModel, genericType, sb, propertiesToExport, hiddenProperties,
                        visiblePropertyList, importNames);
                }
            }
            else
            {
                AppendClassDefinition(classModel, sb, propertiesToExport, hiddenProperties, visiblePropertyList);
            }

            return propertiesToExport;
        }

        protected string GetPropertyOrdinal(TsProperty a)
        {
            if (a.Display?.Order != null)
            {
                return a.Display.Order.ToString("000");
            }

            if (a.DataMember?.Order > 0)
            {
                return a.DataMember.Order.ToString("000");
            }

            return this.FormatPropertyName(a);
        }

        protected override IReadOnlyList<TsProperty> AppendTypeDefinition(
            TsTypeDefinition typeDefintionModel,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            var propertiesToExport = base.AppendTypeDefinition(typeDefintionModel, sb, generatorOptions, importNames);

            sb.AppendLine();

            var hiddenProperties = propertiesToExport.Where(p => p.IsHidden).OrderBy(a => GetPropertyOrdinal(a))
                .ToList();
            var visiblePropertyList = propertiesToExport.Where(p => !p.IsHidden).OrderBy(a => GetPropertyOrdinal(a))
                .ToList();

            if (typeDefintionModel.ImplementedGenericTypes.Count > 0)
            {
                List<TsProperty> properties = new();
                foreach (var genericType in typeDefintionModel.ImplementedGenericTypes.Values)
                {
                    if (genericType.Any(t => t.Type.IsGenericParameter))
                    {
                        throw new NotSupportedException("Partially specified generic argument list is not supported.");
                    }

                    AppendGenericTypeDefinition(typeDefintionModel, genericType, sb, propertiesToExport, hiddenProperties,
                        visiblePropertyList, importNames);
                }
            }
            else
            {
                AppendTypeDefinition(typeDefintionModel, sb, propertiesToExport, hiddenProperties, visiblePropertyList);
            }

            return propertiesToExport;
        }

        protected override IReadOnlyList<string> GetReactHookFormComponentNames(TsNamespace @namespace, TsGeneratorOptions generatorOptions)
        {
            List<string> reactHookFormComponentNames = new() { "useForm", "SubmitHandler" };

            reactHookFormComponentNames.AddRange(base.GetReactHookFormComponentNames(@namespace, generatorOptions));

            return reactHookFormComponentNames;
        }

        protected override bool HasAdditionalImports(TsNamespace @namespace, TsGeneratorOptions generatorOptions)
        {
            return (generatorOptions.HasFlag(TsGeneratorOptions.Properties) || generatorOptions.HasFlag(TsGeneratorOptions.Fields))
                && @namespace.Classes.Any(c => !IsIgnored(c));
        }

        #region Private Methods
        private static void AppendButtonRow(ScriptBuilder sb)
        {
            sb.AppendLineIndented("<div className=\"row\">");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("<div className=\"form-group col-md-12\">");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("<button className=\"btn btn-primary\" type=\"submit\" disabled={isSubmitting}>Submit</button>");
                    sb.AppendLineIndented("<button className=\"btn btn-secondary mx-1\" type=\"reset\" disabled={isSubmitting}>Reset</button>");
                }
                sb.AppendLineIndented("</div>");
            }
            sb.AppendLineIndented("</div>");
        }

        private void AppendClassDefinition(TsClass classModel, ScriptBuilder sb, IReadOnlyList<TsProperty> propertiesToExport,
            List<TsProperty> hiddenProperties, List<TsProperty> visiblePropertyList)
        {
            string typeName = this.GetTypeName(classModel) ?? throw new ArgumentException("Class type name cannot be blank.", nameof(classModel));

            AppendFormComponentDefinition(classModel, sb, propertiesToExport, hiddenProperties, visiblePropertyList, typeName, typeName);
        }

        private void AppendFormComponentDefinition(TsModuleMemberWithHierarchy memberModel, ScriptBuilder sb, IReadOnlyList<TsProperty> propertiesToExport,
            List<TsProperty> hiddenProperties, List<TsProperty> visiblePropertyList, string typeName, string resolverName)
        {
            string str = this.GetTypeVisibility(memberModel, typeName) ? "export " : "";

            sb.AppendLineIndented(str + "type " + resolverName + "FormData = {");
            var typeNameInCamelCase = ToCamelCase(resolverName);
            var hasRequiredConstructorParams = propertiesToExport.Any(p => p.IsRequired && string.IsNullOrEmpty(p.GetDefaultValue()));
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented(typeNameInCamelCase + (hasRequiredConstructorParams ? ": " : "?: ") + typeName + ",");
                sb.AppendLineIndented("onSubmit: SubmitHandler<" + typeName + '>');
            }
            sb.AppendLineIndented("};");

            sb.AppendLine();

            sb.AppendLineIndented(str + "const " + resolverName + "Form = (props: " + resolverName + "FormData) => {");

            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("const formId = useId();");
                sb.AppendLineIndented("const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<" + typeName + ">({");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("mode: \"onTouched\",");
                    sb.AppendLineIndented("resolver: " + resolverName + "Resolver,");
                    var defaultValues = "defaultValues: props." + typeNameInCamelCase;
                    if (!hasRequiredConstructorParams && memberModel is TsClass)
                    {
                        defaultValues += " ?? new " + typeName + "()";
                    }
                    sb.AppendLineIndented(defaultValues);
                }
                sb.AppendLineIndented("});");

                sb.AppendLine();

                sb.AppendLineIndented("return <form onSubmit={handleSubmit(props.onSubmit)}>");
                using (sb.IncreaseIndentation())
                {
                    foreach (var property in hiddenProperties)
                    {
                        var propertyName = this.FormatPropertyName(property);
                        AppendHiddenInput(sb, property, propertyName);
                    }

                    // Horizontally pack properties into each row.
                    // Remainder of the property list go into the next row
                    Int32 defaultColSpan = MaxColCount / ColCount;
                    List<TsProperty> propertiesForRow = new();
                    for (var i = 0; i < visiblePropertyList.Count; ++i)
                    {
                        var property = visiblePropertyList[i];
                        propertiesForRow.Add(property);

                        TsProperty? nextProperty = i + 1 < visiblePropertyList.Count ? visiblePropertyList[i + 1] : null;
                        if (nextProperty == null
                            || propertiesForRow.Count >= ColCount
                            || ColSpanHasReachedMax(propertiesForRow, nextProperty, defaultColSpan))
                        {
                            AppendRow(sb, memberModel, propertiesForRow, defaultColSpan);
                            propertiesForRow.Clear();
                        }
                    }
                    AppendButtonRow(sb);
                }
                sb.AppendLineIndented("</form>;");
            }

            sb.AppendLineIndented("};");
        }

        private void AppendGenericClassDefinition(TsClass classModel, IReadOnlyList<TsType> genericArguments,
            ScriptBuilder sb, IReadOnlyList<TsProperty> propertiesToExport, List<TsProperty> hiddenProperties,            
            List<TsProperty> visiblePropertyList,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames)
        {
            string typeName = SubstituteTypeParameters(this.GetTypeName(classModel), genericArguments,
                classModel.NamespaceName, importNames);
            string resolverName = BuildVariableNameWithGenericArguments(typeName);

            AppendFormComponentDefinition(classModel, sb, propertiesToExport, hiddenProperties, visiblePropertyList, typeName, resolverName);
        }

        private void AppendGenericTypeDefinition(TsTypeDefinition typeDefinitionModel, IReadOnlyList<TsType> genericArguments,
            ScriptBuilder sb, IReadOnlyList<TsProperty> propertiesToExport, List<TsProperty> hiddenProperties,
            List<TsProperty> visiblePropertyList,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames)
        {
            string typeName = SubstituteTypeParameters(this.GetTypeName(typeDefinitionModel), genericArguments,
                typeDefinitionModel.NamespaceName, importNames);
            string resolverName = BuildVariableNameWithGenericArguments(typeName);

            AppendFormComponentDefinition(typeDefinitionModel, sb, propertiesToExport, hiddenProperties, visiblePropertyList,
                typeName, resolverName);
        }

        private static void AppendHiddenInput(ScriptBuilder sb, TsProperty property, string propertyName)
        {
            if (property.PropertyType is TsEnum)
            {
                sb.AppendLineIndented("<input type=\"hidden\" id={formId + \"-" + propertyName + "\"} {...register(\"" + propertyName + "\", { valueAsNumber: true })} />");
            }
            else if (property.PropertyType is TsSystemType tsSystemType)
            {
                if (tsSystemType.Kind == SystemTypeKind.Bool)
                {
                    sb.AppendLineIndented("<input type=\"hidden\" id={formId + \"-" + propertyName + "\"} {...register(\"" + propertyName + "\")} />");
                }
                else if (tsSystemType.Kind == SystemTypeKind.Number)
                {
                    sb.AppendLineIndented("<input type=\"hidden\" id={formId + \"-" + propertyName + "\"} {...register(\"" + propertyName + "\", { valueAsNumber: true })} />");
                }
                else if (tsSystemType.Kind == SystemTypeKind.Date)
                {
                    sb.AppendLineIndented("<input type=\"hidden\" id={formId + \"-" + propertyName + "\"} {...register(\"" + propertyName + "\", { valueAsDate: true })} />");
                }
                else
                {
                    sb.AppendLineIndented("<input type=\"hidden\" id={formId + \"-" + propertyName + "\"} {...register(\"" + propertyName + "\")} />");
                }
            }
            else
            {
                sb.AppendLineIndented("<input type=\"hidden\" id={formId + \"-" + propertyName + "\"} {...register(\"" + propertyName + "\")} />");
            }
        }

        private void AppendRow(ScriptBuilder sb, TsModuleMemberWithHierarchy memberModel,
            IReadOnlyList<TsProperty> properties, Int32 defaultColSpan)
        {
            sb.AppendLineIndented("<div className=\"row mb-3\">");
            using (sb.IncreaseIndentation())
            {
                Int32 totalColSpan = 0;
                foreach (var property in properties)
                {
                    var colSpan = property.GetColSpanHint(defaultColSpan, MaxColCount)
                        ?? (MaxColCount - totalColSpan);
                    totalColSpan += colSpan;
                    var propertyName = this.FormatPropertyName(property);                    
                    sb.AppendLineIndented("<div className=\"form-group col-md-" + colSpan + "\">");
                    using (sb.IncreaseIndentation())
                    {
                        AppendVisibleInput(sb, memberModel, property, propertyName);
                        sb.AppendLineIndented("{getErrorMessage(errors." + propertyName + ")}");
                    }
                    sb.AppendLineIndented("</div>");
                }
            }
            sb.AppendLineIndented("</div>");
        }

        private void AppendTypeDefinition(TsTypeDefinition typeDefinitionModel, ScriptBuilder sb, IReadOnlyList<TsProperty> propertiesToExport,
            List<TsProperty> hiddenProperties, List<TsProperty> visiblePropertyList)
        {
            string typeName = this.GetTypeName(typeDefinitionModel) ?? throw new ArgumentException("Type definition name cannot be blank.", nameof(typeDefinitionModel));

            AppendFormComponentDefinition(typeDefinitionModel, sb, propertiesToExport, hiddenProperties, visiblePropertyList, typeName, typeName);
        }

        private static void AppendVisibleInput(ScriptBuilder sb, TsModuleMemberWithHierarchy memberModel, TsProperty property, string propertyName)
        {
            if (property.UiHint?.UIHint == TsProperty.UiHintSelect)
            {
                var typeContainingOptions = property.UiHint.ControlParameters[TsProperty.UiHintTypeContainingOptions];
                if (typeContainingOptions == null)
                {
                    typeContainingOptions = memberModel.Type;
                }
                var nameOfOptions = property.UiHint.ControlParameters[TsProperty.UiHintNameOfOptions];
                if (typeContainingOptions is not Type optionsType || nameOfOptions is not string options
                    || string.IsNullOrEmpty(options))
                {
                    throw new ArgumentException("Select control must have \"" + TsProperty.UiHintTypeContainingOptions
                        + "\" and \"" + nameOfOptions + "\" specified.", nameof(property));
                }

                var optionsCollection = optionsType.GetField(options, System.Reflection.BindingFlags.Public 
                    |System.Reflection.BindingFlags.Static) ?? optionsType.GetField(options, System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Static);
                if (optionsCollection is null
                    || optionsCollection.GetValue(null) is not IEnumerable<string> optionList) 
                {
                    throw new ArgumentException("Select options \"" + TsProperty.UiHintTypeContainingOptions
                        + "\" and \"" + nameOfOptions + "\" must specify a collection of strings.", nameof(property));
                }
                var displayPrompt = property.GetDisplayPrompt() ?? (property.GetDisplayName() + ':');
                sb.AppendLineIndented("<label htmlFor={formId + \"-" + propertyName + "\"}>" + displayPrompt + "</label>");
                sb.AppendLineIndented("<select className={getClassName(touchedFields." + propertyName + ", errors." + propertyName + ")} id={formId + \"-" + propertyName + "\"} {...register(\"" + propertyName + "\", { valueAsNumber: true })}>");
                using (sb.IncreaseIndentation())
                {
                    if (!property.IsRequired)
                    {
                        sb.AppendLineIndented("<option value=\"\">Select a " + property.GetDisplayName() + "</option>");
                    }
                    foreach (var option in optionList)
                    {
                        sb.AppendLineIndented("<option value=\"" + option + "\">" + option + "</option>");
                    }
                }
                sb.AppendLineIndented("</select>");
            }
            else if (property.PropertyType is TsEnum tsEnum)
            {
                var displayPrompt = property.GetDisplayPrompt() ?? (property.GetDisplayName() + ':');
                sb.AppendLineIndented("<label htmlFor={formId + \"-" + propertyName + "\"}>" + displayPrompt + "</label>");
                sb.AppendLineIndented("<select className={getClassName(touchedFields." + propertyName + ", errors." + propertyName + ")} id={formId + \"-" + propertyName + "\"} {...register(\"" + propertyName + "\", { valueAsNumber: true })}>");
                using (sb.IncreaseIndentation())
                {
                    if (!property.IsRequired)
                    {
                        sb.AppendLineIndented("<option value=\"\">Select a " + property.GetDisplayName() + "</option>");
                    }
                    foreach (var enumValue in tsEnum.Values)
                    {
                        sb.AppendLineIndented("<option value=\"" + enumValue.Value + "\">" + enumValue.GetDisplayName() + "</option>");
                    }
                }
                sb.AppendLineIndented("</select>");
            }
            else if (property.PropertyType is TsSystemType tsSystemType)
            {
                if (tsSystemType.Kind == SystemTypeKind.Bool)
                {
                    // The prompt is different here in that there is no trailing ":"
                    var displayPrompt = property.GetDisplayPrompt() ?? property.GetDisplayName();
                    sb.AppendLineIndented("<input type=\"" + GetInputType(property, tsSystemType.Kind)
                        + "\" className={getCheckBoxClassName(touchedFields." + propertyName + ", errors." + propertyName + ")} id={formId + \"-"
                        + propertyName + "\"} {...register(\"" + propertyName + "\")} />");
                    sb.AppendLineIndented("<label className=\"form-check-label ms-1\" htmlFor={formId + \"-" + propertyName + "\"}>" + displayPrompt + "</label>");
                }
                else if (tsSystemType.Kind == SystemTypeKind.Number)
                {
                    var displayPrompt = property.GetDisplayPrompt() ?? (property.GetDisplayName() + ':');
                    sb.AppendLineIndented("<label htmlFor={formId + \"-" + propertyName + "\"}>" + displayPrompt + "</label>");
                    sb.AppendLineIndented("<input type=\"" + GetInputType(property, tsSystemType.Kind)
                        + "\" className={getClassName(touchedFields." + propertyName + ", errors." + propertyName + ")} id={formId + \"-"
                        + propertyName + "\"} {...register(\"" + propertyName + "\", { valueAsNumber: true })} />");
                }
                else if (tsSystemType.Kind == SystemTypeKind.Date)
                {
                    var displayPrompt = property.GetDisplayPrompt() ?? (property.GetDisplayName() + ':');
                    sb.AppendLineIndented("<label htmlFor={formId + \"-" + propertyName + "\"}>" + displayPrompt + "</label>");
                    sb.AppendLineIndented("<input type=\"" + GetInputType(property, tsSystemType.Kind)
                        + "\" className={getClassName(touchedFields." + propertyName + ", errors." + propertyName + ")} id={formId + \"-"
                        + propertyName + "\"} {...register(\"" + propertyName + "\", { valueAsDate: true })} />");
                }
                else
                {
                    var displayPrompt = property.GetDisplayPrompt() ?? (property.GetDisplayName() + ':');
                    sb.AppendLineIndented("<label htmlFor={formId + \"-" + propertyName + "\"}>" + displayPrompt + "</label>");
                    sb.AppendLineIndented("<input type=\"" + GetInputType(property, tsSystemType.Kind)
                        + "\" className={getClassName(touchedFields." + propertyName + ", errors." + propertyName + ")} id={formId + \"-"
                        + propertyName + "\"} {...register(\"" + propertyName + "\")} />");
                }
            }
            else
            {
                var displayPrompt = property.GetDisplayPrompt() ?? (property.GetDisplayName() + ':');
                sb.AppendLineIndented("<label htmlFor=\"" + propertyName + "\">" + displayPrompt + "</label>");
                sb.AppendLineIndented("<input type=\"text\" className={getClassName(touchedFields." + propertyName + ", errors." + propertyName + ")} id=\"" + propertyName
                    + "\" {...register(\"" + propertyName + "\")} />");
            }
        }

        private static bool ColSpanHasReachedMax(List<TsProperty> propertiesForRow, TsProperty nextProperty, Int32 defaultColSpan)
        {
            var totalColSpanSoFar = propertiesForRow.Sum(p => p.GetColSpanHint(defaultColSpan, MaxColCount) ?? MaxColCount);
            if (totalColSpanSoFar >= MaxColCount)
            {
                return true;
            }

            var nextColSpan = nextProperty.GetColSpanHint(defaultColSpan, MaxColCount);
            if (nextColSpan == null)
            {
                return false;
            }

            return totalColSpanSoFar + nextColSpan.Value > MaxColCount;
        }

        private static string GetInputType(TsProperty property, SystemTypeKind kind)
        {
            if (!string.IsNullOrEmpty(property.UiHint?.UIHint))
            {
                return property.UiHint.UIHint;
            }

            switch (kind)
            {
                case SystemTypeKind.Number:
                    return "number";
                case SystemTypeKind.Bool:
                    return "checkbox";
                case SystemTypeKind.Date:
                    return "date";
                default:
                    return property.ValidationRules switch 
                        { 
                            _ when property.ValidationRules.Any(r => r is UrlRule) => "url",
                            _ when property.ValidationRules.Any(r => r is EmailAddressRule) => "email",
                            _ when property.ValidationRules.Any(r => r is PhoneNumberRule) => "tel",
                            _ => "text"
                        };
            }
        }

        private static bool HasBooleanFields(TsNamespace @namespace)
        {
            return (@namespace.Classes.Any(c => c.Fields.Any(p => p.PropertyType is TsSystemType systemType && systemType.Kind == SystemTypeKind.Bool))
                                        || @namespace.TypeDefinitions.Any(c => c.Fields.Any(p => p.PropertyType is TsSystemType systemType && systemType.Kind == SystemTypeKind.Bool)));
        }

        private static bool HasBooleanProperties(TsNamespace @namespace)
        {
            return (@namespace.Classes.Any(c => c.Properties.Any(p => p.PropertyType is TsSystemType systemType && systemType.Kind == SystemTypeKind.Bool))
                                    || @namespace.TypeDefinitions.Any(c => c.Fields.Any(p => p.PropertyType is TsSystemType systemType && systemType.Kind == SystemTypeKind.Bool)));
        }
        #endregion // Private Methods
    }
}
