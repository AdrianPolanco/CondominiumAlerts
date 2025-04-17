import {Component, computed, effect, OnDestroy, OnInit, signal, viewChild} from '@angular/core';
import {Toolbar} from 'primeng/toolbar';
import {Router} from '@angular/router';
import {NgClass, NgFor, NgOptimizedImage} from '@angular/common';
import {Avatar} from 'primeng/avatar';
import {Dialog, DialogModule} from 'primeng/dialog';
import {SharedFormField} from '../../../../shared/components/form/shared-form-field.interface';
import {FormGroup, Validators} from '@angular/forms';
import {SharedForm} from '../../../../shared/components/form/shared-form.interface';
import {FormComponent} from '../../../../shared/components/form/form.component';
import {Feedback} from '../../../../shared/components/form/feedback.interface';
import {AuthenticationService} from '../../../services/authentication.service';
import {Image} from 'primeng/image';
import {interval, startWith, Subject, switchMap, takeUntil, tap} from 'rxjs';
import {AutoUnsubscribe} from '../../../../shared/decorators/autounsuscribe.decorator';
import {BadgeModule} from 'primeng/badge';
import {Menu} from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { User } from '../auth-layout/user.type';
import { EventService } from '../../../../features/events/services/event.service';
import { CondominiumEvent } from '../../../../features/events/event.type';
import {differenceInMinutes} from "date-fns"
import {CondominiumNotification} from '../../../../features/events/types/condominiumNotification.type';
import {NotificationService} from '../../../../features/notifications/notification.service';
import {TimeAgoPipe} from '../../../../shared/pipes/time-ago.pipe';

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
    Menu,
    NgClass,
    TimeAgoPipe
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
          label: "Mis condominios",
          icon: 'pi pi-home',
          command: () => {
            this.router.navigate(['/condominiums'])
          }
        },
        {
          label: 'Cerrar sesión',
          icon: 'pi pi-sign-out',
          command: () => {
            this.authenticationService.logOut()
            this.router.navigate(['/home'])
          }
        },
        ]
    }
  ]
   events: CondominiumEvent[] = [];
   upcomingEvents: CondominiumEvent[] = []
  // Almacena los IDs de eventos a los que ya te has unido
  private joinedEventIds = new Set<string>();

  constructor(
    private authenticationService: AuthenticationService,
    private router: Router,
    private eventService: EventService,
    private notificationService: NotificationService) {
    effect(() => {
      if(!this.visible()) {
        this.formGroup().reset();
        this.imageUrl.set(this.userData?.profilePictureUrl);
      };
    })

    this.eventService.notification$.pipe(takeUntil(this.destroy$)).subscribe(notification => {
      this.notifications = [...notification];
    })
  }

  ngOnInit() {
    interval(30000)
      .pipe(
        startWith(0), // Ejecuta inmediatamente al cargar el componente
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.refreshEventsAndJoinGroups();
      });

    this.authenticationService.userData$
      .pipe(takeUntil(this.destroy$)) // Se detiene cuando `destroy$` emite un valor en el @AutoUnsubscribe()
      .subscribe(userData => {
        if(userData) this.userData = userData?.data;
        console.log(this.userData)
        console.log("TOKEN", this.token)
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
  userProfileFormFields = signal<SharedFormField[]>([]);
  notifications: CondominiumNotification[] = [];
  get unreadNotifications(): CondominiumNotification[] {
    return this.notifications.filter(n => !n.read);
  }
  get unreadNotificationsCount(): number {
    return this.unreadNotifications.length;
  }

  onLogoClicked(){
    if(this.userData) this.router.navigate(['/condominiums']);
    else this.router.navigate(['/home']);
  }

  onNotificationsClosed(){
    this.notificationService.markAsRead(this.unreadNotifications).pipe(
      switchMap(() => this.notificationService.get()),
      takeUntil(this.destroy$)
    ).subscribe((res) => {
      if (res.isSuccess) {
        this.notifications = res.data.notifications;
      }
    });
  }

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

    this.notificationService.get().subscribe({
      next: (res) => {
        this.notifications = res.data.notifications;
        console.log("NOTIFICACIONES", this.notifications)
      },
      error: (err) => {
        console.error('Error al cargar notificaciones', err);
      }
    });
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

  private refreshEventsAndJoinGroups(): void {
    this.eventService.getSubscribedEvents().subscribe(res => {
      const events = res.data.events

      this.upcomingEvents = events.filter(event => {
        const minutesUntilStart = differenceInMinutes(new Date(event.start), new Date());
        const minutesUntilEnd = differenceInMinutes(new Date(event.end), new Date());
        return (minutesUntilStart >= 0 && minutesUntilStart <= 15) || (minutesUntilEnd >= 0 && minutesUntilEnd <= 15);
        });

      console.log("EVENTOS SUSCRITOS QUE EMPEZARAN PROXIMAMENTE: ", this.upcomingEvents)

      // Si el evento empezara en menos de 15 minutos, nos unimos a su grupo
      this.upcomingEvents.forEach(event => {
        if(this.joinedEventIds.has(event.id)) return;
        this.eventService.joinEventGroup(event.id);
        this.joinedEventIds.add(event.id); // Agregamos el ID del evento a la lista de eventos unidos
      });
    });
  }

  ngOnDestroy() {
    this.formGroup().reset();
    this.destroy$.next(); // Emite un valor para cancelar la suscripción
    this.destroy$.complete(); // Completa el Subject para cancelar la suscripción
    this.upcomingEvents.forEach(e => this.eventService.leaveEventGroup(e.id));
  }
}
