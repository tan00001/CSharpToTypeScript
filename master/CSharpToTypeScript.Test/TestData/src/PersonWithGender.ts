export const enum Gender {
	Unknown = 0,
	Male = 1,
	Female = 2
}

export class Person {
	dateOfBirth?: Date;
	id?: number;
	name?: string;
}

export class PersonWithGender extends Person {
	gender?: Gender;
}
