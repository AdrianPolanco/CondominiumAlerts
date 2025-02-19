import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CondominiumService } from '../../services/condominium.service';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core'; 
@Component({
  selector: 'app-condominium-page',
  imports: [ReactiveFormsModule],
  templateUrl: './condominium-page.component.html',
  styleUrl: './condominium-page.component.css',
  schemas:[ CUSTOM_ELEMENTS_SCHEMA]
})
export class CondominiumPageComponent {

  form: FormGroup;
    constructor(private fb: FormBuilder, private condominiumService: CondominiumService) {
    this.form = this.fb.group({
      name: [''],
      address: [''],
      imageFile: [null],
    });
  }

  onSubmit() {
    if (this.form.valid) {
        this.condominiumService.create(this.form.value);
    }
  }

  onFileSelect(event: any) {
    if (event.files.length > 0) {
      const file = event.files[0];
      this.form.patchValue({
        imageFile: file,
      });
    }
  }
}
