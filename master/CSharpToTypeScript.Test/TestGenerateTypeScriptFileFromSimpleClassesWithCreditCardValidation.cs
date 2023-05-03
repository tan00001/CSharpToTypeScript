using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    public class PersonWithCreditCardNumber
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [CreditCard]
        [Display(Name="Credit Card Number")]
        public string? CreditCardNumber { get; set; }
	}

    [TestClass]
    public class TestGenerateTypeScriptFileFromSimpleClassesWithXreditCardlValidation : IDisposable
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
        public void TestGenerateTypeScriptFileForSimpleClassesWithCreditCardValidations()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithCreditCardNumber>();

            string personTypeScript = ts.Generate();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            Assert.AreEqual("import { Resolver, FieldError } from 'react-hook-form';\r\n\r\nclass PersonWithCreditCardNumber {\r\n\tcreditCardNumber?: string | null;\r\n\tid?: number;\r\n\tname: string;\r\n}\r\n\r\nconst PersonWithCreditCardNumberResolver: Resolver<PersonWithCreditCardNumber> = async (values) => {\r\n\tconst errorBuffer = {\r\n\t\tcreditCardNumber: FieldError[]\r\n\t\tname: FieldError[]\r\n\t};\r\n\r\n\tif (values.creditCardNumber && ([...values.creditCardNumber].filter(c => c >= '0' && c <= '9')\r\n\t\t.reduce((checksum, c, index) => {\r\n\t\t\tvar digitValue = (digit - '0') * ((index & 2) !== 0 ? 2 : 1);\r\n\t\t\twhile (digitValue > 0) {\r\n\t\t\t\tchecksum += digitValue % 10;\r\n\t\t\t\tdigitValue /= 10;\r\n\t\t\t}\r\n\t\t\treturn checksum;\r\n\t\t}, 0) % 10) !== 0) {\r\n\t\terrorBuffer.creditCardNumber.push({\r\n\t\t\ttype: 'pattern',\r\n\t\t\tmessage: 'The Credit Card Number field is not a valid credit card number.\r\n\t\t});\r\n\t}\r\n\tif (!values.name) {\r\n\t\terrorBuffer.name.push({\r\n\t\ttype: 'required',\r\n\t\tmessage: 'Name is required.'\r\n\t};\r\n\tif ((values.name?.Length ?? 0) > 50) {\r\n\t\terrorBuffer.name.push({\r\n\t\t\ttype: 'maxLength',\r\n\t\t\tmessage: 'Name cannot exceed 50 characters.'\r\n\t\t});\r\n\t}\r\n\r\n\tlet returnValues: PersonWithCreditCardNumber = {};\r\n\tlet returnErrors = {};\r\n\r\n\tif (errorBuffer.creditCardNumber.length == 0) {\r\n\t\treturnValues.creditCardNumber = values.creditCardNumber;\r\n\t} else {\r\n\t\treturnErrors.creditCardNumber = errorBuffer.creditCardNumber;\r\n\t};\r\n\treturnValues.id = values.id;\r\n\tif (errorBuffer.name.length == 0) {\r\n\t\treturnValues.name = values.name;\r\n\t} else {\r\n\t\treturnErrors.name = errorBuffer.name;\r\n\t};\r\n\r\n\treturn {\r\n\t\tvalues: returnValues,\r\n\t\terrors: returnErrors\r\n\t};\r\n};\r\n\r\n",
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