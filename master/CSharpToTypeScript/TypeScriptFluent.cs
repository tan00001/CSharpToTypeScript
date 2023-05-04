#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public class TypeScriptFluent
    {
        protected TsModelBuilder _modelBuilder;
        protected TsGenerator _scriptGenerator;

        public TsModelBuilder ModelBuilder => this._modelBuilder;

        public TsGenerator ScriptGenerator => this._scriptGenerator;

        public TypeScriptFluent(bool enableNamespace)
            : this(new TsGenerator(enableNamespace))
        {
        }

        public TypeScriptFluent(TsGenerator scriptGenerator)
        {
            this._modelBuilder = new TsModelBuilder();
            this._scriptGenerator = scriptGenerator;
        }

        protected TypeScriptFluent(TypeScriptFluent fluentConfigurator)
        {
            this._modelBuilder = fluentConfigurator._modelBuilder;
            this._scriptGenerator = fluentConfigurator._scriptGenerator;
        }

        public TypeScriptFluentModuleMember For<T>() => this.For(typeof(T));

        public TypeScriptFluentModuleMember For(Type type)
        {
            Dictionary<Type, TypeConvertor> convertors = this._scriptGenerator._typeConvertors._convertors;
            TsModuleMember member = this._modelBuilder.Add(type, true, convertors);
            switch (member)
            {
                case TsInterface _:
                case TsClass _:
                case TsEnum _:
                    return new TypeScriptFluentModuleMember(this, member);
                default:
                    throw new InvalidOperationException("The type must be a class or an enum");
            }
        }

        public TypeScriptFluent For(Assembly assembly)
        {
            this._modelBuilder.Add(assembly);
            return this;
        }

        public TypeScriptFluent WithTypeFormatter(TsTypeFormatter formatter)
        {
            this._scriptGenerator.RegisterTypeFormatter(formatter);
            return this;
        }

        public TypeScriptFluent WithMemberFormatter(TsMemberIdentifierFormatter formatter)
        {
            this._scriptGenerator.SetIdentifierFormatter(formatter);
            return this;
        }

        public TypeScriptFluent WithMemberTypeFormatter(TsMemberTypeFormatter formatter)
        {
            this._scriptGenerator.SetMemberTypeFormatter(formatter);
            return this;
        }

        public TypeScriptFluent WithModuleNameFormatter(TsModuleNameFormatter formatter)
        {
            this._scriptGenerator.SetModuleNameFormatter(formatter);
            return this;
        }

        public TypeScriptFluent WithVisibility(TsTypeVisibilityFormatter formatter)
        {
            this._scriptGenerator.SetTypeVisibilityFormatter(formatter);
            return this;
        }

        public TypeScriptFluent WithConvertor<TFor>(TypeConvertor convertor)
        {
            this._scriptGenerator.RegisterTypeConvertor<TFor>(convertor);
            return this;
        }

        public TypeScriptFluent WithIndentation(string indentationString)
        {
            this._scriptGenerator.IndentationString = indentationString;
            return this;
        }

        public TypeScriptFluent AsConstEnums(bool value = true)
        {
            this._scriptGenerator.GenerateConstEnums = value;
            return this;
        }

        public IReadOnlyDictionary<string, string> Generate() => this._scriptGenerator.Generate(this._modelBuilder);

        public IReadOnlyDictionary<string, string> Generate(TsGeneratorOutput output) => this._scriptGenerator.Generate(this._modelBuilder, output);

        public override string ToString()
        {
            var results = this.Generate();
            if (results.Count == 1 || this._scriptGenerator.EnableNamespaceInTypeScript)
            {
                return string.Join("\r\n", results.Values);
            }

            return string.Join("\r\n", results.Select(r => "namespace " + r.Key + " {\r\n" + IndentAllLines(r.Value) + "\r\n}\r\n"));
        }

        private string IndentAllLines(string lines) => string.Join("\r\n", lines.Split("\r\n").Select(l => this._scriptGenerator.IndentationString + l)).TrimEnd();
    }
}
