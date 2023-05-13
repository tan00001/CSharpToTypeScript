import { Resolver, FieldErrors } from 'react-hook-form';

export class PersonWithGenericMember<T> {
	id?: number;
	name: string;
	testMember?: T;

	constructor(name?: string) {
		this.name = name ?? "";
	}
}

export const PersonWithGenericMemberDateResolver: Resolver<PersonWithGenericMember<Date>> = async (values) => {
	const errors: FieldErrors<PersonWithGenericMember<Date>> = {};

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
