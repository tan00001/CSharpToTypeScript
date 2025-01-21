using CSharpToTypeScript.Models;

using System.Reflection.Metadata;

namespace CSharpToTypeScript.AlternateGenerators
{
    public class TsGeneratorWithVue : TsGeneratorWithVuelidate
    {
        // Bootstrap column count max. is 12
        public const int MaxColCount = 12;
        public const string TypeScriptXmlFileType = ".vue";
        public const string BootstrapUtilsNamespace = "BootstrapUtils";

        public bool GenerateVuelidateRules { get; private set; }

        public int ColCount { get; init; }

        public string? BootstrapModalTitle { get; init; }

        public TsGeneratorWithVue(int colCount, string? bootstrapModalTitle, bool enableNamespace)
            : base(enableNamespace)
        {
            if (colCount > MaxColCount)
            {
                throw new ArgumentOutOfRangeException(nameof(colCount), "Column count cannot exceed 12.");
            }

            ColCount = colCount;
            BootstrapModalTitle = bootstrapModalTitle;
        }

        protected override void OnNamespaceAppended(TsNamespace @namespace,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importedNames,
            TsGeneratorOutput output,
            TsGeneratorOptions generatorOptions)
        {
            List<TsClass> classes = @namespace.Classes.Where(c => !IsIgnored(c))
                .OrderBy(c => this.GetTypeName(c))
                .ToList();

            List<TsTypeDefinition> typeDefinitions = @namespace.TypeDefinitions.Where(d => !IsIgnored(d))
                .OrderBy(GetTypeName)
                .ToList();

            bool hasForms = (generatorOptions.HasFlag(TsGeneratorOptions.Properties) || generatorOptions.HasFlag(TsGeneratorOptions.Fields))
                && (classes.Any(c => !IsIgnored(c)) || typeDefinitions.Any(d => !IsIgnored(d)));
            
            if (!hasForms)
            {
                return;
            }

            foreach (var typeDefinition in typeDefinitions)
            {
                if (IsIgnored(typeDefinition))
                {
                    continue;
                }

                if (typeDefinition.GenericArguments.Count > 0 && typeDefinition.GenericArguments.Any(a => a.Type.IsGenericParameter))
                {
                    continue;
                }

                AppendTypeDefinition(typeDefinition, generatorOptions, importedNames, output);
            }

            foreach (var @class in classes)
            {
                if (IsIgnored(@class))
                {
                    continue;
                }

                if (@class.GenericArguments.Count > 0 && @class.GenericArguments.Any(a => a.Type.IsGenericParameter))
                {
                    continue;
                }

                AppendClassDefinition(@class, generatorOptions, importedNames, output);
            }
        }

        private void AppendClassDefinition(TsClass @class,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importedNames,
            TsGeneratorOutput output)
        {
            ScriptBuilder sb = new(this.IndentationString);

            string typeName = this.GetTypeName(@class) ?? throw new ArgumentException("Class type name cannot be blank.", nameof(@class));

            sb.AppendLine("<script setup lang=\"ts\">");
            using (sb.IncreaseIndentation())
            {
                CreateVueScript(sb, generatorOptions, typeName, importedNames);
            }
            sb.AppendLine("</script>");

            sb.AppendLine();

            sb.AppendLine("<template>");
            using (sb.IncreaseIndentation())
            {
                AppendClassDefinitionVueTemplate(sb, generatorOptions, @class, importedNames);
            }
            sb.AppendLine("</template>");

            output.OtherFileTypes.Add(typeName + TypeScriptXmlFileType, sb.ToString());
        }

        private void AppendTypeDefinition(TsTypeDefinition typeDefinition,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importedNames,
            TsGeneratorOutput output)
        {
            ScriptBuilder sb = new(this.IndentationString);

            string typeName = this.GetTypeName(typeDefinition) ?? throw new ArgumentException("Class type name cannot be blank.", nameof(typeDefinition));

            sb.AppendLine("<script setup lang=\"ts\">");
            using (sb.IncreaseIndentation())
            {
                CreateVueScript(sb, generatorOptions, typeName, importedNames);
            }
            sb.AppendLine("</script>");

            sb.AppendLine();

            sb.AppendLine("<template>");
            using (sb.IncreaseIndentation())
            {
                AppendTypeDefinitionVueTemplate(sb, generatorOptions, typeDefinition, importedNames);
            }
            sb.AppendLine("</template>");

            output.OtherFileTypes.Add(typeName + TypeScriptXmlFileType, sb.ToString());
        }

        #region Private Methods
        private void CreateVueScript(ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            string typeName,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames)
        {
            sb.AppendLineIndented("import { ref, type PropType } from 'vue';");
            sb.AppendLineIndented("import { type " + typeName + ", " + typeName + "Rules } from './" + typeName + "'");
            sb.AppendLineIndented("import { useVuelidate, type ValidationArgs } from '@vuelidate/core';");
            sb.AppendLine();
            sb.AppendLineIndented($"const inputData = defineModel<{typeName}>('{typeName}');");
            sb.AppendLine();
            sb.AppendLineIndented("const props = defineProps({");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("formId: {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: String,");
                    sb.AppendLineIndented($"default: '{typeName}'");
                }
                sb.AppendLineIndented("});");
            }
            sb.AppendLineIndented("});");
            sb.AppendLine();
            sb.AppendLineIndented($"const formData = ref<{typeName}>(inputData ?? new {typeName}())");
            sb.AppendLineIndented("const submitting = ref<boolean>(false);");
            if (!string.IsNullOrEmpty(BootstrapModalTitle))
            {
                sb.AppendLineIndented($"const {typeName}Dlg = ref<HTMLDivElement | null>(null);");
            }
            sb.AppendLineIndented($"const {typeName}Form = ref<HTMLFormElement | null>(null);");
            sb.AppendLineIndented($"const v$ = useVuelidate<{typeName}>({typeName}Rules, formData.value);");
            sb.AppendLine();
            sb.AppendLineIndented("const getClassName = (dirty: boolean | undefined, hasError: boolean | undefined): string => hasError ? 'form-control is-invalid' : (dirty ? 'form-control is-valid' : 'form-control');");
            sb.AppendLineIndented("const getCheckBoxClassName = (dirty: boolean | undefined, hasError: boolean | undefined): string => hasError ? 'form-check-input is-invalid' : (dirty ? 'form-check-input is-valid' : 'form-check-input');");
            sb.AppendLine();
            sb.AppendLineIndented("const onSubmit = async () => {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("submitting.value = true;");
                sb.AppendLine();
                sb.AppendLineIndented("v$.value.$touch();");
                sb.AppendLineIndented("if (v$.value.$error) {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("console.log('Form is invalid');");
                    sb.AppendLineIndented("return;");
                }
                sb.AppendLineIndented("}");

                sb.AppendLineIndented("submitting.value = false;");
            }

            sb.AppendLineIndented("};");
        }

        private void AppendTypeDefinitionVueTemplate(ScriptBuilder sb, TsGeneratorOptions generatorOptions,
            TsTypeDefinition typeDefinition, IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames)
        {
            IReadOnlyList<TsProperty> propertiesToExport = typeDefinition.GetMemeberInfoForOutput(generatorOptions);

            var hiddenProperties = propertiesToExport.Where(p => p.IsHidden).OrderBy(a => GetPropertyOrdinal(a))
                .ToList();
            var visiblePropertyList = propertiesToExport.Where(p => !p.IsHidden).OrderBy(a => GetPropertyOrdinal(a))
                .ToList();

            if (typeDefinition.GenericArguments.Count > 0)
            {
                AppendVueTemplateForGenericTypeDefinition(typeDefinition, sb, propertiesToExport, hiddenProperties,
                    visiblePropertyList, importNames);
            }
            else
            {
                AppendVueTemplateForTypeDefinition(typeDefinition, sb, propertiesToExport, hiddenProperties, visiblePropertyList);
            }
        }

        private void AppendClassDefinitionVueTemplate(ScriptBuilder sb, TsGeneratorOptions generatorOptions,
            TsClass @class,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames)
        {
            List<TsProperty> propertiesToExport = new();

            if (generatorOptions.HasFlag(TsGeneratorOptions.Properties))
                propertiesToExport.AddRange(@class.GetMemeberInfoForOutput(TsGeneratorOptions.Properties));
            if (generatorOptions.HasFlag(TsGeneratorOptions.Fields))
                propertiesToExport.AddRange(@class.GetMemeberInfoForOutput(TsGeneratorOptions.Fields));

            List<TsProperty> allProperties = @class.GetBaseProperties(generatorOptions.HasFlag(TsGeneratorOptions.Properties),
                generatorOptions.HasFlag(TsGeneratorOptions.Fields)).Where(p => !p.HasIgnoreAttribute).ToList();

            allProperties.AddRange(propertiesToExport);

            var hiddenProperties = allProperties.Where(p => p.IsHidden).OrderBy(a => GetPropertyOrdinal(a))
                .ToList();
            var visiblePropertyList = allProperties.Where(p => !p.IsHidden).OrderBy(a => GetPropertyOrdinal(a))
                .ToList();

            if (@class.GenericArguments.Count > 0)
            {
                AppendVueTemplateForGenericClassDefinition(@class, sb, propertiesToExport, hiddenProperties,
                    visiblePropertyList, importNames);
            }
            else
            {
                AppendVueTemplateForClassDefinition(@class, sb, propertiesToExport, hiddenProperties, visiblePropertyList);
            }
        }

        private void AppendVueTemplateForClassDefinition(TsClass classModel, ScriptBuilder sb, IReadOnlyList<TsProperty> propertiesToExport,
            List<TsProperty> hiddenProperties, List<TsProperty> visiblePropertyList)
        {
            string typeName = GetTypeName(classModel) ?? throw new ArgumentException("Class type name cannot be blank.", nameof(classModel));

            CreateVueTemplate(classModel, sb, propertiesToExport, hiddenProperties, visiblePropertyList, typeName);
        }

        private void CreateVueTemplate(TsModuleMemberWithHierarchy memberModel, ScriptBuilder sb, IReadOnlyList<TsProperty> propertiesToExport,
            List<TsProperty> hiddenProperties, List<TsProperty> visiblePropertyList, string typeName)
        {
            if (string.IsNullOrEmpty(BootstrapModalTitle))
            {
                sb.AppendLineIndented($"<form @submit.prevent=\"onSubmit\" ref=\"{typeName}Form\" class=\"needs-validation\">");
                using (sb.IncreaseIndentation())
                {
                    AppendFormFields(memberModel, sb, hiddenProperties, visiblePropertyList);
                    AppendButtonRow(sb);
                }
                sb.AppendLineIndented("</form>");
            }
            else
            {
                sb.AppendLineIndented($"<div class=\"modal\" tabindex=\"-1\" :id=\"formId\" aria-labelledby=\"{typeName}DlgTitle\" aria-hidden=\"true\" data-bs-backdrop=\"static\" ref=\"{typeName}Dlg\">");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("<div class=\"modal-dialog\">");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("<div class=\"modal-content\">");
                        using (sb.IncreaseIndentation())
                        {
                            sb.AppendLineIndented("<div class=\"modal-header\">");
                            using (sb.IncreaseIndentation())
                            {
                                sb.AppendLineIndented($"<h5 class=\"modal-title\" id=\"{typeName}DlgTitle\">{BootstrapModalTitle}</h5>");
                                sb.AppendLineIndented("<button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"modal\" aria-label=\"Close\"></button>");
                            }
                            sb.AppendLineIndented("</div>");
                            sb.AppendLineIndented($"<form @submit.prevent=\"onSubmit\" ref=\"{typeName}Form\" class=\"needs-validation\">");
                            using (sb.IncreaseIndentation())
                            {
                                sb.AppendLineIndented("<div class=\"modal-body\">");
                                using (sb.IncreaseIndentation())
                                {
                                    AppendFormFields(memberModel, sb, hiddenProperties, visiblePropertyList);
                                }
                                sb.AppendLineIndented("</div>");
                                sb.AppendLineIndented("<div class=\"modal-footer\">");
                                using (sb.IncreaseIndentation())
                                {
                                    AppendButtonRow(sb);
                                }
                                sb.AppendLineIndented("</div>");
                            }
                            sb.AppendLineIndented("</form>");
                        }
                        sb.AppendLineIndented("</div>");
                    }
                    sb.AppendLineIndented("</div>");
                }
                sb.AppendLineIndented("</div>");
            }

            static void AppendButtonRow(ScriptBuilder sb)
            {
                sb.AppendLineIndented("<div className=\"row\">");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("<div className=\"form-group col-md-12\">");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("<button className=\"btn btn-primary\" type=\"submit\" :disabled=\"submitting\">Submit</button>");
                        sb.AppendLineIndented("<button className=\"btn btn-secondary mx-1\" type=\"reset\" :disabled=\"submitting\">Reset</button>");
                    }
                    sb.AppendLineIndented("</div>");
                }
                sb.AppendLineIndented("</div>");
            }

            void AppendFormFields(TsModuleMemberWithHierarchy memberModel, ScriptBuilder sb, List<TsProperty> hiddenProperties, List<TsProperty> visiblePropertyList)
            {
                foreach (var property in hiddenProperties)
                {
                    var propertyName = FormatPropertyName(property);
                    AppendHiddenInput(sb, property, propertyName);
                }

                // Horizontally pack properties into each row.
                // Remainder of the property list go into the next row
                int defaultColSpan = MaxColCount / ColCount;
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
            }

            static void AppendHiddenInput(ScriptBuilder sb, TsProperty property, string propertyName)
            {
                if (property.PropertyType is TsEnum)
                {
                    sb.AppendLineIndented("<input type=\"hidden\" :id=\"formId + '-" + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\"/>");
                }
                else if (property.PropertyType is TsSystemType tsSystemType)
                {
                    if (tsSystemType.Kind == SystemTypeKind.Bool)
                    {
                        sb.AppendLineIndented("<input type=\"hidden\" :id=\"formId + '-" + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\"/>");
                    }
                    else if (tsSystemType.Kind == SystemTypeKind.Number)
                    {
                        sb.AppendLineIndented("<input type=\"hidden\" :id=\"formId + '-" + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\"/>");
                    }
                    else if (tsSystemType.Kind == SystemTypeKind.Date)
                    {
                        sb.AppendLineIndented("<input type=\"hidden\" :id=\"formId + '-" + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\"/>");
                    }
                    else
                    {
                        sb.AppendLineIndented("<input type=\"hidden\" :id=\"formId + '-" + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\"/>");
                    }
                }
                else
                {
                    sb.AppendLineIndented("<input type=\"hidden\" :id=\"formId + '-" + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\"/>");
                }
            }

            void AppendRow(ScriptBuilder sb, TsModuleMemberWithHierarchy memberModel,
                IReadOnlyList<TsProperty> properties, int defaultColSpan)
            {
                sb.AppendLineIndented("<div className=\"row mb-3\">");
                using (sb.IncreaseIndentation())
                {
                    int totalColSpan = 0;
                    foreach (var property in properties)
                    {
                        var colSpan = property.GetColSpanHint(defaultColSpan, MaxColCount)
                            ?? MaxColCount - totalColSpan;
                        totalColSpan += colSpan;
                        var propertyName = FormatPropertyName(property);
                        sb.AppendLineIndented("<div class=\"form-group col-md-" + colSpan + "\">");
                        using (sb.IncreaseIndentation())
                        {
                            AppendVisibleInput(sb, memberModel, property, propertyName);
                            sb.AppendLineIndented($"<div v-if='v$.{propertyName}.$dirty && v$.{propertyName}.$error' class='form-error'>");
                            using (sb.IncreaseIndentation())
                            {
                                sb.AppendLineIndented($"<span v-for='rule in Object.keys($v.{propertyName})' :key='rule'>" + "{{$v." + propertyName + "[rule].$message}}</span>");
                            }
                            sb.AppendLineIndented("</div>");
                        }
                        sb.AppendLineIndented("</div>");
                    }
                }
                sb.AppendLineIndented("</div>");
            }

            static void AppendVisibleInput(ScriptBuilder sb, TsModuleMemberWithHierarchy memberModel, TsProperty property, string propertyName)
            {
                bool isReadOnly = property.UiHint != null && property.UiHint.ControlParameters.TryGetValue(TsProperty.UiHintReadOnly,
                    out var readOnlySetting) && readOnlySetting != null && readOnlySetting is bool readOnly && readOnly == true;
                if (property.UiHint?.UIHint == TsProperty.UiHintSelect)
                {
                    property.UiHint.ControlParameters.TryGetValue(TsProperty.UiHintTypeContainingOptions, out var typeContainingOptions);
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
                    System.Reflection.FieldInfo? optionsCollection = GetStaticField(optionsType, options);
                    if (optionsCollection is null
                        || optionsCollection.GetValue(null) is not IEnumerable<string> optionList)
                    {
                        throw new ArgumentException("Select options \"" + TsProperty.UiHintTypeContainingOptions
                            + "\" and \"" + nameOfOptions + "\" must specify a collection of strings.", nameof(property));
                    }
                    var displayPrompt = property.GetDisplayPrompt() ?? property.GetDisplayName() + ':';
                    sb.AppendLineIndented("<label :for=\"formId + '-" + propertyName + "'\">" + displayPrompt + "</label>");
                    sb.AppendLineIndented("<select :class=\"getClassName(v$." + propertyName + ".$dirty, v$." + propertyName
                        + ".$error)\" :id=\"formId + '-" + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\">");

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
                    var displayPrompt = property.GetDisplayPrompt() ?? property.GetDisplayName() + ':';
                    sb.AppendLineIndented("<label :for=\"formId + '-" + propertyName + "'\">" + displayPrompt + "</label>");
                    sb.AppendLineIndented("<select :class=\"getClassName(v$.." + propertyName + ".$dirty, v$." + propertyName 
                        + ".$error)\" :id=\"formId + '-" + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\">");
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
                    var inputType = GetInputType(property, tsSystemType.Kind);
                    if (tsSystemType.Kind == SystemTypeKind.Bool)
                    {
                        // The prompt is different here in that there is no trailing ":"
                        var displayPrompt = property.GetDisplayPrompt() ?? property.GetDisplayName();
                        sb.AppendLineIndented("<input type=\"" + inputType
                            + "\" :class=\"getCheckBoxClassName(v$." + propertyName + ".$dirty, v$." + propertyName 
                            + ".$error)\" :id=\"formId + '-" + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\"/>");
                        sb.AppendLineIndented("<label class=\"form-check-label ms-1\" :for=\"formId + '-" + propertyName + "'\"}>" + displayPrompt + "</label>");
                    }
                    else if (tsSystemType.Kind == SystemTypeKind.Number)
                    {
                        var displayPrompt = property.GetDisplayPrompt() ?? property.GetDisplayName() + ':';
                        sb.AppendLineIndented("<label :for=\"formId + '-" + propertyName + "'\">" + displayPrompt + "</label>");
                        sb.AppendLineIndented("<input type=\"" + inputType
                            + "\" :class=\"getClassName(v$." + propertyName + ".$dirty, v$." + propertyName
                            + (isReadOnly ? ".$error)\" readonly :id=\"formId + '-" : ".$error)\" :id=\"formId + '-")
                            + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\"/>");
                    }
                    else if (tsSystemType.Kind == SystemTypeKind.Date)
                    {
                        var displayPrompt = property.GetDisplayPrompt() ?? property.GetDisplayName() + ':';
                        sb.AppendLineIndented("<label :for=\"formId + '-" + propertyName + "'\"}>" + displayPrompt + "</label>");
                        sb.AppendLineIndented("<input type=\"" + inputType
                            + "\" :class=\"getClassName(v$." + propertyName + ".$dirty, v$." + propertyName
                            + (isReadOnly ? ".$error)\" readonly :id=\"formId + '-" : ".$error)\" :id=\"formId + '-")
                            + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\"/>");
                    }
                    else
                    {
                        var displayPrompt = property.GetDisplayPrompt() ?? property.GetDisplayName() + ':';
                        sb.AppendLineIndented("<label :for=\"formId + '-" + propertyName + "'\">" + displayPrompt + "</label>");
                        sb.AppendLineIndented(inputType == "textarea" ? "<textarea" : "<input type=\"" + inputType + '"'
                            + " :class=\"getClassName(v$." + propertyName + ".$dirty, v$." + propertyName
                            + (isReadOnly ? ".$error)\" readonly :id=\"formId + '-" : ".$error)\" :id=\"formId + '-")
                            + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\"/>");
                    }
                }
                else
                {
                    var inputType = GetInputType(property);
                    var displayPrompt = property.GetDisplayPrompt() ?? property.GetDisplayName() + ':';
                    sb.AppendLineIndented("<label :for=\"formId + '-" + propertyName + "'\">" + displayPrompt + "</label>");
                    sb.AppendLineIndented(inputType == "textarea" ? "<textarea" : "<input type=\"" + inputType + '"'
                        + " :class=\"getClassName(v$."
                        + propertyName + ".$dirty, v$." + propertyName
                        + (isReadOnly ? ".$error)\" readonly :id=\"formId + '-" : ".$error)\" :id=\"formId + '-")
                        + propertyName + "'\" @blur=\"v$." + propertyName + ".$touch\"/>");
                }

                static System.Reflection.FieldInfo? GetStaticField(Type optionsType, string options, bool includePrivateField = true)
                {
                    var staticField = optionsType.GetField(options, System.Reflection.BindingFlags.Public
                        | System.Reflection.BindingFlags.Static);
                    if (staticField != null)
                    {
                        return staticField;
                    }

                    staticField = optionsType.GetField(options, System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Static);
                    if (staticField != null)
                    {
                        if (!includePrivateField)
                        {
                            if (!staticField.IsFamily)
                            {
                                return null;
                            }
                        }

                        return staticField;
                    }

                    if (optionsType.BaseType != null)
                    {
                        return GetStaticField(optionsType.BaseType, options, false);
                    }

                    return null;
                }
            }
        }

        private void AppendVueTemplateForGenericClassDefinition(TsClass classModel, ScriptBuilder sb,
            IReadOnlyList<TsProperty> propertiesToExport, List<TsProperty> hiddenProperties,
            List<TsProperty> visiblePropertyList,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames)
        {
            string typeName = SubstituteTypeParameters(GetTypeName(classModel), classModel.GenericArguments,
                classModel.NamespaceName, importNames);
            CreateVueTemplate(classModel, sb, propertiesToExport, hiddenProperties, visiblePropertyList, typeName);
        }

        private void AppendVueTemplateForGenericTypeDefinition(TsTypeDefinition typeDefinitionModel,
            ScriptBuilder sb, IReadOnlyList<TsProperty> propertiesToExport, List<TsProperty> hiddenProperties,
            List<TsProperty> visiblePropertyList,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames)
        {
            string typeName = SubstituteTypeParameters(GetTypeName(typeDefinitionModel), typeDefinitionModel.GenericArguments,
                typeDefinitionModel.NamespaceName, importNames);

            CreateVueTemplate(typeDefinitionModel, sb, propertiesToExport, hiddenProperties, visiblePropertyList,
                typeName);
        }

        private void AppendVueTemplateForTypeDefinition(TsTypeDefinition typeDefinitionModel, ScriptBuilder sb, IReadOnlyList<TsProperty> propertiesToExport,
            List<TsProperty> hiddenProperties, List<TsProperty> visiblePropertyList)
        {
            string typeName = GetTypeName(typeDefinitionModel) ?? throw new ArgumentException("Type definition name cannot be blank.", nameof(typeDefinitionModel));

            CreateVueTemplate(typeDefinitionModel, sb, propertiesToExport, hiddenProperties, visiblePropertyList, typeName);
        }

        private static bool ColSpanHasReachedMax(List<TsProperty> propertiesForRow, TsProperty nextProperty, int defaultColSpan)
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

        private static string GetInputType(TsProperty property)
        {
            if (!string.IsNullOrEmpty(property.UiHint?.UIHint))
            {
                return property.UiHint.UIHint;
            }

            return "text";
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
            return @namespace.Classes.Any(c => c.Fields.Any(p => p.PropertyType is TsSystemType systemType && systemType.Kind == SystemTypeKind.Bool))
                                        || @namespace.TypeDefinitions.Any(c => c.Fields.Any(p => p.PropertyType is TsSystemType systemType && systemType.Kind == SystemTypeKind.Bool));
        }

        private static bool HasBooleanProperties(TsNamespace @namespace)
        {
            return @namespace.Classes.Any(c => c.Properties.Any(p => p.PropertyType is TsSystemType systemType && systemType.Kind == SystemTypeKind.Bool))
                || @namespace.TypeDefinitions.Any(c => c.Fields.Any(p => p.PropertyType is TsSystemType systemType && systemType.Kind == SystemTypeKind.Bool));
        }
        #endregion // Private Methods
    }
}
