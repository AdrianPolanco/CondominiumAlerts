import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: "register",
    title: "Registrar usuario",
    loadComponent: () => import('./core/register/pages/register-user-page/register-user-page.component')
                                        .then(c => c.RegisterUserPageComponent)
  }
];
