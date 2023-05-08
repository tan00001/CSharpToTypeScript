import { useForm, SubmitHandler, FieldError, Resolver, FieldErrors, ResolverOptions } from 'react-hook-form';
import { PersonWithNullableName as impPersonWithNullableName, Gender as impGender } from './CSharpToTypeScript.Test';
import { RegistrationStatus as impRegistrationStatus } from './CSharpToTypeScript.TestNamespace.Enums';
import { PersonWithNullableNameResolver as impPersonWithNullableNameResolver } from './CSharpToTypeScript.TestForm';
import { getClassName, getErrorMessage } from './BootstrapUtils';

export class PersonWithNamespace extends impPersonWithNullableName {
	gender?: impGender | null;
	status?: impRegistrationStatus | null;
}

export const PersonWithNamespaceResolver: Resolver<PersonWithNamespace> = async (values) => {
	const errors: FieldErrors<PersonWithNamespace> = {};


	const baseResults = await impPersonWithNullableNameResolver(values, undefined, {} as ResolverOptions<impPersonWithNullableName>);
	return {
		values,
		errors: { ...baseResults.errors, ...errors }
	};
};

export type PersonWithNamespaceFormData = {
	personWithNamespace?: PersonWithNamespace,
	onSubmit: SubmitHandler<PersonWithNamespace>
};

export const PersonWithNamespaceForm = (props: PersonWithNamespaceFormData) => {
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithNamespace>({
		resolver: PersonWithNamespaceResolver,
		defaultValues: props.personWithNamespace ?? new PersonWithNamespace()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<div className="row mb-3">
			<div className="form-group col-md-4">
				<label htmlFor="age">Age:</label>
				<input type="number" className={getClassName(touchedFields.age, errors.age)} id="age" {...register("age")} />
				{getErrorMessage(errors.age)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="dateOfBirth">DateOfBirth:</label>
				<input type="number" className={getClassName(touchedFields.dateOfBirth, errors.dateOfBirth)} id="dateOfBirth" {...register("dateOfBirth")} />
				{getErrorMessage(errors.dateOfBirth)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="gender">Gender:</label>
				<select className={getClassName(touchedFields.gender, errors.gender)} id="gender" {...register('gender')}>
					<option value="">Select a Gender</option>
					<option value="0">Unknown</option>
					<option value="1">Male</option>
					<option value="2">Female</option>
				</select>
				{getErrorMessage(errors.gender)}
			</div>
		</div>
		<div className="row mb-3">
			<div className="form-group col-md-4">
				<label htmlFor="id">Id:</label>
				<input type="number" className={getClassName(touchedFields.id, errors.id)} id="id" {...register("id")} />
				{getErrorMessage(errors.id)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="name">Name:</label>
				<input type="text" className={getClassName(touchedFields.name, errors.name)} id="name" {...register("name")} />
				{getErrorMessage(errors.name)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="status">Status:</label>
				<select className={getClassName(touchedFields.status, errors.status)} id="status" {...register('status')}>
					<option value="">Select a Status</option>
					<option value="0">Unknown</option>
					<option value="1">Registered</option>
					<option value="2">Unregistered</option>
					<option value="3">Approval Pending</option>
				</select>
				{getErrorMessage(errors.status)}
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
