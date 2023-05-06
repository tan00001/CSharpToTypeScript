#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using CSharpToTypeScript.Models;
using Microsoft.Win32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSharpToTypeScript.AlternateGenerators
{
    public class TsGeneratorWithForm : TsGeneratorWithResolver
    {
        // Bootstrap column count max. is 12
        public const Int32 MaxColCount = 12;

        public Int32 ColCount { get; init; }

        public readonly IReadOnlyList<Int32> ColumnWidths;

        public TsGeneratorWithForm(int colCount, bool enableNamespace)
            : base(enableNamespace)
        {
            if (colCount > MaxColCount)
            {
                throw new ArgumentOutOfRangeException(nameof(colCount), "Column count cannot exceed 12.");
            }

            ColCount = colCount;

            var columnWidths = new int[colCount];
            var averageColumnWidth = MaxColCount / colCount;
            for (var i = 0; i < colCount; ++i)
            {
                if (i < colCount - 1)
                {
                    columnWidths[i] = averageColumnWidth;
                }
                else
                {
                    var remainder = MaxColCount % colCount;
                    columnWidths[i] = remainder == 0 ? averageColumnWidth : remainder;
                }
            }

            ColumnWidths = columnWidths;
        }

        protected override void AppendNamespace(
          TsNamespace @namespace,
          ScriptBuilder sb,
          TsGeneratorOutput generatorOutput,
          IReadOnlyDictionary<string, IReadOnlyCollection<TsModuleMember>> dependencies)
        {
            if (@namespace.Classes.Any(c => !this._typeConvertors.IsConvertorRegistered(c.Type) && !c.IsIgnored))
            {
                sb.AppendLine("import { useState } from 'react';");
            }

            base.AppendNamespace(@namespace, sb, generatorOutput, dependencies);
        }

        protected override IReadOnlyList<TsProperty> AppendClassDefinition(
            TsClass classModel,
            ScriptBuilder sb,
            TsGeneratorOutput generatorOutput,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            var propertiesToExport = base.AppendClassDefinition(classModel, sb, generatorOutput, importNames);

            List<TsProperty> allProperties = classModel.GetBaseProperties((generatorOutput & TsGeneratorOutput.Properties) == TsGeneratorOutput.Properties,
                (generatorOutput & TsGeneratorOutput.Fields) == TsGeneratorOutput.Fields);

            allProperties.AddRange(propertiesToExport);

            sb.AppendLine();

            var propertyList = allProperties.ToImmutableSortedDictionary(a => this.FormatPropertyName(a), a => a)
                .ToList();

            string? typeName = this.GetTypeName(classModel);
            string str = this.GetTypeVisibility(classModel, typeName) ? "export " : "";
            sb.AppendLineIndented(str + "const " + typeName + "FormBase = (props: " + typeName + ") => {");

            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("const { register, handleSubmit, formState: { errors } } = useForm<" + typeName+ ">({");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("resolver: " + typeName + "Resolver,");
                    sb.AppendLineIndented("defaultValues: props");
                }
                sb.AppendLineIndented("});");

                sb.AppendLine();

                sb.AppendLineIndented("const onSubmit: SubmitHandler<" + typeName + "> = async (data: " + typeName + ") => {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("// TODO: fill in submit action details");
                }
                sb.AppendLineIndented("};");

                sb.AppendLine();

                sb.AppendLineIndented("return <form onSubmit={handleSubmit(onSubmit)}>");
                using (sb.IncreaseIndentation())
                {
                    var rowCount = (propertyList.Count + ColCount - 1) / ColCount;
                    for (var i = 0; i < rowCount; ++i)
                    {
                        var startIndex = i * ColCount;
                        AppendRow(sb, startIndex, Math.Min(ColCount, propertyList.Count - startIndex), propertyList);
                    }
                    AppendButtonRow(sb);
                }
                sb.AppendLineIndented("</form>;");
            }

            sb.AppendLineIndented("};");

            return propertiesToExport;
        }

        protected override IReadOnlyList<string> GetReactHookFormComponentNames(IEnumerable<TsClass> classes)
        {
            var reactHookFormComponentNames = new List<string>() { "useForm", "SubmitHandler" };

            reactHookFormComponentNames.AddRange(base.GetReactHookFormComponentNames(classes));

            return reactHookFormComponentNames;
        }

        private static void AppendButtonRow(ScriptBuilder sb)
        {
            sb.AppendLineIndented("<div className=\"row\">");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("<div className=\"form-group col-md-12\">");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("<button className=\"btn btn-primary\" type=\"submit\">Submit</button>");
                    sb.AppendLineIndented("<button className=\"btn btn-secondary\" type=\"reset\">Reset</button>");
                }
                sb.AppendLineIndented("</div>");
            }
            sb.AppendLineIndented("</div>");
        }

        private void AppendRow(ScriptBuilder sb, int startIndex, int count, IReadOnlyList<KeyValuePair<string, TsProperty>> properties)
        {
            sb.AppendLineIndented("<div className=\"row\">");
            using (sb.IncreaseIndentation())
            {
                for (var i = 0; i < count; ++i)
                {
                    var propertyValuePair = properties[i + startIndex];
                    var property = propertyValuePair.Value;
                    var propertyName = propertyValuePair.Key;
                    var columnWidth = ColumnWidths[i];
                    sb.AppendLineIndented("<div className=\"form-group col-md-" + columnWidth + "\">");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("<label htmlFor=\"" + propertyName + "\">" + property.GetDisplayName() + ":</label>");
                        if (property.PropertyType is TsEnum tsEnum)
                        {
                            sb.AppendLineIndented("<select id=\"" + propertyName + "\" {...register('" + propertyName + "')}>");
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
                            sb.AppendLineIndented("<input type=\"" + GetInputType(property, tsSystemType.Kind) 
                                + "\" className=\"form-control\" id=\"" 
                                + propertyName + "\" {...register(\"" + propertyName + "\")} />");
                        }
                        else
                        {
                            sb.AppendLineIndented("<input type=\"text\" className=\"form-control\" id=\"" + propertyName
                                + "\" {...register(\"" + propertyName + "\")} />");
                        }
                        sb.AppendLineIndented("{errors." + propertyName + " && <span className=\"invalid-feedback\">{errors." 
                            + propertyName + ".message}</span>}");
                    }
                    sb.AppendLineIndented("</div>");
                }
            }
            sb.AppendLineIndented("</div>");
        }

        private static string GetInputType(TsProperty property, SystemTypeKind kind)
        {
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
    }
}
