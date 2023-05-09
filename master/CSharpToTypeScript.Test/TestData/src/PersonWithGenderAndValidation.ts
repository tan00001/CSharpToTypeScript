import { Resolver, FieldErrors, ResolverOptions } from 'react-hook-form';

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

export const PersonWithValidationResolver: Resolver<PersonWithValidation> = async (values) => {
	const errors: FieldErrors<PersonWithValidation> = {};

	if (values.age || values.age === 0) {
		if (values.age > 120) {
			errors.age = {
				type: 'max',
				message: 'Age cannot exceed 120.'
			};
		}
		if (values.age < 20) {
			errors.age = {
				type: 'min',
				message: 'Age cannot be less than 20.'
			};
		}
	}
	if ((values.location?.length ?? 0) > 120) {
		errors.location = {
			type: 'maxLength',
			message: 'Location cannot exceed 120 characters.'
		};
	}
	if ((values.location?.length ?? 0) < 2) {
		errors.location = {
			type: 'minLength',
			message: 'Location cannot be less than 2 characters long.'
		};
	}
	if (!values.name) {
		errors.name = {
			type: 'required',
			message: 'Name is required.'
		};
	}
	if ((values.name?.length ?? 0) > 50) {
		errors.name = {
			type: 'maxLength',
			message: 'Name cannot exceed 50 characters.'
		};
	}

	return {
		values,
		errors
	};
};

export class PersonWithGenderAndValidation extends PersonWithValidation {
	gender: Gender | null;

	constructor(gender?: Gender | null, name?: string) {
		super(name);
		this.gender = gender ?? null;
	}
}

export const PersonWithGenderAndValidationResolver: Resolver<PersonWithGenderAndValidation> = async (values) => {
	const errors: FieldErrors<PersonWithGenderAndValidation> = {};

	if (!values.gender && values.gender !== 0) {
		errors.gender = {
			type: 'required',
			message: 'Gender is required.'
		};
	}

	const baseResults = await PersonWithValidationResolver(values, undefined, {} as ResolverOptions<PersonWithValidation>);
	return {
		values,
		errors: { ...baseResults.errors, ...errors }
	};
};
