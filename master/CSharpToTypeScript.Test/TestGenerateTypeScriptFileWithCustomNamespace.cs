using System;
using System.Runtime.Serialization;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    [DataContract(Namespace = "CSharpToTypeScript.TestNamespace.Enums")]
    public enum RegistrationStatus
    {
        Unknown,
        Registered,
        Unregistered
    }


    [DataContract(Namespace = "CSharpToTypeScript.TestNamespace")]
    public class PersonWithNamespace : PersonWithNullableName
    {
        public Gender? Gender { get; set; }

        public RegistrationStatus? Status { get; set; }
    }

    [TestClass]
    public class TestGenerateTypeScriptFileWithCustomNamespace : IDisposable
    {
        private bool disposedValue;

        public TestContext TestContext { get; set; } = null!;

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForPersonWithCustomNamespace()
        {
            var ts = TypeScript.Definitions()
               .For<PersonWithNamespace>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            Assert.AreEqual("namespace CSharpToTypeScript.Test {\r\n\texport const enum Gender {\r\n\t\tUnknown = 0,\r\n\t\tMale = 1,\r\n\t\tFemale = 2\r\n\t}\r\n\t\r\n\tclass PersonWithNullableName {\r\n\t\tage?: number | null;\r\n\t\tdateOfBirth?: number;\r\n\t\tid?: number;\r\n\t\tname?: string | null;\r\n\t}\r\n}\r\n\r\nnamespace CSharpToTypeScript.TestNamespace {\r\n\timport { PersonWithNullableName as impPersonWithNullableName, Gender as impGender } from './CSharpToTypeScript.Test';\r\n\timport { RegistrationStatus as impRegistrationStatus } from './CSharpToTypeScript.TestNamespace.Enums';\r\n\t\r\n\tclass PersonWithNamespace extends impPersonWithNullableName {\r\n\t\tgender?: impGender | null;\r\n\t\tstatus?: impRegistrationStatus | null;\r\n\t}\r\n}\r\n\r\nnamespace CSharpToTypeScript.TestNamespace.Enums {\r\n\texport const enum RegistrationStatus {\r\n\t\tUnknown = 0,\r\n\t\tRegistered = 1,\r\n\t\tUnregistered = 2\r\n\t}\r\n}\r\n",
                script);
        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForPersonWithCustomNamespaceAndValidation()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithNamespace>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            Assert.AreEqual("namespace CSharpToTypeScript.Test {\r\n\timport { Resolver, FieldError } from 'react-hook-form';\r\n\t\r\n\texport const enum Gender {\r\n\t\tUnknown = 0,\r\n\t\tMale = 1,\r\n\t\tFemale = 2\r\n\t}\r\n\t\r\n\texport class PersonWithNullableName {\r\n\t\tage?: number | null;\r\n\t\tdateOfBirth?: number;\r\n\t\tid?: number;\r\n\t\tname?: string | null;\r\n\t}\r\n\t\r\n\texport export const PersonWithNullableNameResolver: Resolver<PersonWithNullableName> = async (values) => {\r\n\t\tconst errorBuffer = {\r\n\t\t};\r\n\t\r\n\t\r\n\t\tlet returnValues: PersonWithNullableName = {};\r\n\t\tlet returnErrors = {};\r\n\t\r\n\t\treturnValues.age = values.age;\r\n\t\treturnValues.dateOfBirth = values.dateOfBirth;\r\n\t\treturnValues.id = values.id;\r\n\t\treturnValues.name = values.name;\r\n\t\r\n\t\treturn {\r\n\t\t\tvalues: returnValues,\r\n\t\t\terrors: returnErrors\r\n\t\t};\r\n\t};\r\n}\r\n\r\nnamespace CSharpToTypeScript.TestNamespace {\r\n\timport { Resolver, FieldError } from 'react-hook-form';\r\n\timport { PersonWithNullableName as impPersonWithNullableName, Gender as impGender } from './CSharpToTypeScript.Test';\r\n\timport { RegistrationStatus as impRegistrationStatus } from './CSharpToTypeScript.TestNamespace.Enums';\r\n\t\r\n\texport class PersonWithNamespace extends impPersonWithNullableName {\r\n\t\tgender?: impGender | null;\r\n\t\tstatus?: impRegistrationStatus | null;\r\n\t}\r\n\t\r\n\texport export const PersonWithNamespaceResolver: Resolver<PersonWithNamespace> = async (values) => {\r\n\t\tconst errorBuffer = {\r\n\t\t};\r\n\t\r\n\t\r\n\t\tconst baseResults = await PersonWithNullableNameResolver(values);\r\n\t\tlet returnValues: PersonWithNamespace = { ...baseResults.Values };\r\n\t\tlet returnErrors = { ...baseResults.Errors };\r\n\t\r\n\t\treturnValues.gender = values.gender;\r\n\t\treturnValues.status = values.status;\r\n\t\r\n\t\treturn {\r\n\t\t\tvalues: returnValues,\r\n\t\t\terrors: returnErrors\r\n\t\t};\r\n\t};\r\n}\r\n\r\nnamespace CSharpToTypeScript.TestNamespace.Enums {\r\n\texport const enum RegistrationStatus {\r\n\t\tUnknown = 0,\r\n\t\tRegistered = 1,\r\n\t\tUnregistered = 2\r\n\t}\r\n}\r\n",
                script);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}