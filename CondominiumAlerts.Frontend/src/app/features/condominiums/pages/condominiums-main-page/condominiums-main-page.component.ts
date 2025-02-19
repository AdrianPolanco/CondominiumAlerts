import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CondominiumService } from '../../services/condominium.service';
import { NgFor, CommonModule } from '@angular/common';
import { Router } from '@angular/router';
@Component({
  selector: 'app-condominiums-main-page',
  imports: [NgFor,CommonModule ],
  templateUrl: './condominiums-main-page.component.html',
  styleUrl: './condominiums-main-page.component.css'
})
export class CondominiumsMainPageComponent {
 form: FormGroup
  constructor(private fb: FormBuilder, private CondominiumService: CondominiumService, private router: Router ){
    this.form = this.fb.group({
      condominiumCode: [''],
      UserId: [''],
    })
  }

  OnSubmit(){
    if(!this.form.invalid)
      return 
    this.CondominiumService.join(this.form.value)
  }
  condominiums = [
    { id: 1, name: 'Sunset Villas', location: 'Miami, FL' },
    { id: 2, name: 'Ocean Breeze Condos', location: 'Los Angeles, CA' }
  ];

  joinCondominium() {
    console.log('Joining a condominium...');
    // Implement logic here (modal, API call, etc.)
  }

  createCondominium() {
    console.log('Creating a new condominium...');
  
      this.router.navigate(['/condominium-create']); // Navigates to "condominium/{id}"
  }

  viewCondominium(id: number) {
    console.log(`Viewing condominium ID: ${id}`);
    // Implement navigation or API call
  }
 
}
