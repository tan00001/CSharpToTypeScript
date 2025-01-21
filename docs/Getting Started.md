# Getting Started

* You can install CSharpToTypeScript by going to the "Extensions" menu on the Visual Studio menu bar: ![Download CSharpToTypeScript in Visual Studio](https://github.com/tan00001/CSharpToTypeScript/blob/main/docs/DownloadScreenshot.png)

  Alternatively, CSharpToTypeScript can be downloaded from [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=ProgramountInc.CSharpToTypeScript).

* Before installing CSharpToTypeScript, please make sure that you have **.NET Framework 4.8.1** and **.NET 7 or later** installed.
	* To check if you have **.NET Framework 4.8.1** installed, please go to the "View" menu on the Visual Studio menu bar, and select "Terminal". In the terminal window, enter
	  `reg query "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"`. 
	  The returned result should contain `Release    REG_DWORD    0x82348` if you are on Windows 11, and should contain `Release    REG_DWORD    0x80ff4` if you are on Windows 10. If you do not have .NET Framework 4.8.1, you can download it from Microsoft at [https://dotnet.microsoft.com/en-us/download/dotnet-framework/net481](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net481). 
	* To check if you have **.NET 7 or later** installed, please also go to the "View" menu on the Visual Studio menu bar, and select "Terminal". In the terminal window, enter 
	  `dotnet --list-runtimes`
	  The returned result should contain `Microsoft.NETCore.App 7.0.5` and `Microsoft.WindowsDesktop.App 7.0.5`. or later. If you do not have .NET 9 installed, you can download it from Microsoft at [https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-9.0.1-windows-x64-installer](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-9.0.1-windows-x64-installer)
* To use the generated TypeScript with React, please make sure that the version of React that you are using is version 18.0 or newer.
* Exit Visual Studio 2022 to install CSharpToTypeScript. If you downloaded CSharpToTypeScript from the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=ProgramountInc.CSharpToTypeScript), open the downloaded CSharpToTypeScript.vsix file. You will be prompted to start installation:
  ![Install CSharpToTypeScript](https://github.com/tan00001/CSharpToTypeScript/blob/main/docs/InstallScreenshot.png)
  
  Often, you will see the install waiting for other processes to end, such as `node.js`:
  ![Wait for node.js to exist](https://github.com/tan00001/CSharpToTypeScript/blob/main/docs/NodeJsScreenshot.png)
  
  You can either terminate such process from Windows Task Manager, or you can restart your computer and then proceed.
* Wait for the installer to complete.
  ![Complete installing CSharpToTypeScript](https://github.com/tan00001/CSharpToTypeScript/blob/main/docs/InstallCompleteScreenshot.png)
* Start up Visual Studio 2022, and open a C# project for a web API. Build the project first. Because CSharpToTypeScript reads the DLL output from the project to generate the TypeScript code, if the project dos not build, CSharpToTypeScript will not be able to proceed.
* Open a C# source file inside this project that defines a class that is used to send data to and receive data from a web client. For example:
	```
	using System.ComponentModel.DataAnnotations;
	using System.Runtime.Serialization;

	[DataContract(Name = "Person")]
	public class PersonPlusAddress
	{
		[Required]
		[StringLength(50)]
		[Display(Name = "First Name", Order = 1)]
		[UIHint("", "HTML", new object[] { "bsColSpan", "4" })]
		public string? FirstName { get; set; }

		[Required]
		[StringLength(50)]
		[Display(Name = "Last Name", Order = 2)]
		[UIHint("", "HTML", new object[] { "bsColSpan", "4" })]
		public string? LastName { get; set; }

		[Required]
		[StringLength(255)]
		[Display(Name = "Street Address", Order = 3)]
		[UIHint("", "HTML", new object[] { "bsColSpan", "4" })]
		public string? StreetAddress { get; set; }

		[Required]
		[StringLength(50)]
		[Display(Order = 4)]
		[UIHint("", "HTML", new object[] { "bsColSpan", "2" })]
		public string? City { get; set; }

		[Required]
		[StringLength(50)]
		[Display(Order = 5)]
		[UIHint("", "HTML", new object[] { "bsColSpan", "1" })]
		public string? State { get; set; }

		[Required]
		[StringLength(50)]
		[Display(Name = "ZIP", Order = 6)]
		[UIHint("", "HTML", new object[] { "bsColSpan", "1" })]
		public string? Zip { get; set; }
	}
	```
	Here `bsColSpan` is translated into Bootstrap column span directly. If you would like to specify the column span with column count defined in the CSharpToTypeScript UI, you can specify `colSpan` instead.
    Right click inside this class definition with your mouse. You must click inside the class definition because CSharpToTypeScript uses this location to identify the class to export.

* There are three menu items for CSharpToTypeScript on the popup menu: "Generate TypeScript...", "Generate React Hook Form Resolver...", "Generate React+Bootstrap Form...", "Generate Vuelidate Rules...", and "Generate Vue+Bootstrap Form...". 
    ![Poup menu](https://github.com/tan00001/CSharpToTypeScript/blob/main/VSMenuScreenShot.png)

    * "Generate TypeScript..." will bring up Windows "Save as" dialog box, and prompt you to save the gnerated TypeScript file.
	  ![Save TypeScript file as](https://github.com/tan00001/CSharpToTypeScript/blob/main/docs/SaveAsScreenshot.png)

	  The content of the file is as follows:
		```
		export class Person {
			city: string | null;
			firstName: string | null;
			lastName: string | null;
			state: string | null;
			streetAddress: string | null;
			zip: string | null;

			constructor(city?: string | null, firstName?: string | null, lastName?: string | null, state?: string | null, streetAddress?: string | null, zip?: string | null) {
				this.city = city ?? null;
				this.firstName = firstName ?? null;
				this.lastName = lastName ?? null;
				this.state = state ?? null;
				this.streetAddress = streetAddress ?? null;
				this.zip = zip ?? null;
			}
		}
		```
		Because each property has a `Required` attribute, a constructor is defined to initialize the required members.
		* **When there are references to code in other namespaces, additional files will be saved in the same folder. The file names will be the corresponding namespaces. Please make sure that you do not have files in this folder that cannot be overwritten. Your TypeScript will import from these files.**
		* **CSharpToTypeScript will try to save the folder path into the project file if you are exporting from this project for the first time, or if you have chosen a different folder path when saving the TypeScript file output.**
		
		  ![Output folder setting](https://github.com/tan00001/CSharpToTypeScript/blob/main/OutputFolderSettingScreenshot.png)

		  **Please edit your project file manually and add this path if CSharpToTypeScript is not permitted to update the project file directly.**
 
    * "Generate React Hook Form Resolver..." will also bring up Windows "Save as" dialog box, and prompt you to save the gnerated TypeScript file. The difference is that in addition to saving the TypeScript for the class definition, it will also include a definition for a resolver function that is used for data validation in a React Hook form.
		```
		export const PersonResolver: Resolver<Person> = async (values) => {
			const errors: FieldErrors<Person> = {};

			if (!values.city) {
				errors.city = {
					type: 'required',
					message: 'City is required.'
				};
			}
			if ((values.city?.length ?? 0) > 50) {
				errors.city = {
					type: 'maxLength',
					message: 'City cannot exceed 50 characters.'
				};
			}
			if (!values.firstName) {
				errors.firstName = {
					type: 'required',
					message: 'First Name is required.'
				};
			}
			if ((values.firstName?.length ?? 0) > 50) {
				errors.firstName = {
					type: 'maxLength',
					message: 'First Name cannot exceed 50 characters.'
				};
			}
			if (!values.lastName) {
				errors.lastName = {
					type: 'required',
					message: 'Last Name is required.'
				};
			}
			if ((values.lastName?.length ?? 0) > 50) {
				errors.lastName = {
					type: 'maxLength',
					message: 'Last Name cannot exceed 50 characters.'
				};
			}
			if (!values.state) {
				errors.state = {
					type: 'required',
					message: 'State is required.'
				};
			}
			if ((values.state?.length ?? 0) > 50) {
				errors.state = {
					type: 'maxLength',
					message: 'State cannot exceed 50 characters.'
				};
			}
			if (!values.streetAddress) {
				errors.streetAddress = {
					type: 'required',
					message: 'Street Address is required.'
				};
			}
			if ((values.streetAddress?.length ?? 0) > 255) {
				errors.streetAddress = {
					type: 'maxLength',
					message: 'Street Address cannot exceed 255 characters.'
				};
			}
			if (!values.zip) {
				errors.zip = {
					type: 'required',
					message: 'ZIP is required.'
				};
			}
			if ((values.zip?.length ?? 0) > 50) {
				errors.zip = {
					type: 'maxLength',
					message: 'ZIP cannot exceed 50 characters.'
				};
			}

			return {
				values,
				errors
			};
		};
		```
	* "Generate React+Bootstrap Form..." will first bring up a dialog box to prompt you for the number of columns in the grid of the form to be created: 

	  ![Set column count](https://github.com/tan00001/CSharpToTypeScript/blob/main/docs/ColumnCountDlgScreenshot.png)

	  For the example above, we would enter 4 for the column count. `FirstName`, `LastName`, and `StreeAddress` each spans 4 columns, so each of them will occupy an entire row. On the fourth row, `City` occupies the first two columns, whereas `State` and `ZIP` would each occupy a single column.
	  Once you have entered a column count, CSharpToTypeScript will bring up Windows "Save as" dialog box, and prompt you to save the gnerated TypeScript file.
	  ```
	  export type PersonFormData = {
			person?: Person,
			onSubmit: SubmitHandler<Person>
		};

	  export const PersonForm = (props: PersonFormData) => {
		 const formId = useId();
		 const { register, handleSubmit, formState: { errors, touchedFields, isSubmitting } } = useForm<Person>({
			mode: "onTouched",
			resolver: PersonResolver,
			defaultValues: props.person ?? new Person()
		 });

		return <form onSubmit={handleSubmit(props.onSubmit)}>
			<div className="row mb-3">
				<div className="form-group col-md-12">
					<label htmlFor={formId + "-firstName"}>First Name:</label>
					<input type="text" className={getClassName(touchedFields.firstName, errors.firstName)} id={formId + "-firstName"} {...register("firstName")} />
					{getErrorMessage(errors.firstName)}
				</div>
			</div>
			<div className="row mb-3">
				<div className="form-group col-md-12">
					<label htmlFor={formId + "-lastName"}>Last Name:</label>
					<input type="text" className={getClassName(touchedFields.lastName, errors.lastName)} id={formId + "-lastName"} {...register("lastName")} />
					{getErrorMessage(errors.lastName)}
				</div>
			</div>
			<div className="row mb-3">
				<div className="form-group col-md-12">
					<label htmlFor={formId + "-streetAddress"}>Street Address:</label>
					<input type="text" className={getClassName(touchedFields.streetAddress, errors.streetAddress)} id={formId + "-streetAddress"} {...register("streetAddress")} />
					{getErrorMessage(errors.streetAddress)}
				</div>
			</div>
			<div className="row mb-3">
				<div className="form-group col-md-6">
					<label htmlFor={formId + "-city"}>City:</label>
					<input type="text" className={getClassName(touchedFields.city, errors.city)} id={formId + "-city"} {...register("city")} />
					{getErrorMessage(errors.city)}
				</div>
				<div className="form-group col-md-3">
					<label htmlFor={formId + "-state"}>State:</label>
					<input type="text" className={getClassName(touchedFields.state, errors.state)} id={formId + "-state"} {...register("state")} />
					{getErrorMessage(errors.state)}
				</div>
				<div className="form-group col-md-3">
					<label htmlFor={formId + "-zip"}>ZIP:</label>
					<input type="text" className={getClassName(touchedFields.zip, errors.zip)} id={formId + "-zip"} {...register("zip")} />
					{getErrorMessage(errors.zip)}
				</div>
			</div>
			<div className="row">
				<div className="form-group col-md-12">
					<button className="btn btn-primary" type="submit" disabled={isSubmitting}>Submit</button>
					<button className="btn btn-secondary mx-1" type="reset" disabled={isSubmitting}>Reset</button>
				</div>
			</div>
		</form>;
	  };
	  ```
	  * "Generate Vuelidate Rules..." will also bring up Windows "Save as" dialog box, and prompt you to save the gnerated TypeScript file. The difference is that in addition to saving the TypeScript for the class definition, it will also include a definition for a Vuelidate rule that is used for data validation in a Vue form.
		```
		import { useVuelidate, type ValidationArgs } from '@vuelidate/core';
		import { required, email, minValue, maxValue, minLength, maxLength, helpers } from '@vuelidate/validators';

		export class Person {
			city: string | null;
			firstName: string | null;
			lastName: string | null;
			state: string | null;
			streetAddress: string | null;
			zip: string | null;

			constructor(city?: string | null, firstName?: string | null, lastName?: string | null, state?: string | null, streetAddress?: string | null, zip?: string | null) {
				this.city = city ?? null;
				this.firstName = firstName ?? null;
				this.lastName = lastName ?? null;
				this.state = state ?? null;
				this.streetAddress = streetAddress ?? null;
				this.zip = zip ?? null;
			}

			static ValidateZip(value: string, values : Person): boolean {
				// {
				//     if (value is string zip)
				//     {
				//         if (zip.Length != 5)
				//         {
				//             return ValidationResult.Success;
				//         }
				//         return new ValidationResult("Balance cannot be less than 0.");
				//     }
				//     return new ValidationResult("ZIP data type is incorrect.");
				// }
				// TODO: Please implement this function.
				return true;
			}

			static ValidateZip2(value: string, values : Person): boolean {
				if (values.zip?.length === 5 || (values.firstName === null && values.lastName === null))
				{
					return true;
				}
				return false;
			}

		}

		export const PersonValidationRules: ValidationArgs<Person> = {
			city: {
				required, 
				maxLength: maxLength(50)
			},
			firstName: {
				required, 
				maxLength: maxLength(50)
			},
			lastName: {
				required, 
				maxLength: maxLength(50)
			},
			state: {
				required, 
				maxLength: maxLength(50)
			},
			streetAddress: {
				required, 
				maxLength: maxLength(255)
			},
			zip: {
				required, 
				maxLength: maxLength(50), 
				ValidateZip: helpers.withMessage('', (value, siblings) => Person.ValidateZip(value, siblings)), 
				ValidateZip2: helpers.withMessage('', (value, siblings) => Person.ValidateZip2(value, siblings))
			}
		};
		```
	* "Generate Vue+Bootstrap Form..." will first bring up a dialog box to prompt you for the number of columns in the grid of the form to be created: 

	  ![Set column count](https://github.com/tan00001/CSharpToTypeScript/blob/main/docs/ColumnCountDlgScreenshot.png)

	  For the example above, we would enter 4 for the column count. `FirstName`, `LastName`, and `StreeAddress` each spans 4 columns, so each of them will occupy an entire row. On the fourth row, `City` occupies the first two columns, whereas `State` and `ZIP` would each occupy a single column.
	  Once you have entered a column count, CSharpToTypeScript will bring up Windows "Save as" dialog box, and prompt you to save the gnerated Vue file and the Vuelidate rules TypeScript file.
	  ```
	  <script setup lang="ts">
			import { ref, type PropType } from 'vue';
			import { type Person, PersonRules } from './Person'
			import { useVuelidate, type ValidationArgs } from '@vuelidate/core';

			const inputData = defineModel<Person>('Person');

			const props = defineProps({
				formId: {
					type: String,
					default: 'Person'
				});
			});

			const formData = ref<Person>(inputData ?? new Person())
			const submitting = ref<boolean>(false);
			const PersonForm = ref<HTMLFormElement | null>(null);
			const v$ = useVuelidate<Person>(PersonRules, formData.value);

			const getClassName = (dirty: boolean | undefined, hasError: boolean | undefined): string => hasError ? 'form-control is-invalid' : (dirty ? 'form-control is-valid' : 'form-control');
			const getCheckBoxClassName = (dirty: boolean | undefined, hasError: boolean | undefined): string => hasError ? 'form-check-input is-invalid' : (dirty ? 'form-check-input is-valid' : 'form-check-input');

			const onSubmit = async () => {
				submitting.value = true;

				v$.value.$touch();
				if (v$.value.$error) {
					console.log('Form is invalid');
					return;
				}
				submitting.value = false;
			};
		</script>

		<template>
			<form @submit.prevent="onSubmit" ref="PersonForm" class="needs-validation">
				<div className="row mb-3">
					<div class="form-group col-md-12">
						<label :for="formId + '-firstName'">First Name:</label>
						<input type="text" :class="getClassName(v$.firstName.$dirty, v$.firstName.$error)" :id="formId + '-firstName'" @blur="v$.firstName.$touch"/>
						<div v-if='v$.firstName.$dirty && v$.firstName.$error' class='form-error'>
							<span v-for='rule in Object.keys($v.firstName)' :key='rule'>{{$v.firstName[rule].$message}}</span>
						</div>
					</div>
				</div>
				<div className="row mb-3">
					<div class="form-group col-md-12">
						<label :for="formId + '-lastName'">Last Name:</label>
						<input type="text" :class="getClassName(v$.lastName.$dirty, v$.lastName.$error)" :id="formId + '-lastName'" @blur="v$.lastName.$touch"/>
						<div v-if='v$.lastName.$dirty && v$.lastName.$error' class='form-error'>
							<span v-for='rule in Object.keys($v.lastName)' :key='rule'>{{$v.lastName[rule].$message}}</span>
						</div>
					</div>
				</div>
				<div className="row mb-3">
					<div class="form-group col-md-12">
						<label :for="formId + '-streetAddress'">Street Address:</label>
						<input type="text" :class="getClassName(v$.streetAddress.$dirty, v$.streetAddress.$error)" :id="formId + '-streetAddress'" @blur="v$.streetAddress.$touch"/>
						<div v-if='v$.streetAddress.$dirty && v$.streetAddress.$error' class='form-error'>
							<span v-for='rule in Object.keys($v.streetAddress)' :key='rule'>{{$v.streetAddress[rule].$message}}</span>
						</div>
					</div>
				</div>
				<div className="row mb-3">
					<div class="form-group col-md-6">
						<label :for="formId + '-city'">City:</label>
						<input type="text" :class="getClassName(v$.city.$dirty, v$.city.$error)" :id="formId + '-city'" @blur="v$.city.$touch"/>
						<div v-if='v$.city.$dirty && v$.city.$error' class='form-error'>
							<span v-for='rule in Object.keys($v.city)' :key='rule'>{{$v.city[rule].$message}}</span>
						</div>
					</div>
					<div class="form-group col-md-3">
						<label :for="formId + '-state'">State:</label>
						<input type="text" :class="getClassName(v$.state.$dirty, v$.state.$error)" :id="formId + '-state'" @blur="v$.state.$touch"/>
						<div v-if='v$.state.$dirty && v$.state.$error' class='form-error'>
							<span v-for='rule in Object.keys($v.state)' :key='rule'>{{$v.state[rule].$message}}</span>
						</div>
					</div>
					<div class="form-group col-md-3">
						<label :for="formId + '-zip'">ZIP:</label>
						<input type="text" :class="getClassName(v$.zip.$dirty, v$.zip.$error)" :id="formId + '-zip'" @blur="v$.zip.$touch"/>
						<div v-if='v$.zip.$dirty && v$.zip.$error' class='form-error'>
							<span v-for='rule in Object.keys($v.zip)' :key='rule'>{{$v.zip[rule].$message}}</span>
						</div>
					</div>
				</div>
				<div className="row">
					<div className="form-group col-md-12">
						<button className="btn btn-primary" type="submit" :disabled="submitting">Submit</button>
						<button className="btn btn-secondary mx-1" type="reset" :disabled="submitting">Reset</button>
					</div>
				</div>
			</form>
		</template>
	  ```
	  Please note that the `UIHint` attributes are optional. Without these `UIHint` attributes, each property will just occupy a single column in a grid of four columns. The number of rows depend on the number of properties in the class.
