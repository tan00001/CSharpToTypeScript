import { useId } from 'react';
import { useForm, SubmitHandler, Resolver, FieldErrors } from 'react-hook-form';

import { getClassName, getErrorMessage } from './BootstrapUtils';

export type PersonStructureWithField = {
	description?: string;
	id?: number;
	name: string;
};

export const PersonStructureWithFieldResolver: Resolver<PersonStructureWithField> = async (values) => {
	const errors: FieldErrors<PersonStructureWithField> = {};

	if ((values.description?.length ?? 0) > 10024) {
		errors.description = {
			type: 'maxLength',
			message: 'Description cannot exceed 10024 characters.'
		};
	}
	if (values.id || values.id === 0) {
		if (values.id > 2147483647) {
			errors.id = {
				type: 'max',
				message: 'Id cannot exceed 2147483647.'
			};
		}
		if (values.id < 0) {
			errors.id = {
				type: 'min',
				message: 'Id cannot be less than 0.'
			};
		}
	}
	if (!values.name) {
		errors.name = {
			type: 'required',
			message: 'Name is required.'
		};
	}

	return {
		values,
		errors
	};
};

export type PersonStructureWithFieldFormData = {
	personStructureWithField?: PersonStructureWithField,
	onSubmit: SubmitHandler<PersonStructureWithField>
};

export const PersonStructureWithFieldForm = (props: PersonStructureWithFieldFormData) => {
	const formId = useId();
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonStructureWithField>({
		mode: "onTouched",
		resolver: PersonStructureWithFieldResolver,
		defaultValues: props.personStructureWithField
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<div className="row mb-3">
			<div className="form-group col-md-12">
				<label htmlFor={formId + "-description"}>Description:</label>
				<input type="text" className={getClassName(touchedFields.description, errors.description)} id={formId + "-description"} {...register("description")} />
				{getErrorMessage(errors.description)}
			</div>
		</div>
		<div className="row mb-3">
			<div className="form-group col-md-12">
				<label htmlFor={formId + "-id"}>Id:</label>
				<input type="number" className={getClassName(touchedFields.id, errors.id)} id={formId + "-id"} {...register("id", { valueAsNumber: true })} />
				{getErrorMessage(errors.id)}
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
