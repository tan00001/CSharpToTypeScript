export interface IPerson {
	dateOfBirth?: Date;
	id?: number;
	name?: string;
}

export interface IPersonWithGenericParameter<T> extends IPerson {
	tesProperty?: T;
}
