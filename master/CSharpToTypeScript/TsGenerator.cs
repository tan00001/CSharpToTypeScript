﻿using System.Collections;
using System.Collections.Generic;
using System.Formats.Asn1;

using CSharpToTypeScript.Extensions;
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public class TsGenerator
    {
        private const string TypeScriptFileType = ".ts";
        protected TsTypeFormatterCollection _typeFormatters;
        internal TypeConvertorCollection _typeConvertors;
        protected TsMemberIdentifierFormatter _memberFormatter;
        protected TsMemberTypeFormatter _memberTypeFormatter;
        protected TsTypeVisibilityFormatter _typeVisibilityFormatter;
        protected TsModuleNameFormatter _moduleNameFormatter;
        protected TsNamespaceNameFormatter _namespaceNameFormatter;
        protected IDocAppender _docAppender;
        protected HashSet<TsClass> _generatedClasses;
        protected HashSet<TsTypeDefinition> _generatedTypeDefinitions;
        protected HashSet<TsInterface> _generatedInterfaces;
        protected HashSet<TsEnum> _generatedEnums;
        protected readonly TsClassDependencyComparer TsClassComparer = new();
        protected readonly TsInterfaceDependencyComparer TsInterfaceComparer = new();

        public string IndentationString { get; set; }

        public bool GenerateConstEnums { get; set; }

        public bool EnableNamespaceInTypeScript { get; set; }

        public TsGenerator(bool enableNamespace)
        {
            EnableNamespaceInTypeScript = enableNamespace;

            this._generatedClasses = new HashSet<TsClass>();
            this._generatedTypeDefinitions = new HashSet<TsTypeDefinition>();
            this._generatedInterfaces = new HashSet<TsInterface>();
            this._generatedEnums = new HashSet<TsEnum>();
            this._typeFormatters = new TsTypeFormatterCollection();
            this._typeFormatters.RegisterTypeFormatter<TsClass>((type, formatter) =>
            {
                TsClass tsClass = (TsClass)type;

                var name = tsClass.Name;
                if (!tsClass.GenericArguments.Any())
                {
                    return name;
                }
                return RemoveGenericArgumentsFromName(tsClass.Name) + "<"
                    + string.Join(", ", tsClass.GenericArguments.Select(a => GetFullyQualifiedTypeNameAndArraySuffixIfNecessary(a))) + ">";
            });
            this._typeFormatters.RegisterTypeFormatter<TsTypeDefinition>((type, formatter) =>
            {
                TsTypeDefinition tsTypeDefinition = (TsTypeDefinition)type;

                var name = tsTypeDefinition.Name;
                if (!tsTypeDefinition.GenericArguments.Any())
                {
                    return name;
                }
                return RemoveGenericArgumentsFromName(tsTypeDefinition.Name) + "<"
                    + string.Join(", ", tsTypeDefinition.GenericArguments.Select(a => GetFullyQualifiedTypeNameAndArraySuffixIfNecessary(a))) + ">";
            });
            this._typeFormatters.RegisterTypeFormatter<TsInterface>((type, formatter) =>
            {
                TsInterface tsInterface = (TsInterface)type;

                var name = tsInterface.Name;
                if (!tsInterface.GenericArguments.Any())
                {
                    return name;
                }

                return RemoveGenericArgumentsFromName(tsInterface.Name) + "<"
                    + string.Join(", ", tsInterface.GenericArguments.Select(a => (a is not TsCollection) ? this.GetFullyQualifiedTypeName(a) : this.GetFullyQualifiedTypeName(a) + "[]")) + ">";
            });
            this._typeFormatters.RegisterTypeFormatter<TsSystemType>((type, formatter) => ((TsSystemType)type).Kind.ToTypeScriptString());
            this._typeFormatters.RegisterTypeFormatter<TsCollection>((type, formatter) => CollectionTypeFormatter((TsCollection)type));
            this._typeFormatters.RegisterTypeFormatter<TsEnum>((type, formatter) => ((TsModuleMember)type).Name);
            this._typeConvertors = new TypeConvertorCollection();
            this._docAppender = new NullDocAppender();
            this._memberFormatter = new TsMemberIdentifierFormatter(DefaultMemberFormatter);
            this._memberTypeFormatter = new TsMemberTypeFormatter(DefaultMemberTypeFormatter);
            this._typeVisibilityFormatter = new TsTypeVisibilityFormatter(DefaultTypeVisibilityFormatter);
            this._moduleNameFormatter = new TsModuleNameFormatter(DefaultModuleNameFormatter);
            this._namespaceNameFormatter = new TsNamespaceNameFormatter(DefaultNamespaceNameFormatter);
            this.IndentationString = "\t";
            this.GenerateConstEnums = true;
        }

        private string CollectionTypeFormatter(TsCollection type)
        {
            if (type.KeyType != null)
            {
                return "{ [key: " + (this.GetTypeName(type.KeyType) ?? throw new Exception("Invalid collection: key type has no name."))
                    + "]: " + (this.GetTypeName(type.ItemsType) ?? throw new Exception("Invalid collection: item type has no name.")) + " }";
            }

            return this.GetTypeName(type.ItemsType) ?? throw new Exception("Invalid collection: item type has no name.");
        }

        private string? GetFullyQualifiedTypeNameAndArraySuffixIfNecessary(TsType a)
        {
            var fullyQualifiedTypeName = GetFullyQualifiedTypeName(a);

            if (a is TsCollection collection)
            {
                for (var i = 0; i < collection.Dimension; ++i)
                {
                    fullyQualifiedTypeName += "[]";
                }
            }

            return fullyQualifiedTypeName;
        }

        public bool DefaultTypeVisibilityFormatter(TsType tsType, string? typeName) => !EnableNamespaceInTypeScript && tsType is TsModuleMember;

        public static string DefaultModuleNameFormatter(TsModule module) => module.Name;

        public static string DefaultNamespaceNameFormatter(TsNamespace @namespace) => @namespace.Name.Equals("System") || @namespace.Name.StartsWith("System.") ? string.Empty : @namespace.Name;

        public static string DefaultMemberFormatter(TsProperty identifier) => ToCamelCase(identifier.Name);

        public string DefaultMemberTypeFormatter(TsProperty tsProperty, string? memberTypeName, string currentNamespaceName,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            if (string.IsNullOrEmpty(memberTypeName))
            {
                memberTypeName = "any";
            }

            if (tsProperty.PropertyType is TsCollection collectionType)
            {
                memberTypeName += string.Concat(Enumerable.Repeat<string>("[]", collectionType.Dimension));
            }


            if (!EnableNamespaceInTypeScript)
            {
                var @namespace = memberTypeName;
                var genericTypeParamIndex = @namespace.IndexOf('<');
                if (genericTypeParamIndex > 0)
                {
                    @namespace = @namespace.Substring(0, genericTypeParamIndex);
                }
                var classNameIndex = @namespace.LastIndexOf('.');
                if (classNameIndex > 0)
                {
                    var typeNameWithoutNamespace = @namespace.Substring(classNameIndex + 1);
                    @namespace = @namespace.Substring(0, classNameIndex);

                    if (@namespace == currentNamespaceName)
                    {
                        memberTypeName = typeNameWithoutNamespace;
                    }
                    else
                    {
                        if (!importNames.TryGetValue(@namespace, out var importedNamesForNamespace))
                        {
                            throw new Exception("Cannot find imported names for namespace \"" + @namespace + "\".");
                        }

                        if (!importedNamesForNamespace.TryGetValue(typeNameWithoutNamespace, out var usageCount))
                        {
                            throw new Exception("Cannot find imported name for type \"" + typeNameWithoutNamespace + "\"");
                        }

                        memberTypeName = BuildImportName(typeNameWithoutNamespace, usageCount);
                    }
                }
            }

            if (tsProperty.IsNullable)
            {
                return memberTypeName + " | null";
            }

            return memberTypeName;
        }

        public void RegisterTypeFormatter<TFor>(TsTypeFormatter formatter) where TFor : TsType => this._typeFormatters.RegisterTypeFormatter<TFor>(formatter);

        public void RegisterTypeFormatter(TsTypeFormatter formatter) => this._typeFormatters.RegisterTypeFormatter<TsClass>(formatter);

        public void RegisterTypeConvertor<TFor>(TypeConvertor convertor) => this._typeConvertors.RegisterTypeConverter<TFor>(convertor);

        public void SetIdentifierFormatter(TsMemberIdentifierFormatter formatter) => this._memberFormatter = formatter;

        public void SetMemberTypeFormatter(TsMemberTypeFormatter formatter) => this._memberTypeFormatter = formatter;

        public void SetTypeVisibilityFormatter(TsTypeVisibilityFormatter formatter) => this._typeVisibilityFormatter = formatter;

        public void SetModuleNameFormatter(TsModuleNameFormatter formatter) => this._moduleNameFormatter = formatter;

        public void SetDocAppender(IDocAppender appender) => this._docAppender = appender;

        public IReadOnlyDictionary<string, TsGeneratorOutput> Generate(TsModelBuilder tsModelBuilder) => this.Generate(tsModelBuilder, TsGeneratorOptions.Properties | TsGeneratorOptions.Enums);

        public virtual IReadOnlyDictionary<string, TsGeneratorOutput> Generate(TsModelBuilder tsModelBuilder, TsGeneratorOptions generatorOptions)
        {
            tsModelBuilder.Build();

            var results = new Dictionary<string, TsGeneratorOutput>();

            if (generatorOptions.HasFlag(TsGeneratorOptions.Properties) || generatorOptions.HasFlag(TsGeneratorOptions.Fields))
            {
                if (generatorOptions.HasFlag(TsGeneratorOptions.Constants))
                    throw new InvalidOperationException("Cannot generate constants together with properties or fields");
            }

            foreach (TsNamespace @namespace in tsModelBuilder.TsModuleService.GetNamespaces()
                .Where(n => n.HasExportableMembers(generatorOptions))
                .OrderBy<TsNamespace, string>(n => this.FormatNamespaceName(n)))
            {
                ScriptBuilder sb = new(this.IndentationString);
                @namespace.Dependencies = tsModelBuilder.TsModuleService.GetDependentNamespaces(@namespace, generatorOptions);
                var fileType = this.AppendNamespace(@namespace, sb, generatorOptions);
                results.Add(this.FormatNamespaceName(@namespace), new TsGeneratorOutput(fileType, sb.ToString()));
            }

            AppendAdditionalDependencies(results);

            return results;
        }

        protected virtual void AppendAdditionalDependencies(Dictionary<string, TsGeneratorOutput> results)
        {
        }

        protected virtual string AppendNamespace(
          TsNamespace @namespace,
          ScriptBuilder sb,
          TsGeneratorOptions generatorOptions)
        {
            List<TsClass> classes = @namespace.Classes.Where(c => !IsIgnored(c))
                .OrderBy(c => this.GetTypeName(c))
                .ToList();
            List<TsTypeDefinition> typeDefinitions = @namespace.TypeDefinitions.Where(d => !IsIgnored(d))
                .OrderBy(GetTypeName)
                .ToList();
            List<TsInterface> interfaces = @namespace.Interfaces.Where(i => !IsIgnored(i))
                .OrderBy(GetTypeName)
                .ToList();
            List<TsEnum> enums = @namespace.Enums.Where(e => !IsIgnored(e))
                .OrderBy(GetTypeName)
                .ToList();

            if (generatorOptions == TsGeneratorOptions.Enums && enums.Count == 0
                || enums.Count == 0 && classes.Count == 0 && typeDefinitions.Count == 0 && interfaces.Count == 0
                || generatorOptions == TsGeneratorOptions.Properties 
                    && !classes.Any(c => c.Fields.Any(f => !f.HasIgnoreAttribute) || c.Properties.Any(p => !p.HasIgnoreAttribute))
                    && !typeDefinitions.Any(c => c.Fields.Any(f => !f.HasIgnoreAttribute) || c.Properties.Any(p => !p.HasIgnoreAttribute))
                    && !interfaces.Any(i => i.Properties.Any(p => !p.HasIgnoreAttribute))
                || generatorOptions == TsGeneratorOptions.Constants 
                    && !typeDefinitions.Any(c => c.Constants.Any(c => !c.HasIgnoreAttribute))
                    && !classes.Any(c => c.Constants.Any(c => !c.HasIgnoreAttribute)))
                return TypeScriptFileType;

            string namespaceName = this.FormatNamespaceName(@namespace);
            string fileType;
            if (EnableNamespaceInTypeScript)
            {
                sb.AppendLine(string.Format("namespace {0} {{", namespaceName));
                using (sb.IncreaseIndentation())
                {
                    AppendImports(@namespace, sb, generatorOptions);
                    fileType = AppendNamespace(sb, generatorOptions, classes, typeDefinitions,
                        interfaces, enums, new Dictionary<string, IReadOnlyDictionary<string, Int32>>());
                }
                sb.AppendLine("}");
            }
            else
            {
                var importedNames = AppendImports(@namespace, sb, generatorOptions);
                fileType = AppendNamespace(sb, generatorOptions, classes, typeDefinitions,
                    interfaces, enums, importedNames);
            }

            return fileType;
        }

        /// <summary>
        /// Append import statements
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="sb"></param>
        /// <param name="generatorOptions"></param>
        /// <returns>Import indices for each dependent namespace</returns>
        protected virtual IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> AppendImports(
            TsNamespace @namespace,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions)
        {
            Dictionary<string, IReadOnlyDictionary<string, Int32>> importIndices = new();

            if (!EnableNamespaceInTypeScript)
            {
                if (@namespace.Dependencies != null)
                {
                    foreach (var dependency in @namespace.Dependencies)
                    {
                        if (TsType.ExcludedNamespacePrefixes.Any(p => dependency.Key.StartsWith(p) == true || p.StartsWith(dependency.Key)))
                        {
                            continue;
                        }

                        var importIndicesForNamespace = new Dictionary<string, Int32>();
                        sb.AppendLine("import { " + string.Join(", ", dependency.Value.Select(v => GetImportName(v.Name, importIndices, importIndicesForNamespace))) + " } from './" + dependency.Key + "';");
                        importIndices[dependency.Key] = importIndicesForNamespace;
                    }
                }
            }

            AppendAdditionalImports(@namespace, sb, generatorOptions, importIndices);

            if (importIndices.Count > 0)
            {
                sb.AppendLine();
            }

            return importIndices;
        }

        protected virtual void AppendAdditionalImports(
            TsNamespace @namespace,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            Dictionary<string, IReadOnlyDictionary<string, Int32>> importIndices)
        {
        }

        protected virtual bool HasAdditionalImports(TsNamespace @namespace, TsGeneratorOptions generatorOptions)
        {
            return false;
        }

        protected static string GetImportName(string name, Dictionary<string, IReadOnlyDictionary<string, int>> importIndices, Dictionary<string, Int32> currentIndices)
        {
            Int32 usageCount = 0;
            
            foreach (var importIndicesForNamespace in importIndices.Values)
            {
                usageCount += importIndicesForNamespace.Keys.Count(k => k == name);
            }

            currentIndices[name] = usageCount;

            return name + " as " + BuildImportName(name, usageCount);
        }

        protected virtual string AppendNamespace(ScriptBuilder sb, TsGeneratorOptions generatorOptions,
            IReadOnlyList<TsClass> classes, 
            IReadOnlyList<TsTypeDefinition> typeDefinitions,
            IReadOnlyList<TsInterface> interfaces,
            IReadOnlyList<TsEnum> enums, 
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            int generatedSectionCount = 0;

            generatedSectionCount = AppendNamceSpaceConstants(sb, generatorOptions, classes, typeDefinitions, importNames,
                generatedSectionCount);

            generatedSectionCount = AppendNameSpaceEnums(sb, generatorOptions, enums, generatedSectionCount);

            AppendNamespaceMembersWithHierarchies(sb, generatorOptions, classes, typeDefinitions, interfaces,
                importNames, generatedSectionCount);

            return TypeScriptFileType;
        }

        protected virtual void AppendNamespaceMembersWithHierarchies(ScriptBuilder sb, TsGeneratorOptions generatorOptions,
            IReadOnlyList<TsClass> classes, IReadOnlyList<TsTypeDefinition> typeDefinitions,
            IReadOnlyList<TsInterface> interfaces, IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames,
            int generatedSectionCount)
        {
            if (generatorOptions.HasFlag(TsGeneratorOptions.Properties) || generatorOptions.HasFlag(TsGeneratorOptions.Fields))
            {
                if (interfaces.Count > 0)
                {
                    if (generatedSectionCount > 0)
                    {
                        sb.AppendLine();
                    }

                    interfaces = interfaces.Where(i =>
                        {
                            var nameSpaceName = i.Type.Namespace ?? i.Type.FullName;
                            if (nameSpaceName == null)
                            {
                                return true;
                            }
                            return !TsType.ExcludedNamespacePrefixes.Any(p => nameSpaceName.StartsWith(p) == true || p.StartsWith(nameSpaceName));
                        })
                        .OrderBy(i => i, TsInterfaceComparer)
                        .ThenBy(GetTypeName).ToList();

                    for (var i = 0; i < interfaces.Count; ++i)
                    {
                        TsInterface interfaceModel = interfaces[i];

                        // Only export interface definition when the interface is not generic or all generic arguments are generic parameters
                        if (interfaceModel.GenericArguments.Count == 0 || interfaceModel.GenericArguments.All(a => a.Type.IsGenericParameter))
                        {
                            this.AppendInterfaceDefinition(interfaceModel, sb, generatorOptions, importNames);
                            if (i < interfaces.Count - 1)
                            {
                                sb.AppendLine();
                            }
                            ++generatedSectionCount;
                        }
                    }
                }

                if (typeDefinitions.Count > 0)
                {
                    if (generatedSectionCount > 0)
                    {
                        sb.AppendLine();
                    }

                    typeDefinitions = typeDefinitions.OrderBy(GetTypeName).ToList();

                    for (var i = 0; i < typeDefinitions.Count; ++i)
                    {
                        TsTypeDefinition typeDefinitionModel = typeDefinitions[i];
                        (var properties, var hasOutput) = this.AppendTypeDefinition(typeDefinitionModel, sb, generatorOptions, importNames);
                        if (hasOutput && i < typeDefinitions.Count - 1)
                        {
                            sb.AppendLine();
                        }
                    }
                }

                if (classes.Count > 0)
                {
                    if (generatedSectionCount > 0)
                    {
                        sb.AppendLine();
                    }

                    classes = classes.Where(c => !IsIgnored(c)).OrderBy(c => c, TsClassComparer)
                        .ThenBy(GetTypeName).ToList();

                    for (var i = 0; i < classes.Count; ++i)
                    {
                        TsClass classModel = classes[i];
                        (var propertyList, var hasOutput) = this.AppendClassDefinition(classModel, sb, generatorOptions, importNames);
                        if (hasOutput && i < classes.Count - 1)
                        {
                            sb.AppendLine();
                        }
                    }
                }
            }
        }

        protected virtual int AppendNameSpaceEnums(ScriptBuilder sb, TsGeneratorOptions generatorOptions,
            IReadOnlyList<TsEnum> enums, int generatedSectionCount)
        {
            if (generatorOptions.HasFlag(TsGeneratorOptions.Enums))
            {
                if (enums.Count > 0)
                {
                    if (generatedSectionCount > 0)
                    {
                        sb.AppendLine();
                    }

                    for (var i = 0; i < enums.Count; ++i)
                    {
                        TsEnum enumModel = enums[i];
                        this.AppendEnumDefinition(enumModel, sb, generatorOptions);
                        if (i < enums.Count - 1)
                        {
                            sb.AppendLine();
                        }
                        ++generatedSectionCount;
                    }
                }
            }

            return generatedSectionCount;
        }

        protected int AppendNamceSpaceConstants(ScriptBuilder sb, TsGeneratorOptions generatorOptions, 
            IReadOnlyList<TsClass> classes, IReadOnlyList<TsTypeDefinition> typeDefinitions, 
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames, int generatedSectionCount)
        {
            if (generatorOptions.HasFlag(TsGeneratorOptions.Constants))
            {
                var constants = classes.SelectMany(c => c.Constants.Where(ct => !ct.HasIgnoreAttribute)).ToList();
                if (constants.Count > 0)
                {
                    string namespaceName = FormatNamespaceName(classes[0].Namespace!);

                    for (var i = 0; i < constants.Count; ++i)
                    {
                        var ct = constants[i];
                        this.AppendConstantDefinition(namespaceName, ct, sb, importNames);
                    }
                    ++generatedSectionCount;
                }

                constants = typeDefinitions.SelectMany(c => c.Constants.Where(ct => !ct.HasIgnoreAttribute)).ToList();
                if (constants.Count > 0)
                {
                    string namespaceName = FormatNamespaceName(typeDefinitions[0].Namespace!);

                    for (var i = 0; i < constants.Count; ++i)
                    {
                        var ct = constants[i];
                        this.AppendConstantDefinition(namespaceName, ct, sb, importNames);
                    }
                    ++generatedSectionCount;
                }
            }

            return generatedSectionCount;
        }

        protected virtual (IReadOnlyList<TsProperty> Properties, bool HasOutput) AppendClassDefinition(
            TsClass classModel,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            List<TsProperty> propertiesToExport = new();

            if (generatorOptions.HasFlag(TsGeneratorOptions.Properties))
                propertiesToExport.AddRange(classModel.GetMemeberInfoForOutput(TsGeneratorOptions.Properties));
            if (generatorOptions.HasFlag(TsGeneratorOptions.Fields))
                propertiesToExport.AddRange(classModel.GetMemeberInfoForOutput(TsGeneratorOptions.Fields));

            // Only export class definition when the class is not generic or all generic arguments are generic parameters
            if (classModel.GenericArguments.Count > 0 && !classModel.GenericArguments.All(a => a.Type.IsGenericParameter))
            {
                return (propertiesToExport, false);
            }

            string? typeName = this.GetTypeName(classModel);
            string str = this.GetTypeVisibility(classModel, typeName) ? "export " : "";
            this._docAppender.AppendClassDoc(sb, classModel, typeName);
            sb.AppendFormatIndented("{0}class {1}", str, typeName);
            if (classModel.BaseType != null)
            {
                sb.AppendFormat(" extends {0}", FormatTypeName(classModel.NamespaceName, classModel.BaseType, importNames));
            }

            var classInterfaces = classModel.Interfaces.Where(i => !i.IsExcludedFromExport()).ToList();
            if (classInterfaces.Count > 0)
            {
                string?[] array = classInterfaces.Select(t => FormatTypeName(classModel.NamespaceName, t, importNames)).ToArray();
                sb.AppendFormat(" implements {0}", string.Join(", ", array));
            }

            sb.AppendLine(" {");

            string namespaceName = FormatNamespaceName(classModel.Namespace!);

            using (sb.IncreaseIndentation())
            {
                List<TsProperty> properties = AppendProperties(sb, classModel, importNames, propertiesToExport, namespaceName, generatorOptions);

                var requiredProperties = properties.Where(p => p.IsRequired).ToList();
                if (requiredProperties.Count > 0)
                {
                    sb.AppendLine();
                    var constructorParameters = string.Join(", ", requiredProperties.Select(p => string.Format("{0}: {1}",
                        this.FormatPropertyName(p) + (string.IsNullOrEmpty(p.GetDefaultValue()) ? "" : "?"),
                        this.FormatPropertyType(namespaceName, p, importNames))
                        ));
                    var baseRequiredProperties = classModel.GetBaseRequiredProperties(generatorOptions.HasFlag(TsGeneratorOptions.Properties),
                        generatorOptions.HasFlag(TsGeneratorOptions.Fields));
                    if (baseRequiredProperties.Count > 0)
                    {
                        constructorParameters += ", " + string.Join(", ", baseRequiredProperties.Select(p => string.Format("{0}: {1}",
                            this.FormatPropertyName(p) + (string.IsNullOrEmpty(p.GetDefaultValue()) ? "" : "?"),
                            this.FormatPropertyType(namespaceName, p, importNames))
                            ));
                    }
                    sb.AppendLineIndented("constructor(" + constructorParameters + ") {");
                    using (sb.IncreaseIndentation())
                    {
                        if (baseRequiredProperties.Count > 0)
                        {
                            sb.AppendLineIndented("super(" + string.Join(", ", baseRequiredProperties.Select(p => this.FormatPropertyName(p))) + ");");
                        }

                        foreach (var requiredProperty in requiredProperties)
                        {
                            var propertyName = this.FormatPropertyNameWithOptionalModifier(requiredProperty);
                            var defaultValue = requiredProperty.GetDefaultValue();
                            sb.AppendLineIndented("this." + propertyName + " = " + propertyName + (string.IsNullOrEmpty(defaultValue) ? string.Empty : (" ?? " + defaultValue)) + ";");
                        }
                    }
                    sb.AppendLineIndented("}");
                }

                OnPropertiesAppended(sb, classModel, importNames, properties, namespaceName, generatorOptions);
            }
            sb.AppendLineIndented("}");
            this._generatedClasses.Add(classModel);

            return (propertiesToExport, true);
        }

        protected virtual List<TsProperty> AppendProperties(ScriptBuilder sb, TsModuleMemberWithHierarchy tsModuleMemberWithHierarchy,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames,
            IReadOnlyList<TsProperty> propertiesToExport, string namespaceName, TsGeneratorOptions generatorOptions)
        {
            var properties = propertiesToExport.Where(p => !p.HasIgnoreAttribute).OrderBy(p => this.FormatPropertyName(p)).ToList();

            foreach (TsProperty property in properties)
            {
                this._docAppender.AppendPropertyDoc(sb, property, this.FormatPropertyNameWithOptionalModifier(property),
                    this.FormatPropertyType(namespaceName, property, importNames));
                sb.AppendLineIndented(string.Format("{0}: {1};", this.FormatPropertyNameWithOptionalModifier(property),
                    this.FormatPropertyType(namespaceName, property, importNames)));
            }

            return properties;
        }

        protected virtual (IReadOnlyList<TsProperty> Properties, bool HasOutput) AppendTypeDefinition(
            TsTypeDefinition typeDefinitionModel,
            ScriptBuilder sb,
            TsGeneratorOptions generatorOptions,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            IReadOnlyList<TsProperty> propertiesToExport = typeDefinitionModel.GetMemeberInfoForOutput(generatorOptions);

            // Only export type definition when the type is not generic or all generic arguments are generic parameters
            if (typeDefinitionModel.GenericArguments.Count > 0 && !typeDefinitionModel.GenericArguments.All(a => a.Type.IsGenericParameter))
            {
                return (propertiesToExport, false);
            }

            string? typeName = this.GetTypeName(typeDefinitionModel);
            string str = this.GetTypeVisibility(typeDefinitionModel, typeName) ? "export " : "";
            this._docAppender.AppendTypeDefinitionDoc(sb, typeDefinitionModel, typeName);
            sb.AppendLineIndented(str + "type " + typeName + " = {");

            string namespaceName = FormatNamespaceName(typeDefinitionModel.Namespace!);

            using (sb.IncreaseIndentation())
            {
                AppendProperties(sb, typeDefinitionModel, importNames, propertiesToExport, namespaceName, generatorOptions);
            }
            sb.AppendLineIndented("};");
            this._generatedTypeDefinitions.Add(typeDefinitionModel);

            return (propertiesToExport, true);
        }

        protected string FormatTypeName(string namespaceName, TsType type, IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            var typeName = GetFullyQualifiedTypeName(type) ?? throw new ArgumentException("Type name cannot be blank.", nameof(type));

            if (!EnableNamespaceInTypeScript)
            {
                var index = typeName.LastIndexOf('.');
                if (index > 0)
                {
                    var @namespace = typeName.Substring(0, index);
                    var typeNameWithoutNamespace = typeName.Substring(index + 1);

                    if (@namespace == namespaceName)
                    {
                        return typeNameWithoutNamespace;
                    }

                    if (!importNames.TryGetValue(@namespace, out var importedNamesForNamespace))
                    {
                        throw new Exception("Cannot find imported names for namespace \"" + @namespace + "\".");
                    }

                    if (!importedNamesForNamespace.TryGetValue(typeNameWithoutNamespace, out var usageCount))
                    {
                        throw new Exception("Cannot find imported name for type \"" + typeNameWithoutNamespace + "\"");
                    }

                    return BuildImportName(typeNameWithoutNamespace, usageCount);
                }
            }

            if (typeName.StartsWith(namespaceName + '.'))
            {
                return typeName.Substring(namespaceName.Length + 1);
            }

            return typeName;
        }

        protected virtual void AppendInterfaceDefinition(
          TsInterface interfaceModel,
          ScriptBuilder sb,
          TsGeneratorOptions generatorOptions,
          IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            string? typeName = this.GetTypeName(interfaceModel);
            string str = this.GetTypeVisibility(interfaceModel, typeName) ? "export " : "";
            this._docAppender.AppendInterfaceDoc(sb, interfaceModel, typeName);
            sb.AppendFormatIndented("{0}interface {1}", str, typeName);
            if (interfaceModel.Interfaces.Count > 0)
            {
                string?[] array = interfaceModel.Interfaces.Select(t => FormatTypeName(interfaceModel.NamespaceName, t, importNames)).ToArray();
                sb.AppendFormat(" extends {0}", string.Join(" ,", array));
            }
            sb.AppendLine(" {");
            if (generatorOptions.HasFlag(TsGeneratorOptions.Properties))
            {
                string namespaceName = FormatNamespaceName(interfaceModel.Namespace!);
                using (sb.IncreaseIndentation())
                {
                    foreach (TsProperty property in interfaceModel.Properties.Where(p => !p.HasIgnoreAttribute)
                        .OrderBy(p => this.FormatPropertyNameWithOptionalModifier(p)))
                    {
                        this._docAppender.AppendPropertyDoc(sb, property, this.FormatPropertyNameWithOptionalModifier(property),
                            FormatPropertyType(namespaceName, property, importNames));
                        sb.AppendLineIndented(string.Format("{0}: {1};", this.FormatPropertyNameWithOptionalModifier(property),
                            FormatPropertyType(namespaceName, property, importNames)));
                    }
                }
            }
            sb.AppendLineIndented("}");
            this._generatedInterfaces.Add(interfaceModel);
        }

        protected virtual void AppendEnumDefinition(
          TsEnum enumModel,
          ScriptBuilder sb,
          TsGeneratorOptions output)
        {
            string? typeName = this.GetTypeName(enumModel);
            string str1 = (output.HasFlag(TsGeneratorOptions.Enums) || output.HasFlag(TsGeneratorOptions.Constants)) ? "export " : "";
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

        protected virtual void AppendConstantDefinition(string namespaceName, TsProperty constant, ScriptBuilder sb,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        { 
            this._docAppender.AppendConstantModuleDoc(sb, constant, this.FormatPropertyName(constant),
                FormatPropertyType(namespaceName, constant, importNames));
            sb.AppendFormatIndented("export const {0}: {1} = {2};", this.FormatPropertyName(constant),
                FormatPropertyType(namespaceName, constant, importNames), GetPropertyConstantValue(constant));
            sb.AppendLine();
        }

        protected static string BuildImportName(string typeName, int usageCount)
        {
            if (typeName.Length < 1)
            {
                throw new Exception("Type name cannot be blank.");
            }

            if (usageCount == 0)
            {
                return "imp" + char.ToUpper(typeName[0]) + typeName.Substring(1);
            }

            return "imp" + char.ToUpper(typeName[0]) + typeName.Substring(1) + usageCount;
        }

        protected string FormatNamespaceName(TsNamespace @namespace) => this._namespaceNameFormatter(@namespace);

        protected string FormatPropertyNameWithOptionalModifier(TsProperty property)
        {
            string propertyName = this._memberFormatter(property);
            if (!property.IsRequired)
                propertyName += "?";
            return propertyName;
        }

        protected string FormatPropertyName(TsProperty property)
        {
            return this._memberFormatter(property);
        }

        protected string FormatPropertyType(string currentNamespaceName, TsProperty property,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames)
        {
            string? qualifiedTypeName = this.GetFullyQualifiedTypeName(property.PropertyType);

            var typeName = this._memberTypeFormatter(property, qualifiedTypeName, currentNamespaceName, importNames);

            if (string.IsNullOrEmpty(typeName))
            {
                return "any";
            }

            if (typeName.StartsWith(currentNamespaceName + '.'))
            {
                return typeName.Substring(currentNamespaceName.Length + 1);
            }

            return typeName;
        }

        protected string GetCollectionModuleName(TsCollection collectionType)
        {
            if (collectionType.ItemsType is TsModuleMember tsModuleMember
                && !this._typeConvertors.IsConvertorRegistered(collectionType.ItemsType.Type)
                && !collectionType.ItemsType.Type.IsGenericParameter)
            {
                return tsModuleMember.Namespace != null ? this.FormatNamespaceName(tsModuleMember.Namespace) : string.Empty;
            }
            else if (collectionType.ItemsType is TsCollection tsCollection)
            {
                return this.GetCollectionModuleName(tsCollection);
            }

            return string.Empty;
        }

        protected string? GetFullyQualifiedTypeName(TsType? type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.Type.IsGenericParameter)
            {
                return type.Type.Name;
            }

            string namespaceName = string.Empty;
            if (type is TsModuleMember tsModuleMember && !this._typeConvertors.IsConvertorRegistered(type.Type))
            {
                namespaceName = (tsModuleMember.Namespace != null) ? this.FormatNamespaceName(tsModuleMember.Namespace) 
                    : string.Empty;
            }
            else if (type is TsCollection tsCollection)
            {
                namespaceName = this.GetCollectionModuleName(tsCollection);
            }

            return type.Type.IsGenericParameter || string.IsNullOrEmpty(namespaceName) ? this.GetTypeName(type) : namespaceName + "." + this.GetTypeName(type);
        }

        protected string? GetTypeNamespaceName(TsType? type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.Type.IsGenericParameter)
            {
                return string.Empty;
            }

            if (type is TsModuleMember tsModuleMember && !this._typeConvertors.IsConvertorRegistered(type.Type))
            {
                return (tsModuleMember.Namespace != null) ? this.FormatNamespaceName(tsModuleMember.Namespace)
                    : string.Empty;
            }
            else if (type is TsCollection tsCollection)
            {
                return this.GetCollectionModuleName(tsCollection);
            }

            var namespaceSeparator = type.Type.FullName?.LastIndexOf('.');
            if (namespaceSeparator.HasValue && namespaceSeparator.Value > 0)
            {
                return type.Type.FullName?.Substring(0, namespaceSeparator.Value);
            }

            return string.Empty;
        }

        protected static string GetPropertyConstantValue(TsProperty property)
        {
            string str = property.PropertyType.Type == typeof(string) ? "\"" : "";
            return str + property.ConstantValue?.ToString() + str;
        }

        protected string? GetTypeName(TsType type) => this._typeConvertors.IsConvertorRegistered(type.Type) ? this._typeConvertors.ConvertType(type.Type)
            : this._typeFormatters.FormatType(type);

        protected bool GetTypeVisibility(TsType tsType, string? typeName) => this._typeVisibilityFormatter(tsType, typeName);

        protected bool IsIgnored(TsModuleMember tsModuleMember)
        {
            return tsModuleMember.IsIgnored || this._typeConvertors.IsConvertorRegistered(tsModuleMember.Type);
        }

        protected virtual void OnPropertiesAppended(ScriptBuilder sb, TsModuleMemberWithHierarchy tsModuleMemberWithHierarchy,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> importNames,
            List<TsProperty> propertiesAppended, string namespaceName, TsGeneratorOptions generatorOptions)
        {
        }

        public static string ToCamelCase(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            var charList = s.ToList();
            var lowerCaseCharIndex = charList.FindIndex(c => char.IsLower(c));

            if (lowerCaseCharIndex == 0)
            {
                return s;
            }

            if (lowerCaseCharIndex < 0)
            {
                return s.ToLower();
            }

            var lowerCaseRange = lowerCaseCharIndex > 1 ? lowerCaseCharIndex - 1 : 1;

            for (var i = 0; i < lowerCaseRange; ++i)
            {
                charList[i] = char.ToLower(charList[i]);
            }

            return new string(charList.ToArray());
        }

        private static string RemoveGenericArgumentsFromName(string name)
        {
            var genericTypeParamIndex = name.IndexOf('<');
            if (genericTypeParamIndex > 0)
            {
                return name.Substring(0, genericTypeParamIndex);
            }

            return name;
        }
    }
}
