import { Component,OnInit } from '@angular/core';
import { CondominiumService } from '../../services/condominium.service';
import { AuthenticationService } from '../../../../core/services/authentication.service';
import { Subject, takeUntil } from 'rxjs';
import { User } from '../../../../core/auth/layout/auth-layout/user.type';
import { ActivatedRoute, Router } from '@angular/router';
import { Button } from 'primeng/button';
@Component({
  selector: 'app-joinwith-token',
  imports: [Button],
  templateUrl: './joinwith-token.component.html',
  styleUrl: './joinwith-token.component.css'
})
export class JoinwithTokenComponent {
    user: User|null = null
    destroy$ = new Subject<void>();
    success: Boolean = false
    token: string |null = null;
    message: string = ""
  constructor(
    private condominiumService: CondominiumService, 
      private authenticationService: AuthenticationService,
      private route: ActivatedRoute,
      private router: Router
    ){
 
      }

  ngOnInit(){

    this.token = decodeURIComponent(this.route.snapshot.paramMap.get('token') ?? "");

      this.authenticationService.userData$.pipe(takeUntil(this.destroy$)).subscribe((userData) => {
      if(userData?.data) {
          this.user = userData?.data
            //  (userData)
            //  (this.user)

            this.condominiumService.join({
            userId: this.user?.id  === undefined ? "" : this.user.id,
            condominiumToken: this.token ?? "",
            condominiumCode: "",
      }).subscribe({
          next: (result) => {
           this.success = true;
           this.message = "Se unio correctamente al condominio"
       }, error: (err) => {
        console.error(this.message = err.error.Errors[0].Message)
        this.success = false;
      },
    })
      };
    })
  
  }

  goToCondominiumUndex(){
    this.router.navigate(['condominiums'])
  }
}
