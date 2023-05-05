export const enum Gender {
	Unknown = 0,
	Male = 1,
	Female = 2
}

export class PersonWithNullableName {
	age?: number | null;
	dateOfBirth?: number;
	id?: number;
	name?: string | null;
}

export class PersonWithNullableGender extends PersonWithNullableName {
	gender?: Gender | null;
}
