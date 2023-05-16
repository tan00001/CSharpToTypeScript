import { useId } from 'react';
import { useForm, SubmitHandler, Resolver, FieldErrors } from 'react-hook-form';
import { getClassName, getErrorMessage } from '../BootstrapUtils';

export class PersonWithSelectOptions {
	age?: number | null;
	id?: number;
	location?: string | null;
	name: string;

	constructor(name?: string) {
		this.name = name ?? "";
	}
}

export const PersonWithSelectOptionsResolver: Resolver<PersonWithSelectOptions> = async (values) => {
	const errors: FieldErrors<PersonWithSelectOptions> = {};

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

export type PersonWithSelectOptionsFormData = {
	personWithSelectOptions?: PersonWithSelectOptions,
	onSubmit: SubmitHandler<PersonWithSelectOptions>
};

export const PersonWithSelectOptionsForm = (props: PersonWithSelectOptionsFormData) => {
	const formId = useId();
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithSelectOptions>({
		mode: "onTouched",
		resolver: PersonWithSelectOptionsResolver,
		defaultValues: props.personWithSelectOptions ?? new PersonWithSelectOptions()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<input type="hidden" id={formId + "-id"} {...register("id", { valueAsNumber: true })} />
		<div className="row mb-3">
			<div className="form-group col-md-12">
				<label htmlFor={formId + "-age"}>Age:</label>
				<input type="number" className={getClassName(touchedFields.age, errors.age)} id={formId + "-age"} {...register("age", { valueAsNumber: true })} />
				{getErrorMessage(errors.age)}
			</div>
		</div>
		<div className="row mb-3">
			<div className="form-group col-md-12">
				<label htmlFor={formId + "-location"}>Location:</label>
				<select className={getClassName(touchedFields.location, errors.location)} id={formId + "-location"} {...register("location", { valueAsNumber: true })}>
					<option value="">Select a Location</option>
					<option value="NY">NY</option>
					<option value="CA">CA</option>
				</select>
				{getErrorMessage(errors.location)}
			</div>
		</div>
		<div className="row mb-3">
			<div className="form-group col-md-12">
				<label htmlFor={formId + "-name"}>Name:</label>
				<input type="text" className={getClassName(touchedFields.name, errors.name)} id={formId + "-name"} {...register("name")} />
				{getErrorMessage(errors.name)}
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
