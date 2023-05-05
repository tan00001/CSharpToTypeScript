import { Resolver, FieldErrors } from 'react-hook-form';

export class PersonWithPasswordAndSSN {
	confirmPassword?: string | null;
	id?: number;
	name: string;
	password: string | null;
	ssn?: string | null;

	constructor(name: string, password: string | null) {
		this.name = name;
		this.password = password;
	}
}

export const PersonWithPasswordAndSSNResolver: Resolver<PersonWithPasswordAndSSN> = async (values) => {
	const errors: FieldErrors<PersonWithPasswordAndSSN> = {};

	if (values.confirmPassword !== values.password) {
		errors.confirmPassword = {
			type: 'pattern',
			message: 'Password does not match.'
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
	if (!values.password) {
		errors.password = {
			type: 'required',
			message: 'Password is required.'
		};
	}
	if (values.ssn && !/\d{3}-\d{2}-\d{4}$/.test(values.ssn)) {
		errors.ssn = {
			type: 'pattern',
			message: 'Ssn is invalid.'
		};
	}

	return {
		values,
		errors
	};
};
