export type PersonWithDictionaryMember = {
	id?: number;
	name: string;
	notes?: { [key: string]: string };
};
