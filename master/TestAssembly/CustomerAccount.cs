using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly
{
    [DataContract]
    public class CustomerAccount
    {
        [DataMember]
        public int Id { get; set; }

        [CustomValidation(typeof(CustomerAccount), nameof(ValidateBalance))]
        [CustomValidation(typeof(CustomerAccount), nameof(ValidateBalance2))]
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public decimal? Balance { get; set; }

        public virtual ValidationResult ValidateBalance(object value)
        {
            return new ValidationResult(value != null ? null : "Value cannot be null");
        }

        public virtual ValidationResult ValidateBalance2(object value, ValidationContext context)
        {
            return new ValidationResult(value != null ? null : "Value cannot be null");
        }
    }
}
