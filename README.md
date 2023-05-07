# CSharpToTypeScript

## Table of Contents
1. [What is CSharpToTypeScript](#What-is-CSharpToTypeScript)
2. [Prerequisites](#Prerequisites)
3. [Installation](#Installation)
4. [How it works](#How-it-works)
5. [Acknowledgements](#Acknowledgements)
4. [License](#License)

## What is CSharpToTypeScript
CSharpToTypeScript generates Typescript code, React-Hook-Form Resolver code, and Bootstrap styled React form code (.tsx file) from a C# class definition. It features
1. Support for C# nullable types.
2. Support for C# enums, interfaces, and classes with inheritance hierarchies from multiple namespaces.
3. Utilization of C# attributes in TypeScript data validation, variable names, label text, error messages, displayed texts for enumerated values, etc.
4. Customizable column count in generated TypeScript XML forms.

Start with the C# class

```
using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Test
{
    public class PersonWithValidation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(20, 120)]
        public Int32? Age { get; set; }

        [StringLength(120, MinimumLength = 2)]
        public string? Location { get; set; }
    }
}
```

this tool can generate the following TypeScript:
```
import { useForm, SubmitHandler, FieldError, Resolver, FieldErrors } from 'react-hook-form';

const getClassName = (isValidated: boolean | undefined, error: FieldError | undefined): string =>
	error ? "form-control is-invalid" : (isValidated ? "form-control is-valid" : "form-control");

const getErrorMessage = (error: FieldError | undefined) =>
	error && <span className="invalid-feedback">{error.message}</span>;

export class PersonWithValidation {
	age?: number | null;
	id?: number;
	location?: string | null;
	name: string;

	constructor(name?: string) {
		this.name = name ?? "";
	}
}

export const PersonWithValidationResolver: Resolver<PersonWithValidation> = async (values) => {
	const errors: FieldErrors<PersonWithValidation> = {};

	if (values.age) {
		if (values.age > 120) {
			errors.age = {
				type: 'max',
				message: 'Age cannot exceed 120.'
			};
		}
		if (values.age < 20) {
			errors.age = {
				type: 'min',
				message: 'Age cannot be less than 20.'
			};
		}
	}
	if ((values.location?.length ?? 0) > 120) {
		errors.location = {
			type: 'maxLength',
			message: 'Location cannot exceed 120 characters.'
		};
	}
	if ((values.location?.length ?? 0) < 2) {
		errors.location = {
			type: 'minLength',
			message: 'Location cannot be less than 2 characters long.'
		};
	}
	if (!values.name) {
		errors.name = {
			type: 'required',
			message: 'Name is required.'
		};
	}
	if ((values.name?.length ?? 0) > 50) {
		errors.name = {
			type: 'maxLength',
			message: 'Name cannot exceed 50 characters.'
		};
	}

	return {
		values,
		errors
	};
};

export type PersonWithValidationFormData = {
	personWithValidation?: PersonWithValidation,
	onSubmit: SubmitHandler<PersonWithValidation>
};

export const PersonWithValidationForm = (props: PersonWithValidationFormData) => {
	const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<PersonWithValidation>({
		resolver: PersonWithValidationResolver,
		defaultValues: props.personWithValidation ?? new PersonWithValidation()
	});

	return <form onSubmit={handleSubmit(props.onSubmit)}>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="age">Age:</label>
				<input type="number" className={getClassName(touchedFields.age, errors.age)} id="age" {...register("age")} />
				{getErrorMessage(errors.age)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="id">Id:</label>
				<input type="number" className={getClassName(touchedFields.id, errors.id)} id="id" {...register("id")} />
				{getErrorMessage(errors.id)}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="location">Location:</label>
				<input type="text" className={getClassName(touchedFields.location, errors.location)} id="location" {...register("location")} />
				{getErrorMessage(errors.location)}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="name">Name:</label>
				<input type="text" className={getClassName(touchedFields.name, errors.name)} id="name" {...register("name")} />
				{getErrorMessage(errors.name)}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-12">
				<button className="btn btn-primary" type="submit" disabled={isSubmitting}>Submit</button>
				<button className="btn btn-secondary" type="reset" disabled={isSubmitting}>Reset</button>
			</div>
		</div>
	</form>;
};
```

This tool can be launched from the command line, or from Visual Studio via its popup menu in C# source files:

![alt text](https://github.com/tan00001/CSharpToTypeScript/blob/main/VSMenuScreenShot.png)

## Prerequisites
1. **.NET Framework 4.8.1**. This is because Visual Studio Extension projects do not support .NET Core versions such as .NET 5, .NET 6, and .NET 7.
2. **.NET 7**. In order to support all the latest C# syntax, we use .NET 7 for processing the C# type definitions.
3. **Visual Studio 2022**. .NET 7 is only supported in Visual Studio 2022 and later.

## Installation
For now, you will need to build everything from the source code in Visual Studio 2022. Although the CSharpToTypeScript.vsix project has a "net7" folder that contains some binary files, they are auto generated from the CSharpToTypeScript project, and can be safely delete.
Once you have rebuilt everything locally, you can install from the file "CSharpToTypeScript.vsix\bin\Release\CSharpToTypeScript.vsix" that you have built.

## How it works
CSharpToTypeScript.vsix adds three menu items on the context menu for C# source files. When you right click your mouse in a C# source file, CSharpToTypeScript will detect the class definition you are in, and then generate TypeScript code for that class and all of its dependencies. It will utilize the following C# attributes:
1. From `System.ComponentModel.DataAnnotations`, CSharpToTypeScript supports `Display`, `Required`, `StringLength`, `Range`, `RegularExpression`, `EmailAddress`, `Phone`, `Compare`, `CreditCard`, and `Url`.
2. From `System.Text.Json.Serialization`, CSharpToTypeScript supports `JsonIgnore`. Because the C# class is expected to be serialized/de-serialized and sent to/received from React client as JSON objects, marking a member of the C# class with `JsonIgnore` will make the member being ingored by CSharpToTypeScript.
3. From `System.Runtime.Serialization`, CSharpToTypeScript supports `DataContract` for customizing namespaces. By default, namespaces are not used. You will have to manually turn it on by setting `<CSharpToTypeScriptEnableNamespace>true</CSharpToTypeScriptEnableNamespace>` in your C# project file.
4. All dependencies of the C# class from the same namespace will be saved in a TypeScript file. You will be prompted to enter a file path with Window's "Save as" dialog box. Once a TypeScript file is saved, the folder path will be stored in the C# project file itself as `<CSharpToTypeScriptOutputFolder>...</CSharpToTypeScriptOutputFolder>`. For this project, you will be prompted to save TypeScript files in the same folder subsequently.
5. All dependencies of the C# class from other namespaces will be saved in their separate files, respectively. For example, if your C# class references `enum` definitions in a namepsace called `Data.Common`, a file `Data.Common.ts` will be generated in the same folder, and the TypeScript file containing the class definition will import the `enum` definitions from `Data.Common`. If different C# classes depend on different types in a given namespace, you will need to merge them manually. If the file is checked into git, the merging can be done in git, for example.
6. The generated TypeScript files for react-hook-form resolvers will import components from "react-hook-form". The generated TypeScript XML files for react forms will import components from "react-hook-form" as well. There are no other external imports right now.

## Acknowledgements
This project started from TypeLITE 1.8.2.0 source code, with the assumption that the license statement at http://type.litesolutions.net/license grants permission to develop open source code project such as this one.

I would like to express my gratitude to Mr. Lukas Kabrt for his inspiring TypeLITE project, which is the basis of this open source project. I greatly appreciate his dedication to creating valuable open source tools and encourage everyone to explore TypeLITE at http://type.litesolutions.net/.

## License
CSharpToTypeScript is under the <a href="https://opensource.org/license/mit/">MIT License</a>.
