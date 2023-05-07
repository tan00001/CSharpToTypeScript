import { useForm, SubmitHandler, FieldError, Resolver, FieldErrors } from 'react-hook-form';

const getClassName = (isValidated: boolean | undefined, error: FieldError | undefined): string =>
	error ? "form-control is-invalid" : (isValidated ? "form-control is-valid" : "form-control");

const getErrorMessage = (error: FieldError | undefined) =>
	error && <span className="invalid-feedback">{error.message}</span>;

export class PersonWithEmailAddressAndPhoneAndSiteUrl {
	emailAddress?: string | null;
	homePage?: string | null;
	id?: number;
	name: string;
	phoneNumber?: string | null;

	constructor(name?: string) {
		this.name = name ?? "";
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

export type PersonWithEmailAddressAndPhoneAndSiteUrlFormData = {
	personWithEmailAddressAndPhoneAndSiteUrl?: PersonWithEmailAddressAndPhoneAndSiteUrl,
	onSubmit: SubmitHandler<PersonWithEmailAddressAndPhoneAndSiteUrl>
};

export const PersonWithEmailAddressAndPhoneAndSiteUrlForm = (props: PersonWithEmailAddressAndPhoneAndSiteUrlFormData) => {
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithEmailAddressAndPhoneAndSiteUrl>({
		resolver: PersonWithEmailAddressAndPhoneAndSiteUrlResolver,
		defaultValues: props.personWithEmailAddressAndPhoneAndSiteUrl ?? new PersonWithEmailAddressAndPhoneAndSiteUrl()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="emailAddress">EmailAddress:</label>
				<input type="email" className={getClassName(touchedFields.emailAddress, errors.emailAddress)} id="emailAddress" {...register("emailAddress")} />
				{getErrorMessage(errors.emailAddress)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="homePage">HomePage:</label>
				<input type="url" className={getClassName(touchedFields.homePage, errors.homePage)} id="homePage" {...register("homePage")} />
				{getErrorMessage(errors.homePage)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="id">Id:</label>
				<input type="number" className={getClassName(touchedFields.id, errors.id)} id="id" {...register("id")} />
				{getErrorMessage(errors.id)}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="name">Name:</label>
				<input type="text" className={getClassName(touchedFields.name, errors.name)} id="name" {...register("name")} />
				{getErrorMessage(errors.name)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="phoneNumber">PhoneNumber:</label>
				<input type="tel" className={getClassName(touchedFields.phoneNumber, errors.phoneNumber)} id="phoneNumber" {...register("phoneNumber")} />
				{getErrorMessage(errors.phoneNumber)}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-12">
				<button className="btn btn-primary" type="submit" disabled={isSubmitting}>Submit</button>
				<button className="btn btn-secondary" type="reset" disabled={isSubmitting}>Reset</button>
			</div>
		</div>
	</form>;
};
