import { useVuelidate, type ValidationArgs } from '@vuelidate/core';
import { required, email, minValue, maxValue, minLength, maxLength, helpers } from '@vuelidate/validators';

export class Person {
	city: string | null;
	firstName: string | null;
	lastName: string | null;
	state: string | null;
	streetAddress: string | null;
	zip: string | null;

	constructor(city?: string | null, firstName?: string | null, lastName?: string | null, state?: string | null, streetAddress?: string | null, zip?: string | null) {
		this.city = city ?? null;
		this.firstName = firstName ?? null;
		this.lastName = lastName ?? null;
		this.state = state ?? null;
		this.streetAddress = streetAddress ?? null;
		this.zip = zip ?? null;
	}

	static ValidateZip(value: string, values : Person): boolean {
		// {
		//     if (value is string zip)
		//     {
		//         if (zip.Length != 5)
		//         {
		//             return ValidationResult.Success;
		//         }
		//         return new ValidationResult("Balance cannot be less than 0.");
		//     }
		//     return new ValidationResult("ZIP data type is incorrect.");
		// }
		// TODO: Please implement this function.
		return true;
	}

	static ValidateZip2(value: string, values : Person): boolean {
		if (values.zip?.length === 5 || (values.firstName === null && values.lastName === null))
		{
		    return true;
		}
		return false;
	}

}

export const PersonValidationRules: ValidationArgs<Person> = {
	city: {
		required, 
		maxLength: maxLength(50)
	},
	firstName: {
		required, 
		maxLength: maxLength(50)
	},
	lastName: {
		required, 
		maxLength: maxLength(50)
	},
	state: {
		required, 
		maxLength: maxLength(50)
	},
	streetAddress: {
		required, 
		maxLength: maxLength(255)
	},
	zip: {
		required, 
		maxLength: maxLength(50), 
		ValidateZip: helpers.withMessage('', (value, siblings) => Person.ValidateZip(value, siblings)), 
		ValidateZip2: helpers.withMessage('', (value, siblings) => Person.ValidateZip2(value, siblings))
	}
};
