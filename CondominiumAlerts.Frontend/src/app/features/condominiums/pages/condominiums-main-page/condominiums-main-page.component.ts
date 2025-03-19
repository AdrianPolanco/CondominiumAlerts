import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { CondominiumService } from '../../services/condominium.service';
import { NgFor, CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Button } from 'primeng/button';
import { Toolbar } from 'primeng/toolbar';
import { NgOptimizedImage } from '@angular/common';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { GetCondominiumsJoinedByUserResponse } from "../../models/getCondominiumsJoinedByUser.response";

@Component({
  selector: 'app-condominiums-main-page',
  imports: [
    NgFor,
    CommonModule,
    ReactiveFormsModule,
    Button,
    Toolbar,
    NgOptimizedImage,
  ],
  templateUrl: './condominiums-main-page.component.html',
  styleUrls: ['./condominiums-main-page.component.css'],
})
export class CondominiumsMainPageComponent {
  form: FormGroup;
  isModalOpen: boolean = false;
  errorText: string = '';
  constructor(
    private fb: FormBuilder,
    private condominiumService: CondominiumService,
    private router: Router,
    private authService: AuthService
  ) {
    // console.log(this.authService.currentUser?.uid)
    this.form = this.fb.group({
      condominiumCode: ['', Validators.required],
      userId: [''],
    });
  }

  condominiums: Array<GetCondominiumsJoinedByUserResponse> = [
    {
      id: '2d2c3c52-de6d-4697-8634-4e460f9d9516',
      name: 'Sunset Villas',
      address: 'Miami, FL',
      imageUrl: 'cadf546w65',
    },
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
      },
    });
  }

  goToCreateCondominium() {
    console.log('Creating a new condominium...');
    this.router.navigate(['/condominium/create']);
  }
  goHome() {
    this.authService.logout();
    this.router.navigate(['']);
  }
  viewCondominium(id: string) {
    console.log(`Viewing condominium ID: ${id}`);
    this.router.navigate(['/condominium/index', id]);
  }

  changeModalState() {
    this.isModalOpen = !this.isModalOpen;
    this.errorText = '';
  }

  ngOnInit() {
    this.loadUserCondominiums();
  }

  loadUserCondominiums(): void {
    this.condominiumService
      .getCondominiumsJoinedByUser({
        userId: this.authService.currentUser?.uid ?? '',
      })
      .subscribe({
        next: (result) => {
          this.condominiums = result.data;
        },
        error: (err) => {
          console.log(err);
        },
      });
  }
}
