export const enum Gender {
	Unknown = 0,
	Male = 1,
	Female = 2
}

export interface IPerson {
	dateOfBirth?: Date;
	id?: number;
	name?: string;
}

export interface IPersonWithGender extends IPerson {
	gender?: Gender;
}
