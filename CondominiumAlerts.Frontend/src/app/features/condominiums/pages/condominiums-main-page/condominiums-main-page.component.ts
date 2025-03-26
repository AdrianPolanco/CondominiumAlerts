import { Component } from '@angular/core';
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
import { GetCondominiumsJoinedByUserResponse } from "../../models/getCondominiumsJoinedByUser.response";
import { AuthenticationService } from '../../../../core/services/authentication.service';
import { User } from '../../../../core/auth/layout/auth-layout/user.type';
import { Subject, takeUntil } from 'rxjs';
import { AutoUnsubscribe } from '../../../../shared/decorators/autounsuscribe.decorator';

@AutoUnsubscribe()
@Component({
  selector: 'app-condominiums-main-page',
  imports: [
    NgFor,
    CommonModule,
    ReactiveFormsModule,
    Button
  ],
  templateUrl: './condominiums-main-page.component.html',
  styleUrls: ['./condominiums-main-page.component.css'],
})
export class CondominiumsMainPageComponent {
  form: FormGroup;
  isModalOpen: boolean = false;
  errorText: string = '';
  user: User|null = null
  destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private condominiumService: CondominiumService,
    private router: Router,
    private authenticationService: AuthenticationService
  ) {
    // console.log(this.authService.currentUser?.uid)
    this.form = this.fb.group({
      condominiumCode: ['', Validators.required],
      userId: [''],
    });

    this.authenticationService.userData$.pipe(takeUntil(this.destroy$)).subscribe((userData) => {
      if(userData?.data) {
        this.user = userData?.data
        this.loadUserCondominiums();
      };
    })
  }

  condominiums: Array<GetCondominiumsJoinedByUserResponse> = [];

  joinCondominium() {
    console.log('Joining a condominium...');
    //  console.log(this.authService.currentUser?.uid);
    this.form.patchValue({
      userId: this.user?.id,
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

  private setCurrentCondominium(condominium: GetCondominiumsJoinedByUserResponse) {
    this.condominiumService.setCondominium(condominium);
  }

  getBackgroundImage(imageUrl: string | null): string {
    return imageUrl ? `url('${imageUrl}')` : 'none';
  }

  goToCreateCondominium() {
    this.router.navigate(['/condominium/create']);
  }
  goHome() {
    this.authenticationService.logOut();
    this.router.navigate(['']);
  }
  viewCondominium(condominium: GetCondominiumsJoinedByUserResponse) {
    console.log(`Viewing condominium ID: ${condominium.id}`);
    this.setCurrentCondominium(condominium)
    this.router.navigate(['/condominium/index', condominium.id]);
  }

  changeModalState() {
    this.isModalOpen = !this.isModalOpen;
    this.errorText = '';
  }

  loadUserCondominiums(): void {
    this.condominiumService
      .getCondominiumsJoinedByUser({
        userId: this.user?.id ?? '',
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
