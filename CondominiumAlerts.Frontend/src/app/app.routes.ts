import { Routes } from '@angular/router';

export const routes: Routes = [
  {

    path: 'register',
    title: 'Registrar usuario',
    loadComponent: () =>
      import(
        './core/register/pages/register-user-page/register-user-page.component'
      ).then((c) => c.RegisterUserPageComponent),
  },
  {
    path: 'login',
    title: 'Iniciar sesión',
    loadComponent: () =>
      import('./core/auth/pages/sing-in/sing-in.component').then(
        (c) => c.SingInComponent
      ),
  },
  {
    path: 'home',  
    title: 'Home',
    loadComponent: () =>
      import('./home/home.component')
        .then((c) => c.HomeComponent),
  },
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
    {
        path: "condominium/create",
        loadComponent: () => import('./features/condominiums/pages/condominium-page/condominium-page.component')
            .then(x => x.CondominiumPageComponent)
    },

  {
    path: "condominium/main-page",
    loadComponent: () => import('./features/condominiums/pages/condominiums-main-page/condominiums-main-page.component')
                                        .then(c => c.CondominiumsMainPageComponent)
  },
  {
    path: "condominium/index",
    loadComponent: () => import('./features/condominiums/pages/condominum-index/condominum-index.component')
                                        .then(c => c.CondominumIndexComponent)
  },
];
