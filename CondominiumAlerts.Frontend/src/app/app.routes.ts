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
    path: "",
    title: "Home",
    loadComponent: () => import('./home/home.component') // Ruta corregida
      .then(c => c.HomeComponent) // Referencia al componente correcto
  }
];
