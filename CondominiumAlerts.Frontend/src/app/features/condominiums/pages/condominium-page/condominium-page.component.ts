
import { Component, signal, viewChild } from '@angular/core';
import { FormComponent } from '../../../../shared/components/form/form.component';
import { SharedFormField } from '../../../../shared/components/form/shared-form-field.interface';
import { FormGroup, Validators } from '@angular/forms';
import { SharedForm } from '../../../../shared/components/form/shared-form.interface';
import { CondominiumService } from '../../services/condominium.service';
import { AddCondominiumCommand } from '../../models/condominium.model';
import { Router } from '@angular/router';
import { Feedback } from '../../../../shared/components/form/feedback.interface';

@Component({
    selector: 'app-condominium-page',
    templateUrl: './condominium-page.component.html',
    styleUrls: ['./condominium-page.component.css'],
    imports: [FormComponent]
})
export class CondominiumPageComponent {

    constructor(private condominiumService: CondominiumService, private router: Router) {}

    // Signal for the form group
    private readonly formGroup = signal<FormGroup>(new FormGroup({}));

    // Reference to the form component
    formComponent = viewChild(FormComponent);

    // Define form fields using signal
    condominiumFormFields = signal<SharedFormField[]>([
        {
            name: 'name',
            label: 'Name',
            type: 'text',
            validators: [Validators.required],
            errorMessages: {
                required: 'Name is required'
            }
        },
        {
            name: 'address',
            label: 'Address',
            type: 'text',
            validators: [Validators.required],
            errorMessages: {
                required: 'Address is required'
            }
        },
        {
            name: 'imageFile',
            label: 'Upload Image',
            type: 'file',
            filetype: 'image/*',
            onFileSelect: (event: any) => {
                if (event.files.length > 0) {
                    const file = event.files[0];
                    this.formGroup().patchValue({
                        imageFile: file,
                    });
                }
            }
        }
    ]);

    condominiumFormSettings = signal<SharedForm>({
        fields: this.condominiumFormFields(),
        baseButtonLabel: 'Submit',
        submittedButtonLabel: 'Submitted Successfully'
    });

    onFormCreated(form: FormGroup) {
        this.formGroup.set(form);
    }
    goToMainPage(){
      this.router.navigate(["condominiums/main-page"])
    }

    onSubmit(value: AddCondominiumCommand) {
        const formComponent = this.formComponent();
        this.condominiumService.create(value).subscribe({
            next: (response) => {
                formComponent?.resetForm({
                    status: 'success',
                    message: 'Condominium created successfully!',
                });
            },
            error: (err) => {
                formComponent?.resetForm({
                    status: 'error',
                    message: err.error?.message || 'An error occurred while creating the condominium.',
                });
            }
        });
    }
}
