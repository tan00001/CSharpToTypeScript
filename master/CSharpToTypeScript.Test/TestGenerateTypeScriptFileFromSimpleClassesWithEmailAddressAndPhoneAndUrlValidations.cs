using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    public class PersonWithEmailAddressAndPhoneAndSiteUrl
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string? EmailAddress { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Url]
        public string? HomePage { get; set; }
	}

    [TestClass]
    public class TestGenerateTypeScriptFileFromSimpleClassesWithEmailAddressAndPhoneAndUrlValidations : IDisposable
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
        public void TestGenerateTypeScriptFileForSimpleClassesWithEmailAddressAndPhoneAndUrlValidations()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithEmailAddressAndPhoneAndSiteUrl>();

            string personTypeScript = ts.Generate();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));
             
            Assert.AreEqual("import { Resolver, FieldError } from 'react-hook-form';\r\n\r\nclass PersonWithEmailAddressAndPhoneAndSiteUrl {\r\n\temailAddress?: string | null;\r\n\thomePage?: string | null;\r\n\tid?: number;\r\n\tname: string;\r\n\tphoneNumber?: string | null;\r\n}\r\n\r\nconst PersonWithEmailAddressAndPhoneAndSiteUrlResolver: Resolver<PersonWithEmailAddressAndPhoneAndSiteUrl> = async (values) => {\r\n\tconst errorBuffer = {\r\n\t\temailAddress: FieldError[]\r\n\t\thomePage: FieldError[]\r\n\t\tname: FieldError[]\r\n\t\tphoneNumber: FieldError[]\r\n\t};\r\n\r\n\tif (values.emailAddress && !/\\S+@\\S+\\.\\S+/.test(values.emailAddress)) {\r\n\t\terrorBuffer.emailAddress.push({\r\n\t\t\ttype: 'pattern',\r\n\t\t\tmessage: 'The EmailAddress field is not a valid e-mail address.\r\n\t\t});\r\n\t}\r\n\tif (values.homePage && !/^(http:\\/\\/|https:\\/\\/|ftp:\\/\\/)/.test(values.homePage)) {\r\n\t\terrorBuffer.homePage.push({\r\n\t\t\ttype: 'pattern',\r\n\t\t\tmessage: 'The HomePage field is not a valid fully-qualified http, https, or ftp URL.\r\n\t\t});\r\n\t}\r\n\tif (!values.name) {\r\n\t\terrorBuffer.name.push({\r\n\t\ttype: 'required',\r\n\t\tmessage: 'Name is required.'\r\n\t};\r\n\tif ((values.name?.Length ?? 0) > 50) {\r\n\t\terrorBuffer.name.push({\r\n\t\t\ttype: 'maxLength',\r\n\t\t\tmessage: 'Name cannot exceed 50 characters.'\r\n\t\t});\r\n\t}\r\n\tif (values.phoneNumber && !/^(\\+1|1)?[ -]?(\\([2-9][0-9]{2}\\)|[2-9][0-9]{2})[ -]?[2-9][0-9]{2}[ -]?[0-9]{4}$/.test(values.phoneNumber)) {\r\n\t\terrorBuffer.phoneNumber.push({\r\n\t\t\ttype: 'pattern',\r\n\t\t\tmessage: 'The PhoneNumber field is not a valid phone number.\r\n\t\t});\r\n\t}\r\n\r\n\tlet returnValues: PersonWithEmailAddressAndPhoneAndSiteUrl = {};\r\n\tlet returnErrors = {};\r\n\r\n\tif (errorBuffer.emailAddress.length == 0) {\r\n\t\treturnValues.emailAddress = values.emailAddress;\r\n\t} else {\r\n\t\treturnErrors.emailAddress = errorBuffer.emailAddress;\r\n\t};\r\n\tif (errorBuffer.homePage.length == 0) {\r\n\t\treturnValues.homePage = values.homePage;\r\n\t} else {\r\n\t\treturnErrors.homePage = errorBuffer.homePage;\r\n\t};\r\n\treturnValues.id = values.id;\r\n\tif (errorBuffer.name.length == 0) {\r\n\t\treturnValues.name = values.name;\r\n\t} else {\r\n\t\treturnErrors.name = errorBuffer.name;\r\n\t};\r\n\tif (errorBuffer.phoneNumber.length == 0) {\r\n\t\treturnValues.phoneNumber = values.phoneNumber;\r\n\t} else {\r\n\t\treturnErrors.phoneNumber = errorBuffer.phoneNumber;\r\n\t};\r\n\r\n\treturn {\r\n\t\tvalues: returnValues,\r\n\t\terrors: returnErrors\r\n\t};\r\n};\r\n\r\n",
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