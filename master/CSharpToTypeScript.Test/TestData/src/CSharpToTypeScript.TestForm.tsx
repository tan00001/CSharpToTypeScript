import { useId } from 'react';
import { useForm, SubmitHandler, FieldError, Resolver, FieldErrors } from 'react-hook-form';
import { getClassName, getErrorMessage } from './BootstrapUtils';

export const enum Gender {
	Unknown = 0,
	Male = 1,
	Female = 2
}

export class PersonWithNullableName {
	age?: number | null;
	dateOfBirth?: Date;
	id?: number;
	name?: string | null;
}

export const PersonWithNullableNameResolver: Resolver<PersonWithNullableName> = async (values) => {
	const errors: FieldErrors<PersonWithNullableName> = {};


	return {
		values,
		errors
	};
};

export type PersonWithNullableNameFormData = {
	personWithNullableName?: PersonWithNullableName,
	onSubmit: SubmitHandler<PersonWithNullableName>
};

export const PersonWithNullableNameForm = (props: PersonWithNullableNameFormData) => {
	const formId = useId();
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithNullableName>({
		mode: "onTouched",
		resolver: PersonWithNullableNameResolver,
		defaultValues: props.personWithNullableName ?? new PersonWithNullableName()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<div className="row mb-3">
			<div className="form-group col-md-4">
				<label htmlFor={formId + "-age"}>Age:</label>
				<input type="number" className={getClassName(touchedFields.age, errors.age)} id={formId + "-age"} {...register("age", { valueAsNumber: true })} />
				{getErrorMessage(errors.age)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor={formId + "-dateOfBirth"}>DateOfBirth:</label>
				<input type="date" className={getClassName(touchedFields.dateOfBirth, errors.dateOfBirth)} id={formId + "-dateOfBirth"} {...register("dateOfBirth")} />
				{getErrorMessage(errors.dateOfBirth)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor={formId + "-id"}>Id:</label>
				<input type="number" className={getClassName(touchedFields.id, errors.id)} id={formId + "-id"} {...register("id", { valueAsNumber: true })} />
				{getErrorMessage(errors.id)}
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
