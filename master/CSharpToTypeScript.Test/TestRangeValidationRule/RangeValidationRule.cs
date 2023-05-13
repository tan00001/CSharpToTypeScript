using CSharpToTypeScript.AlternateGenerators;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;

namespace CSharpToTypeScript.Test.TestRangeValidationRule
{
    [DataContract(Name = "Person")]
    public class PersonWithAgeRange
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Range(20, 120)]
        [Display(Order = 3)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public Int32? Age { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonWithAgeRangeAndMessage
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Range(20, 120, ErrorMessage = "Age is not between 20 and 120.")]
        [Display(Order = 3)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public Int32? Age { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonWithBirthDateRange
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Range(typeof(string), "1900-01-01", "2003-01-01")]
        [Display(Order = 3)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public DateTime? BirthDate { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonWithBirthDateStringRange
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Range(typeof(string), "1900-01-01", "2003-01-01")]
        [Display(Order = 3), DataType(DataType.Date)]
        [UIHint("date", "HTML", new object[] { "colSpan", "1" })]
        public string? BirthDate { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonWithDepositAmountRange
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Range(typeof(string), "0", "100000")]
        [Display(Order = 3), DataType(DataType.Currency)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? DepositAmount { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonWithContractLengthRange
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Range(typeof(string), "100:00:00:0", "200:00:00:0", ErrorMessage = "Contract length must be between 100 and 200 days.")]
        [Display(Order = 3), DataType(DataType.Duration)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public TimeSpan? ContractLength { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonWithContractLengthRange2
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Range(24 * 3600 * 1000, 48 * 3600 * 1000, ErrorMessage = "Contract length must be between 100 and 200 days.")]
        [Display(Order = 3), DataType(DataType.Duration)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public TimeSpan? ContractLength { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonWithContractLengthRange3
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Range(100D * 24 * 3600 * 1000, 200D * 24 * 3600 * 1000, ErrorMessage = "Contract length must be between 100 and 200 days.")]
        [Display(Order = 3), DataType(DataType.Duration)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public TimeSpan? ContractLength { get; set; }
    }

    public interface IAnyComparable
    {
        decimal AnyNumber { get; }
    }

    [DataContract(Name = "Person")]
    public class PersonWithAnyRange
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Range(typeof(string), "'zh-HZ'", "'en-US'", ErrorMessage = "This is for testing only. IAnyComparable range does not really make any sense.")]
        [Display(Order = 3)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public IAnyComparable? AnyComparable { get; set; }
    }

    [TestClass]
    public class RangeValidationRule
    {
        static readonly DateTime _Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
        public void TestNumberRange()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                .For<PersonWithAgeRange>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RangeValidationRule), nameof(PersonWithAgeRange));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'"));
        }

        [TestMethod]
        public void TestNumberRangeWithMessage()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                .For<PersonWithAgeRangeAndMessage>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RangeValidationRule), nameof(PersonWithAgeRangeAndMessage));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'"));
        }

        [TestMethod]
        public void TestDateRange()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                .For<PersonWithBirthDateRange>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RangeValidationRule), nameof(PersonWithBirthDateRange));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'")
                .Replace(GetTimeSinceEpoch("1900-01-01"), "-2208960000000")
                .Replace(GetTimeSinceEpoch("2003-01-01"), "1041408000000"));
        }

        [TestMethod]
        public void TestDateStringRange()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                .For<PersonWithBirthDateStringRange>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RangeValidationRule), nameof(PersonWithBirthDateStringRange));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'")
                .Replace(GetTimeSinceEpoch("1900-01-01"), "-2208960000000")
                .Replace(GetTimeSinceEpoch("2003-01-01"), "1041408000000"));
        }

        [TestMethod]
        public void TestCurrencyRange()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                .For<PersonWithDepositAmountRange>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RangeValidationRule), nameof(PersonWithDepositAmountRange));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'"));
        }

        [TestMethod]
        public void TestDurationRange()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                .For<PersonWithContractLengthRange>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RangeValidationRule), nameof(PersonWithContractLengthRange));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'"));
        }

        [TestMethod]
        public void TestDurationRange2()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                .For<PersonWithContractLengthRange2>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RangeValidationRule), nameof(PersonWithContractLengthRange2));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'"));
        }

        [TestMethod]
        public void TestDurationRange3()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                .For<PersonWithContractLengthRange3>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RangeValidationRule), nameof(PersonWithContractLengthRange));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'"));
        }

        [TestMethod]
        public void TestAnyRange()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                .For<PersonWithAnyRange>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RangeValidationRule), nameof(PersonWithAnyRange));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'"));
        }

        private static string GetTimeSinceEpoch(object dateTime)
        {
            if (!DateTimeOffset.TryParse(dateTime.ToString(), out var dateTimeOffset))
            {
                throw new ArgumentException("Invalid date time.", nameof(dateTime));
            }

            return ((dateTimeOffset - _Epoch).TotalMilliseconds).ToString();
        }
    }
}
