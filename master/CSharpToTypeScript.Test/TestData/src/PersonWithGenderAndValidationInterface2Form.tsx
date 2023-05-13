import { useId } from 'react';
import { useForm, SubmitHandler, Resolver, FieldErrors, ResolverOptions } from 'react-hook-form';
import { getClassName, getErrorMessage } from './BootstrapUtils';

export const enum Gender {
	Unknown = 0,
	Male = 1,
	Female = 2
}

export interface IPersonWithValidation {
	age?: number | null;
	id?: number;
	location?: string | null;
	name?: string;
}

export interface IPersonWithGenderAndValidation2 extends IPersonWithValidation {
	gender?: Gender | null;
}

export class PersonWithValidationAndInterface implements IPersonWithValidation {
	age?: number | null;
	id?: number;
	location?: string | null;
	name: string;

	constructor(name?: string) {
		this.name = name ?? "";
	}
}

export const PersonWithValidationAndInterfaceResolver: Resolver<PersonWithValidationAndInterface> = async (values) => {
	const errors: FieldErrors<PersonWithValidationAndInterface> = {};

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

export type PersonWithValidationAndInterfaceFormData = {
	personWithValidationAndInterface?: PersonWithValidationAndInterface,
	onSubmit: SubmitHandler<PersonWithValidationAndInterface>
};

export const PersonWithValidationAndInterfaceForm = (props: PersonWithValidationAndInterfaceFormData) => {
	const formId = useId();
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithValidationAndInterface>({
		mode: "onTouched",
		resolver: PersonWithValidationAndInterfaceResolver,
		defaultValues: props.personWithValidationAndInterface ?? new PersonWithValidationAndInterface()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<div className="row mb-3">
			<div className="form-group col-md-2">
				<label htmlFor={formId + "-age"}>Age:</label>
				<input type="number" className={getClassName(touchedFields.age, errors.age)} id={formId + "-age"} {...register("age", { valueAsNumber: true })} />
				{getErrorMessage(errors.age)}
			</div>
			<div className="form-group col-md-2">
				<label htmlFor={formId + "-id"}>Id:</label>
				<input type="number" className={getClassName(touchedFields.id, errors.id)} id={formId + "-id"} {...register("id", { valueAsNumber: true })} />
				{getErrorMessage(errors.id)}
			</div>
			<div className="form-group col-md-2">
				<label htmlFor={formId + "-location"}>Location:</label>
				<input type="text" className={getClassName(touchedFields.location, errors.location)} id={formId + "-location"} {...register("location")} />
				{getErrorMessage(errors.location)}
			</div>
			<div className="form-group col-md-2">
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

export class PersonWithGenderAndValidationInterface2 extends PersonWithValidationAndInterface implements IPersonWithValidation, IPersonWithGenderAndValidation2 {
	gender: Gender | null;

	constructor(gender?: Gender | null, name?: string) {
		super(name);
		this.gender = gender ?? null;
	}
}

export const PersonWithGenderAndValidationInterface2Resolver: Resolver<PersonWithGenderAndValidationInterface2> = async (values) => {
	const errors: FieldErrors<PersonWithGenderAndValidationInterface2> = {};

	if (!values.gender && values.gender !== 0) {
		errors.gender = {
			type: 'required',
			message: 'Gender is required.'
		};
	}

	const baseResults = await PersonWithValidationAndInterfaceResolver(values, undefined, {} as ResolverOptions<PersonWithValidationAndInterface>);
	return {
		values,
		errors: { ...baseResults.errors, ...errors }
	};
};

export type PersonWithGenderAndValidationInterface2FormData = {
	personWithGenderAndValidationInterface2?: PersonWithGenderAndValidationInterface2,
	onSubmit: SubmitHandler<PersonWithGenderAndValidationInterface2>
};

export const PersonWithGenderAndValidationInterface2Form = (props: PersonWithGenderAndValidationInterface2FormData) => {
	const formId = useId();
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithGenderAndValidationInterface2>({
		mode: "onTouched",
		resolver: PersonWithGenderAndValidationInterface2Resolver,
		defaultValues: props.personWithGenderAndValidationInterface2 ?? new PersonWithGenderAndValidationInterface2()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<div className="row mb-3">
			<div className="form-group col-md-2">
				<label htmlFor={formId + "-age"}>Age:</label>
				<input type="number" className={getClassName(touchedFields.age, errors.age)} id={formId + "-age"} {...register("age", { valueAsNumber: true })} />
				{getErrorMessage(errors.age)}
			</div>
			<div className="form-group col-md-2">
				<label htmlFor={formId + "-gender"}>Gender:</label>
				<select className={getClassName(touchedFields.gender, errors.gender)} id={formId + "-gender"} {...register("gender", { valueAsNumber: true })}>
					<option value="0">Unknown</option>
					<option value="1">Male</option>
					<option value="2">Female</option>
				</select>
				{getErrorMessage(errors.gender)}
			</div>
			<div className="form-group col-md-2">
				<label htmlFor={formId + "-id"}>Id:</label>
				<input type="number" className={getClassName(touchedFields.id, errors.id)} id={formId + "-id"} {...register("id", { valueAsNumber: true })} />
				{getErrorMessage(errors.id)}
			</div>
			<div className="form-group col-md-2">
				<label htmlFor={formId + "-location"}>Location:</label>
				<input type="text" className={getClassName(touchedFields.location, errors.location)} id={formId + "-location"} {...register("location")} />
				{getErrorMessage(errors.location)}
			</div>
			<div className="form-group col-md-2">
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
