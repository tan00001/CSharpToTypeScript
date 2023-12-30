import { Resolver, FieldErrors, FieldError } from 'react-hook-form';

export namespace TestAssembly {
	export class CustomerAccount {
		static ValidateBalance(values: CustomerAccount): FieldError | undefined {
			// if (value is decimal balance)
			// {
			//     if (balance > 0)
			//     {
			//         return ValidationResult.Success;
			//     }
			//     return new ValidationResult("Balance cannot be less than 0.");
			// }
			// return new ValidationResult("Balance data type is incorrect.");
			// TODO: Please implement this function.
			return {
				type: "custom",
				message: "ValidateBalance is to be implemented"
			};
		}

		static ValidateBalance2(values: CustomerAccount): FieldError | undefined {
			if ((values.balance !== null && values.balance > 0) || values.id === 0)
			{
			    return undefined;
			}
			return { type: "custom", message: "Balance must be greater than 0 when Id is not 0." };
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
		const balanceError = CustomerAccount.ValidateBalance(values);
		if (balanceError) {
			errors.balance ??= balanceError;
		}
		const balanceError1 = CustomerAccount.ValidateBalance2(values);
		if (balanceError1) {
			errors.balance ??= balanceError1;
		}	

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
