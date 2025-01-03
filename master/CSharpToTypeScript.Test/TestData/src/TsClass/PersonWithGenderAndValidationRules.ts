import { useVuelidate, type ValidationArgs } from '@vuelidate/core';
import { required, email, minValue, maxValue, minLength, maxLength, helpers } from '@vuelidate/validators';

export const enum Gender {
	Unknown = 0,
	Male = 1,
	Female = 2
}

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

export class PersonWithGenderAndValidation extends PersonWithValidation {
	gender: Gender | null;

	constructor(gender?: Gender | null, name?: string) {
		super(name);
		this.gender = gender ?? null;
	}
}

export const PersonWithGenderAndValidationValidationRules: ValidationArgs<PersonWithGenderAndValidation> = {
	gender: {
		required
	}
};
