import { useId } from 'react';
import { useForm, SubmitHandler, Resolver, FieldErrors } from 'react-hook-form';
import { getClassName, getErrorMessage } from './BootstrapUtils';

export class Person {
	cars?: string[] | null;
	city: string | null;
	firstName: string | null;
	lastName: string | null;
	state: string | null;
	streetAddress: string | null;
	zip: string | null;

	constructor(city?: string | null, firstName?: string | null, lastName?: string | null, state?: string | null, streetAddress?: string | null, zip?: string | null) {
		this.city = city ?? null;
		this.firstName = firstName ?? null;
		this.lastName = lastName ?? null;
		this.state = state ?? null;
		this.streetAddress = streetAddress ?? null;
		this.zip = zip ?? null;
	}
}

export const PersonResolver: Resolver<Person> = async (values) => {
	const errors: FieldErrors<Person> = {};

	if (!values.city) {
		errors.city = {
			type: 'required',
			message: 'City is required.'
		};
	}
	if ((values.city?.length ?? 0) > 50) {
		errors.city = {
			type: 'maxLength',
			message: 'City cannot exceed 50 characters.'
		};
	}
	if (!values.firstName) {
		errors.firstName = {
			type: 'required',
			message: 'First Name is required.'
		};
	}
	if ((values.firstName?.length ?? 0) > 50) {
		errors.firstName = {
			type: 'maxLength',
			message: 'First Name cannot exceed 50 characters.'
		};
	}
	if (!values.lastName) {
		errors.lastName = {
			type: 'required',
			message: 'Last Name is required.'
		};
	}
	if ((values.lastName?.length ?? 0) > 50) {
		errors.lastName = {
			type: 'maxLength',
			message: 'Last Name cannot exceed 50 characters.'
		};
	}
	if (!values.state) {
		errors.state = {
			type: 'required',
			message: 'State is required.'
		};
	}
	if ((values.state?.length ?? 0) > 50) {
		errors.state = {
			type: 'maxLength',
			message: 'State cannot exceed 50 characters.'
		};
	}
	if (!values.streetAddress) {
		errors.streetAddress = {
			type: 'required',
			message: 'Street Address is required.'
		};
	}
	if ((values.streetAddress?.length ?? 0) > 255) {
		errors.streetAddress = {
			type: 'maxLength',
			message: 'Street Address cannot exceed 255 characters.'
		};
	}
	if (!values.zip) {
		errors.zip = {
			type: 'required',
			message: 'ZIP is required.'
		};
	}
	if ((values.zip?.length ?? 0) > 50) {
		errors.zip = {
			type: 'maxLength',
			message: 'ZIP cannot exceed 50 characters.'
		};
	}

	return {
		values,
		errors
	};
};

export type PersonFormData = {
	person?: Person,
	onSubmit: SubmitHandler<Person>
};

export const PersonForm = (props: PersonFormData) => {
	const formId = useId();
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<Person>({
		mode: "onTouched",
		resolver: PersonResolver,
		defaultValues: props.person ?? new Person()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<div className="row mb-3">
			<div className="form-group col-md-12">
				<label htmlFor={formId + "-firstName"}>First Name:</label>
				<input type="text" className={getClassName(touchedFields.firstName, errors.firstName)} id={formId + "-firstName"} {...register("firstName")} />
				{getErrorMessage(errors.firstName)}
			</div>
		</div>
		<div className="row mb-3">
			<div className="form-group col-md-12">
				<label htmlFor={formId + "-lastName"}>Last Name:</label>
				<input type="text" className={getClassName(touchedFields.lastName, errors.lastName)} id={formId + "-lastName"} {...register("lastName")} />
				{getErrorMessage(errors.lastName)}
			</div>
		</div>
		<div className="row mb-3">
			<div className="form-group col-md-12">
				<label htmlFor={formId + "-streetAddress"}>Street Address:</label>
				<input type="text" className={getClassName(touchedFields.streetAddress, errors.streetAddress)} id={formId + "-streetAddress"} {...register("streetAddress")} />
				{getErrorMessage(errors.streetAddress)}
			</div>
		</div>
		<div className="row mb-3">
			<div className="form-group col-md-6">
				<label htmlFor={formId + "-city"}>City:</label>
				<input type="text" className={getClassName(touchedFields.city, errors.city)} id={formId + "-city"} {...register("city")} />
				{getErrorMessage(errors.city)}
			</div>
			<div className="form-group col-md-3">
				<label htmlFor={formId + "-state"}>State:</label>
				<input type="text" className={getClassName(touchedFields.state, errors.state)} id={formId + "-state"} {...register("state")} />
				{getErrorMessage(errors.state)}
			</div>
			<div className="form-group col-md-3">
				<label htmlFor={formId + "-zip"}>ZIP:</label>
				<input type="text" className={getClassName(touchedFields.zip, errors.zip)} id={formId + "-zip"} {...register("zip")} />
				{getErrorMessage(errors.zip)}
			</div>
		</div>
		<div className="row mb-3">
			<div className="form-group col-md-12">
				<label htmlFor="cars">Cars:</label>
				<input type="text" className={getClassName(touchedFields.cars, errors.cars)} id={formId + "-cars"} {...register("cars")} />
				{getErrorMessage(errors.cars)}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-12">
				<button className="btn btn-primary" type="submit" disabled={isSubmitting}>Submit</button>
				<button className="btn btn-secondary mx-1" type="reset" disabled={isSubmitting}>Reset</button>
			</div>
		</div>
	</form>;
};
