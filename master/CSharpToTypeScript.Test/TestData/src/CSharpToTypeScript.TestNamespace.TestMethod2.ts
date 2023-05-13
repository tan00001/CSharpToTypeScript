import { Gender } from './CSharpToTypeScript.Test.TsClass';
import { RegistrationStatus } from './CSharpToTypeScript.TestNamespace.Enums';

export const getGenderStatusClassName = (isValidated: boolean | undefined, status: RegistrationStatus, gender: Gender): string =>
	gender !== Gender.Unknown && status !== RegistrationStatus.ApprovalPending ? status.toString() : (isValidated ? "approval-pending is-valid" : "approval-pending");

export const getStatusClassName = (isValidated: boolean | undefined, status: RegistrationStatus): string =>
	status !== RegistrationStatus.ApprovalPending ? status.toString() : (isValidated ? "approval-pending is-valid" : "approval-pending");
