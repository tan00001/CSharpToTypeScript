using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace CSharpToTypeScript.Commands
{
    public class ModalDlgTitleValidationRule : ValidationRule
    {
        public const int MaxTitleLength = 256;
        const string ErrorMessage = "Modal title must be a string no longer than 256 characters";
        public const string ErrorMsgWhenRequired = "Modal title is required.";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return ValidationResult.ValidResult;
            }

            if (value is not string strValue || strValue.Length > MaxTitleLength)
            {
                return new ValidationResult(false, ErrorMessage);
            }

            return ValidationResult.ValidResult;
        }
    }
}
