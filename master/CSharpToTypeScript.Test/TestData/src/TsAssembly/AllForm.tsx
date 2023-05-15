import { Resolver, FieldErrors, FieldError } from 'react-hook-form';

export namespace TestAssembly {
	export class CustomerAccount {
		static ValidateBalance(values: CustomerAccount): FieldError | undefined {
			// TODO: Please implement this function.
			//         {
			//             return new ValidationResult(value != null ? null : "Value cannot be null");
			//         }
			return {
				type: "custom",
				message: "ValidateBalance is to be implemented"
			};
		};
		static ValidateBalance2(values: CustomerAccount): FieldError | undefined {
			// TODO: Please implement this function.
			//         {
			//             return new ValidationResult(value != null ? null : "Value cannot be null");
			//         }
			return {
				type: "custom",
				message: "ValidateBalance2 is to be implemented"
			};
		};
		balance: number | null;
		id?: number;

		constructor(balance?: number | null) {
			this.balance = balance ?? null;
		}
	}

	export const CustomerAccountResolver: Resolver<CustomerAccount> = async (values) => {
		const errors: FieldErrors<CustomerAccount> = {};

		if (!values.balance) {
			errors.balance = {
				type: 'required',
				message: 'Balance is required.'
			};
		}
		errors.balance ??= CustomerAccount.ValidateBalance(values);
		errors.balance ??= CustomerAccount.ValidateBalance2(values);

		return {
			values,
			errors
		};
	};

	export class Person {
		description?: string;
		id?: number;
		name: string;
		title?: string | null;

		constructor(name?: string) {
			this.name = name ?? "";
		}
	}

	export const PersonResolver: Resolver<Person> = async (values) => {
		const errors: FieldErrors<Person> = {};

		if ((values.description?.length ?? 0) > 1024) {
			errors.description = {
				type: 'maxLength',
				message: 'Description cannot exceed 1024 characters.'
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
		if ((values.title?.length ?? 0) > 50) {
			errors.title = {
				type: 'maxLength',
				message: 'Title cannot exceed 50 characters.'
			};
		}

		return {
			values,
			errors
		};
	};
}

export namespace TestAssembly.Enums {
	export const enum RegistrationStatus {
		Unknown = 0,
		Registered = 1,
		Unregistered = 2,
		ApprovalPending = 3
	}
}
