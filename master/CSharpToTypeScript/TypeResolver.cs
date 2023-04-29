#nullable enable
using System;
using System.Collections.Generic;
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    internal class TypeResolver : TsModelVisitor
    {
        private readonly TsModel _model;
        private readonly Dictionary<Type, TsType> _knownTypes = new();
        private readonly Dictionary<string, TsModule> _modules = new ();

        public TypeResolver(TsModel model)
        {
            this._model = model;
            foreach (TsClass tsClass in model.Classes)
                this._knownTypes[tsClass.Type] = tsClass;
            foreach (TsEnum tsEnum in model.Enums)
                this._knownTypes[tsEnum.Type] = tsEnum;
        }

        public override void VisitClass(TsClass classModel)
        {
            if (classModel.Module != null)
                classModel.Module = this.ResolveModule(classModel.Module.Name);

            if (classModel.BaseType != null && classModel.BaseType != TsType.Any)
                classModel.BaseType = this.ResolveType(classModel.BaseType, false);

            for (int index = 0; index < classModel.Interfaces.Count; ++index)
            {
                var resolvedType = this.ResolveType(classModel.Interfaces[index], false) ?? throw new Exception("Cannot resolve type \"" + classModel.Interfaces[index] + "\".");
                classModel.Interfaces[index] = resolvedType;
            }
        }

        public override void VisitEnum(TsEnum enumModel)
        {
            if (enumModel.Module == null)
                return;
            enumModel.Module = this.ResolveModule(enumModel.Module.Name);
        }

        public override void VisitProperty(TsProperty property)
        {
            if (property.JsonIgnore != null)
                return;

            property.PropertyType = this.ResolveType(property.PropertyType);
            if (property.GenericArguments == null)
                return;

            for (int index = 0; index < property.GenericArguments.Count; ++index)
                property.GenericArguments[index] = this.ResolveType(property.GenericArguments[index]);
        }

        private TsType? ResolveType(TsType? toResolve, bool useOpenGenericDefinition = true)
        {
            if (toResolve == null)
                return toResolve;
            if (_knownTypes.TryGetValue(toResolve.Type, out TsType? value))
                return value;
            if (toResolve.Type.IsGenericType & useOpenGenericDefinition)
            {
                if (this._knownTypes.TryGetValue(toResolve.Type.GetGenericTypeDefinition(), out var tsType))
                    return tsType;
            }
            else if (toResolve.Type.IsGenericType)
            {
                TsType tsType = TsType.Create(toResolve.Type);
                this._knownTypes[toResolve.Type] = tsType;
                return tsType;
            }
            TsType tsType1;
            switch (TsType.GetTypeFamily(toResolve.Type))
            {
                case TsTypeFamily.System:
                    tsType1 = (TsType)new TsSystemType(toResolve.Type);
                    break;
                case TsTypeFamily.Collection:
                    tsType1 = (TsType)this.CreateCollectionType(toResolve);
                    break;
                case TsTypeFamily.Enum:
                    tsType1 = (TsType)new TsEnum(toResolve.Type);
                    break;
                default:
                    tsType1 = TsType.Any;
                    break;
            }
            this._knownTypes[toResolve.Type] = tsType1;
            return tsType1;
        }

        private TsCollection CreateCollectionType(TsType type)
        {
            TsCollection collectionType = new (type.Type);
            collectionType.ItemsType = this.ResolveType(collectionType.ItemsType, false);
            return collectionType;
        }

        private TsModule ResolveModule(string? name)
        {
            name ??= string.Empty;
            if (_modules.TryGetValue(name, out TsModule? value))
                return value;
            TsModule tsModule = new (name);
            this._modules[name] = tsModule;
            this._model.Modules.Add(tsModule);
            return tsModule;
        }
    }
}
