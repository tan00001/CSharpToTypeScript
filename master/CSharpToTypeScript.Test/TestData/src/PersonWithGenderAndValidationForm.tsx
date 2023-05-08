import { useId } from 'react';
import { useForm, SubmitHandler, FieldError, Resolver, FieldErrors, ResolverOptions } from 'react-hook-form';
import { getClassName, getErrorMessage } from './BootstrapUtils';

export const enum Gender {
	Unknown = 0,
	Male = 1,
	Female = 2
}

export class PersonWithValidation {
	age?: number | null;
	id?: number;
	location?: string | null;
	name: string;

	constructor(name?: string) {
		this.name = name ?? "";
	}
}

export const PersonWithValidationResolver: Resolver<PersonWithValidation> = async (values) => {
	const errors: FieldErrors<PersonWithValidation> = {};

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

export type PersonWithValidationFormData = {
	personWithValidation?: PersonWithValidation,
	onSubmit: SubmitHandler<PersonWithValidation>
};

export const PersonWithValidationForm = (props: PersonWithValidationFormData) => {
	const formId = useId();
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithValidation>({
		mode: "onTouched",
		resolver: PersonWithValidationResolver,
		defaultValues: props.personWithValidation ?? new PersonWithValidation()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<div className="row mb-3">
			<div className="form-group col-md-4">
				<label htmlFor={formId + "-age"}>Age:</label>
				<input type="number" className={getClassName(touchedFields.age, errors.age)} id={formId + "-age"} {...register("age", { valueAsNumber: true })} />
				{getErrorMessage(errors.age)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor={formId + "-id"}>Id:</label>
				<input type="hidden" className={getClassName(touchedFields.id, errors.id)} id={formId + "-id"} {...register("id", { valueAsNumber: true })} />
				{getErrorMessage(errors.id)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor={formId + "-location"}>Location:</label>
				<input type="text" className={getClassName(touchedFields.location, errors.location)} id={formId + "-location"} {...register("location")} />
				{getErrorMessage(errors.location)}
			</div>
		</div>
		<div className="row mb-3">
			<div className="form-group col-md-4">
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

export class PersonWithGenderAndValidation extends PersonWithValidation {
	gender: Gender | null;

	constructor(gender?: Gender | null, name?: string) {
		super(name);
		this.gender = gender ?? null;
	}
}

export const PersonWithGenderAndValidationResolver: Resolver<PersonWithGenderAndValidation> = async (values) => {
	const errors: FieldErrors<PersonWithGenderAndValidation> = {};

	if (!values.gender) {
		errors.gender = {
			type: 'required',
			message: 'Gender is required.'
		};
	}

	const baseResults = await PersonWithValidationResolver(values, undefined, {} as ResolverOptions<PersonWithValidation>);
	return {
		values,
		errors: { ...baseResults.errors, ...errors }
	};
};

export type PersonWithGenderAndValidationFormData = {
	personWithGenderAndValidation?: PersonWithGenderAndValidation,
	onSubmit: SubmitHandler<PersonWithGenderAndValidation>
};

export const PersonWithGenderAndValidationForm = (props: PersonWithGenderAndValidationFormData) => {
	const formId = useId();
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithGenderAndValidation>({
		mode: "onTouched",
		resolver: PersonWithGenderAndValidationResolver,
		defaultValues: props.personWithGenderAndValidation ?? new PersonWithGenderAndValidation()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<div className="row mb-3">
			<div className="form-group col-md-4">
				<label htmlFor={formId + "-age"}>Age:</label>
				<input type="number" className={getClassName(touchedFields.age, errors.age)} id={formId + "-age"} {...register("age", { valueAsNumber: true })} />
				{getErrorMessage(errors.age)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor={formId + "-gender"}>Gender:</label>
				<select className={getClassName(touchedFields.gender, errors.gender)} id={formId + "-gender"} {...register('gender')}>
					<option value="0">Unknown</option>
					<option value="1">Male</option>
					<option value="2">Female</option>
				</select>
				{getErrorMessage(errors.gender)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor={formId + "-id"}>Id:</label>
				<input type="hidden" className={getClassName(touchedFields.id, errors.id)} id={formId + "-id"} {...register("id", { valueAsNumber: true })} />
				{getErrorMessage(errors.id)}
			</div>
		</div>
		<div className="row mb-3">
			<div className="form-group col-md-4">
				<label htmlFor={formId + "-location"}>Location:</label>
				<input type="text" className={getClassName(touchedFields.location, errors.location)} id={formId + "-location"} {...register("location")} />
				{getErrorMessage(errors.location)}
			</div>
			<div className="form-group col-md-4">
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
