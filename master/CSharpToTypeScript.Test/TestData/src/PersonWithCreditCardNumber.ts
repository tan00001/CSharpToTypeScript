import { Resolver, FieldErrors } from 'react-hook-form';

export class PersonWithCreditCardNumber {
	creditCardNumber?: string | null;
	id?: number;
	name: string;

	constructor(name: string) {
		this.name = name;
	}
}

export const PersonWithCreditCardNumberResolver: Resolver<PersonWithCreditCardNumber> = async (values) => {
	const errors: FieldErrors<PersonWithCreditCardNumber> = {};

	if (values.creditCardNumber && ([...values.creditCardNumber].filter(c => c >= '0' && c <= '9')
		.reduce((checksum, c, index) => {
			let digitValue = (c.charCodeAt(0) - 48) * ((index & 2) !== 0 ? 2 : 1);
			while (digitValue > 0) {
				checksum += digitValue % 10;
				digitValue /= 10;
			}
			return checksum;
		}, 0) % 10) !== 0) {
		errors.creditCardNumber = {
			type: 'pattern',
			message: 'The Credit Card Number field is not a valid credit card number.'
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
