import { useState } from 'react';
import { useForm, SubmitHandler, Resolver, FieldErrors, ResolverOptions } from 'react-hook-form';

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

	constructor(name: string) {
		this.name = name;
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

export const PersonWithValidationFormBase = (props: PersonWithValidation) => {
	const { register, handleSubmit, formState: { errors } } = useForm<PersonWithValidation>({
		resolver: PersonWithValidationResolver,
		defaultValues: props
	});

	const onSubmit: SubmitHandler<PersonWithValidation> = async (data: PersonWithValidation) => {
		// TODO: fill in submit action details
	};

	return <form onSubmit={handleSubmit(onSubmit)}>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="age">Age:</label>
				<input type="number" className="form-control" id="age" {...register("age")} />
				{errors.age && <span className="invalid-feedback">{errors.age.message}</span>}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="id">Id:</label>
				<input type="number" className="form-control" id="id" {...register("id")} />
				{errors.id && <span className="invalid-feedback">{errors.id.message}</span>}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="location">Location:</label>
				<input type="text" className="form-control" id="location" {...register("location")} />
				{errors.location && <span className="invalid-feedback">{errors.location.message}</span>}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="name">Name:</label>
				<input type="text" className="form-control" id="name" {...register("name")} />
				{errors.name && <span className="invalid-feedback">{errors.name.message}</span>}
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

export class PersonWithGengerAndValidation extends PersonWithValidation {
	gender: Gender | null;

	constructor(gender: Gender | null, name: string) {
		super(name);
		this.gender = gender;
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

export const PersonWithGengerAndValidationFormBase = (props: PersonWithGengerAndValidation) => {
	const { register, handleSubmit, formState: { errors } } = useForm<PersonWithGengerAndValidation>({
		resolver: PersonWithGengerAndValidationResolver,
		defaultValues: props
	});

	const onSubmit: SubmitHandler<PersonWithGengerAndValidation> = async (data: PersonWithGengerAndValidation) => {
		// TODO: fill in submit action details
	};

	return <form onSubmit={handleSubmit(onSubmit)}>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="age">Age:</label>
				<input type="number" className="form-control" id="age" {...register("age")} />
				{errors.age && <span className="invalid-feedback">{errors.age.message}</span>}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="gender">Gender:</label>
				<select id="gender" {...register('gender')}>
					<option value="0">Unknown</option>
					<option value="1">Male</option>
					<option value="2">Female</option>
				</select>
				{errors.gender && <span className="invalid-feedback">{errors.gender.message}</span>}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="id">Id:</label>
				<input type="number" className="form-control" id="id" {...register("id")} />
				{errors.id && <span className="invalid-feedback">{errors.id.message}</span>}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="location">Location:</label>
				<input type="text" className="form-control" id="location" {...register("location")} />
				{errors.location && <span className="invalid-feedback">{errors.location.message}</span>}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="name">Name:</label>
				<input type="text" className="form-control" id="name" {...register("name")} />
				{errors.name && <span className="invalid-feedback">{errors.name.message}</span>}
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
