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
