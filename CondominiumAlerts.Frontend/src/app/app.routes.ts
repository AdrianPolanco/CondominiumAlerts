import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: "register",
    title: "Registrar usuario",
    loadComponent: () => import('./core/register/pages/register-user-page/register-user-page.component')
                                        .then(c => c.RegisterUserPageComponent)
  },
    {
        path: "condominium/create",
        loadComponent: () => import('./features/condominiums/pages/condominium-page/condominium-page.component')
            .then(x => x.CondominiumPageComponent)
    },

  {
    path: "condominiums-main-page",
    title: "Condominium main page",
    loadComponent: () => import('./features/condominiums/pages/condominiums-main-page/condominiums-main-page.component')
                                        .then(c => c.CondominiumsMainPageComponent)
  },
];
