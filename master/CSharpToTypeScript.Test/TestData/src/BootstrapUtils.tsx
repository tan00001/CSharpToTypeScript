import { FieldError } from 'react-hook-form';

export const getClassName = (isValidated: boolean | undefined, error: FieldError | undefined): string =>
	error ? "form-control is-invalid" : (isValidated ? "form-control is-valid" : "form-control");

export const getCheckBoxClassName = (isValidated: boolean | undefined, error: FieldError | undefined): string =>
	error ? "form-check-input is-invalid" : (isValidated ? "form-check-input is-valid" : "form-check-input");

export const getErrorMessage = (error: FieldError | undefined) =>
	error && <span className="invalid-feedback">{error.message}</span>;
