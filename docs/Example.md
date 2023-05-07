# Example for Using a Generated .tsx File

Assume that you have the generated file [PersonWithValidationForm.tsx](https://github.com/tan00001/CSharpToTypeScript/blob/main/master/CSharpToTypeScript.Test/TestData/src/PersonWithValidationForm.tsx) in the same folder as the file below, and you are building a modal dialog box that posts the data for an instance of `PersonWithValidation` collected from the `PersonWithValidation` to the .NET 7 API server that implemented `PersonWithValidation`. The Controller name on the server is called `PersonsController`:

```
import { useState } from 'react';
import { Navigate } from 'react-router-dom';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { PersonWithValidation, PersonWithValidationForm, PersonWithValidationFormData } from './PersonWithValidation'

type MyDialogBoxData = {
    isOpen: boolean;
    setIsOpen: (flag: boolean) => void;
};

const MyDialogBox = (props: MyDialogBoxData) => {
    const [errorMessage, setErrorMessage] = useState("");
    const [completionUrl, setCompletionUrl] = useState('');
    const onSubmit = async (data: PersonWithValidation) => {
            const token = await authService.getAccessToken();
            const headers: Record<string, string> = {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            };
            if (token) {
                headers['Authorization'] = `Bearer ${token}`;
            }
            try {
                const response = await fetch('persons', {
                    method: 'POST',
                    headers,
                    body: JSON.stringify(data)
                });
                if (!response.ok) {
                    setErrorMessage(`Unable to save the new person. '${response.statusText}'`);
                }
                const responseData = await response.json();
                if (responseData && responseData.id) {
                    setCompletionUrl('/PersonDetails?id=' + responseData.id');
                } else {
                    setErrorMessage('Unable to save the new record.');
                }
            } catch (error) {
                setErrorMessage(`Unable to save the new record. '${error}'`);
            }
        };

    const closeDlg = () => props.setIsOpen(false);

    return (completionUrl ? <Navigate to={completionUrl} /> :
        <Modal
            isOpen={props.isOpen}
            toggle={closeDlg}>
            <ModalHeader>Enter a New Person</ModalHeader>
            <ModalBody>
                {errorMessage && <div className="row">
                    <div className="col-md-12 invalid-feedback">{errorMessage}</div>
                </div>}
                <PersonWithValidationForm onSubmit={onSubmit} />
            </ModalBody>
        </Modal>);
}

export default MyDialogBox;
```