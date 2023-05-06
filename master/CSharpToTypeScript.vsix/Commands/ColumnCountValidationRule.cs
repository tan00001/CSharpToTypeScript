using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CSharpToTypeScript.Commands
{
    public class ColumnCountValidationRule : ValidationRule
    {
        // Bootstrap column count max. is 12
        const Int32 MaxColCount = 12;
        const string ErrorMessage = "Column count must be a positive integer no greater than 12";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Column count cannot be blank.");
            }

            if (value is int intValue)
            {
                if (intValue <= 0 || intValue > MaxColCount)
                {
                    return new ValidationResult(false, ErrorMessage);
                }
            }

            if (value is not string strValue)
            {
                return new ValidationResult(false, ErrorMessage);
            }

            if (!int.TryParse(strValue, out intValue))
            {
                return new ValidationResult(false, ErrorMessage);
            }

            if (intValue <= 0 || intValue > MaxColCount)
            {
                return new ValidationResult(false, ErrorMessage);
            }

            return new ValidationResult(true, null);
        }
    }
}
