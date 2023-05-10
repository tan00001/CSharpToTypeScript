import { RegistrationStatus } from './CSharpToTypeScript.TestNamespace.Enums';


export const getStatusClassName = (isValidated: boolean | undefined, status: RegistrationStatus): string =>
	status !== RegistrationStatus.ApprovalPending ? status.toString() : (isValidated ? "approval-pending is-valid" : "approval-pending");
