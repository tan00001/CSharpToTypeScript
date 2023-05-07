import { useForm, SubmitHandler, FieldError, Resolver, FieldErrors, ResolverOptions } from 'react-hook-form';

const getClassName = (isValidated: boolean | undefined, error: FieldError | undefined): string =>
	error ? "form-control is-invalid" : (isValidated ? "form-control is-valid" : "form-control");

const getErrorMessage = (error: FieldError | undefined) =>
	error && <span className="invalid-feedback">{error.message}</span>;

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

	if (values.age) {
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
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithValidation>({
		resolver: PersonWithValidationResolver,
		defaultValues: props.personWithValidation ?? new PersonWithValidation()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="age">Age:</label>
				<input type="number" className={getClassName(touchedFields.age, errors.age)} id="age" {...register("age")} />
				{getErrorMessage(errors.age)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="id">Id:</label>
				<input type="number" className={getClassName(touchedFields.id, errors.id)} id="id" {...register("id")} />
				{getErrorMessage(errors.id)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="location">Location:</label>
				<input type="text" className={getClassName(touchedFields.location, errors.location)} id="location" {...register("location")} />
				{getErrorMessage(errors.location)}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="name">Name:</label>
				<input type="text" className={getClassName(touchedFields.name, errors.name)} id="name" {...register("name")} />
				{getErrorMessage(errors.name)}
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

export class PersonWithGengerAndValidation extends PersonWithValidation {
	gender: Gender | null;

	constructor(gender?: Gender | null, name?: string) {
		super(name);
		this.gender = gender ?? null;
	}
}

export const PersonWithGengerAndValidationResolver: Resolver<PersonWithGengerAndValidation> = async (values) => {
	const errors: FieldErrors<PersonWithGengerAndValidation> = {};

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

export type PersonWithGengerAndValidationFormData = {
	personWithGengerAndValidation?: PersonWithGengerAndValidation,
	onSubmit: SubmitHandler<PersonWithGengerAndValidation>
};

export const PersonWithGengerAndValidationForm = (props: PersonWithGengerAndValidationFormData) => {
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithGengerAndValidation>({
		resolver: PersonWithGengerAndValidationResolver,
		defaultValues: props.personWithGengerAndValidation ?? new PersonWithGengerAndValidation()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="age">Age:</label>
				<input type="number" className={getClassName(touchedFields.age, errors.age)} id="age" {...register("age")} />
				{getErrorMessage(errors.age)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="gender">Gender:</label>
				<select className={getClassName(touchedFields.gender, errors.gender)} id="gender" {...register('gender')}>
					<option value="0">Unknown</option>
					<option value="1">Male</option>
					<option value="2">Female</option>
				</select>
				{getErrorMessage(errors.gender)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="id">Id:</label>
				<input type="number" className={getClassName(touchedFields.id, errors.id)} id="id" {...register("id")} />
				{getErrorMessage(errors.id)}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="location">Location:</label>
				<input type="text" className={getClassName(touchedFields.location, errors.location)} id="location" {...register("location")} />
				{getErrorMessage(errors.location)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="name">Name:</label>
				<input type="text" className={getClassName(touchedFields.name, errors.name)} id="name" {...register("name")} />
				{getErrorMessage(errors.name)}
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
