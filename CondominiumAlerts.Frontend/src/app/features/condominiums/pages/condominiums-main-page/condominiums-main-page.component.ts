import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators,ReactiveFormsModule  } from '@angular/forms';
import { CondominiumService } from '../../services/condominium.service';
import { NgFor, CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-condominiums-main-page',
  imports: [NgFor, CommonModule,ReactiveFormsModule ],
  templateUrl: './condominiums-main-page.component.html',
  styleUrls: ['./condominiums-main-page.component.css']
})
export class CondominiumsMainPageComponent {
  form: FormGroup;
  isModalOpen: boolean = false;

  constructor(private fb: FormBuilder, private condominiumService: CondominiumService, private router: Router) {
    this.form = this.fb.group({
      condominiumCode: ['', Validators.required],
      userId: ['08098098']
    });
  }

  condominiums = [
    { id: 1, name: 'Sunset Villas', location: 'Miami, FL' },
    { id: 2, name: 'Ocean Breeze Condos', location: 'Los Angeles, CA' }
  ];

  joinCondominium() {
    console.log('Joining a condominium...');

    if (this.form.invalid) {
      console.log('Form is invalid.');
      return;
    }
    const formData = this.form.value;
    
    this.condominiumService.join(formData).subscribe({
      next: (result) => {
        console.log('Joined successfully:', result);
      },
      error: (err) => {
        console.error('Error joining condominium:', err);
      }
    });
  }

  goToCreateCondominium() {
    console.log('Creating a new condominium...');
    this.router.navigate(['/condominium/create']);
  }

  viewCondominium(id: number) {
    console.log(`Viewing condominium ID: ${id}`);
  }

  changeModalState() {
    this.isModalOpen = !this.isModalOpen;
  }
}
