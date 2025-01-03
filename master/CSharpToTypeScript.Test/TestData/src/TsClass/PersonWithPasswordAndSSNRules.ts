import { useVuelidate, type ValidationArgs } from '@vuelidate/core';
import { required, email, minValue, maxValue, minLength, maxLength, helpers } from '@vuelidate/validators';

export class PersonWithPasswordAndSSN {
	confirmPassword?: string | null;
	id?: number;
	name: string;
	password: string | null;
	ssn?: string | null;

	constructor(name?: string, password?: string | null) {
		this.name = name ?? "";
		this.password = password ?? null;
	}
}

export const PersonWithPasswordAndSSNValidationRules: ValidationArgs<PersonWithPasswordAndSSN> = {
	confirmPassword: {
		compareToPassword: helpers.withMessage('Password does not match.', (value, siblings) => value === siblings.Password)
	},
	id: {
	},
	name: {
		required, 
		maxLength: maxLength(50)
	},
	password: {
		required
	},
	ssn: {
		regExPattern: helpers.regex(/\d{3}-\d{2}-\d{4}$/)
	}
};
