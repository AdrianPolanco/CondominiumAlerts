import { Component, computed, effect, inject, model, OnDestroy, OnInit, signal, viewChild } from "@angular/core";
import { AutoUnsubscribe } from "../../../../shared/decorators/autounsuscribe.decorator";
import { Dialog, DialogModule } from "primeng/dialog";
import { NotificationService } from "../../../../features/notifications/notification.service";
import { NotificationSignalRService } from "../../../services/notification-signal-r.service";
import { CondominiumService } from "../../../../features/condominiums/services/condominium.service";
import { AuthenticationService } from "../../../services/authentication.service";
import { Router } from "@angular/router";
import { NotificationDto } from "../../../../features/notifications/models/notification.model";
import { FormComponent } from "../../../../shared/components/form/form.component";
import { FormGroup, FormsModule, Validators } from "@angular/forms";
import { User } from "../auth-layout/user.type";
import { SharedFormField } from "../../../../shared/components/form/shared-form-field.interface";
import { SharedForm } from "../../../../shared/components/form/shared-form.interface";
import { MenuItem } from "primeng/api";
import { Subject, takeUntil } from "rxjs";
import { CommonModule } from "@angular/common";
import { ButtonModule } from "primeng/button";
import { ImageModule } from "primeng/image";
import { Feedback } from "../../../../shared/components/form/feedback.interface";
import { AvatarModule } from "primeng/avatar";
import { MenuModule } from "primeng/menu";
import { BadgeModule } from "primeng/badge";
import { ToolbarModule } from "primeng/toolbar";

@AutoUnsubscribe()
@Component({
    selector: 'core-user-options',
    imports: [
        Dialog,
        DialogModule,
        BadgeModule,
        MenuModule,
        ButtonModule,
        CommonModule,
        FormsModule,
        FormComponent,
        ImageModule,
        AvatarModule,
        ToolbarModule,
    ],
    templateUrl: './user-options.component.html',
    styleUrl: './user-options.component.css',
})
export class UserOptionsComponent implements OnInit, OnDestroy {
    // Services
    private notificationService = inject(NotificationService);
    private notificationSignalR = inject(NotificationSignalRService);
    private condominiumService = inject(CondominiumService);
    private authenticationService = inject(AuthenticationService);
    private router = inject(Router);

    // State signals
    visible = signal(false);
    showNotificationsDialog = model(false);
    showDrawer = signal(false);
    unreadCount = signal(0);
    notifications = signal<NotificationDto[]>([]);
    currentCondominiumId = computed(() => this.condominiumService.currentCondominium?.id);

    // Form related
    formComponent = viewChild(FormComponent);
    private readonly formGroup = signal<FormGroup>(new FormGroup({}));
    userData: User | null = null;
    token: string | null = null;
    imageUrl = signal(this.userData?.profilePictureUrl);
    userProfileFormFields = signal<SharedFormField[]>([]);

    profileFormSettings = signal<SharedForm>({
        fields: this.userProfileFormFields(),
        baseButtonLabel: 'Editar perfil',
        submittedButtonLabel: '¡Perfil editado satisfactoriamente!',
    });

    // Menu items with notifications
    menuItems = computed<MenuItem[]>(() => [
        {
            label: 'Opciones',
            items: [
                {
                    label: 'Notificaciones',
                    icon: 'pi pi-bell',
                    badge: this.unreadCount() > 0 ? this.unreadCount().toString() : '',
                    command: () => {
                        this.showNotificationsDialog.set(true);
                        this.markAsRead();
                    }
                },
                {
                    label: 'Mi perfil',
                    icon: 'pi pi-user-edit',
                    command: () => this.showForm()
                },
                {
                    label: 'Mis condominios',
                    icon: 'pi pi-home',
                    command: () => this.router.navigate(['/condominiums'])
                },
                {
                    label: 'Cerrar sesión',
                    icon: 'pi pi-sign-out',
                    command: () => {
                        this.authenticationService.logOut();
                        this.router.navigate(['/home']);
                    }
                }
            ]
        }
    ]);

    private destroy$ = new Subject<void>();

    constructor() {
        effect(() => {
            if (!this.visible()) {
                this.formGroup().reset();
                this.imageUrl.set(this.userData?.profilePictureUrl);
            }
        });

        effect(() => {
            this.loadNotifications();
            this.joinNotificationGroup();
        })
    }

    ngOnInit() {
        this.setupUserData();
        this.setupNotifications();
    }

    private setupUserData() {
        this.authenticationService.userData$
            .pipe(takeUntil(this.destroy$))
            .subscribe((userData) => {
                if (userData) {
                    this.userData = userData.data;
                    this.updateFormFields();
                    this.formGroup().patchValue(this.mapUserDataToForm(userData.data));
                }
            });

        this.authenticationService.userToken$
            .pipe(takeUntil(this.destroy$))
            .subscribe((token) => (this.token = token));
    }

    private setupNotifications() {
        this.notificationSignalR.notification$
            .pipe(takeUntil(this.destroy$))
            .subscribe(notification => {
                this.notifications.update(notifs => [notification, ...notifs]);
                this.unreadCount.update(count => count + 1);
            });

        this.notificationSignalR.startConnection();
    }

    private loadNotifications() {
        this.authenticationService.userData$
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: (userData) => {
                    if (!userData) return;
                    this.notificationService.get()
                        .pipe(takeUntil(this.destroy$))
                        .subscribe({
                            next: (notifications) => {
                                this.notifications.set(notifications.data);
                                console.log(this.notifications());
                                console.log("Loaded notifications");
                            },
                            error: (err) => console.error('Error loading notifications', err)
                        });
                }
            });
    }

    private joinNotificationGroup() {
        this.authenticationService.userData$
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: (userData) => {
                    if (!userData) return;
                    this.condominiumService.getCondominiumsJoinedByUser({
                        userId: userData.data.id
                    }).pipe(takeUntil(this.destroy$))
                        .subscribe({
                            next: (condos) => {
                                condos.data.forEach(condo => {
                                    this.notificationSignalR
                                        .joinCondominiumGroup(condo.id);
                                })
                            }
                        });
                }
            });
    }

    markAsRead() {
        this.unreadCount.set(0);
    }

    onLogoClicked() {
        if (this.userData) this.router.navigate(['/condominiums']);
        else this.router.navigate(['/home']);
    }

    onSubmit(value: any) {
        const formComponent = this.formComponent();
        const formGroup = this.formGroup();
        const visible = this.visible;

        this.authenticationService.editProfile(value, this.token).subscribe({
            next(response) {
                const status = response.isSuccess ? 'success' : 'error';
                const message = 'Perfil editado correctamente.';
                formGroup.reset(response.data);

                setTimeout(() => {
                    const feedback: Feedback = { status, message };
                    formComponent?.resetForm(feedback);
                }, 100);

                setTimeout(() => {
                    visible.set(false);
                    formGroup.reset();
                }, 5000);
            },
            error(err) {
                const status = 'error';
                const message = err.error.Errors[0].Message;
                const feedback: Feedback = { status, message };
                formComponent?.resetForm(feedback);
            },
        });
    }

    onFormCreated(form: FormGroup) {
        this.formGroup.set(form);
    }

    showForm() {
        this.visible.set(!this.visible());
        if (this.visible() && this.userData) {
            this.formGroup().patchValue(this.mapUserDataToForm(this.userData));
            this.imageUrl.set(this.userData?.profilePictureUrl);
        }
    }

    private updateFormFields() {
        this.userProfileFormFields.set([
            {
                name: 'name',
                label: 'Nombre',
                type: 'text',
                defaultValue: this.userData?.name,
                validators: [Validators.required],
                errorMessages: {
                    required: 'El nombre es requerido.',
                },
            },
            {
                name: 'lastname',
                label: 'Apellido',
                type: 'text',
                defaultValue: this.userData?.lastname,
                validators: [
                    Validators.required,
                    Validators.minLength(3),
                    Validators.maxLength(200),
                ],
                errorMessages: {
                    required: 'El apellido es requerido.',
                    minLength: 'El apellido debe tener al menos 3 caracteres.',
                    maxLength: 'El apellido no puede tener más de 200 caracteres.',
                },
            },
            {
                name: 'username',
                label: 'Usuario',
                type: 'text',
                defaultValue: this.userData?.username,
                validators: [
                    Validators.required,
                    Validators.minLength(4),
                    Validators.maxLength(25),
                ],
                errorMessages: {
                    required: 'El nombre de usuario es requerido.',
                    minLength: 'El nombre de usuario debe tener al menos 4 caracteres.',
                    maxLength: 'El nombre de usuario no puede tener más de 25 caracteres.',
                },
            },
            {
                name: 'email',
                defaultValue: this.userData?.email,
                label: 'Correo electrónico',
                type: 'text',
                icon: 'pi-at',
                disabled: true,
            },
            {
                name: 'street',
                label: 'Calle',
                type: 'text',
                defaultValue: this.userData?.address
                    ? this.userData?.address.street
                    : '',
                icon: 'pi-map-marker',
                validators: [Validators.required, Validators.maxLength(100)],
                errorMessages: {
                    required: 'La calle es requerida.',
                    maxLength: 'La calle no puede tener más de 100 caracteres.',
                },
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
                    maxLength: 'La ciudad no puede tener más de 100 caracteres.',
                },
            },
            {
                name: 'postalCode',
                label: 'Código postal',
                icon: 'pi-address-book',
                type: 'text',
                defaultValue: this.userData?.address
                    ? this.userData?.address.postalCode
                    : '',
                validators: [
                    Validators.required,
                    Validators.minLength(4),
                    Validators.maxLength(6),
                    Validators.pattern('^d{4,6}$'),
                ],
                errorMessages: {
                    required: 'La calle es requerida.',
                    minLength: 'El código postal debe tener al menos 4 caracteres.',
                    maxLength: 'El código postal no puede tener más de 6 caracteres.',
                    pattern: 'El código postal debe ser un número.',
                },
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
                        if (file) this.imageUrl.set(URL.createObjectURL(file));
                    }
                },
            },
        ]);

        this.profileFormSettings.set({
            ...this.profileFormSettings(),
            fields: this.userProfileFormFields(),
        });
    }

    private mapUserDataToForm(user: User) {
        return {
            name: user.name,
            lastname: user.lastname,
            username: user.username,
            email: user.email,
            street: user.address?.street ?? '',
            city: user.address?.city ?? '',
            postalCode: user.address?.postalCode ?? '',
        };
    }

    ngOnDestroy() {
        if (this.currentCondominiumId() && this.userData?.id) {
            this.notificationSignalR.leaveCondominiumGroup(this.currentCondominiumId()!);
        }
        this.formGroup().reset();
        this.destroy$.next();
        this.destroy$.complete();
    }
}
