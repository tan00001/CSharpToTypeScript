#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSharpToTypeScript.Models;
using CSharpToTypeScript.Extensions;
using System.Runtime.Serialization;
using Microsoft.VisualBasic;

namespace CSharpToTypeScript
{
    public class TsModelBuilder
    {
        public readonly ITsModuleService TsModuleService = new TsModuleService();

        internal Dictionary<Type, TsClass> Classes { get; set; }

        internal Dictionary<Type, TsInterface> Interfaces { get; set; }

        internal Dictionary<Type, TsEnum> Enums { get; set; }

        public TsModelBuilder()
        {
            this.Classes = new Dictionary<Type, TsClass>();
            this.Interfaces = new Dictionary<Type, TsInterface>();
            this.Enums = new Dictionary<Type, TsEnum>();
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
                    if (clrType.IsNullableValueType())
                        return this.Add(clrType.GetNullableValueType(), includeReferences, typeConvertors);

                    if (Classes.TryGetValue(clrType, out TsClass? classType))
                        return classType;

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
                    foreach (TsType tsType in classType.Interfaces)
                        this.Add(tsType.Type);
                    return classType;

                case TsTypeFamily.Interface:
                    if (clrType.IsNullableValueType())
                        return this.Add(clrType.GetNullableValueType(), includeReferences, typeConvertors);

                    if (Interfaces.TryGetValue(clrType, out TsInterface? interfaceType))
                        return interfaceType;

                    if (clrType.IsGenericType)
                    {
                        ProcessGenericDefinitions(clrType, includeReferences, typeConvertors);
                    }

                    interfaceType = TsModuleService.GetOrAddTsInterface(clrType);
                    this.Interfaces[clrType] = interfaceType;
                    if (clrType.IsGenericParameter)
                        interfaceType.IsIgnored = true;
                    if (clrType.IsGenericType)
                        interfaceType.IsIgnored = true;
                    if (interfaceType.BaseType != null)
                        this.Add(interfaceType.BaseType.Type);
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
                        this.Classes[genericTypeDefinition] = classModel;
                        if (includeReferences)
                        {
                            this.AddReferences(classModel, typeConvertors);
                            foreach (TsProperty tsProperty in classModel.Properties.Where(p => p.PropertyType.Type.IsEnum))
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
                    throw new ArgumentException(string.Format("Type '{0}' isn't class or struct. Only classes and structures can be added to the model", clrType.FullName));
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
                        || TsType.GetTypeFamily(t) == TsTypeFamily.Class)))
                this.Add(clrType);
        }

        public TsModel Build()
        {
            TsModel model = new (this, this.Classes.Values, this.Interfaces.Values, this.Enums.Values);
            model.RunVisitor(TsModuleService, new TypeResolver(model));
            return model;
        }

        private void AddReferences(TsModuleMemberWithPropertiesAndGenericArguments model, Dictionary<Type, TypeConvertor>? typeConvertors)
        {
            foreach (TsProperty tsProperty in model.Properties.Where(p => p.JsonIgnore == null))
            {
                switch (TsType.GetTypeFamily(tsProperty.PropertyType.Type))
                {
                    case TsTypeFamily.Collection:
                        Type? enumerableType = TsType.GetEnumerableType(tsProperty.PropertyType.Type);
                        while (enumerableType != null)
                        {
                            switch (TsType.GetTypeFamily(enumerableType))
                            {
                                case TsTypeFamily.Collection:
                                    Type type2 = enumerableType;
                                    enumerableType = TsType.GetEnumerableType(enumerableType);
                                    if (enumerableType == type2)
                                    {
                                        enumerableType = null;
                                        continue;
                                    }
                                    continue;
                                case TsTypeFamily.Class:
                                    this.Add(enumerableType);
                                    enumerableType = null;
                                    continue;
                                case TsTypeFamily.Enum:
                                    this.AddEnumIfNotAdded(TsModuleService.GetOrAddTsEnum(enumerableType));
                                    enumerableType = null;
                                    continue;
                                default:
                                    enumerableType = null;
                                    continue;
                            }
                        }
                        continue;
                    case TsTypeFamily.Class:
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
                        Type? enumerableType = TsType.GetEnumerableType(genericArgument.Type);
                        if (enumerableType != null)
                        {
                            switch (TsType.GetTypeFamily(enumerableType))
                            {
                                case TsTypeFamily.Class:
                                    this.Add(enumerableType);
                                    continue;
                                case TsTypeFamily.Enum:
                                    this.AddEnumIfNotAdded(TsModuleService.GetOrAddTsEnum(enumerableType));
                                    continue;
                                default:
                                    continue;
                            }
                        }
                        else
                            continue;
                    case TsTypeFamily.Class:
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
