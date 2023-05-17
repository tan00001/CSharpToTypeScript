import { useId } from 'react';
import { useForm, SubmitHandler, Resolver, FieldErrors } from 'react-hook-form';
import { RegistrationStatus as impRegistrationStatus } from './CSharpToTypeScript.TestNamespace.Enums';
import { getClassName, getErrorMessage } from './BootstrapUtils';

export class PersonWithGenericMember<T> {
	id?: number;
	name: string;
	testMember?: T;

	constructor(name?: string) {
		this.name = name ?? "";
	}
}

export const PersonWithGenericMemberimpRegistrationStatusResolver: Resolver<PersonWithGenericMember<impRegistrationStatus>> = async (values) => {
	const errors: FieldErrors<PersonWithGenericMember<impRegistrationStatus>> = {};

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

export type PersonWithGenericMemberimpRegistrationStatusFormData = {
	personWithGenericMemberimpRegistrationStatus?: PersonWithGenericMember<impRegistrationStatus>,
	onSubmit: SubmitHandler<PersonWithGenericMember<impRegistrationStatus>>
};

export const PersonWithGenericMemberimpRegistrationStatusForm = (props: PersonWithGenericMemberimpRegistrationStatusFormData) => {
	const formId = useId();
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithGenericMember<impRegistrationStatus>>({
		mode: "onTouched",
		resolver: PersonWithGenericMemberimpRegistrationStatusResolver,
		defaultValues: props.personWithGenericMemberimpRegistrationStatus ?? new PersonWithGenericMember<impRegistrationStatus>()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<input type="hidden" id={formId + "-id"} {...register("id", { valueAsNumber: true })} />
		<div className="row mb-3">
			<div className="form-group col-md-12">
				<label htmlFor={formId + "-name"}>Name:</label>
				<input type="text" className={getClassName(touchedFields.name, errors.name)} id={formId + "-name"} {...register("name")} />
				{getErrorMessage(errors.name)}
			</div>
		</div>
		<div className="row mb-3">
			<div className="form-group col-md-12">
				<label htmlFor="testMember">TestMember:</label>
				<input type="text" className={getClassName(touchedFields.testMember, errors.testMember)} id={formId + "-testMember"} {...register("testMember")} />
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
