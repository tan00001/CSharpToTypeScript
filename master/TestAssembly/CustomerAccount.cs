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

        public virtual ValidationResult? ValidateBalance(object? value)
        {
            if (value is decimal balance)
            {
                if (balance > 0)
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult("Balance cannot be less than 0.");
            }
            return new ValidationResult("Balance data type is incorrect.");
        }

        public virtual ValidationResult? ValidateBalance2(object? value, ValidationContext context)
        {
            if (context.ObjectInstance is CustomerAccount values)
            {
                #region CSharpToTypeScript
                if ((values.Balance != null && values.Balance > 0) || values.Id == 0)
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult("Balance must be greater than 0 when Id is not 0.");
                #endregion // CSharpToTypeScript
            }
            return new ValidationResult("Balance data type is incorrect.");
        }
    }
}
