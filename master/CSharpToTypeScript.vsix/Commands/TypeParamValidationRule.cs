using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CSharpToTypeScript.Commands
{
    public class TypeParamValidationRule : ValidationRule
    {
        const string ErrorMessage = "Type name cannot be blank. It must be the full type name, including namespace name.";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Column count cannot be blank.");
            }

            if (value is not string strValue)
            {
                return new ValidationResult(false, ErrorMessage);
            }

            if (string.IsNullOrWhiteSpace(strValue))
            {
                return new ValidationResult(false, ErrorMessage);
            }

            if (strValue.StartsWith("System.") && !strValue.Contains(','))
            {
                try
                {
                    if (Type.GetType(strValue) == null)
                    {
                        return new ValidationResult(false, ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    return new ValidationResult(false, ex.Message);
                }
            }

            return new ValidationResult(true, null);
        }
    }
}
