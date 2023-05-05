import { PersonWithNullableName as impPersonWithNullableName, Gender as impGender } from './CSharpToTypeScript.Test';
import { RegistrationStatus as impRegistrationStatus } from './CSharpToTypeScript.TestNamespace.Enums';

export class PersonWithNamespace extends impPersonWithNullableName {
	gender?: impGender | null;
	status?: impRegistrationStatus | null;
}
