import { useId } from 'react';
import { useForm, SubmitHandler, Resolver, FieldErrors } from 'react-hook-form';
import { getClassName, getErrorMessage } from '../BootstrapUtils';

export class Person {
	birthDate?: string | null;
	firstName: string | null;
	lastName: string | null;

	constructor(firstName?: string | null, lastName?: string | null) {
		this.firstName = firstName ?? null;
		this.lastName = lastName ?? null;
	}
}

export const PersonResolver: Resolver<Person> = async (values) => {
	const errors: FieldErrors<Person> = {};

	if (values.birthDate) {
		const propValue: number = Date.parse(values.birthDate);
		if (isNaN(propValue) || propValue > 1041408000000) {
			errors.birthDate = {
				type: 'max',
				message: 'BirthDate cannot be later than 2003-01-01.'
			};
		}
		if (isNaN(propValue) || propValue < -2208960000000) {
			errors.birthDate = {
				type: 'min',
				message: 'BirthDate cannot be earlier than 1900-01-01.'
			};
		}
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
				<label htmlFor={formId + "-birthDate"}>BirthDate:</label>
				<input type="date" className={getClassName(touchedFields.birthDate, errors.birthDate)} id={formId + "-birthDate"} {...register("birthDate")} />
				{getErrorMessage(errors.birthDate)}
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
