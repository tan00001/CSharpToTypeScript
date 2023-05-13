﻿using CSharpToTypeScript.AlternateGenerators;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;

namespace CSharpToTypeScript.Test.TestRangeValidationRule
{
    [DataContract(Name = "Person")]
    public class PersonWithBadDateRange
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

        [Range(typeof(string), "AAA", "BBB")]
        [Display(Order = 3), DataType(DataType.Date)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public DateTime? RegistrationDate { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonWithBadDepositAmountRange
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

        [Range(typeof(string), "AAA", "BBB")]
        [Display(Order = 3), DataType(DataType.Currency)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? DepositAmount { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonWithBadDepositAmountRangeMin
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

        [Range(typeof(string), "AAA", "100000")]
        [Display(Order = 3), DataType(DataType.Currency)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? DepositAmount { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonWithBadContractLengthRange
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

        [Range(typeof(string), "a:00:00:0", "b:00:00:0", ErrorMessage = "Contract length must be between a and b days.")]
        [Display(Order = 3), DataType(DataType.Duration)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public TimeSpan? ContractLength { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonWithBadContractLengthRangeMin
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

        [Range(typeof(string), "a:00:00:0", "10:00:00:0", ErrorMessage = "Contract length must be between a and 100 days.")]
        [Display(Order = 3), DataType(DataType.Duration)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public TimeSpan? ContractLength { get; set; }
    }

    public struct BadDurationStruct
    {
        public override string? ToString()
        {
            return null;
        }
    }

    [DataContract(Name = "Person")]
    public class PersonWithBadContractLengthRangeMin2
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

        // For testing purpose only.
        [Range(typeof(TimeSpan), "\r\n", "", ErrorMessage = "Contract length must be between a and 100 days.")]
        [Display(Order = 3), DataType(DataType.Duration)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public TimeSpan? ContractLength { get; set; }
    }

    [TestClass]
    public class TestInvalidRangeValidationRule
    {
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
        public void TestDateRangeException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                    .For<PersonWithBadDateRange>();

                string personTypeScript = ts.ToString();
            });
        }

        [TestMethod]
        public void TestCurrencyRangeException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                    .For<PersonWithBadDepositAmountRange>();

                string personTypeScript = ts.ToString();
            });
        }

        [TestMethod]
        public void TestCurrencyRangeMinException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                    .For<PersonWithBadDepositAmountRangeMin>();

                string personTypeScript = ts.ToString();
            });
        }

        [TestMethod]
        public void TestDurationRangeException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            { 
                var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                    .For<PersonWithBadContractLengthRange>();

                string personTypeScript = ts.ToString();
            });
        }

        [TestMethod]
        public void TestDurationRangeMinException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                    .For<PersonWithBadContractLengthRangeMin>();

                string personTypeScript = ts.ToString();
            });
        }

        [TestMethod]
        public void TestDurationRangeMin2Exception()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
                    .For<PersonWithBadContractLengthRangeMin2>();

                string personTypeScript = ts.ToString();
            });
        }
    }
}
