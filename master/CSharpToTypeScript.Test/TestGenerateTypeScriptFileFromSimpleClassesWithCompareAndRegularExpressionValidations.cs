using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    public class PersonWithPasswordAndSSN
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string? Password { get; set; }

        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }

        [RegularExpression("^\\d{3}-\\d{2}-\\d{4}$")]
        public string? Ssn { get; set; }
	}

    [TestClass]
    public class TestGenerateTypeScriptFileFromSimpleClassesWithCompareAndRegularExpressionValidations : IDisposable
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
        public void TestGenerateTypeScriptFileForSimpleClassesWithValidations()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithPasswordAndSSN>();

            string personTypeScript = ts.Generate();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            Assert.AreEqual("import { Resolver, FieldError } from 'react-hook-form';\r\n\r\nexport class PersonWithPasswordAndSSN {\r\n\tconfirmPassword?: string | null;\r\n\tid?: number;\r\n\tname: string;\r\n\tpassword: string | null;\r\n\tssn?: string | null;\r\n}\r\n\r\nexport export const PersonWithPasswordAndSSNResolver: Resolver<PersonWithPasswordAndSSN> = async (values) => {\r\n\tconst errorBuffer = {\r\n\t\tconfirmPassword: FieldError[]\r\n\t\tname: FieldError[]\r\n\t\tpassword: FieldError[]\r\n\t\tssn: FieldError[]\r\n\t};\r\n\r\n\tif (values.confirmPassword !== values.password) {\r\n\t\terrorBuffer.confirmPassword.push({\r\n\t\t\ttype: 'pattern',\r\n\t\t\tmessage: 'Password does not match.'\r\n\t\t});\r\n\t}\r\n\tif (!values.name) {\r\n\t\terrorBuffer.name.push({\r\n\t\ttype: 'required',\r\n\t\tmessage: 'Name is required.'\r\n\t};\r\n\tif ((values.name?.Length ?? 0) > 50) {\r\n\t\terrorBuffer.name.push({\r\n\t\t\ttype: 'maxLength',\r\n\t\t\tmessage: 'Name cannot exceed 50 characters.'\r\n\t\t});\r\n\t}\r\n\tif (!values.password) {\r\n\t\terrorBuffer.password.push({\r\n\t\ttype: 'required',\r\n\t\tmessage: 'Password is required.'\r\n\t};\r\n\tif (values.ssn && !^\\d{3}-\\d{2}-\\d{4}$.test(values.ssn)) {\r\n\t\terrorBuffer.ssn.push({\r\n\t\t\ttype: 'pattern',\r\n\t\t\tmessage: 'Ssn is invalid.'\r\n\t\t});\r\n\t}\r\n\r\n\tlet returnValues: PersonWithPasswordAndSSN = {};\r\n\tlet returnErrors = {};\r\n\r\n\tif (errorBuffer.confirmPassword.length == 0) {\r\n\t\treturnValues.confirmPassword = values.confirmPassword;\r\n\t} else {\r\n\t\treturnErrors.confirmPassword = errorBuffer.confirmPassword;\r\n\t};\r\n\treturnValues.id = values.id;\r\n\tif (errorBuffer.name.length == 0) {\r\n\t\treturnValues.name = values.name;\r\n\t} else {\r\n\t\treturnErrors.name = errorBuffer.name;\r\n\t};\r\n\tif (errorBuffer.password.length == 0) {\r\n\t\treturnValues.password = values.password;\r\n\t} else {\r\n\t\treturnErrors.password = errorBuffer.password;\r\n\t};\r\n\tif (errorBuffer.ssn.length == 0) {\r\n\t\treturnValues.ssn = values.ssn;\r\n\t} else {\r\n\t\treturnErrors.ssn = errorBuffer.ssn;\r\n\t};\r\n\r\n\treturn {\r\n\t\tvalues: returnValues,\r\n\t\terrors: returnErrors\r\n\t};\r\n};\r\n\r\n",
                personTypeScript);
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