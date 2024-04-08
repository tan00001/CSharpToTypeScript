export type DataRecord = {
	id?: number;
	name?: string | null;
};
export class PersonWithDataRecords {
	dataRecords?: DataRecord[];
	description?: string;
	id?: number;
	name: string;

	constructor(name?: string) {
		this.name = name ?? "";
	}
}
