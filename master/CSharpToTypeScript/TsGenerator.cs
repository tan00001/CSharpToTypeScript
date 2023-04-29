#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using CSharpToTypeScript.Extensions;
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public class TsGenerator
    {
        protected TsTypeFormatterCollection _typeFormatters;
        internal TypeConvertorCollection _typeConvertors;
        protected TsMemberIdentifierFormatter _memberFormatter;
        protected TsMemberTypeFormatter _memberTypeFormatter;
        protected TsTypeVisibilityFormatter _typeVisibilityFormatter;
        protected TsModuleNameFormatter _moduleNameFormatter;
        protected IDocAppender _docAppender;
        protected HashSet<TsClass> _generatedClasses;
        protected HashSet<TsEnum> _generatedEnums;
        protected List<string> _references;

        public IReadOnlyDictionary<Type, TsTypeFormatter> Formaters => new Dictionary<Type, TsTypeFormatter>((IDictionary<Type, TsTypeFormatter>)this._typeFormatters._formatters);

        public string IndentationString { get; set; }

        public bool GenerateConstEnums { get; set; }

        public TsGenerator()
        {
            this._references = new List<string>();
            this._generatedClasses = new HashSet<TsClass>();
            this._generatedEnums = new HashSet<TsEnum>();
            this._typeFormatters = new TsTypeFormatterCollection();
            this._typeFormatters.RegisterTypeFormatter<TsClass>((TsTypeFormatter)((type, formatter) =>
            {
                TsClass tsClass = (TsClass)type;
                return !tsClass.GenericArguments.Any() ? tsClass.Name : tsClass.Name + "<" 
                    + string.Join(", ", tsClass.GenericArguments.Select(a => (a is not TsCollection) ? this.GetFullyQualifiedTypeName(a) : this.GetFullyQualifiedTypeName(a) + "[]")) + ">";
            }));
            this._typeFormatters.RegisterTypeFormatter<TsSystemType>((type, formatter) => ((TsSystemType)type).Kind.ToTypeScriptString());
            this._typeFormatters.RegisterTypeFormatter<TsCollection>((type, formatter) => this.GetTypeName(((TsCollection)type).ItemsType));
            this._typeFormatters.RegisterTypeFormatter<TsEnum>((type, formatter) => ((TsModuleMember)type).Name);
            this._typeConvertors = new TypeConvertorCollection();
            this._docAppender = new NullDocAppender();
            this._memberFormatter = new TsMemberIdentifierFormatter(this.DefaultMemberFormatter);
            this._memberTypeFormatter = new TsMemberTypeFormatter(this.DefaultMemberTypeFormatter);
            this._typeVisibilityFormatter = new TsTypeVisibilityFormatter(this.DefaultTypeVisibilityFormatter);
            this._moduleNameFormatter = new TsModuleNameFormatter(this.DefaultModuleNameFormatter);
            this.IndentationString = "\t";
            this.GenerateConstEnums = true;
        }

        public bool DefaultTypeVisibilityFormatter(TsClass tsClass, string? typeName) => false;

        public string DefaultModuleNameFormatter(TsModule module) => module.Name;

        public string DefaultMemberFormatter(TsProperty identifier) => identifier.Name;

        public string DefaultMemberTypeFormatter(TsProperty tsProperty, string? memberTypeName)
        {
            if (tsProperty.PropertyType is TsCollection propertyType)
            {
                return memberTypeName + string.Concat(Enumerable.Repeat<string>("[]", propertyType.Dimension));
            }

            return memberTypeName ?? string.Empty;
        }

        public void RegisterTypeFormatter<TFor>(TsTypeFormatter formatter) where TFor : TsType => this._typeFormatters.RegisterTypeFormatter<TFor>(formatter);

        public void RegisterTypeFormatter(TsTypeFormatter formatter) => this._typeFormatters.RegisterTypeFormatter<TsClass>(formatter);

        public void RegisterTypeConvertor<TFor>(TypeConvertor convertor) => this._typeConvertors.RegisterTypeConverter<TFor>(convertor);

        public void SetIdentifierFormatter(TsMemberIdentifierFormatter formatter) => this._memberFormatter = formatter;

        public void SetMemberTypeFormatter(TsMemberTypeFormatter formatter) => this._memberTypeFormatter = formatter;

        public void SetTypeVisibilityFormatter(TsTypeVisibilityFormatter formatter) => this._typeVisibilityFormatter = formatter;

        public void SetModuleNameFormatter(TsModuleNameFormatter formatter) => this._moduleNameFormatter = formatter;

        public void SetDocAppender(IDocAppender appender) => this._docAppender = appender;

        public void AddReference(string reference) => this._references.Add(reference);

        public string Generate(TsModel model) => this.Generate(model, TsGeneratorOutput.Properties | TsGeneratorOutput.Enums);

        public string Generate(TsModel model, TsGeneratorOutput generatorOutput)
        {
            ScriptBuilder sb = new (this.IndentationString);
            if ((generatorOutput & TsGeneratorOutput.Properties) == TsGeneratorOutput.Properties || (generatorOutput & TsGeneratorOutput.Fields) == TsGeneratorOutput.Fields)
            {
                if ((generatorOutput & TsGeneratorOutput.Constants) == TsGeneratorOutput.Constants)
                    throw new InvalidOperationException("Cannot generate constants together with properties or fields");
                foreach (string reference in this._references.Concat<string>((IEnumerable<string>)model.References))
                    this.AppendReference(reference, sb);
                sb.AppendLine();
            }
            foreach (TsModule module in (IEnumerable<TsModule>)model.Modules.OrderBy<TsModule, string>((Func<TsModule, string>)(m => this.GetModuleName(m))))
                this.AppendModule(module, sb, generatorOutput);
            return sb.ToString();
        }

        protected virtual void AppendReference(string reference, ScriptBuilder sb)
        {
            sb.AppendFormat("/// <reference path=\"{0}\" />", reference);
            sb.AppendLine();
        }

        protected virtual void AppendModule(
          TsModule module,
          ScriptBuilder sb,
          TsGeneratorOutput generatorOutput)
        {
            List<TsClass> moduleClasses = module.Classes.Where(c => !this._typeConvertors.IsConvertorRegistered(c.Type) && !c.IsIgnored)
                .OrderBy(c => this.GetTypeName(c))
                .ToList();
            List<TsEnum> moduleEnums = module.Enums.Where(e => !this._typeConvertors.IsConvertorRegistered(e.Type) && !e.IsIgnored)
                .OrderBy(e => this.GetTypeName(e))
                .ToList();

            if (generatorOutput == TsGeneratorOutput.Enums && moduleEnums.Count == 0 || generatorOutput == TsGeneratorOutput.Properties && moduleClasses.Count == 0
                || moduleEnums.Count == 0 && moduleClasses.Count == 0 || generatorOutput == TsGeneratorOutput.Properties 
                    && !moduleClasses.Any(c => c.Fields.Any() || c.Properties.Any()) 
                        || generatorOutput == TsGeneratorOutput.Constants 
                            && !moduleClasses.Any(c => c.Constants.Any()))
                return;

            string moduleName = this.GetModuleName(module);
            bool flag = moduleName != string.Empty;
            if (flag)
            {
                if (generatorOutput != TsGeneratorOutput.Enums && (generatorOutput & TsGeneratorOutput.Constants) != TsGeneratorOutput.Constants)
                    sb.Append("declare ");
                sb.AppendLine(string.Format("namespace {0} {{", moduleName));
            }
            using (sb.IncreaseIndentation())
            {
                if ((generatorOutput & TsGeneratorOutput.Enums) == TsGeneratorOutput.Enums)
                {
                    foreach (TsEnum enumModel in moduleEnums)
                        this.AppendEnumDefinition(enumModel, sb, generatorOutput);
                }
                if ((generatorOutput & TsGeneratorOutput.Properties) == TsGeneratorOutput.Properties || (generatorOutput & TsGeneratorOutput.Fields) == TsGeneratorOutput.Fields)
                {
                    foreach (TsClass classModel in moduleClasses)
                        this.AppendClassDefinition(classModel, sb, generatorOutput);
                }
                if ((generatorOutput & TsGeneratorOutput.Constants) == TsGeneratorOutput.Constants)
                {
                    foreach (TsClass classModel in moduleClasses)
                    {
                        if (!classModel.IsIgnored)
                            this.AppendConstantModule(classModel, sb);
                    }
                }
            }
            if (!flag)
                return;
            sb.AppendLine("}");
        }

        protected virtual void AppendClassDefinition(
          TsClass classModel,
          ScriptBuilder sb,
          TsGeneratorOutput generatorOutput)
        {
            string? typeName = this.GetTypeName(classModel);
            string str = this.GetTypeVisibility(classModel, typeName) ? "export " : "";
            this._docAppender.AppendClassDoc(sb, classModel, typeName);
            sb.AppendFormatIndented("{0}interface {1}", str, typeName);
            if (classModel.BaseType != null)
                sb.AppendFormat(" extends {0}", this.GetFullyQualifiedTypeName(classModel.BaseType));
            if (classModel.Interfaces.Count > 0)
            {
                string?[] array = classModel.Interfaces.Select(t => this.GetFullyQualifiedTypeName(t)).ToArray();
                string format = classModel.Type.IsInterface ? " extends {0}" : (classModel.BaseType != null ? " , {0}" : " extends {0}");
                sb.AppendFormat(format, string.Join(" ,", array));
            }
            sb.AppendLine(" {");
            List<TsProperty> source = new ();
            if ((generatorOutput & TsGeneratorOutput.Properties) == TsGeneratorOutput.Properties)
                source.AddRange((IEnumerable<TsProperty>)classModel.Properties);
            if ((generatorOutput & TsGeneratorOutput.Fields) == TsGeneratorOutput.Fields)
                source.AddRange((IEnumerable<TsProperty>)classModel.Fields);
            using (sb.IncreaseIndentation())
            {
                foreach (TsProperty property in (IEnumerable<TsProperty>)source.Where<TsProperty>((Func<TsProperty, bool>)(p => p.JsonIgnore == null)).OrderBy<TsProperty, string>((Func<TsProperty, string>)(p => this.GetPropertyName(p))))
                {
                    this._docAppender.AppendPropertyDoc(sb, property, this.GetPropertyName(property), this.GetPropertyType(property));
                    sb.AppendLineIndented(string.Format("{0}: {1};", this.GetPropertyName(property), this.GetPropertyType(property)));
                }
            }
            sb.AppendLineIndented("}");
            this._generatedClasses.Add(classModel);
        }

        protected virtual void AppendEnumDefinition(
          TsEnum enumModel,
          ScriptBuilder sb,
          TsGeneratorOutput output)
        {
            string? typeName = this.GetTypeName(enumModel);
            string str1 = (output & TsGeneratorOutput.Enums) == TsGeneratorOutput.Enums || (output & TsGeneratorOutput.Constants) == TsGeneratorOutput.Constants ? "export " : "";
            this._docAppender.AppendEnumDoc(sb, enumModel, typeName);
            string str2 = this.GenerateConstEnums ? "const " : string.Empty;
            sb.AppendLineIndented(string.Format("{0}{2}enum {1} {{", str1, typeName, str2));
            using (sb.IncreaseIndentation())
            {
                int num = 1;
                foreach (TsEnumValue tsEnumValue in (IEnumerable<TsEnumValue>)enumModel.Values)
                {
                    this._docAppender.AppendEnumValueDoc(sb, tsEnumValue);
                    sb.AppendLineIndented(string.Format(num < enumModel.Values.Count ? "{0} = {1}," : "{0} = {1}", tsEnumValue.Name, tsEnumValue.Value));
                    ++num;
                }
            }
            sb.AppendLineIndented("}");
            this._generatedEnums.Add(enumModel);
        }

        protected virtual void AppendConstantModule(TsClass classModel, ScriptBuilder sb)
        {
            if (!classModel.Constants.Any<TsProperty>())
                return;
            string? typeName = this.GetTypeName(classModel);
            sb.AppendLineIndented(string.Format("export namespace {0} {{", typeName));
            using (sb.IncreaseIndentation())
            {
                foreach (TsProperty constant in (IEnumerable<TsProperty>)classModel.Constants)
                {
                    if (constant.JsonIgnore == null)
                    {
                        this._docAppender.AppendConstantModuleDoc(sb, constant, this.GetPropertyName(constant), this.GetPropertyType(constant));
                        sb.AppendFormatIndented("export const {0}: {1} = {2};", this.GetPropertyName(constant), this.GetPropertyType(constant), this.GetPropertyConstantValue(constant));
                        sb.AppendLine();
                    }
                }
            }
            sb.AppendLineIndented("}");
            this._generatedClasses.Add(classModel);
        }

        public string? GetFullyQualifiedTypeName(TsType? type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            string moduleName = string.Empty;
            if (type is TsModuleMember tsModuleMember && !this._typeConvertors.IsConvertorRegistered(type.Type))
            {
                moduleName = tsModuleMember.Module != null ? this.GetModuleName(tsModuleMember.Module) : string.Empty;
            }
            else if (type is TsCollection tsCollection)
            {
                moduleName = this.GetCollectionModuleName(tsCollection, moduleName);
            }

            return type.Type.IsGenericParameter || string.IsNullOrEmpty(moduleName) ? this.GetTypeName(type) : moduleName + "." + this.GetTypeName(type);
        }

        public string GetCollectionModuleName(TsCollection collectionType, string moduleName)
        {
            if (collectionType.ItemsType is TsModuleMember tsModuleMember
                && !this._typeConvertors.IsConvertorRegistered(collectionType.ItemsType.Type)
                && !collectionType.ItemsType.Type.IsGenericParameter)
            {
                moduleName = tsModuleMember.Module != null ? this.GetModuleName(tsModuleMember.Module) : string.Empty;
            }
            else if (collectionType.ItemsType is TsCollection tsCollection)
            {
                moduleName = this.GetCollectionModuleName(tsCollection, moduleName);
            }

            return moduleName;
        }

        public string? GetTypeName(TsType type) => this._typeConvertors.IsConvertorRegistered(type.Type) ? this._typeConvertors.ConvertType(type.Type)
            : this._typeFormatters.FormatType(type);

        public string GetPropertyName(TsProperty property)
        {
            string propertyName = this._memberFormatter(property);
            if (property.Required == null)
                propertyName += "?";
            return propertyName;
        }

        public string GetPropertyType(TsProperty property)
        {
            string? qualifiedTypeName = this.GetFullyQualifiedTypeName(property.PropertyType);
            return this._memberTypeFormatter(property, qualifiedTypeName);
        }

        public string GetPropertyConstantValue(TsProperty property)
        {
            string str = property.PropertyType.Type == typeof(string) ? "\"" : "";
            return str + property.ConstantValue?.ToString() + str;
        }

        public bool GetTypeVisibility(TsClass tsClass, string? typeName) => this._typeVisibilityFormatter(tsClass, typeName);

        public string GetModuleName(TsModule module) => this._moduleNameFormatter(module);
    }
}
