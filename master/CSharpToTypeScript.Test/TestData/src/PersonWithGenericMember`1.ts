export class PersonWithGenericMember<T> {
	id?: number;
	name: string;
	testMember?: T;

	constructor(name?: string) {
		this.name = name ?? "";
	}
}
