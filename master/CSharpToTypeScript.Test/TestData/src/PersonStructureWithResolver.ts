import { Resolver, FieldErrors } from 'react-hook-form';

export type PersonStructure = {
	description?: string;
	id?: number;
	name: string;
};

export const PersonStructureResolver: Resolver<PersonStructure> = async (values) => {
	const errors: FieldErrors<PersonStructure> = {};

	if ((values.description?.length ?? 0) > 10024) {
		errors.description = {
			type: 'maxLength',
			message: 'Description cannot exceed 10024 characters.'
		};
	}
	if (values.id || values.id === 0) {
		if (values.id > 2147483647) {
			errors.id = {
				type: 'max',
				message: 'Id cannot exceed 2147483647.'
			};
		}
		if (values.id < 0) {
			errors.id = {
				type: 'min',
				message: 'Id cannot be less than 0.'
			};
		}
	}
	if (!values.name) {
		errors.name = {
			type: 'required',
			message: 'Name is required.'
		};
	}

	return {
		values,
		errors
	};
};
