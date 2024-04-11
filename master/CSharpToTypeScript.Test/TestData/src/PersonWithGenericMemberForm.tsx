import { useId } from 'react';
import { useForm, SubmitHandler, Resolver, FieldErrors } from 'react-hook-form';
import { getClassName, getErrorMessage } from './BootstrapUtils';

export class PersonWithGenericMember<T> {
	id?: number;
	name: string;
	testMember?: T;

	constructor(name?: string) {
		this.name = name ?? "";
	}
}

export const PersonWithGenericMemberDateStringResolver: Resolver<PersonWithGenericMember<Date | string>> = async (values) => {
	const errors: FieldErrors<PersonWithGenericMember<Date | string>> = {};

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

export type PersonWithGenericMemberDateStringFormData = {
	personWithGenericMemberDateString?: PersonWithGenericMember<Date | string>,
	onSubmit: SubmitHandler<PersonWithGenericMember<Date | string>>
};

export const PersonWithGenericMemberDateStringForm = (props: PersonWithGenericMemberDateStringFormData) => {
	const formId = useId();
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithGenericMember<Date | string>>({
		mode: "onTouched",
		resolver: PersonWithGenericMemberDateStringResolver,
		defaultValues: props.personWithGenericMemberDateString ?? new PersonWithGenericMember<Date | string>()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<input type="hidden" id={formId + "-id"} {...register("id", { valueAsNumber: true })} />
		<div className="row mb-3">
			<div className="form-group col-md-4">
				<label htmlFor={formId + "-name"}>Name:</label>
				<input type="text" className={getClassName(touchedFields.name, errors.name)} id={formId + "-name"} {...register("name")} />
				{getErrorMessage(errors.name)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor={formId + "-testMember"}>TestMember:</label>
				<input type="date" className={getClassName(touchedFields.testMember, errors.testMember)} id={formId + "-testMember"} {...register("testMember", { valueAsDate: true })} />
				{getErrorMessage(errors.testMember)}
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
