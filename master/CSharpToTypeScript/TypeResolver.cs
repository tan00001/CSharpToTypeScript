using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    internal class TypeResolver : TsModelVisitor
    {
        private readonly Dictionary<Type, TsType> _knownTypes = new();

        public TypeResolver(TsModel model)
        {
            foreach (TsClass tsClass in model.Classes)
                this._knownTypes[tsClass.Type] = tsClass;
            foreach (TsInterface tsInterface in model.Interfaces)
                this._knownTypes[tsInterface.Type] = tsInterface;
            foreach (TsEnum tsEnum in model.Enums)
                this._knownTypes[tsEnum.Type] = tsEnum;
        }

        public override void VisitClass(ITsModuleService tsModuleService, TsClass classModel)
        {
            classModel.Module = tsModuleService.GetModule(classModel.ModuleName);

            for (int index = 0; index < classModel.Interfaces.Count; ++index)
            {
                var resolvedType = this.ResolveType(tsModuleService, classModel.Interfaces[index], false);
                if (resolvedType is not TsInterface tsInterface)
                {
                    throw new Exception("Cannot resolve type \"" + classModel.Interfaces[index] + "\".");
                }
                classModel.Interfaces[index] = tsInterface;
            }
        }

        public override void VisitInterface(ITsModuleService tsModuleService, TsInterface interfaceModel)
        {
            interfaceModel.Module = tsModuleService.GetModule(interfaceModel.ModuleName);

            for (int index = 0; index < interfaceModel.Interfaces.Count; ++index)
            {
                var resolvedType = this.ResolveType(tsModuleService, interfaceModel.Interfaces[index], false);
                if (resolvedType is not TsInterface tsInterface)
                {
                    throw new Exception("Cannot resolve type \"" + interfaceModel.Interfaces[index] + "\".");
                }
                interfaceModel.Interfaces[index] = tsInterface;
            }
        }

        public override void VisitEnum(ITsModuleService tsModuleService, TsEnum enumModel)
        {
            enumModel.Module = tsModuleService.GetModule(enumModel.ModuleName);
        }

        public override void VisitProperty(ITsModuleService tsModuleService, TsProperty property)
        {
            if (property.HasIgnoreAttribute)
                return;

            property.PropertyType = this.ResolveType(tsModuleService, property.PropertyType);

            for (int index = 0; index < property.GenericArguments.Count; ++index)
            {
                property.GenericArguments[index] = this.ResolveType(tsModuleService, property.GenericArguments[index]);
            }
        }

        private TsType ResolveType(ITsModuleService tsModuleService, TsType toResolve, bool useOpenGenericDefinition = true)
        {
            if (_knownTypes.TryGetValue(toResolve.Type, out TsType? tsType))
                return tsType;

            if (toResolve.Type.IsGenericType & useOpenGenericDefinition)
            {
                if (this._knownTypes.TryGetValue(toResolve.Type.GetGenericTypeDefinition(), out tsType))
                    return tsType;
            }
            else if (toResolve.Type.IsGenericType)
            {
                tsType = TsType.Create(tsModuleService, toResolve.Type);
                this._knownTypes[toResolve.Type] = tsType;
                return tsType;
            }

            switch (TsType.GetTypeFamily(toResolve.Type))
            {
                case TsTypeFamily.System:
                    tsType = new TsSystemType(toResolve.Type);
                    break;
                case TsTypeFamily.Collection:
                    tsType = this.CreateCollectionType(tsModuleService, toResolve);
                    break;
                case TsTypeFamily.Enum:
                    tsType = tsModuleService.GetOrAddTsEnum(toResolve.Type);
                    break;
                default:
                    tsType = TsType.Any;
                    break;
            }
            this._knownTypes[toResolve.Type] = tsType;
            return tsType;
        }

        private TsCollection CreateCollectionType(ITsModuleService tsModuleService, TsType type)
        {
            TsCollection collectionType = new (tsModuleService, type.Type);
            collectionType.ItemsType = this.ResolveType(tsModuleService, collectionType.ItemsType, false);
            return collectionType;
        }
    }
}
