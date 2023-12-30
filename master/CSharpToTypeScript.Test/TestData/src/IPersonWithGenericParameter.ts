export interface IPerson {
	dateOfBirth?: Date | string;
	id?: number;
	name?: string;
}

export interface IPersonWithGenericParameter<T> extends IPerson {
	tesProperty?: T;
}
