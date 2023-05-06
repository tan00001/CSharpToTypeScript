# CSharpToTypeScript
Generate Typescript code, React-Hook-Form Resolver code, and Bootstrap styled React form code (.tsx file) from a C# class definition. This project started from TypeLITE 1.8.2.0 source code, with the assumption that the license statement at http://type.litesolutions.net/license grants permission to develop open source code project such as this one.

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
import { useState } from 'react';
import { useForm, SubmitHandler, Resolver, FieldErrors } from 'react-hook-form';

export class PersonWithValidation {
	age?: number | null;
	id?: number;
	location?: string | null;
	name: string;

	constructor(name: string) {
		this.name = name;
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

export const PersonWithValidationFormBase = (props: PersonWithValidation) => {
	const { register, handleSubmit, formState: { errors } } = useForm<PersonWithValidation>({
		resolver: PersonWithValidationResolver,
		defaultValues: props
	});

	const onSubmit: SubmitHandler<PersonWithValidation> = async (data: PersonWithValidation) => {
		// TODO: fill in submit action details
	};

	return <form onSubmit={handleSubmit(onSubmit)}>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="age">Age:</label>
				<input type="number" className="form-control" id="age" {...register("age")} />
				{errors.age && <span className="invalid-feedback">{errors.age.message}</span>}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="id">Id:</label>
				<input type="number" className="form-control" id="id" {...register("id")} />
				{errors.id && <span className="invalid-feedback">{errors.id.message}</span>}
			</div>
			<div className="form-group col-md-4">
				<label htmlFor="location">Location:</label>
				<input type="text" className="form-control" id="location" {...register("location")} />
				{errors.location && <span className="invalid-feedback">{errors.location.message}</span>}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-4">
				<label htmlFor="name">Name:</label>
				<input type="text" className="form-control" id="name" {...register("name")} />
				{errors.name && <span className="invalid-feedback">{errors.name.message}</span>}
			</div>
		</div>
		<div className="row">
			<div className="form-group col-md-12">
				<button className="btn btn-primary" type="submit">Submit</button>
				<button className="btn btn-secondary" type="reset">Reset</button>
			</div>
		</div>
	</form>;
};

```

This tool can be launched from the command line, or from Visual Studio via its ppopup menu in C# source files:

![alt text](https://github.com/tan00001/CSharpToTypeScript/blob/main/VSMenuScreenShot.png)

I would like to express my gratitude to Mr. Lukas Kabrt for his inspiring TypeLITE project, which is the basis of this open source project. I greatly appreciate his dedication to creating valuable open source tools and encourage everyone to explore TypeLITE at http://type.litesolutions.net/.

CSharpToTypeScript is under the <a href="https://opensource.org/license/mit/">MIT License</a>.