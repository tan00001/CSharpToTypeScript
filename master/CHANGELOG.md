# Change log
All notable changes to CSharpToTypeScript will be documented in this file.

## 1.0.0.9 (2023-05-13)

### Added

* Support `DataMemberAttribute`. When `DataMemberAttribute.EmitDefaultValue` is set to false, it is considered as equivalent to `RequiredAttribute.DisallowAllDefaultValues`.
* Support `DataMemberAttribute.Order` in addition to `DisplayAttribute.Order`. `DisplayAttribute.Order` takes precedence when present.
* `RangeAttribute.OperandType` is used when `DateTypeAttribute` is not set.
* Export Options dialog box can now only be resized horizontally, with the minimum width set to the startup width.
* Error messages in the Export Options dialog box are now set as tool tips.

## 1.0.0.8 (2023-05-12)

### Changed

* Generic types are supported. For generating React Hook Form resolvers and React + Bootstrap forms, generic types with specific type parameters are supported.
* React + Bootstrap Layout dialog box is now renamed as Export Options dialog box. When exporting generic types, this dialog box is used for specifying generic type parameters.

## 1.0.0.7 (2023-05-10)

### Changed

* Check if Date type values are indeed Date type in React Hook Form resolver for `Required` validation rule.
* Allow default values for enums, booleans, and numbers when check for `Required` validation rule. We will add support for `System.ComponentModel.DataAnnotations.RequiredAttribute.DisallowAllDefaultValues` when .NET 8 is released.
* Detect existing dependent scripts, and do not re-generate if not necessary.

### Fixed

* Remove control group and label for hidden controls.
* Updated generated checkbox script.

## 1.0.0.5 (2023-05-09)

### Changed

Generate `valueAsNumber = true` for enum types in React + Bootstrap forms because C# API server cannot deserialize the values as strings. Sending a '1' would cause JSON deserialization failure, for example. With `valueAsNumber = true`, a value of 1 will be sent instead:
```
<div className="form-group col-md-4">
	<label htmlFor={formId + "-status"}>Status:</label>
	<select className={getClassName(touchedFields.status, errors.status)} id={formId + "-status"} {...register("status", { valueAsNumber: true })}>
		<option value="">Select a Status</option>
		<option value="0">Unknown</option>
		<option value="1">Registered</option>
		<option value="2">Unregistered</option>
		<option value="3">Approval Pending</option>
	</select>
	{getErrorMessage(errors.status)}
</div>
```
