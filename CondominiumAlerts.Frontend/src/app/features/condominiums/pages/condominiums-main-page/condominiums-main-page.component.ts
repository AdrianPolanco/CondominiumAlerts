import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators,ReactiveFormsModule  } from '@angular/forms';
import { CondominiumService } from '../../services/condominium.service';
import { NgFor, CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import {Button} from 'primeng/button';
import {Toolbar} from 'primeng/toolbar';
import {NgOptimizedImage} from '@angular/common';
import { AuthService } from '../../../../core/auth/services/auth.service';
@Component({
  selector: 'app-condominiums-main-page',
  imports: [NgFor, CommonModule,ReactiveFormsModule,Button,Toolbar,NgOptimizedImage],
  templateUrl: './condominiums-main-page.component.html',
  styleUrls: ['./condominiums-main-page.component.css']
})
export class CondominiumsMainPageComponent {
  form: FormGroup;
  isModalOpen: boolean = false;
  errorText:string = "";
  constructor(private fb: FormBuilder, private condominiumService: CondominiumService, private router: Router, private authService: AuthService) {
 // console.log(this.authService.currentUser?.uid)
    this.form = this.fb.group({
      condominiumCode: ['', Validators.required],
      userId: ['']
    });
  }

  condominiums = [
    { id: 1, name: 'Sunset Villas', location: 'Miami, FL' },
    { id: 2, name: 'Sunset Villas', location: 'Miami, FL' },
    { id: 3, name: 'Sunset Villas', location: 'Miami, FL' },
    { id: 4, name: 'Sunset Villas', location: 'Miami, FL' },
    { id: 5, name: 'Sunset Villas', location: 'Miami, FL' },
    { id: 6, name: 'Sunset Villas', location: 'Miami, FL' },
    { id: 7, name: 'Sunset Villas', location: 'Miami, FL' },
    { id: 8, name: 'Ocean Breeze Condos', location: 'Los Angeles, CA' }
  ];

  joinCondominium() {
    console.log('Joining a condominium...');
  //  console.log(this.authService.currentUser?.uid);
    this.form.patchValue({
      userId: this.authService.currentUser?.uid,
    });

    if (this.form.invalid) {
      console.log('Form is invalid.');
      return;
    }
    const formData = this.form.value;
    //    console.log(formData);

    this.condominiumService.join(formData).subscribe({
      next: (result) => {
       // console.log('Joined successfully:', result);
      },
      error: (err) => {
       // console.error('Error joining condominium:', err);
        this.errorText = err.error.Errors[0].Message;
      }
    });
  }

  goToCreateCondominium() {
    console.log('Creating a new condominium...');
    this.router.navigate(['/condominium/create']);
  }
  goHome(){
    this.authService.logout();
    this.router.navigate(['']);
  }
  viewCondominium(id: number) {
    console.log(`Viewing condominium ID: ${id}`);
  }

  changeModalState() {
    this.isModalOpen = !this.isModalOpen;
    this.errorText = "";
  }
}
