import {Component, effect, OnDestroy, OnInit, signal, viewChild} from '@angular/core';
import {Toolbar} from 'primeng/toolbar';
import {Router, RouterOutlet} from '@angular/router';
import {NgFor, NgOptimizedImage} from '@angular/common';
import {Avatar} from 'primeng/avatar';
import {Dialog, DialogModule} from 'primeng/dialog';
import {SharedFormField} from '../../../../shared/components/form/shared-form-field.interface';
import {FormGroup, Validators} from '@angular/forms';
import {SharedForm} from '../../../../shared/components/form/shared-form.interface';
import {FormComponent} from '../../../../shared/components/form/form.component';
import {Feedback} from '../../../../shared/components/form/feedback.interface';
import {AuthenticationService} from '../../../services/authentication.service';
import {Image} from 'primeng/image';
import {Subject, takeUntil, tap} from 'rxjs';
import {AutoUnsubscribe} from '../../../../shared/decorators/autounsuscribe.decorator';
import {BadgeModule} from 'primeng/badge';
import {Menu} from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { User } from '../auth-layout/user.type';

@AutoUnsubscribe()
@Component({
  selector: 'core-user-options',
  imports: [
    Toolbar,
    NgOptimizedImage,
    Avatar,
    Dialog,
    FormComponent,
    Image,
    DialogModule,
    NgFor,
    BadgeModule,
    Menu
  ],
  templateUrl: './user-options.component.html',
  styleUrl: './user-options.component.css'
})
export class UserOptionsComponent {
menuItems: MenuItem[] = [
    {
      label: 'Opciones',
      items: [
        {
          label: 'Mi perfil',
          icon: 'pi pi-user-edit',
          command: () => {
            this.showForm();
          }
        },
        {
          label: 'Cerrar sesión',
          icon: 'pi pi-sign-out',
          command: () => {
            this.authenticationService.logOut()
          }
        },
        {
          label: "Mis condominios",
          icon: 'pi pi-home',
          command: () => {
            this.router.navigate(['/condominiums'])
          }
        }
        ]
    }
  ]
  
  constructor(private authenticationService: AuthenticationService, private router: Router) {
    effect(() => {
      if(!this.visible()) {
        this.formGroup().reset();
        this.imageUrl.set(this.userData?.profilePictureUrl);
      };
    })
  }

  ngOnInit() {
    this.authenticationService.userData$
      .pipe(takeUntil(this.destroy$)) // Se detiene cuando `destroy$` emite un valor en el @AutoUnsubscribe()
      .subscribe(userData => {
        if(userData) this.userData = userData?.data;
        console.log(this.userData)
        this.updateFormFields(); // <== Actualiza los valores del formulario
        if(userData) this.formGroup().patchValue(this.mapUserDataToForm(userData.data));
    });

    this.authenticationService.userToken$
      .pipe(takeUntil(this.destroy$))
      .subscribe(token => this.token = token);
  }

  private destroy$ = new Subject<void>(); // Se usa para cancelar la suscripción
  visible = signal(false)
  formComponent = viewChild(FormComponent);
  private readonly formGroup = signal<FormGroup>(new FormGroup({}));
  userData: User | null = null;
  token: string | null = null;
  imageUrl = signal(this.userData?.profilePictureUrl)
  showNotifications = false;
  showDrawer = false;
  userProfileFormFields = signal<SharedFormField[]>([]);
  notifications = [
    { message: 'Nuevo mensaje de Juan', time: 'Hace 5 minutos' },
    { message: 'Carlos ha publicado algo nuevo', time: 'Hace 1 hora' },
    { message: 'María ha reaccionado a tu publicación', time: 'Hace 2 horas' },
  ];

  onSubmit(value: any) {
    const formComponent = this.formComponent();
    const formGroup = this.formGroup();
    const visible = this.visible;
    console.log('VALUE', value)
    this.authenticationService.editProfile(value, this.token).subscribe({
      next(response) {

        const status = response.isSuccess ? "success" : "error";
        console.log('RESPONSE SUCCESS', response)
        const message = "Perfil editado correctamente.";
        formGroup.reset(response.data)

        setTimeout(() => {
          const feedback: Feedback = { status, message };
          formComponent?.resetForm(feedback);
        }, 100);

        // Cierra el modal después de la actualización exitosa
        setTimeout(() => {
          visible.set(false);
          formGroup.reset();
        }, 5000);
      },
      error(err) {
        const status = "error";
        console.log(err)
        const message = err.error.Errors[0].Message;
        console.log("Message: ", message);
        const feedback: Feedback = { status, message };
        formComponent?.resetForm(feedback);
      }
    });
  }


  showNotificationsDialog(): void {
    this.showNotifications = true;
  }

  showDrawerDialog(): void {
    this.showDrawer = true;
  }
  profileFormSettings = signal<SharedForm>({
    fields: this.userProfileFormFields(),
    baseButtonLabel: 'Editar perfil',
    submittedButtonLabel: '¡Perfil editado satisfactoriamente!'
  });

  onFormCreated(form: FormGroup) {
    this.formGroup.set(form);
  }

  showForm(){
    this.visible.set(!this.visible());
    if (this.visible() && this.userData) {
      this.formGroup().patchValue(this.mapUserDataToForm(this.userData)); // Vuelve a llenar el formulario
      this.imageUrl.set(this.userData?.profilePictureUrl); // Restablece la imagen del usuario
    }
  }

  private updateFormFields(){
    this.userProfileFormFields.set([
      {
        name: 'name',
        label: 'Nombre',
        type: 'text',
        defaultValue: this.userData?.name,
        validators: [Validators.required],
        errorMessages: {
          required: 'El nombre es requerido.'
        }
      },
      {
        name: 'lastname',
        label: 'Apellido',
        type: 'text',
        defaultValue: this.userData?.lastname,
        validators: [Validators.required, Validators.minLength(3), Validators.maxLength(200)],
        errorMessages: {
          required: 'El apellido es requerido.',
          minLength: 'El apellido debe tener al menos 3 caracteres.',
          maxLength: 'El apellido no puede tener más de 200 caracteres.'
        }
      },
      {
        name: 'username',
        label: 'Usuario',
        type: 'text',
        defaultValue: this.userData?.username,
        validators: [Validators.required, Validators.minLength(4), Validators.maxLength(25)],
        errorMessages: {
          required: 'El nombre de usuario es requerido.',
          minLength: 'El nombre de usuario debe tener al menos 4 caracteres.',
          maxLength: 'El nombre de usuario no puede tener más de 25 caracteres.'
        }
      },
      {
        name: 'email',
        defaultValue: this.userData?.email,
        label: 'Correo electrónico',
        type: 'text',
        icon: 'pi-at',
        disabled: true
      },
      {
        name: 'street',
        label: 'Calle',
        type: 'text',
        defaultValue: this.userData?.address ? this.userData?.address.street : '',
        icon: 'pi-map-marker',
        validators: [Validators.required, Validators.maxLength(100)],
        errorMessages: {
          required: 'La calle es requerida.',
          maxLength: 'La calle no puede tener más de 100 caracteres.'
        }
      },
      {
        name: 'city',
        label: 'Ciudad',
        type: 'text',
        icon: 'pi-map',
        defaultValue: this.userData?.address ? this.userData?.address.city : '',
        validators: [Validators.required, Validators.maxLength(100)],
        errorMessages: {
          required: 'La ciudad es requerida.',
          maxLength: 'La ciudad no puede tener más de 100 caracteres.'
        }
      },
      {
        name: 'postalCode',
        label: 'Código postal',
        icon: 'pi-address-book',
        type: 'text',
        defaultValue: this.userData?.address ? this.userData?.address.postalCode : '',
        validators: [Validators.required, Validators.minLength(4), Validators.maxLength(6), Validators.pattern('^d{4,6}$')],
        errorMessages: {
          required: 'La calle es requerida.',
          minLength: 'El código postal debe tener al menos 4 caracteres.',
          maxLength: 'El código postal no puede tener más de 6 caracteres.',
          pattern: 'El código postal debe ser un número.'
        }
      },
      {
        name: 'profilePic',
        label: 'Subir imagen',
        type: 'file',
        validators: [],
        filetype: 'image/*',
        onFileSelect: (event: any) => {
          if (event.files.length > 0) {
            const file = event.files[0];
            this.formGroup().patchValue({
              profilePic: file,
            });
            if(file) this.imageUrl.set(URL.createObjectURL(file));
          }
        }
      }
    ])

    this.profileFormSettings.set({
      ...this.profileFormSettings(),
      fields: this.userProfileFormFields()
    });
  }
  private mapUserDataToForm(user: User) {
    return {
      name: user.name,
      lastname: user.lastname,
      username: user.username,
      email: user.email,
      street: user.address?.street ?? '',
      city: user.address?.city  ?? '',
      postalCode: user.address?.postalCode  ?? '',
    };
  }

  ngOnDestroy() {
    this.formGroup().reset();
  }
}
