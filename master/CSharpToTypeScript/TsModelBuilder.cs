#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSharpToTypeScript.Models;
using CSharpToTypeScript.Extensions;

namespace CSharpToTypeScript
{
    public class TsModelBuilder
    {
        internal Dictionary<Type, TsClass> Classes { get; set; }

        internal Dictionary<Type, TsEnum> Enums { get; set; }

        public TsModelBuilder()
        {
            this.Classes = new Dictionary<Type, TsClass>();
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
                case TsTypeFamily.Class:
                case TsTypeFamily.Enum:
                    if (clrType.IsNullableValueType())
                        return this.Add(clrType.GetNullableValueType(), includeReferences, typeConvertors);
                    if (typeFamily == TsTypeFamily.Enum)
                    {
                        TsEnum tsEnum = new (clrType);
                        this.AddEnum(tsEnum);
                        return tsEnum;
                    }
                    if (clrType.IsGenericType && !this.Classes.ContainsKey(clrType))
                    {
                        Type genericTypeDefinition = clrType.GetGenericTypeDefinition();
                        TsClass classModel = new (genericTypeDefinition);
                        this.Classes[genericTypeDefinition] = classModel;
                        if (includeReferences)
                        {
                            this.AddReferences(classModel, typeConvertors);
                            foreach (TsProperty tsProperty in classModel.Properties.Where(p => p.PropertyType.Type.IsEnum))
                                this.AddEnum((TsEnum)tsProperty.PropertyType);
                        }
                    }
                    if (Classes.TryGetValue(clrType, out TsClass? value))
                        return value;
                    TsClass classModel1 = new (clrType);
                    this.Classes[clrType] = classModel1;
                    if (clrType.IsGenericParameter)
                        classModel1.IsIgnored = true;
                    if (clrType.IsGenericType)
                        classModel1.IsIgnored = true;
                    if (classModel1.BaseType != null)
                        this.Add(classModel1.BaseType.Type);
                    if (includeReferences)
                    {
                        this.AddReferences(classModel1, typeConvertors);
                        foreach (TsProperty tsProperty in classModel1.Properties.Where(p => p.PropertyType.Type.IsEnum))
                            this.AddEnum((TsEnum)tsProperty.PropertyType);
                    }
                    foreach (TsType tsType in classModel1.Interfaces)
                        this.Add(tsType.Type);
                    return classModel1;
                default:
                    throw new ArgumentException(string.Format("Type '{0}' isn't class or struct. Only classes and structures can be added to the model", clrType.FullName));
            }
        }

        private void AddEnum(TsEnum tsEnum)
        {
            if (this.Enums.ContainsKey(tsEnum.Type))
                return;
            this.Enums[tsEnum.Type] = tsEnum;
        }

        public void Add(Assembly assembly)
        {
            foreach (Type clrType in assembly.GetTypes().Where(t => t.GetCustomAttribute<TsNamespaceAttribute>(false) != null 
                    && (TsType.GetTypeFamily(t) == TsTypeFamily.Class 
                        || TsType.GetTypeFamily(t) == TsTypeFamily.Enum
                        || TsType.GetTypeFamily(t) == TsTypeFamily.Class)))
                this.Add(clrType);
        }

        public TsModel Build()
        {
            TsModel model = new (this.Classes.Values, this.Enums.Values);
            model.RunVisitor(new TypeResolver(model));
            return model;
        }

        private void AddReferences(TsClass classModel, Dictionary<Type, TypeConvertor>? typeConvertors)
        {
            foreach (TsProperty tsProperty in classModel.Properties.Where(p => p.JsonIgnore == null))
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
                                    this.AddEnum(new TsEnum(enumerableType));
                                    enumerableType = null;
                                    continue;
                                default:
                                    enumerableType = null;
                                    continue;
                            }
                        }
                        continue;
                    case TsTypeFamily.Class:
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
            foreach (TsType genericArgument in classModel.GenericArguments)
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
                                    this.AddEnum(new TsEnum(enumerableType));
                                    continue;
                                default:
                                    continue;
                            }
                        }
                        else
                            continue;
                    case TsTypeFamily.Class:
                        this.Add(genericArgument.Type);
                        continue;
                    default:
                        continue;
                }
            }
        }
    }
}
