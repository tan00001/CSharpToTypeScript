import { useVuelidate, type ValidationArgs } from '@vuelidate/core';
import { required, email, minValue, maxValue, minLength, maxLength, helpers } from '@vuelidate/validators';

export class PersonWithCreditCardNumber {
	creditCardNumber?: string | null;
	id?: number;
	name: string;

	constructor(name?: string) {
		this.name = name ?? "";
	}
}

export const PersonWithCreditCardNumberValidationRules: ValidationArgs<PersonWithCreditCardNumber> = {
	creditCardNumber: {
		helpers.regex(/^(?:4\d{12}(?:\d{3})?|5[1-5]\d{14}|6(?:011|5\d{2})\d{12}|3[47]\d{13}|3(?:0[0-5]|[68]\d)\d{11}|(?:2131|1800|35\d{3})\d{11})$/)
	},
	id: {
	},
	name: {
		required, 
		maxLength: maxLength(50)
	}
};
