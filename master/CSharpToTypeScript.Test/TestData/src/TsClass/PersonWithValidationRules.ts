import { useVuelidate, type ValidationArgs } from '@vuelidate/core';
import { required, email, minValue, maxValue, minLength, maxLength, helpers } from '@vuelidate/validators';

export class PersonWithValidation {
	age?: number | null;
	id?: number;
	location?: string | null;
	name: string;

	constructor(name?: string) {
		this.name = name ?? "";
	}
}

export const PersonWithValidationValidationRules: ValidationArgs<PersonWithValidation> = {
	age: {
		minValue: minValue(20),
		maxValue: maxValue(120)
	},
	id: {
	},
	location: {
		minLength: minLength(2),
		maxLength: maxLength(120)
	},
	name: {
		required, 
		maxLength: maxLength(50)
	}
};
