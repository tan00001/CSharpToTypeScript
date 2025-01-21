using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test.TsClass
{
    [DataContract(Name = "Person")]
    public class PersonWithCarsPlusAddressAndUiHints
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "bsColSpan", "12" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "bsColSpan", "12" })]
        public string? LastName { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Street Address", Order = 3)]
        [UIHint("", "HTML", new object[] { "bsColSpan", "12" })]
        public string? StreetAddress { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Order = 4)]
        [UIHint("", "HTML", new object[] { "bsColSpan", "6" })]
        public string? City { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Order = 5)]
        [UIHint("", "HTML", new object[] { "bsColSpan", "3" })]
        public string? State { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "ZIP", Order = 6)]
        [UIHint("", "HTML", new object[] { "bsColSpan", "3" })]
        public string? Zip { get; set; }

        [UIHint("", "HTML", new object[] { "bsColSpan", "12" })]
        public List<string>? Cars { get; set; }
    }

    [TestClass]
    public class TestPersonWithCarsPlusUiHintsWithForm : IDisposable
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
        public void TestPersonWithCarsPlusAddressAndUiHintsWithForm()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(12, null, false))
                .For<PersonWithCarsPlusAddressAndUiHints>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(PersonWithCarsPlusAddressAndUiHints));

            Assert.AreEqual(expectedData, personTypeScript);
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
