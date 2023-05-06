import { useState } from 'react';
import { useForm, SubmitHandler, Resolver, FieldErrors } from 'react-hook-form';

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

export const PersonWithEmailAddressAndPhoneAndSiteUrlFormBase = (props: PersonWithEmailAddressAndPhoneAndSiteUrl) => {
	const { register, handleSubmit, formState: { errors } } = useForm<PersonWithEmailAddressAndPhoneAndSiteUrl>({
		resolver: PersonWithEmailAddressAndPhoneAndSiteUrlResolver,
		defaultValues: props
	});

	const onSubmit: SubmitHandler<PersonWithEmailAddressAndPhoneAndSiteUrl> = async (data: PersonWithEmailAddressAndPhoneAndSiteUrl) => {
		// TODO: fill in submit action details
	};

	return <form onSubmit={handleSubmit(onSubmit)}>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="emailAddress">EmailAddress:</label>
				<input type="email" className="form-control" id="emailAddress" {...register("emailAddress")} />
				{errors.emailAddress && <span className="invalid-feedback">{errors.emailAddress.message}</span>}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="homePage">HomePage:</label>
				<input type="url" className="form-control" id="homePage" {...register("homePage")} />
				{errors.homePage && <span className="invalid-feedback">{errors.homePage.message}</span>}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="id">Id:</label>
				<input type="number" className="form-control" id="id" {...register("id")} />
				{errors.id && <span className="invalid-feedback">{errors.id.message}</span>}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="name">Name:</label>
				<input type="text" className="form-control" id="name" {...register("name")} />
				{errors.name && <span className="invalid-feedback">{errors.name.message}</span>}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="phoneNumber">PhoneNumber:</label>
				<input type="tel" className="form-control" id="phoneNumber" {...register("phoneNumber")} />
				{errors.phoneNumber && <span className="invalid-feedback">{errors.phoneNumber.message}</span>}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-12">
				<button className="btn btn-primary" type="submit">Submit</button>
				<button className="btn btn-secondary" type="reset">Reset</button>
			</div>
		</div>
	</form>;
};
