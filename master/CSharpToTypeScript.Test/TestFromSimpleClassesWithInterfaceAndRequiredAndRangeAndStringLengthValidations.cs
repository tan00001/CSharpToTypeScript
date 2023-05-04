using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    public interface IPersonWithValidation
    {
        int Id { get; set; }

        string Name { get; set; }

        Int32? Age { get; set; }

        string? Location { get; set; }
    }

    public class PersonWithValidationAndInterface : IPersonWithValidation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(20, 120)]
        public Int32? Age { get; set; }

        [StringLength(120, MinimumLength = 2)]
        public string? Location { get; set; }
    }

    public interface IPersonWithGenderAndValidation
    {
        Gender? Gender { get; set; }
    }

    public interface IPersonWithGenderAndValidation2 : IPersonWithValidation
    {
        Gender? Gender { get; set; }
    }

    public class PersonWithGenderAndValidationInterface: PersonWithValidationAndInterface, IPersonWithGenderAndValidation
    {
        [Required]
        public Gender? Gender { get; set; }
    }

    public class PersonWithGenderAndValidationInterface2 : PersonWithValidationAndInterface, IPersonWithGenderAndValidation2
    {
        [Required]
        public Gender? Gender { get; set; }
    }

    [TestClass]
    public class TestFromSimpleClassesWithInterfaceAndRequiredAndRangeAndStringLengthValidations : IDisposable
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
        public void TestGenerateTypeScriptFileForSimpleClassesWithRequiredAndRangeAndStringLengthValidations()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithValidationAndInterface>();

            string personTypeScript = ts.Generate();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            Assert.AreEqual("import { Resolver, FieldError } from 'react-hook-form';\r\n\r\nexport interface IPersonWithValidation {\r\n\tage?: number | null;\r\n\tid?: number;\r\n\tlocation?: string | null;\r\n\tname?: string;\r\n}\r\n\r\nexport class PersonWithValidationAndInterface implements IPersonWithValidation {\r\n\tage?: number | null;\r\n\tid?: number;\r\n\tlocation?: string | null;\r\n\tname: string;\r\n}\r\n\r\nexport export const PersonWithValidationAndInterfaceResolver: Resolver<PersonWithValidationAndInterface> = async (values) => {\r\n\tconst errorBuffer = {\r\n\t\tage: FieldError[]\r\n\t\tlocation: FieldError[]\r\n\t\tname: FieldError[]\r\n\t};\r\n\r\n\tif (values.age) {\r\n\t\tif (values.age.Length > 120) {\r\n\t\t\terrorBuffer.age.push({\r\n\t\t\t\ttype: 'max',\r\n\t\t\t\tmessage: 'Age cannot exceed 120.'\r\n\t\t\t});\r\n\t\t}\r\n\t\tif (values.age.Length < 20) {\r\n\t\t\terrorBuffer.age.push({\r\n\t\t\t\ttype: 'min',\r\n\t\t\t\tmessage: 'Age cannot be less than 20.'\r\n\t\t\t});\r\n\t\t}\r\n\t}\r\n\tif ((values.location?.Length ?? 0) > 120) {\r\n\t\terrorBuffer.location.push({\r\n\t\t\ttype: 'maxLength',\r\n\t\t\tmessage: 'Location cannot exceed 120 characters.'\r\n\t\t});\r\n\t}\r\n\tif ((values.location?.Length ?? 0) < 2) {\r\n\t\terrorBuffer.location.push({\r\n\t\t\ttype: 'minLength',\r\n\t\t\tmessage: 'Location cannot be less than 2 characters long.'\r\n\t\t});\r\n\t}\r\n\tif (!values.name) {\r\n\t\terrorBuffer.name.push({\r\n\t\ttype: 'required',\r\n\t\tmessage: 'Name is required.'\r\n\t};\r\n\tif ((values.name?.Length ?? 0) > 50) {\r\n\t\terrorBuffer.name.push({\r\n\t\t\ttype: 'maxLength',\r\n\t\t\tmessage: 'Name cannot exceed 50 characters.'\r\n\t\t});\r\n\t}\r\n\r\n\tlet returnValues: PersonWithValidationAndInterface = {};\r\n\tlet returnErrors = {};\r\n\r\n\tif (errorBuffer.age.length == 0) {\r\n\t\treturnValues.age = values.age;\r\n\t} else {\r\n\t\treturnErrors.age = errorBuffer.age;\r\n\t};\r\n\treturnValues.id = values.id;\r\n\tif (errorBuffer.location.length == 0) {\r\n\t\treturnValues.location = values.location;\r\n\t} else {\r\n\t\treturnErrors.location = errorBuffer.location;\r\n\t};\r\n\tif (errorBuffer.name.length == 0) {\r\n\t\treturnValues.name = values.name;\r\n\t} else {\r\n\t\treturnErrors.name = errorBuffer.name;\r\n\t};\r\n\r\n\treturn {\r\n\t\tvalues: returnValues,\r\n\t\terrors: returnErrors\r\n\t};\r\n};\r\n\r\n",
                personTypeScript);
        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForSimpleClassesWithValidationsWithEnum()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithGenderAndValidationInterface>();

            string script = ts.Generate();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            Assert.AreEqual("import { Resolver, FieldError } from 'react-hook-form';\r\n\r\nexport const enum Gender {\r\n\tUnknown = 0,\r\n\tMale = 1,\r\n\tFemale = 2\r\n}\r\n\r\nexport interface IPersonWithGenderAndValidation {\r\n\tgender?: Gender | null;\r\n}\r\n\r\nexport interface IPersonWithValidation {\r\n\tage?: number | null;\r\n\tid?: number;\r\n\tlocation?: string | null;\r\n\tname?: string;\r\n}\r\n\r\nexport class PersonWithValidationAndInterface implements IPersonWithValidation {\r\n\tage?: number | null;\r\n\tid?: number;\r\n\tlocation?: string | null;\r\n\tname: string;\r\n}\r\n\r\nexport export const PersonWithValidationAndInterfaceResolver: Resolver<PersonWithValidationAndInterface> = async (values) => {\r\n\tconst errorBuffer = {\r\n\t\tage: FieldError[]\r\n\t\tlocation: FieldError[]\r\n\t\tname: FieldError[]\r\n\t};\r\n\r\n\tif (values.age) {\r\n\t\tif (values.age.Length > 120) {\r\n\t\t\terrorBuffer.age.push({\r\n\t\t\t\ttype: 'max',\r\n\t\t\t\tmessage: 'Age cannot exceed 120.'\r\n\t\t\t});\r\n\t\t}\r\n\t\tif (values.age.Length < 20) {\r\n\t\t\terrorBuffer.age.push({\r\n\t\t\t\ttype: 'min',\r\n\t\t\t\tmessage: 'Age cannot be less than 20.'\r\n\t\t\t});\r\n\t\t}\r\n\t}\r\n\tif ((values.location?.Length ?? 0) > 120) {\r\n\t\terrorBuffer.location.push({\r\n\t\t\ttype: 'maxLength',\r\n\t\t\tmessage: 'Location cannot exceed 120 characters.'\r\n\t\t});\r\n\t}\r\n\tif ((values.location?.Length ?? 0) < 2) {\r\n\t\terrorBuffer.location.push({\r\n\t\t\ttype: 'minLength',\r\n\t\t\tmessage: 'Location cannot be less than 2 characters long.'\r\n\t\t});\r\n\t}\r\n\tif (!values.name) {\r\n\t\terrorBuffer.name.push({\r\n\t\ttype: 'required',\r\n\t\tmessage: 'Name is required.'\r\n\t};\r\n\tif ((values.name?.Length ?? 0) > 50) {\r\n\t\terrorBuffer.name.push({\r\n\t\t\ttype: 'maxLength',\r\n\t\t\tmessage: 'Name cannot exceed 50 characters.'\r\n\t\t});\r\n\t}\r\n\r\n\tlet returnValues: PersonWithValidationAndInterface = {};\r\n\tlet returnErrors = {};\r\n\r\n\tif (errorBuffer.age.length == 0) {\r\n\t\treturnValues.age = values.age;\r\n\t} else {\r\n\t\treturnErrors.age = errorBuffer.age;\r\n\t};\r\n\treturnValues.id = values.id;\r\n\tif (errorBuffer.location.length == 0) {\r\n\t\treturnValues.location = values.location;\r\n\t} else {\r\n\t\treturnErrors.location = errorBuffer.location;\r\n\t};\r\n\tif (errorBuffer.name.length == 0) {\r\n\t\treturnValues.name = values.name;\r\n\t} else {\r\n\t\treturnErrors.name = errorBuffer.name;\r\n\t};\r\n\r\n\treturn {\r\n\t\tvalues: returnValues,\r\n\t\terrors: returnErrors\r\n\t};\r\n};\r\n\r\nexport class PersonWithGenderAndValidationInterface extends PersonWithValidationAndInterface implements IPersonWithValidation, IPersonWithGenderAndValidation {\r\n\tgender: Gender | null;\r\n}\r\n\r\nexport export const PersonWithGenderAndValidationInterfaceResolver: Resolver<PersonWithGenderAndValidationInterface> = async (values) => {\r\n\tconst errorBuffer = {\r\n\t\tgender: FieldError[]\r\n\t};\r\n\r\n\tif (!values.gender) {\r\n\t\terrorBuffer.gender.push({\r\n\t\ttype: 'required',\r\n\t\tmessage: 'Gender is required.'\r\n\t};\r\n\r\n\tconst baseResults = await PersonWithValidationAndInterfaceResolver(values);\r\n\tlet returnValues: PersonWithGenderAndValidationInterface = { ...baseResults.Values };\r\n\tlet returnErrors = { ...baseResults.Errors };\r\n\r\n\tif (errorBuffer.gender.length == 0) {\r\n\t\treturnValues.gender = values.gender;\r\n\t} else {\r\n\t\treturnErrors.gender = errorBuffer.gender;\r\n\t};\r\n\r\n\treturn {\r\n\t\tvalues: returnValues,\r\n\t\terrors: returnErrors\r\n\t};\r\n};\r\n\r\n",
                script);
        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForSimpleClassesWithValidationsWithEnum2()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithGenderAndValidationInterface2>();

            string script = ts.Generate();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            Assert.AreEqual("import { Resolver, FieldError } from 'react-hook-form';\r\n\r\nexport const enum Gender {\r\n\tUnknown = 0,\r\n\tMale = 1,\r\n\tFemale = 2\r\n}\r\n\r\nexport interface IPersonWithValidation {\r\n\tage?: number | null;\r\n\tid?: number;\r\n\tlocation?: string | null;\r\n\tname?: string;\r\n}\r\n\r\nexport interface IPersonWithGenderAndValidation2 extends IPersonWithValidation {\r\n\tgender?: Gender | null;\r\n}\r\n\r\nexport class PersonWithValidationAndInterface implements IPersonWithValidation {\r\n\tage?: number | null;\r\n\tid?: number;\r\n\tlocation?: string | null;\r\n\tname: string;\r\n}\r\n\r\nexport export const PersonWithValidationAndInterfaceResolver: Resolver<PersonWithValidationAndInterface> = async (values) => {\r\n\tconst errorBuffer = {\r\n\t\tage: FieldError[]\r\n\t\tlocation: FieldError[]\r\n\t\tname: FieldError[]\r\n\t};\r\n\r\n\tif (values.age) {\r\n\t\tif (values.age.Length > 120) {\r\n\t\t\terrorBuffer.age.push({\r\n\t\t\t\ttype: 'max',\r\n\t\t\t\tmessage: 'Age cannot exceed 120.'\r\n\t\t\t});\r\n\t\t}\r\n\t\tif (values.age.Length < 20) {\r\n\t\t\terrorBuffer.age.push({\r\n\t\t\t\ttype: 'min',\r\n\t\t\t\tmessage: 'Age cannot be less than 20.'\r\n\t\t\t});\r\n\t\t}\r\n\t}\r\n\tif ((values.location?.Length ?? 0) > 120) {\r\n\t\terrorBuffer.location.push({\r\n\t\t\ttype: 'maxLength',\r\n\t\t\tmessage: 'Location cannot exceed 120 characters.'\r\n\t\t});\r\n\t}\r\n\tif ((values.location?.Length ?? 0) < 2) {\r\n\t\terrorBuffer.location.push({\r\n\t\t\ttype: 'minLength',\r\n\t\t\tmessage: 'Location cannot be less than 2 characters long.'\r\n\t\t});\r\n\t}\r\n\tif (!values.name) {\r\n\t\terrorBuffer.name.push({\r\n\t\ttype: 'required',\r\n\t\tmessage: 'Name is required.'\r\n\t};\r\n\tif ((values.name?.Length ?? 0) > 50) {\r\n\t\terrorBuffer.name.push({\r\n\t\t\ttype: 'maxLength',\r\n\t\t\tmessage: 'Name cannot exceed 50 characters.'\r\n\t\t});\r\n\t}\r\n\r\n\tlet returnValues: PersonWithValidationAndInterface = {};\r\n\tlet returnErrors = {};\r\n\r\n\tif (errorBuffer.age.length == 0) {\r\n\t\treturnValues.age = values.age;\r\n\t} else {\r\n\t\treturnErrors.age = errorBuffer.age;\r\n\t};\r\n\treturnValues.id = values.id;\r\n\tif (errorBuffer.location.length == 0) {\r\n\t\treturnValues.location = values.location;\r\n\t} else {\r\n\t\treturnErrors.location = errorBuffer.location;\r\n\t};\r\n\tif (errorBuffer.name.length == 0) {\r\n\t\treturnValues.name = values.name;\r\n\t} else {\r\n\t\treturnErrors.name = errorBuffer.name;\r\n\t};\r\n\r\n\treturn {\r\n\t\tvalues: returnValues,\r\n\t\terrors: returnErrors\r\n\t};\r\n};\r\n\r\nexport class PersonWithGenderAndValidationInterface2 extends PersonWithValidationAndInterface implements IPersonWithValidation, IPersonWithGenderAndValidation2 {\r\n\tgender: Gender | null;\r\n}\r\n\r\nexport export const PersonWithGenderAndValidationInterface2Resolver: Resolver<PersonWithGenderAndValidationInterface2> = async (values) => {\r\n\tconst errorBuffer = {\r\n\t\tgender: FieldError[]\r\n\t};\r\n\r\n\tif (!values.gender) {\r\n\t\terrorBuffer.gender.push({\r\n\t\ttype: 'required',\r\n\t\tmessage: 'Gender is required.'\r\n\t};\r\n\r\n\tconst baseResults = await PersonWithValidationAndInterfaceResolver(values);\r\n\tlet returnValues: PersonWithGenderAndValidationInterface2 = { ...baseResults.Values };\r\n\tlet returnErrors = { ...baseResults.Errors };\r\n\r\n\tif (errorBuffer.gender.length == 0) {\r\n\t\treturnValues.gender = values.gender;\r\n\t} else {\r\n\t\treturnErrors.gender = errorBuffer.gender;\r\n\t};\r\n\r\n\treturn {\r\n\t\tvalues: returnValues,\r\n\t\terrors: returnErrors\r\n\t};\r\n};\r\n\r\n",
                script);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
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