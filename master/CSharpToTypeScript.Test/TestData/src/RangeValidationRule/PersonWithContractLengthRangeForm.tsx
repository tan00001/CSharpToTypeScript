import { useId } from 'react';
import { useForm, SubmitHandler, Resolver, FieldErrors } from 'react-hook-form';
import { getClassName, getErrorMessage } from '../BootstrapUtils';

export class Person {
	contractLength?: string | null;
	firstName: string | null;
	lastName: string | null;

	constructor(firstName?: string | null, lastName?: string | null) {
		this.firstName = firstName ?? null;
		this.lastName = lastName ?? null;
	}
}

export const PersonResolver: Resolver<Person> = async (values) => {
	const errors: FieldErrors<Person> = {};

	if (values.contractLength) {
		const multipliers = [1000, 60000, 3600000, 86400000];
		const parts = values.contractLength.split(':').reverse().map(parseFloat);
		const propValueForMax: number = parts.some(p => isNaN(p)) ? 17280000001 : parts.reduce((total, part, index) =>
			(index < multipliers.length ? total + part * multipliers[index] : total), 0);
		if (propValueForMax > 17280000000) {
			errors.contractLength = {
				type: 'max',
				message: 'Contract length must be between 100 and 200 days.'
			};
		}
		const propValueForMin: number = parts.some(p => isNaN(p)) ? 8639999999 : parts.reduce((total, part, index) =>
			(index < multipliers.length ? total + part * multipliers[index] : total), 0);
		if (propValueForMin < 8640000000) {
			errors.contractLength = {
				type: 'min',
				message: 'Contract length must be between 100 and 200 days.'
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
				<label htmlFor={formId + "-contractLength"}>ContractLength:</label>
				<input type="text" className={getClassName(touchedFields.contractLength, errors.contractLength)} id={formId + "-contractLength"} {...register("contractLength")} />
				{getErrorMessage(errors.contractLength)}
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
