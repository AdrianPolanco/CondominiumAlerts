import {AbstractControl, ValidationErrors, ValidatorFn} from '@angular/forms';

export function passwordsMatchValidator(passwordField: string, confirmPasswordField: string): ValidatorFn {
  return (formGroup: AbstractControl): ValidationErrors | null => {
    const password = formGroup.get(passwordField);
    const confirmPassword = formGroup.get(confirmPasswordField);

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      // Solo aplicamos el error al campo de confirmación
      confirmPassword.setErrors({ passwordMismatch: true });
      return null; // No necesitamos error a nivel de grupo
    }

    // Limpiamos el error si las contraseñas coinciden
    if (confirmPassword?.hasError('passwordMismatch')) {
      const errors = {...confirmPassword.errors};
      delete errors['passwordMismatch'];
      confirmPassword.setErrors(Object.keys(errors).length ? errors : null);
    }

    return null;
  };
}
