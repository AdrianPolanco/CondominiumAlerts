# Documentación
## Índice
- [¿De que tratá?](#¿de-que-trata?)
- [¿Por qué?](#¿por-qué?)
- [Arquitectura y estructura del proyecto](#arquitectura-y-estructura-del-proyecto)
  - [Tecnologías a utilizar](#tecnologías-a-utilizar)
- [Backend](#backend)
  - [Patrones del backend](#patrones-del-backend)
  - [Diagrama del backend](#diagrama-del-backend)
  - [Capas de la aplicación backend](#capas-de-la-aplicación-backend)
    - [Domain](#domain)
      - [Paquetes usados en Domain](#paquetes-usados-en-domain)
      - [Dependencias en Domain](#dependencias-en-domain)
    - [Infrastructure](#infrastructure)
      - [Paquetes usados en Infrastructure](#paquetes-usados-en-infrastructure)
      - [Dependencias en Infrastructure](#dependencias-en-infrastructure)
    - [Features](#features)
      - [Paquetes usados en Features](#paquetes-usados-en-features)
      - [Dependencias en Features](#dependencias-en-features)
    - [API](#api)
      - [Paquetes usados en API](#paquetes-usados-en-api)
      - [Dependencias en API](#dependencias-en-api)
    - [Cross Cutting](#cross-cutting)
      - [Paquetes usados en Cross Cutting](#paquetes-usados-en-cross-cutting)
      - [Dependencias en Cross Cutting](#dependencias-en-cross-cutting)
    - [Tests](#tests)
      - [Paquetes usados en Tests](#paquetes-usados-en-tests)
      - [Dependencias en Tests](#dependencias-en-tests)
    - [Más sobre la arquitectura del backend](#más-sobre-la-arquitectura-del-backend)
- [Frontend](#frontend)
  - [Patrones del frontend](#patrones-del-frontend)
  - [Diagrama del frontend](#diagrama-del-frontend)
  - [Capas del frontend](#capas-del-frontend)
    - [Core](#core)
    - [Features (Frontend)](#features-(frontend))
    - [Shared](#shared)
    - [State](#state)
  - [Paquetes usados en el frontend](#paquetes-usados-en-el-frontend)
  - [Más sobre la arquitectura del frontend](#más-sobre-la-arquitectura-del-frontend)
- [Branching](#branching)
  - [Crear y cambiar a rama develop](#crear-y-cambiar-a-rama-develop)
  - [Crear ramas para features](#crear-ramas-para-features)
  - [Confirmar cambios](#confirmar-cambios)
  - [Subir cambios](#subir-cambios)
  - [Finalizar feature e integrarla a develop](#finalizar-feature-e-integrarla-a-develop)
- [Commits](#commits)
  - [Tipos de commits](#tipos-de-commits)
  - [Formato de los commits](#formato-de-los-commits)
  - [Ejemplos de commits](#ejemplos-de-commits)
    - [Buen commit](#buen-commit)
    - [Mal commit](#mal-commit)
    
## ¿De que trata?
El proyecto trata, a grandes rasgos, de una app que permitirá a los miembros de una comunidad comunicarse por canales en tiempo real, similar a Discord.
## ¿Por qué?
Para ahorrarnos tiempos y complicaciones, y que podamos desarrollar el proyecto de forma rápida y ordenada, cumpliendo con los aspectos que nos piden, es necesaria esta redacción para ponernos de acuerdo y estandarizar el como trabajaremos en el proyecto.
## Arquitectura y estructura del proyecto
### Tecnologías a utilizar

- .NET 9: Versión del SDK de .NET.
- ASP.NET Core: Framework utilizado para crear la Web Api en el backend.
- Angular 19: Framework utilizado para crear el frontend.
- PostgreSQL: Base de datos usada para persistir los datos NO RELACIONADOS a la autenticación.
- MongoDB: Base de datos NoSQL usada para persistir los datos RELACIONADOS a los mensajes.
- Firebase: Utilizaremos el servicio de autenticación **Firebase Auth** para simplificar todo el proceso del registro, autorización y autenticación de usuarios en la app, autenticación social (Google, etc.), de ser requerido u, opcionalmente funcionalidades como enviar correos de confirmación.
- Docker(opcional): No requerido pero recomendado, para las pruebas de integración y para montar contenedores de bases de datos de diversos ambientes tanto de desarrollo como la que usaremos al presentar el proyecto. Manteniendo una coherencia garantizada entre las versiones que usaremos al compartir todos un ambiente identico, evitando problemas y ahorrandonos tiempo debido a que alguno descargo la versión de SQL que no era, etc.
- Cloudinary: Servicio que usaremos para la gestión de subida, recuperación y borrado de imágenes.
  
### Backend

#### Patrones del backend
Dado que se evaluarán estos aspectos, es necesario clarificar que patrones estaremos utilizando.
**Repository**: Patrón usado para abstraer el acceso a los datos.

**CQRS**: Patrón usado para segregar las operaciones de escritura (Commands) de las operaciones de lecture (Queries).

**Mediator**: Patrón usado para desacoplar el código de los endpoints a través de handlers.

**Middleware**: Patrón usado para ejecutar ciertas acciones antes o después de procesar una solicitud. Lo usaremos para ejecutar validaciones antes de procesar realmente una solicitud a la API.

**Strategy**: Patrón usado para tomar diferentes "estrategias" o "implementaciones" dependiendo de una elección tomada por el usuario. Lo podemos usar, por ejemplo, para diferenciar la publicación un comentario sin ninguna foto adjunta de un comentario con una foto adjunta.

**Inyección de dependencias**: Patrón usado para proveer las dependencias necesarias a una clase sin que esta se tenga que preocupar por eso.

#### Diagrama del backend

Para desarrollar el backend utilizaremos C# con su framework ASP.NET Core para desarrollar la Web API que soportará la aplicación. A continuación, hablaremos más a detalle de que hara cada una de las capas que sale en la imagen:
![Imagen de WhatsApp 2025-01-21 a las 13 28 18_11211e3b](https://github.com/user-attachments/assets/b80c1c45-6f55-48bb-89d2-1b0240ff2cd3)

#### Capas del backend

##### **Domain**
Aquí residirá el dominio y lo relacionado a la lógica de negocio de la aplicación, componiendose, pero no limitandose a los siguientes elementos:

**Entities**: Son los modelos que representan objetos clave del dominio, por ejemplo, en nuestro caso serían *Users*, *Comments* o *Condominiums*. Estos objetos se identifican de forma única a través de un *Id* en lugar de por sus atributos, dígase, un *User* que se llame Pepito y tenga el *Id 1* es diferente de otro *User* que se llame también Pepito y tengo el *Id 2*.

**Value Objects**: Son clases u objetos que se identifican por su **contenido** en lugar de por su *Id*, por ejemplo, *Address* o *Phone*, por ejemplo, un *Phone* 123456789 es el mismo que otro *Phone* 123456789. Suelen ser inmutables.

**Enums**: Son objetos utilizados para representar estados de forma significativa, evitando **"números mágicos"** o **"palabras mágicas"**, de modo que en vez de usar el número 1 para representar el estado "muteado" de un canal, usamos el miembro *MUTED* del enum *ChannelStates*.

**Repositories**: En esta capa de dominio, los *repositories* son interfaces que definen los métodos de las abstracciones que usaremos para acceder a los datos en la base de datos cumpliendo el **Dependency Inversion Principle**, haciendo el código más flexible y escalable (aspecto que se evaluará en el proyecto final TDS). La implementación real de estas interfaces se harán en la capa **Infrastructure**.

**Aggregates**: Es una agrupación de **entities**, **agreggates roots** y **value objects** relacionadas que deben trabajar juntas.

**Aggregate Root**: Es la entidad principal de un *aggreggate*, a través de la cual se interactua con todos los demás elementos del *aggregate*, asegurandose que se cumplan las funcionalidades y reglas de la aplicación, por ejemplo, en nuestro caso, podría ser una entidad *Event* o *Channel*, toda la interacción que se dé con las entidades internas al *aggregate* deberían de darse a través del *aggregate root*, facilitando la implementación de funcionalidades y validaciones como validar que los eventos solo puedan ser creados por X tipo de usuarios que esten dentro del canal dado, o que no se superé el limite de usuarios en un canal.

**Domain Events**: Son eventos que se dispararán despues de una acción determinada en el dominio. Esto facilitará la implementación de funcionalidades como enviar correos de confirmación a los usuarios después de registrarse. Notése que esto es diferente y no necesariamente igual a la entidad *Event* que mencionamos anteriormente y podríamos o no usar *Domain Events* para desarrollar la funcionalidad de eventos que requiere la app.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

**Interfaces**: Abstracciones de servicios relevantes que serán implementados en la capa **infrastructure**, similares a los *repositories* pero orientados a aspectos no relacionados directamente a la persistencia en la base de datos.

###### Paquetes usados en Domain

**MediatR**: Usaremos la librería **MediatR**, que es la única dependencia que tiene esta capa para disparar los *Domain Events*.

###### Dependencias en Domain

- Ninguna

##### **Infrastructure**
Es la capa que proveerá todo el acceso a la infrastructura y librerías necesarias para que la aplicación realmente sea funcional. Contendrá los siguientes elementos:

**DbContext**: Es la clase personalizada que haremos heredar de la clase *DbContext* que nos ofrece la librería **Entity Framework Core**. Esta clase se encargará del acceso a la base de datos.

**Repositories**: Son las clases que implementarán las interfaces de la capa **domain**, haciendo uso del *DbContext* para acceder a los datos mediante los métodos. Las responsabilidades de estas clases se limitan a operaciones CRUD, nada más.

**Providers o Services**: Son clases que proveerán acceso a demás aspectos relacionados a **infrastructure** que **no sea la conexión a la base de datos** como por ejemplo una clase para gestionar los archivos (fotos) de la app y otra para gestionar la autenticación, los roles, etc.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

###### Paquetes usados en Infrastructure

**Entity Framework Core**: Usaremos este ORM para conectarnos a la base de datos.

**Entity Framework Core NgPsql**: Usaremos este paquete para poder acceder a nuestro PostgreSQL.

**FirebaseAdmin**: Usaremos este paquete para conectarnos a Firebase.

**CloudinaryDotnet**: Usaremos este paquete para conectarnos con Cloudinary.

###### Dependencias en Infrastructure

- Domain
- CrossCutting

##### **Features**
Es la capa que hará uso de los elementos de la capa **domain** e **infrastructure** para hacer funcionalidades concretas, como registrar usuario o iniciar sesión.

Por cada **feature** en esta capa se creará una carpeta donde se agruparán todas las *features* relacionadas, por ejemplo, crearemos una carpeta **Users** que contendrá, por ejemplo, las **features** **Create y Login**. Notése que los nombres de las carpetas que agrupan las *features* debe de ser en **plural** y las *features* deben tener nombres signficativos, que ayuden a identificar rápidamente el código que estará almacenado dentro de ellas. 

En las carpetas que agrupan una *feature* debe contener todo el código relevante a esa *feature*, de modo que si se necesita hacer un cambio en esa funcionalidad, en esa carpeta este todo lo que necesito para hacerlo. Por ejemplo en una **feature Register** de *Users* debería de estar su *Command*, su *Command Handler**, *Result*, etc. De modo que no tenga que ir a ninguna otra parte si quiero cambiar el código, agregarle una nueva funcionalidad o simplemente leer y comprender la funcionalidad.

Comentado esto, veamos los demás elementos que compondrán esta capa:

**Commands**: Son records que contienen toda la información necesaria para hacer una operación de escritura (creación, actualización o borrado) en la infraestructura, como registrar un usuario o actualizar su nombre. Estos records deben heredar de la interfaz *ICommand* para poder ser procesados por el handler.

**Queries**: Son records que contienen toda la información necesaria para hacer una operación de lectura (query) en la infraestructura, como recuperar una foto o un usuario. Estos records deben heredar de la interfaz **IQuery** para poder ser procesados por el handler y siempre devuelven un resultado.

**Command Handlers**: Son handlers que proveé MediatR para responder y procesar las solicitudes hechas por los *Commands*, lo utilizaremos en operaciones de escritura. Deben heredar de la interfaz *ICommandHandler* en la que especificarás el tipo que recibe (requerido) y el tipo que devuelve (opcional, en caso de no especificarlo se devolvera *Unit*, que es igual a no devolver nada). Pueden o no devolver un resultado dependiendo de cual de las dos interfaces *ICommandHandler* se utilicé

**Query Handlers**: Son handlers que proveé MediatR para responder y procesar las solicitudes hechas por las *Queries*, lo utilizaremos para operaciones de escritura. Deben heredar de la interfaz **IQueryHandler**, en la que debemos especificar tanto el tipo que recibe como el tipo que retornará, ya que siempre devuelven un resultado.

**Event Handlers**: Son handlers que procesan y responden a los eventos que se disparán a través de MediatR después de cierta acción.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

###### Paquetes usados en Features

**MediatR**: Usaremos esta librería para la gestión de CQRS y todo lo que tiene que ver con los events.

**FluentValidation**: Usaremos esta librería para facilitar las validaciones.

**Mapster**: Usaremos esta librería para mapear las distintas clases **sin necesidad de configuración como en AutoMapper de seguir ciertos estándares (como que las propiedades clases tengan los mismos nombres)**.

###### Dependencias en Features

- Domain
- CrossCutting
- Infrastructure
  
##### **API**
Esta es la capa que expondrá los endpoints correspondientes, utilizaremos las **Minimal APIs** de ASP.NET Core. Contiene los siguientes elementos:

**Endpoint**: Son los módulos o endpoints que expondrán el punto de entrada a todas las demás capas que comentamos anteriormente.

**Results**: Son las clases que utilizaremos para devolver los resultados finales en formato JSON al frontend, a petición de uno de los integrantes del grupo.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

###### Paquetes usados en API

**MediatR**: Utilizado aquí para proveer las configuraciones e inyección de dependencias necesaria.

**FluentValidation**: Utilizado aquí para proveer las configuraciones e inyección de dependencias necesaria.

**Mapster**: Utilizado aquí para proveer las configuraciones e inyección de dependencias necesaria.

**FirebaseAdmin**: Utilizado aquí para proveer las configuraciones e inyección de dependencias necesaria.

**CloudinaryDotNet**: Utilizado aquí para proveer las configuraciones e inyección de dependencias necesaria.

**Microsoft.AspNetCore.OpenApi**: Paquete instalado por defecto para probar los endpoints de la API.

**Microsoft.Extensions.Http.Polly**: Paquete instalado para proveer resiliencia y rate limiting, entre otras cosas. Aumentando la seguridad y usabilidad de la aplicación.

**Serilog**: Utilizado aquí para proveer las configuraciones e inyección de dependencias necesaria.

**Serilog.Sinks.Console**: Utilizado aquí para proveer las configuraciones e inyección de dependencias necesaria.

**System.Text.Json**: Paquete para gestionar la serialización JSON.

**Carter**: Paquete para poder crear los endpoints de la minimal API en archivos separados, de forma modular y escalable.

###### Dependencias en API

- Domain
- CrossCutting
- Infrastructure
- Features

##### **Cross Cutting**
Es una capa que utilizaremos para colocar código transversal a las distintas capas. Se compone, pero no se limita a los siguientes elementos:

**Logging**: Las clases relevantes para proveer logs al backend de la app.

**Validators**: Las clases que harán las validaciones que se ejecutarán antes de procesar una solicitud.

**Error handlers**: Las clases que manejarán los errores, en caso de presentarse, devolviendo una respuesta adecuada.

**Middlewares**: Middlewares personalizados que crearemos, de ser necesarios, durante el desarrollo de la app.

**Results**: Resultados que pueden devolverse en caso de, por ejemplo, no pasar una validación.

**CQRS**: Aquí definiremos todas las interfaces relevantes a CQRS como *ICommand, IQuery, ICommandHandler, IQueryHandler y IEventHandler*

**Extension Methods**: Métodos de extensión relevantes a esta capa.

**Cualquier otro elemento que sea reutilizable y transversal a varias capas**

###### Paquetes usados en Cross Cutting

**MediatR**

**Mapster**

**FluentValidation**

**Serilog**: Paquete para gestionar los logs de la app.

**Serilog.Sinks.Console**: Paquete para gestionar los logs de la app a nivel de consola, dado que no es una app de producción no necesitaremos otros Sinks para centralizar los logs, basta con los de consola.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

###### Dependencias en Cross Cutting

- Domain
  
##### **Tests**
Es la capa que utilizaremos para hacer las pruebas unitarias y de integración, requeridas en el proyecto final TDS. Se compone de estos elementos:

**Extension Methods**: Métodos de extensión relevantes a esta capa.

###### Paquetes usados en Tests
**xUnit**: Librería para hacer pruebas unitarias en C#.

**Moq**: Librería para mockear las dependencias en pruebas unitarias.

**TestContainers**: Librería para hacer tests de integración en C# usando contenedores. Debes tener **Docker** instalado para que funcioné.

###### Dependencias en Tests

- Domain
- Features
- Infrastructure
- CrossCutting
- API

#### Más sobre la arquitectura del backend
https://antondevtips.com/blog/the-best-way-to-structure-your-dotnet-projects-with-clean-architecture-and-vertical-slices

### Frontend

#### Patrones del frontend

**Smart Components**: Son componentes que tienen conocimiento o contienen alguna lógica de negocio, como un componente que sirva de card para mostrar datos de un condominio, etc.

**Dumb components**: Son componentes agnósticos a la lógica de negocio, como por ejemplo un componente que sea una campanita para mostrar y manejar las notificaciones, en principio debería de funcionar independientemente del tipo de la notificación, sea por un mensaje, un evento de un condominio, etc.

**Singleton**: Este patrón consiste en siempre proveer una misma instancia de una clase, es provisto por el sistema de inyección de dependencias de Angular, usando *providedIn: 'root'*.

**Inyección de dependencias**: Ya descrito antes, Angular también provee inyección de dependencias a través del decorador *@Injectable*.

**Observer**: Es un patrón que permite a los *suscriptores* reaccionar automáticamente a cambios hechos en el observable al que estan suscritos. Angular se basa mucho en este patrón, usando paquetes incluidos como RxJs y sus distintos operadores, así como las nuevas signals.

**Directives**: Permite crear clases reutilizables que alterán el comportamiento de un componente sin necesidad de cambiar su código interno. Angular permite crear *directivas* a través del decorador *@Directive*

**Interceptor**: Consiste en interceptar (usualmente solicitudes HTTP o cargas de páginas) y cambiar su comportamiento. Angular proveé este patrón a través de los *Interceptors* (para modificar solicitudes HTTP) y los *Guards* (usualmente para controlar el acceso a las vistas).

**Lazy Loading**: Consiste en cargar los componentes solamente cuando sean necesarios, de modo que si por ejemplo tienes un componente LoginPage, este nunca se cargará si el usuario nunca entra a una página que utilicé este componente, aumentando el rendimiento y reduciendo el tamaño de la compilación y las descargas.

**Pipes**: Son clases reutilizables que alteran el *formato* de la vista de los datos sin alterar los datos en sí, por ejemplo, pueden alterar una variable money que tiene 750 como valor para que el usuario vea por ejemplo "$750" pero el valor de la variable siga siendo el número 750, evitando la necesidad de pasar el valor a string, concatenarlo, etc.

**Facade**: Consiste en una interfaz o clase que centraliza y abstrae la complejidad de los distintos servicios, de modo que si tenemos varios servicios relacionados (caso que pasa muy típicamente en Angular) como un UserService, un EventService y un RoleService, podemos centralizar, integrar y llamar a todos esos servicios desde una sola clase que servirá de Facade para los demás servicios.

#### Diagrama del frontend

![image](https://github.com/user-attachments/assets/24915c8c-a94d-4a84-aa68-f878036f5ae4)

#### Capas del frontend
En el frontend, la arquitectura se dividirá en cuatro grandes capas:

##### Core 

En esta capa se trabajarán las características o *features* no específicas de la lógica de negocio de la aplicación, como la autenticación, la página para registro de usuario, los servicios para la autenticación, la moderación, el resumen diario del chat, etc.

##### Features 

En esta capa se trabajarán características o *features* propias de la lógica de negocio de la aplicación, como las relacionadas a los condominios, la página para crear condominios, los servicios para conectarse al endpoint que creé el condominio, etc.
También colocaremos aquí los llamados *Smart Components*.

##### Shared

En esta capa se colocará todo el código que es reutilizable en varias *features* de las capas **Core** y **Features**, por ejemplo, directivas, pipes, o también los llamados *Dumb Components*.

##### State
En esta carpeta se centralizará lo relacionado a la gestión del estado de la aplicación, independientemente de que decidamos utilizar después para gestionar el estado (BehaviorSubjects, Services, NgRx, etc.)

#### Paquetes usados en el frontend
- **PrimeNG**: Es una librería UI moderna que proveé componentes y piezas UI ya hechas con interactividad y lógica ya implementadas
- **NgxPermissions**: Librería de Angular para gestionar permisos.

#### Más sobre la arquitectura del frontend
https://www.gerome.dev/blog/standalone-angular-folder-structure/

### Branching
El manejo de las ramas (branches) será muy simple, lo manejaremos por *features*, de la siguiente manera:

- Tendremos una rama principal (master) que contendrá todos los cambios integrados previamente en la rama *develop*, siendo esta la rama que mostraremos ante el ITLA.
- Tendremos una rama de desarrollo (develop), que será la rama donde integraremos todos los cambios de las *features* que cada uno trabaje.
- Por cada feature a trabajar, se abrira una rama con el formato *feature/name-of-feature*, en la que se trabajará la *feature* respectiva, por ejemplo, para trabajar la feature de autenticación, podemos crear una rama *feature/auth*. Todos los integrantes involucrados en una *feature* específica trabajarán en base a esa rama u opcionalmente, pueden segmentar la rama aún más, usando el siguiente formato *feature/name-of-feature/name-of-developer* en caso de que varios integrantes esten involucrados en una misma *feature*, por ejemplo, *feature/auth/adrian* en caso de que yo quiera mantener mi propia branch para desarrollar mi parte de la funcionalidad.
- Al acabar una *feature*, se integran esos cambios a la rama *develop*. Pudiendo preservar la rama de la *feature* o borrarla despues de ello.
- **Las *features* solo se podrán integrar a la rama *develop*, no se puede integrar cambios de las *features* directamente a la rama *master*. Solo la rama *develop* puede integrarse a la rama master**.
- **Solo integraremos la rama *develop* a la rama *master* cuando tengamos que mostrar avances al ITLA y en la presentación final**.

![image](https://github.com/user-attachments/assets/560d86a2-49a3-46b6-bfaa-28ce857ba867)

#### Crear y cambiar a rama develop
```
git fetch
git checkout -b develop origin/develop
```
#### Crear ramas para features
```
git checkout develop
git pull origin develop
git checkout -b feature/name-of-feature
```
Alternativamente, en caso de querer segmentar la *feature* y la rama por integrante, tambien se puede usar:

```
git checkout feature/name-of-feature
git checkout -b feature/name-of-feature/name-of-developer
```

#### Confirmar cambios
```
git add .
git commit
```
Se escribe un mensaje **DESCRIPTIVO** que dé la suficiente información acerca de que se hizo en el commit en el formato que enseñaremos más tarde. Le damos Ctrl + X para confirmar el commit y subimos el cambio.

#### Subir cambios
```
git push -u origin feature/name-of-feature
```
O, si la rama local ya esta sincronizada con la rama remota:
```
git push origin feature/name-of-feature
```

#### Finalizar feature e integrarla a develop
```
git checkout develop
git merge feature_branch
git push origin develop
```
### Commits
Los commits deben ser lo más pequeños posible, por favor, evitar commits en los que se hagan demasiadas cosas.

#### Tipos de commits
Utilizaremos los llamados *conventional commits* para cada *commit* que se haga en el proyecto, de manera que brinden la mayor cantidad de información posible sobre que se hizo con solo verlos:

**feat**: Añadir una nueva funcionalidad.
Ejemplo: feat: add authentication module

**fix**: Corregir un error.
Ejemplo: fix: resolve issue with login timeout

**docs**: Cambios en la documentación.
Ejemplo: docs: update README with setup instructions

**style**: Cambios que no afectan la lógica del código (formateo, espacios, etc.).
Ejemplo: style: format code using Prettier

**refactor**: Mejorar el código sin cambiar su funcionalidad.
Ejemplo: refactor: simplify user validation logic

**test**: Agregar o corregir pruebas.
Ejemplo: test: add unit tests for login service

**chore**: Cambios menores o tareas rutinarias (actualización de dependencias, configuración).
Ejemplo: chore: update dependencies

**perf**: Cambios que mejoran el rendimiento.
Ejemplo: perf: optimize database queries

##### Formato de los commits
Los commits deberán de tener el siguiente formato:
```
**type(module)**: Title
Description

- Task1
- Task2
- Task3
````

**type**: Es el tipo de commit relacionado al cambio que se hizo, por ejemplo si es un *feat*, *refactor*, *test*, etc. Es **requerido**.

**module**: Información sobre el módulo o característica que se esta trabajando, ej: *feat(authentication*, *feat(mute-user)* o *feat(comment)*. Es **opcional** pero **recomendado**.

**Description**: Descripción detallada del cambio que se hizo en ese commit. Es **requerido**.

**Tasks**: Una lista de tareas o cosas que se hicieron en ese commit, que da más información a la descripción principal. Es **opcional** pero **recomendado**.

##### Ejemplos de commits
###### Buen commit
```
feat(auth): add JWT authentication

- Implemented JWT token generation and validation
- Created login endpoint for user authentication
- Updated authentication middleware to validate JWT tokens
```

Este commit es buena práctica y debería de ser así los commits que haremos todos, porque es pequeño, específico, y proporciona información clara sobre lo que se hizo.

###### Mal commit
```
feat: add authentication, improve login, update UI, and fix bugs

- Added authentication system
- Updated login page with new design
- Fixed issues in the user dashboard
- Improved UI responsiveness
```
Este commit es pésimo, porque está haciendo demasiados cambios en un solo commit:

- Está cubriendo más de una característica.
- Mezcla funcionalidad nueva con corrección de errores y mejoras en el diseño de la UI
- Debería dividirse en commits más pequeños y específicos.
