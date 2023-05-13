export namespace TestAssembly {
	export class CustomerAccount {
		balance: number | null;
		id?: number;

		constructor(balance?: number | null) {
			this.balance = balance ?? null;
		}
	}

	export class Person {
		description?: string;
		id?: number;
		name: string;
		title?: string | null;

		constructor(name?: string) {
			this.name = name ?? "";
		}
	}
}

export namespace TestAssembly.Enums {
	export const enum RegistrationStatus {
		Unknown = 0,
		Registered = 1,
		Unregistered = 2,
		ApprovalPending = 3
	}
}
