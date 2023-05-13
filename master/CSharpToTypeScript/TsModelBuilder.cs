using System.Reflection;
using CSharpToTypeScript.Models;
using CSharpToTypeScript.Extensions;
using System.Runtime.Serialization;

namespace CSharpToTypeScript
{
    public class TsModelBuilder
    {
        public readonly ITsModuleService TsModuleService = new TsModuleService();

        internal Dictionary<Type, TsClass> Classes { get; set; }

        internal Dictionary<Type, TsTypeDefinition> TypeDefinitions { get; set; }

        internal Dictionary<Type, TsInterface> Interfaces { get; set; }

        internal Dictionary<Type, TsEnum> Enums { get; set; }

        public TsModelBuilder(ITsModuleService? tsModuleService = null)
        {
            TsModuleService = tsModuleService ?? new TsModuleService();

            Classes = new Dictionary<Type, TsClass>();
            Interfaces = new Dictionary<Type, TsInterface>();
            Enums = new Dictionary<Type, TsEnum>();
            TypeDefinitions = new Dictionary<Type, TsTypeDefinition>();
        }

        public TsModuleMember Add<T>() => this.Add<T>(true);

        public TsModuleMember Add<T>(bool includeReferences) => this.Add(typeof(T), includeReferences);

        public TsModuleMember Add(Type clrType) => this.Add(clrType, true);

        public TsModuleMember Add(
          Type clrType,
          bool includeReferences,
          Dictionary<Type, TypeConvertor>? typeConvertors = null)
        {
            TsTypeFamily typeFamily = TsType.GetTypeFamily(clrType);
            switch (typeFamily)
            {
                case TsTypeFamily.Enum:
                    if (clrType.IsNullableValueType())
                        return this.Add(clrType.GetNullableValueType(), includeReferences, typeConvertors);

                    if (this.Enums.TryGetValue(clrType, out TsEnum? enumType))
                    {
                        return enumType;
                    }
                    enumType = TsModuleService.GetOrAddTsEnum(clrType);
                    this.Enums[clrType] = enumType;
                    return enumType;

                case TsTypeFamily.Class:
                    System.Diagnostics.Debug.Assert(!clrType.IsNullableValueType());

                    if (Classes.TryGetValue(clrType, out TsClass? classType))
                    {
                        return classType;
                    }

                    if (clrType.IsGenericType)
                    {
                        ProcessGenericDefinitions(clrType, includeReferences, typeConvertors);
                    }

                    classType = TsModuleService.GetOrAddTsClass(clrType);
                    this.Classes[clrType] = classType;
                    if (clrType.IsGenericParameter)
                        classType.IsIgnored = true;
                    if (clrType.IsGenericType)
                        classType.IsIgnored = true;
                    if (classType.BaseType != null)
                        this.Add(classType.BaseType.Type);
                    if (includeReferences)
                    {
                        this.AddReferences(classType, typeConvertors);
                        foreach (TsProperty tsProperty in classType.Properties.Where(p => p.PropertyType.Type.IsEnum))
                            this.AddEnumIfNotAdded((TsEnum)tsProperty.PropertyType);
                    }
                    foreach (TsInterface tsInterface in classType.Interfaces)
                        this.Add(tsInterface.Type);
                    return classType;

                case TsTypeFamily.TypeDefinition:
                    if (clrType.IsNullableValueType())
                        return this.Add(clrType.GetNullableValueType(), includeReferences, typeConvertors);

                    if (TypeDefinitions.TryGetValue(clrType, out TsTypeDefinition? typeDefinitionType))
                    {
                        return typeDefinitionType;
                    }

                    if (clrType.IsGenericType)
                    {
                        ProcessGenericDefinitions(clrType, includeReferences, typeConvertors);
                    }

                    typeDefinitionType = TsModuleService.GetOrAddTsTypeDefinition(clrType);
                    this.TypeDefinitions[clrType] = typeDefinitionType;

                    if (clrType.IsGenericParameter)
                        typeDefinitionType.IsIgnored = true;

                    if (clrType.IsGenericType)
                        typeDefinitionType.IsIgnored = true;

                    if (includeReferences)
                    {
                        this.AddReferences(typeDefinitionType, typeConvertors);
                        foreach (TsProperty tsProperty in typeDefinitionType.Properties.Where(p => p.PropertyType.Type.IsEnum))
                            this.AddEnumIfNotAdded((TsEnum)tsProperty.PropertyType);
                    }

                    return typeDefinitionType;

                case TsTypeFamily.Interface:
                    System.Diagnostics.Debug.Assert(!clrType.IsNullableValueType());

                    if (Interfaces.TryGetValue(clrType, out TsInterface? interfaceType))
                    {
                        return interfaceType;
                    }

                    if (clrType.IsGenericType)
                    {
                        ProcessGenericDefinitions(clrType, includeReferences, typeConvertors);
                    }

                    interfaceType = TsModuleService.GetOrAddTsInterface(clrType);
                    this.Interfaces[clrType] = interfaceType;

                    // Generic parameter would never be added to a model directly.
                    System.Diagnostics.Debug.Assert (!clrType.IsGenericParameter);

                    if (clrType.IsGenericType)
                        interfaceType.IsIgnored = true;

                    if (includeReferences)
                    {
                        this.AddReferences(interfaceType, typeConvertors);
                        foreach (TsProperty tsProperty in interfaceType.Properties.Where(p => p.PropertyType.Type.IsEnum))
                            this.AddEnumIfNotAdded((TsEnum)tsProperty.PropertyType);
                    }

                    foreach (TsType tsType in interfaceType.Interfaces)
                        this.Add(tsType.Type);

                    return interfaceType;

                default:
                    throw new ArgumentException(string.Format("Type '{0}' isn't class or struct. Only classes and structures can be added to the model", clrType.FullName));
            }
        }

        private void ProcessGenericDefinitions(Type clrType, bool includeReferences, Dictionary<Type, TypeConvertor>? typeConvertors)
        {
            Type genericTypeDefinition = clrType.GetGenericTypeDefinition();

            TsTypeFamily typeFamily = TsType.GetTypeFamily(genericTypeDefinition);
            switch (typeFamily)
            {
                case TsTypeFamily.Enum:
                    if (!this.Enums.ContainsKey(genericTypeDefinition))
                    {
                        TsEnum enumModel = TsModuleService.GetOrAddTsEnum(genericTypeDefinition);
                        this.Enums[genericTypeDefinition] = enumModel;
                    }
                    break;

                case TsTypeFamily.Class:
                    if (!this.Classes.ContainsKey(genericTypeDefinition))
                    {
                        TsClass classModel = TsModuleService.GetOrAddTsClass(genericTypeDefinition);
                        classModel.ImplementedGenericTypes[clrType] = TsModuleService.BuildGenericArgumentList(clrType);
                        this.Classes[genericTypeDefinition] = classModel;
                        if (includeReferences)
                        {
                            this.AddReferences(classModel, typeConvertors);
                            foreach (TsProperty tsProperty in classModel.Properties.Where(p => p.PropertyType.Type.IsEnum))
                                this.AddEnumIfNotAdded((TsEnum)tsProperty.PropertyType);
                        }
                    }
                    break;

                case TsTypeFamily.TypeDefinition:
                    if (!this.TypeDefinitions.ContainsKey(genericTypeDefinition))
                    {
                        TsTypeDefinition typeDefinitionModel = TsModuleService.GetOrAddTsTypeDefinition(genericTypeDefinition);
                        typeDefinitionModel.ImplementedGenericTypes[clrType] = TsModuleService.BuildGenericArgumentList(clrType);
                        this.TypeDefinitions[genericTypeDefinition] = typeDefinitionModel;
                        if (includeReferences)
                        {
                            this.AddReferences(typeDefinitionModel, typeConvertors);
                            foreach (TsProperty tsProperty in typeDefinitionModel.Properties.Where(p => p.PropertyType.Type.IsEnum))
                                this.AddEnumIfNotAdded((TsEnum)tsProperty.PropertyType);
                        }
                    }
                    break;

                case TsTypeFamily.Interface:
                    if (!this.Interfaces.ContainsKey(genericTypeDefinition))
                    {
                        TsInterface interfaceModel = TsModuleService.GetOrAddTsInterface(genericTypeDefinition);
                        this.Interfaces[genericTypeDefinition] = interfaceModel;
                        if (includeReferences)
                        {
                            this.AddReferences(interfaceModel, typeConvertors);
                            foreach (TsProperty tsProperty in interfaceModel.Properties.Where(p => p.PropertyType.Type.IsEnum))
                                this.AddEnumIfNotAdded((TsEnum)tsProperty.PropertyType);
                        }
                    }
                    break;

                default:
                    throw new ArgumentException(string.Format("Type '{0}' isn't an enum, a class, an interface, or a struct. Only enums, classes, interfaces, and structures can be added to the model", clrType.FullName));
            }
        }

        private void AddEnumIfNotAdded(TsEnum tsEnum)
        {
            if (this.Enums.ContainsKey(tsEnum.Type))
                return;
            this.Enums[tsEnum.Type] = tsEnum;
        }

        public void Add(Assembly assembly)
        {
            foreach (Type clrType in assembly.GetTypes().Where(t => t.GetCustomAttribute<DataContractAttribute>(false) != null 
                    && (TsType.GetTypeFamily(t) == TsTypeFamily.Class 
                        || TsType.GetTypeFamily(t) == TsTypeFamily.Enum
                        || TsType.GetTypeFamily(t) == TsTypeFamily.Interface)))
                this.Add(clrType);
        }

        public TsModel Build()
        {
            TsModel model = new (this, this.Classes.Values, this.Interfaces.Values, this.Enums.Values);
            model.RunVisitor(new TypeResolver(model));
            return model;
        }

        private void AddReferences(TsModuleMemberWithHierarchy model, Dictionary<Type, TypeConvertor>? typeConvertors)
        {
            foreach (TsProperty tsProperty in model.Properties.Where(p => !p.HasIgnoreAttribute))
            {
                switch (TsType.GetTypeFamily(tsProperty.PropertyType.Type))
                {
                    case TsTypeFamily.Collection:
                        Type? enumeratedType = TsType.GetEnumeratedType(tsProperty.PropertyType.Type);
                        while (enumeratedType != null)
                        {
                            switch (TsType.GetTypeFamily(enumeratedType))
                            {
                                case TsTypeFamily.Collection:
                                    Type temp = enumeratedType;
                                    enumeratedType = TsType.GetEnumeratedType(enumeratedType);
                                    if (enumeratedType == temp)
                                    {
                                        enumeratedType = null;
                                        continue;
                                    }
                                    continue;
                                case TsTypeFamily.Class:
                                case TsTypeFamily.TypeDefinition:
                                case TsTypeFamily.Interface:
                                    this.Add(enumeratedType);
                                    enumeratedType = null;
                                    continue;
                                case TsTypeFamily.Enum:
                                    this.AddEnumIfNotAdded(TsModuleService.GetOrAddTsEnum(enumeratedType));
                                    enumeratedType = null;
                                    continue;
                                default:
                                    enumeratedType = null;
                                    continue;
                            }
                        }
                        continue;
                    case TsTypeFamily.Class:
                    case TsTypeFamily.TypeDefinition:
                    case TsTypeFamily.Interface:
                        if (typeConvertors == null || !typeConvertors.ContainsKey(tsProperty.PropertyType.Type))
                        {
                            this.Add(tsProperty.PropertyType.Type);
                            continue;
                        }
                        this.Add(tsProperty.PropertyType.Type, false, typeConvertors);
                        continue;
                    default:
                        continue;
                }
            }

            foreach (TsType genericArgument in model.GenericArguments)
            {
                switch (TsType.GetTypeFamily(genericArgument.Type))
                {
                    case TsTypeFamily.Collection:
                        Type? enumeratedType = TsType.GetEnumeratedType(genericArgument.Type);
                        if (enumeratedType == null)
                        {
                            continue;
                        }
                        switch (TsType.GetTypeFamily(enumeratedType))
                        {
                            case TsTypeFamily.Class:
                            case TsTypeFamily.TypeDefinition:
                            case TsTypeFamily.Interface:
                                this.Add(enumeratedType);
                                continue;
                            case TsTypeFamily.Enum:
                                this.AddEnumIfNotAdded(TsModuleService.GetOrAddTsEnum(enumeratedType));
                                continue;
                            default:
                                continue;
                        }
                    case TsTypeFamily.Class:
                    case TsTypeFamily.TypeDefinition:
                    case TsTypeFamily.Interface:
                        this.Add(genericArgument.Type);
                        continue;
                    default:
                        continue;
                }
            }
        }
    }
}
