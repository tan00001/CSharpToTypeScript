import { Resolver, FieldErrors } from 'react-hook-form';

export class PersonWithEmailAddressAndPhoneAndSiteUrl {
	emailAddress?: string | null;
	homePage?: string | null;
	id?: number;
	name: string;
	phoneNumber?: string | null;

	constructor(name: string) {
		this.name = name;
	}
}

export const PersonWithEmailAddressAndPhoneAndSiteUrlResolver: Resolver<PersonWithEmailAddressAndPhoneAndSiteUrl> = async (values) => {
	const errors: FieldErrors<PersonWithEmailAddressAndPhoneAndSiteUrl> = {};

	if (values.emailAddress && !/\S+@\S+\.\S+/.test(values.emailAddress)) {
		errors.emailAddress = {
			type: 'pattern',
			message: 'The EmailAddress field is not a valid e-mail address.'
		};
	}
	if (values.homePage && !/^(http:\/\/|https:\/\/|ftp:\/\/)/.test(values.homePage)) {
		errors.homePage = {
			type: 'pattern',
			message: 'The HomePage field is not a valid fully-qualified http, https, or ftp URL.'
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
	if (values.phoneNumber && !/^(\+1|1)?[ -]?(\([2-9][0-9]{2}\)|[2-9][0-9]{2})[ -]?[2-9][0-9]{2}[ -]?[0-9]{4}$/.test(values.phoneNumber)) {
		errors.phoneNumber = {
			type: 'pattern',
			message: 'The PhoneNumber field is not a valid phone number.'
		};
	}

	return {
		values,
		errors
	};
};
