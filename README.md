![04 Hotfix branches](https://github.com/user-attachments/assets/a4665e9b-d084-447c-9d61-4cf140b87a10)# Documentación
## Índice
- [¿De que tratá?](#¿de-que-trata?)
- [¿Por qué?](#¿por-qué?)
- [Arquitectura y estructura del proyecto](#arquitectura-y-estructura-del-proyecto)
  - [Tecnologías a utilizar](#tecnologías-a-utilizar)
- [Backend](#backend)
  - [Patrones](#patrones)
  - [Diagrama](#diagramas)
  - [Capas](#capas)
    - [Domain](#domain)
      - [Paquetes usados](#paquetes-usados)
      - [Dependencias](#dependencias)
    - [Infrastructure](#infrastructure)
      - [Paquetes usados](#paquetes-usados)
      - [Dependencias](#dependencias)
    - [Features](#features)
      - [Paquetes usados](#paquetes-usados)
      - [Dependencias](#dependencias)
    - [API](#api)
      - [Paquetes usados](#paquetes-usados)
      - [Dependencias](#dependencias)
    - [Cross Cutting](#cross-cutting)
      - [Paquetes usados](#paquetes-usados)
      - [Dependencias](#dependencias)
    - [Tests](#tests)
      - [Paquetes usados](#paquetes-usados)
      - [Dependencias](#dependencias)
    - [Más recursos al respecto](#más-recursos-al-respecto)
- [Frontend](#frontend)
  - [Patrones](#patrones)
  - [Diagrama](#diagramas)
  - [Capas](#capas)
    - [Core](#core)
    - [Features](#feature)
    - [Shared](#shared]
    - [State](#state)
  - [Librerías](#líbrerias)
  - [Más recursos al respecto](#más-recursos-al-respecto)
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
  
## Backend

### Patrones
Dado que se evaluarán estos aspectos, es necesario clarificar que patrones estaremos utilizando.
**Repository**: Patrón usado para abstraer el acceso a los datos.

**CQRS**: Patrón usado para segregar las operaciones de escritura (Commands) de las operaciones de lecture (Queries).

**Mediator**: Patrón usado para desacoplar el código de los endpoints a través de handlers.

**Middleware**: Patrón usado para ejecutar ciertas acciones antes o después de procesar una solicitud. Lo usaremos para ejecutar validaciones antes de procesar realmente una solicitud a la API.

**Strategy**: Patrón usado para tomar diferentes "estrategias" o "implementaciones" dependiendo de una elección tomada por el usuario. Lo podemos usar, por ejemplo, para diferenciar la publicación un comentario sin ninguna foto adjunta de un comentario con una foto adjunta.

**Inyección de dependencias**: Patrón usado para proveer las dependencias necesarias a una clase sin que esta se tenga que preocupar por eso.

### Diagrama

Para desarrollar el backend utilizaremos C# con su framework ASP.NET Core para desarrollar la Web API que soportará la aplicación. A continuación, hablaremos más a detalle de que hara cada una de las capas que sale en la imagen:
![Imagen de WhatsApp 2025-01-21 a las 13 28 18_11211e3b](https://github.com/user-attachments/assets/b80c1c45-6f55-48bb-89d2-1b0240ff2cd3)
### Capas

#### **Domain**
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

##### Paquetes usados:

**MediatR**: Usaremos la librería **MediatR**, que es la única dependencia que tiene esta capa para disparar los *Domain Events*.

##### Dependencias:

- Ninguna

#### **Infrastructure**
Es la capa que proveerá todo el acceso a la infrastructura y librerías necesarias para que la aplicación realmente sea funcional. Contendrá los siguientes elementos:

**DbContext**: Es la clase personalizada que haremos heredar de la clase *DbContext* que nos ofrece la librería **Entity Framework Core**. Esta clase se encargará del acceso a la base de datos.

**Repositories**: Son las clases que implementarán las interfaces de la capa **domain**, haciendo uso del *DbContext* para acceder a los datos mediante los métodos. Las responsabilidades de estas clases se limitan a operaciones CRUD, nada más.

**Providers o Services**: Son clases que proveerán acceso a demás aspectos relacionados a **infrastructure** que **no sea la conexión a la base de datos** como por ejemplo una clase para gestionar los archivos (fotos) de la app y otra para gestionar la autenticación, los roles, etc.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

##### Librerías

**Entity Framework Core**: Usaremos este ORM para conectarnos a la base de datos.

**Entity Framework Core NgPsql**: Usaremos este paquete para poder acceder a nuestro PostgreSQL.

**FirebaseAdmin**: Usaremos este paquete para conectarnos a Firebase.

**CloudinaryDotnet**: Usaremos este paquete para conectarnos con Cloudinary.

##### Dependencias

- Domain
- CrossCutting

#### **Features**
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

##### Librerías

**MediatR**: Usaremos esta librería para la gestión de CQRS y todo lo que tiene que ver con los events.

**FluentValidation**: Usaremos esta librería para facilitar las validaciones.

**Mapster**: Usaremos esta librería para mapear las distintas clases **sin necesidad de configuración como en AutoMapper de seguir ciertos estándares (como que las propiedades clases tengan los mismos nombres)**.

##### Dependencias

- Domain
- CrossCutting
- Infrastructure
  
#### **API**
Esta es la capa que expondrá los endpoints correspondientes, utilizaremos las **Minimal APIs** de ASP.NET Core. Contiene los siguientes elementos:

**Endpoint**: Son los módulos o endpoints que expondrán el punto de entrada a todas las demás capas que comentamos anteriormente.

**Results**: Son las clases que utilizaremos para devolver los resultados finales en formato JSON al frontend, a petición de uno de los integrantes del grupo.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

##### Librerías

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

##### Dependencias

- Domain
- CrossCutting
- Infrastructure
- Features

#### **Cross Cutting**
Es una capa que utilizaremos para colocar código transversal a las distintas capas. Se compone, pero no se limita a los siguientes elementos:

**Logging**: Las clases relevantes para proveer logs al backend de la app.

**Validators**: Las clases que harán las validaciones que se ejecutarán antes de procesar una solicitud.

**Error handlers**: Las clases que manejarán los errores, en caso de presentarse, devolviendo una respuesta adecuada.

**Middlewares**: Middlewares personalizados que crearemos, de ser necesarios, durante el desarrollo de la app.

**Results**: Resultados que pueden devolverse en caso de, por ejemplo, no pasar una validación.

**CQRS**: Aquí definiremos todas las interfaces relevantes a CQRS como *ICommand, IQuery, ICommandHandler, IQueryHandler y IEventHandler*

**Extension Methods**: Métodos de extensión relevantes a esta capa.

**Cualquier otro elemento que sea reutilizable y transversal a varias capas**

##### Librerías

**MediatR**

**Mapster**

**FluentValidation**

**Serilog**: Paquete para gestionar los logs de la app.

**Serilog.Sinks.Console**: Paquete para gestionar los logs de la app a nivel de consola, dado que no es una app de producción no necesitaremos otros Sinks para centralizar los logs, basta con los de consola.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

##### Dependencias

- Domain
  
#### **Tests**
Es la capa que utilizaremos para hacer las pruebas unitarias y de integración, requeridas en el proyecto final TDS. Se compone de estos elementos:

**Extension Methods**: Métodos de extensión relevantes a esta capa.

##### Librerías
**xUnit**: Librería para hacer pruebas unitarias en C#.

**Moq**: Librería para mockear las dependencias en pruebas unitarias.

**TestContainers**: Librería para hacer tests de integración en C# usando contenedores. Debes tener **Docker** instalado para que funcioné.

##### Dependencias

- Domain
- Features
- Infrastructure
- CrossCutting
- API

### Más recursos al respecto
https://antondevtips.com/blog/the-best-way-to-structure-your-dotnet-projects-with-clean-architecture-and-vertical-slices

## Frontend

### Patrones

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

### Diagrama

![image](https://github.com/user-attachments/assets/24915c8c-a94d-4a84-aa68-f878036f5ae4)

### Capas
En el frontend, la arquitectura se dividirá en cuatro grandes capas:

#### Core 

En esta capa se trabajarán las características o *features* no específicas de la lógica de negocio de la aplicación, como la autenticación, la página para registro de usuario, los servicios para la autenticación, la moderación, el resumen diario del chat, etc.

#### Features 

En esta capa se trabajarán características o *features* propias de la lógica de negocio de la aplicación, como las relacionadas a los condominios, la página para crear condominios, los servicios para conectarse al endpoint que creé el condominio, etc.
También colocaremos aquí los llamados *Smart Components*.

#### Shared

En esta capa se colocará todo el código que es reutilizable en varias *features* de las capas **Core** y **Features**, por ejemplo, directivas, pipes, o también los llamados *Dumb Components*.

#### State
En esta carpeta se centralizará lo relacionado a la gestión del estado de la aplicación, independientemente de que decidamos utilizar después para gestionar el estado (BehaviorSubjects, Services, NgRx, etc.)

### Librerías
- **PrimeNG**: Es una librería UI moderna que proveé componentes y piezas UI ya hechas con interactividad y lógica ya implementadas
- **NgxPermissions**: Librería de Angular para gestionar permisos.

### Más recursos al respecto
https://www.gerome.dev/blog/standalone-angular-folder-structure/

## Flujo de trabajo de Git
El flujo de trabajo que utilizaremos en Git será **Gitflow**:

![Uploading <?xml version="1.0" encoding="utf-8"?>
<!-- Generator: Adobe Illustrator 25.2.3, SVG Export Plug-In . SVG Version: 6.00 Build 0)  -->
<svg version="1.1" id="Lager_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px"
	 viewBox="0 0 800 574" style="enable-background:new 0 0 800 574;" xml:space="preserve">
<style type="text/css">
	.st0{fill:none;stroke:#404040;stroke-width:4;stroke-miterlimit:10;}
	.st1{fill:#4ED1A1;stroke:#404040;stroke-width:4;stroke-miterlimit:10;}
	.st2{fill:#B18BE8;stroke:#404040;stroke-width:4;stroke-miterlimit:10;}
	.st3{fill:none;stroke:#CCCCCC;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-dasharray:0,14.3051;}
	.st4{fill:none;stroke:#CCCCCC;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;}
	.st5{fill:none;stroke:#CCCCCC;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-dasharray:0,14.1689;}
	.st6{fill:none;stroke:#CCCCCC;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-dasharray:0,13.9788;}
	.st7{fill:none;stroke:#CCCCCC;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-dasharray:0,14.7877;}
	.st8{fill:none;stroke:#CCCCCC;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-dasharray:0,14.9632;}
	.st9{fill:#B3E3FF;stroke:#404040;stroke-width:4;stroke-miterlimit:10;}
	.st10{fill:#B3E3FF;stroke:#404040;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-miterlimit:10;}
	.st11{fill:#404040;}
	.st12{fill:none;stroke:#404040;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-miterlimit:10;}
	.st13{fill:none;stroke:#CCCCCC;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-dasharray:0,12.543;}
	.st14{fill:none;stroke:#CCCCCC;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-dasharray:0,13.6844;}
	.st15{fill:none;stroke:#CCCCCC;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-dasharray:0,13.7717;}
	.st16{fill:none;stroke:#CCCCCC;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-dasharray:0,13.6492;}
	.st17{fill:none;stroke:#CCCCCC;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-dasharray:0,13.907;}
	.st18{fill:#4CD3D6;stroke:#404040;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-miterlimit:10;}
	.st19{fill:#B18BE8;stroke:#404040;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-miterlimit:10;}
	.st20{fill:#FC8363;stroke:#404040;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-miterlimit:10;}
	.st21{fill:#4ED1A1;stroke:#404040;stroke-width:4;stroke-linecap:round;stroke-linejoin:round;stroke-miterlimit:10;}
	.st22{fill:#4CD3D6;stroke:#404040;stroke-width:4;stroke-linejoin:round;stroke-miterlimit:10;}
	.st23{fill:#414141;}
</style>
<path class="st0" d="M424.4,459.9c0-17,13.8-30.8,30.8-30.8"/>
<path class="st0" d="M254.1,459.9c0-17-13.8-30.8-30.8-30.8 M164.5,552.1h213.9"/>
<circle class="st1" cx="315.8" cy="552.1" r="15.8"/>
<circle class="st1" cx="362.7" cy="552.1" r="15.8"/>
<circle class="st1" cx="242.5" cy="552.1" r="15.8"/>
<path class="st0" d="M776.2,429.1H179.9"/>
<circle class="st2" cx="485.2" cy="429.1" r="15.8"/>
<circle class="st2" cx="315.8" cy="429.1" r="15.8"/>
<circle class="st2" cx="776.2" cy="429.1" r="15.8"/>
<path class="st0" d="M254.1,459.9c0,17,13.8,30.8,30.8,30.8h107.9 M84.9,275c0,17,13.8,30.8,30.8,30.8H133"/>
<circle class="st1" cx="315.8" cy="490.6" r="15.8"/>
<circle class="st1" cx="362.7" cy="490.6" r="15.8"/>
<path class="st0" d="M392.8,490.6h0.9c17,0,30.8-13.8,30.8-30.8"/>
<path class="st3" d="M57.2,306.1H21.5"/>
<path class="st4" d="M776.3,306.1"/>
<path class="st5" d="M762.1,306.1H244.9"/>
<path class="st4" d="M545.5,367.6"/>
<path class="st6" d="M531.5,367.6H21.3"/>
<path class="st4" d="M776.3,367.6"/>
<path class="st7" d="M761.5,367.6h-51.8"/>
<path class="st4" d="M776.2,244.6"/>
<path class="st8" d="M761.3,244.6H679"/>
<path class="st0" d="M84.9,275.4c0-17-13.8-30.8-30.8-30.8 M211.7,275c0,17-13.8,30.8-30.8,30.8h-48 M211.7,275.4
	c0-17,13.8-30.8,30.8-30.8 M273.2,429.1c-17,0-30.8-13.8-30.8-30.8v-61.8 M242.4,336.5c0-17-13.8-30.8-30.8-30.8"/>
<circle class="st1" cx="148.7" cy="552.1" r="15.8"/>
<path class="st0" d="M645,244.6H39.2"/>
<ellipse transform="matrix(0.9999 -1.514629e-02 1.514629e-02 0.9999 -3.7021 0.384)" class="st9" cx="23.5" cy="244.6" rx="15.8" ry="15.8"/>
<circle class="st9" cx="315.8" cy="244.6" r="15.8"/>
<circle class="st9" cx="645" cy="244.6" r="15.8"/>
<path class="st10" d="M49,139.1h42.5v37.5H49V139.1z"/>
<path class="st11" d="M62.6,162.4h-1.2l-2.5-6.3h1.4l1.8,4.8l1.8-4.8h1.3L62.6,162.4z M67.3,155.1c0.5-0.8,1.4-1.4,2.6-1.4
	s2,0.5,2.6,1.4c0.6,0.8,0.8,1.9,0.8,3.1s-0.2,2.2-0.8,3.1c-0.5,0.8-1.4,1.4-2.6,1.4s-2-0.5-2.6-1.4c-0.6-0.8-0.8-1.9-0.8-3.1
	S66.7,155.9,67.3,155.1z M68.5,160.9c0.3,0.3,0.7,0.5,1.3,0.5s1-0.2,1.3-0.5c0.5-0.5,0.7-1.4,0.7-2.7s-0.2-2.2-0.7-2.7
	c-0.3-0.3-0.7-0.5-1.3-0.5s-1,0.2-1.3,0.5c-0.5,0.5-0.7,1.4-0.7,2.7S68,160.3,68.5,160.9z M75.8,160.6c0.5,0,0.9,0.4,0.9,0.9
	s-0.4,0.9-0.9,0.9s-0.9-0.4-0.9-0.9S75.3,160.6,75.8,160.6z M79.7,162.4v-5.9h-2v-0.9c1.2,0,2-0.7,2.2-1.6H81v8.4
	C81,162.4,79.7,162.4,79.7,162.4z"/>
<path class="st12" d="M70.3,210.7v-17.6 M78.5,204.2l-8.2,8.2l-8.1-8.2"/>
<path class="st10" d="M294.6,139.1h42.5v37.5h-42.5V139.1z"/>
<path class="st11" d="M306.5,162.4h-1.2l-2.5-6.3h1.4l1.8,4.8l1.8-4.8h1.3L306.5,162.4L306.5,162.4z M311.2,155.1
	c0.5-0.8,1.4-1.4,2.6-1.4s2,0.5,2.6,1.4c0.6,0.8,0.8,1.9,0.8,3.1s-0.2,2.2-0.8,3.1c-0.5,0.8-1.4,1.4-2.6,1.4s-2-0.5-2.6-1.4
	c-0.6-0.8-0.8-1.9-0.8-3.1S310.6,155.9,311.2,155.1z M312.4,160.9c0.3,0.3,0.7,0.5,1.3,0.5s1-0.2,1.3-0.5c0.5-0.5,0.7-1.4,0.7-2.7
	s-0.2-2.2-0.7-2.7c-0.3-0.3-0.7-0.5-1.3-0.5s-1,0.2-1.3,0.5c-0.5,0.5-0.7,1.4-0.7,2.7S311.9,160.3,312.4,160.9z M319.7,160.6
	c0.5,0,0.9,0.4,0.9,0.9s-0.4,0.9-0.9,0.9s-0.9-0.4-0.9-0.9S319.2,160.6,319.7,160.6z M322.7,156.8v-0.3c0-1.4,1.1-2.7,2.9-2.7
	s2.8,1.2,2.8,2.6c0,1.1-0.6,2-1.6,2.6l-1.7,1.1c-0.5,0.3-0.9,0.7-1,1.2h4.3v1.2h-5.9c0-1.4,0.6-2.4,2-3.4l1.4-0.9
	c0.8-0.5,1.2-1.1,1.2-1.8c0-0.8-0.5-1.5-1.6-1.5s-1.6,0.8-1.6,1.7v0.4L322.7,156.8L322.7,156.8z"/>
<path class="st12" d="M315.8,210.7v-17.6 M324,204.2l-8.2,8.2l-8.1-8.2"/>
<path class="st10" d="M623.8,139.1h42.5v37.5h-42.5V139.1z"/>
<path class="st11" d="M636.8,162.4h-1.2l-2.5-6.3h1.4l1.8,4.8l1.8-4.8h1.3L636.8,162.4z M642.4,162.4v-5.9h-2v-0.9
	c1.2,0,2-0.7,2.2-1.6h1.1v8.4H642.4z M647,160.6c0.5,0,0.9,0.4,0.9,0.9s-0.4,0.9-0.9,0.9s-0.9-0.4-0.9-0.9S646.5,160.6,647,160.6z
	 M650.6,155.1c0.5-0.8,1.4-1.4,2.6-1.4s2,0.5,2.6,1.4c0.6,0.8,0.8,1.9,0.8,3.1s-0.2,2.2-0.8,3.1c-0.5,0.8-1.4,1.4-2.6,1.4
	s-2-0.5-2.6-1.4c-0.6-0.8-0.8-1.9-0.8-3.1S650.1,155.9,650.6,155.1z M651.9,160.9c0.3,0.3,0.7,0.5,1.3,0.5s1-0.2,1.3-0.5
	c0.5-0.5,0.7-1.4,0.7-2.7s-0.2-2.2-0.7-2.7c-0.3-0.3-0.7-0.5-1.3-0.5s-1,0.2-1.3,0.5c-0.5,0.5-0.7,1.4-0.7,2.7
	S651.4,160.3,651.9,160.9z"/>
<path class="st12" d="M645,210.7v-17.6 M653.2,204.2l-8.2,8.2l-8.1-8.2"/>
<path class="st13" d="M64.5,429.1H20.6"/>
<path class="st4" d="M260.6,490.6"/>
<path class="st14" d="M246.9,490.6H21.2"/>
<path class="st4" d="M776.3,490.6"/>
<path class="st15" d="M762.5,490.6H425.1"/>
<path class="st4" d="M123.5,552.1"/>
<path class="st16" d="M109.9,552.1H21.1"/>
<path class="st4" d="M776.3,552.1"/>
<path class="st17" d="M762.3,552.1H393.8"/>
<path class="st18" d="M283.5,5.2h96.2v50h-96.2V5.2z"/>
<path class="st11" d="M308.8,31.8h-1.6v4.3h-1.5V25.6h4c2,0,3.3,1.4,3.3,3.2c0,1.5-1,2.7-2.6,3l2.5,4.5h-1.7L308.8,31.8L308.8,31.8z
	 M309.5,30.5c1.2,0,2-0.7,2-1.8s-0.8-1.8-2-1.8h-2.2v3.6C307.3,30.5,309.5,30.5,309.5,30.5z M321.6,34.1c-0.4,1.3-1.6,2.3-3.2,2.3
	c-1.9,0-3.6-1.4-3.6-3.9c0-2.3,1.6-3.8,3.5-3.8c2.2,0,3.5,1.5,3.5,3.8V33h-5.4c0,1.3,1,2.2,2.2,2.2s1.8-0.6,2-1.5L321.6,34.1z
	 M320.2,31.8c0-1-0.7-1.8-2-1.8c-1.2,0-1.9,0.9-2,1.8H320.2z M324.2,36.2V25.3h1.4v10.9H324.2z M335.1,34.1
	c-0.4,1.3-1.6,2.3-3.2,2.3c-1.9,0-3.6-1.4-3.6-3.9c0-2.3,1.6-3.8,3.5-3.8c2.2,0,3.5,1.5,3.5,3.8V33h-5.4c0,1.3,1,2.2,2.2,2.2
	s1.8-0.6,2-1.5L335.1,34.1z M333.7,31.8c0-1-0.7-1.8-2-1.8c-1.2,0-1.9,0.9-2,1.8H333.7z M339.4,32.1l2-0.3c0.4-0.1,0.6-0.3,0.6-0.5
	c0-0.7-0.5-1.3-1.6-1.3c-1,0-1.5,0.6-1.6,1.5l-1.4-0.3c0.2-1.4,1.4-2.3,3-2.3c2.2,0,3,1.2,3,2.6V35c0,0.6,0.1,1,0.1,1.2H342
	c0-0.2-0.1-0.5-0.1-1c-0.3,0.5-1,1.2-2.3,1.2c-1.5,0-2.4-1-2.4-2.2S338.2,32.2,339.4,32.1L339.4,32.1z M341.9,33.1v-0.3l-2.2,0.3
	c-0.6,0.1-1,0.4-1,1.1c0,0.5,0.5,1,1.2,1C341,35.2,341.9,34.7,341.9,33.1z M346.9,33.9c0.1,0.8,0.7,1.3,1.7,1.3c0.8,0,1.2-0.5,1.2-1
	c0-0.4-0.3-0.8-0.9-0.9l-1.2-0.3c-1.1-0.2-1.8-1-1.8-2c0-1.2,1.2-2.3,2.6-2.3c2,0,2.6,1.3,2.7,1.9l-1.2,0.5
	c-0.1-0.4-0.4-1.2-1.5-1.2c-0.7,0-1.2,0.5-1.2,1c0,0.4,0.3,0.8,0.8,0.9l1.2,0.3c1.3,0.3,2,1.1,2,2.1s-0.9,2.2-2.6,2.2
	c-2,0-2.8-1.3-2.9-2.1L346.9,33.9L346.9,33.9z M360,34.1c-0.4,1.3-1.6,2.3-3.2,2.3c-1.9,0-3.6-1.4-3.6-3.9c0-2.3,1.6-3.8,3.5-3.8
	c2.2,0,3.5,1.5,3.5,3.8V33h-5.4c0,1.3,1,2.2,2.2,2.2s1.8-0.6,2-1.5L360,34.1z M358.6,31.8c0-1-0.7-1.8-2-1.8c-1.2,0-1.9,0.9-2,1.8
	H358.6z"/>
<path class="st19" d="M421.3,5.2h96.2v50h-96.2V5.2z"/>
<path class="st11" d="M441.7,36.2V25.6h3.7c2.7,0,5.1,1.8,5.1,5.3s-2.4,5.3-5.1,5.3H441.7z M445.3,34.9c2,0,3.6-1.3,3.6-4
	s-1.6-4-3.6-4h-2.2v8H445.3z M459.3,34.1c-0.4,1.3-1.6,2.3-3.2,2.3c-1.9,0-3.6-1.4-3.6-3.9c0-2.3,1.6-3.8,3.4-3.8
	c2.2,0,3.5,1.5,3.5,3.8V33H454c0,1.3,1,2.2,2.2,2.2s1.8-0.6,2-1.5L459.3,34.1z M457.9,31.8c0-1-0.7-1.8-2-1.8c-1.2,0-1.9,0.9-2,1.8
	H457.9z M465.4,36.2H464l-3-7.2h1.6l2.1,5.6l2.1-5.6h1.5L465.4,36.2L465.4,36.2z M476.1,34.1c-0.4,1.3-1.6,2.3-3.2,2.3
	c-1.9,0-3.6-1.4-3.6-3.9c0-2.3,1.6-3.8,3.5-3.8c2.2,0,3.5,1.5,3.5,3.8V33h-5.4c0,1.3,1,2.2,2.2,2.2s1.8-0.6,2-1.5L476.1,34.1z
	 M474.7,31.8c0-1-0.7-1.8-2-1.8c-1.2,0-1.9,0.9-2,1.8H474.7z M478.7,36.2V25.3h1.4v10.9H478.7z M490.1,32.6c0,2.2-1.5,3.8-3.7,3.8
	s-3.7-1.6-3.7-3.8s1.5-3.8,3.7-3.8C488.5,28.7,490.1,30.4,490.1,32.6z M488.6,32.6c0-1.7-1-2.6-2.2-2.6s-2.2,0.9-2.2,2.6
	s1,2.6,2.2,2.6S488.6,34.2,488.6,32.6z M492.6,39V29h1.4v1.1c0.4-0.7,1.2-1.3,2.4-1.3c2.2,0,3.3,1.7,3.3,3.8s-1.2,3.8-3.4,3.8
	c-1.1,0-2-0.5-2.3-1.2V39H492.6z M496.1,30.1c-1.3,0-2.1,1.1-2.1,2.5c0,1.5,0.9,2.6,2.1,2.6c1.3,0,2.1-1.1,2.1-2.6
	C498.3,31.1,497.5,30.1,496.1,30.1z"/>
<path class="st20" d="M145.8,5.2H242v50h-96.2V5.2z"/>
<path class="st11" d="M180.9,36.2v-4.7h-5.4v4.7H174V25.6h1.5v4.5h5.4v-4.5h1.5v10.6H180.9z M192.4,32.6c0,2.2-1.5,3.8-3.7,3.8
	s-3.7-1.6-3.7-3.8s1.5-3.8,3.7-3.8C190.9,28.7,192.4,30.4,192.4,32.6z M191,32.6c0-1.7-1-2.6-2.2-2.6s-2.2,0.9-2.2,2.6
	s1,2.6,2.2,2.6S191,34.2,191,32.6z M196.8,29h1.6v1.3h-1.6V34c0,0.7,0.3,1,1,1c0.2,0,0.4,0,0.6-0.1v1.2c-0.1,0-0.5,0.1-1,0.1
	c-1.2,0-2-0.8-2-2.1v-4H194V29h0.4c0.8,0,1.1-0.5,1.1-1.1v-1.2h1.3L196.8,29L196.8,29z M204.6,25.4v1.3c-0.1,0-0.3-0.1-0.6-0.1
	c-0.5,0-1.1,0.2-1.1,1.2V29h4.8v7.2h-1.4v-5.9h-3.4v5.9h-1.5v-5.9h-1.3V29h1.3v-1.2c0-1.6,1.1-2.5,2.4-2.5
	C204.2,25.3,204.5,25.4,204.6,25.4z M206.9,25.2c0.6,0,1,0.4,1,1s-0.5,1-1,1c-0.6,0-1-0.5-1-1C205.9,25.6,206.4,25.2,206.9,25.2z
	 M212.4,32.5l-2.6-3.5h1.7l1.8,2.5L215,29h1.7l-2.6,3.5c0.4,0.6,2.2,3.1,2.7,3.7h-1.7l-1.9-2.7l-1.8,2.7h-1.7L212.4,32.5L212.4,32.5
	z"/>
<path class="st21" d="M559.1,5.2h96.2v50h-96.2V5.2z"/>
<path class="st11" d="M581.6,35.7V25.1h6.5v1.4h-5v3.4h4.5v1.4h-4.5v4.5L581.6,35.7L581.6,35.7z M596.3,33.6
	c-0.4,1.3-1.6,2.3-3.2,2.3c-1.9,0-3.6-1.4-3.6-3.9c0-2.3,1.6-3.8,3.5-3.8c2.2,0,3.5,1.5,3.5,3.8v0.5h-5.4c0,1.3,1,2.2,2.2,2.2
	s1.8-0.6,2-1.5L596.3,33.6z M594.8,31.3c0-1-0.7-1.8-2-1.8c-1.2,0-1.9,0.9-2,1.8H594.8z M600.6,31.6l2-0.3c0.4-0.1,0.6-0.3,0.6-0.5
	c0-0.7-0.5-1.3-1.6-1.3c-1,0-1.5,0.6-1.6,1.5l-1.3-0.3c0.2-1.4,1.4-2.3,3-2.3c2.2,0,3,1.2,3,2.6v3.6c0,0.6,0.1,1,0.1,1.2h-1.4
	c0-0.2-0.1-0.5-0.1-1c-0.3,0.5-1,1.2-2.3,1.2c-1.5,0-2.4-1-2.4-2.2C598.4,32.5,599.3,31.7,600.6,31.6z M603.1,32.6v-0.3l-2.2,0.3
	c-0.6,0.1-1,0.4-1,1.1c0,0.5,0.5,1,1.2,1C602.2,34.7,603.1,34.2,603.1,32.6z M609.3,28.5h1.6v1.3h-1.6v3.8c0,0.7,0.3,1,1,1
	c0.2,0,0.4,0,0.6-0.1v1.2c-0.1,0-0.5,0.1-1,0.1c-1.2,0-2-0.8-2-2.1v-4h-1.4v-1.3h0.4c0.8,0,1.1-0.5,1.1-1.1v-1.2h1.3L609.3,28.5
	L609.3,28.5z M616.1,35.9c-1.7,0-2.7-1.3-2.7-2.9v-4.5h1.4v4.3c0,1,0.5,1.9,1.6,1.9s1.7-0.8,1.7-1.8v-4.3h1.4v5.9
	c0,0.6,0,1.1,0.1,1.3h-1.3c0-0.2-0.1-0.6-0.1-0.9C617.9,35.6,617,35.9,616.1,35.9L616.1,35.9z M626.8,29.9h-0.6c-1.2,0-2,0.6-2,2.2
	v3.6h-1.4v-7.2h1.4v1.3c0.5-1.1,1.4-1.4,2.2-1.4h0.5v1.5L626.8,29.9z M635.2,33.6c-0.4,1.3-1.6,2.3-3.2,2.3c-1.9,0-3.6-1.4-3.6-3.9
	c0-2.3,1.6-3.8,3.5-3.8c2.2,0,3.5,1.5,3.5,3.8v0.5H630c0,1.3,1,2.2,2.2,2.2s1.8-0.6,2-1.5L635.2,33.6z M633.8,31.3
	c0-1-0.7-1.8-2-1.8c-1.2,0-1.9,0.9-2,1.8H633.8z"/>
<path class="st21" d="M696.8,5.2H793v50h-96.2V5.2z"/>
<path class="st11" d="M719.4,35.7V25.1h6.5v1.4h-5v3.4h4.5v1.4h-4.5v4.5L719.4,35.7L719.4,35.7z M734,33.6c-0.4,1.3-1.6,2.3-3.2,2.3
	c-1.9,0-3.6-1.4-3.6-3.9c0-2.3,1.6-3.8,3.4-3.8c2.2,0,3.5,1.5,3.5,3.8v0.5h-5.4c0,1.3,1,2.2,2.2,2.2s1.8-0.6,2-1.5L734,33.6z
	 M732.6,31.3c0-1-0.7-1.8-2-1.8c-1.2,0-1.9,0.9-2,1.8H732.6z M738.4,31.6l1.9-0.3c0.4-0.1,0.6-0.3,0.6-0.5c0-0.7-0.5-1.3-1.6-1.3
	c-1,0-1.5,0.6-1.6,1.5l-1.4-0.3c0.2-1.4,1.4-2.3,3-2.3c2.2,0,3,1.2,3,2.6v3.6c0,0.6,0.1,1,0.1,1.2H741c0-0.2-0.1-0.5-0.1-1
	c-0.3,0.5-1,1.2-2.3,1.2c-1.5,0-2.4-1-2.4-2.2C736.2,32.5,737.1,31.7,738.4,31.6z M740.9,32.6v-0.3l-2.2,0.3c-0.6,0.1-1,0.4-1,1.1
	c0,0.5,0.4,1,1.2,1C739.9,34.7,740.9,34.2,740.9,32.6z M747.1,28.5h1.6v1.3h-1.6v3.8c0,0.7,0.3,1,1,1c0.2,0,0.4,0,0.6-0.1v1.2
	c-0.1,0-0.5,0.1-1,0.1c-1.2,0-2-0.8-2-2.1v-4h-1.4v-1.3h0.4c0.8,0,1.1-0.5,1.1-1.1v-1.2h1.3L747.1,28.5L747.1,28.5z M753.9,35.9
	c-1.7,0-2.7-1.3-2.7-2.9v-4.5h1.4v4.3c0,1,0.5,1.9,1.6,1.9s1.7-0.8,1.7-1.8v-4.3h1.4v5.9c0,0.6,0,1.1,0.1,1.3h-1.3
	c0-0.2-0.1-0.6-0.1-0.9C755.7,35.6,754.8,35.9,753.9,35.9L753.9,35.9z M764.5,29.9h-0.6c-1.2,0-2,0.6-2,2.2v3.6h-1.4v-7.2h1.4v1.3
	c0.5-1.1,1.4-1.4,2.2-1.4h0.5v1.5L764.5,29.9z M773,33.6c-0.4,1.3-1.6,2.3-3.2,2.3c-1.9,0-3.6-1.4-3.6-3.9c0-2.3,1.6-3.8,3.5-3.8
	c2.2,0,3.5,1.5,3.5,3.8v0.5h-5.4c0,1.3,1,2.2,2.2,2.2s1.8-0.6,2-1.5L773,33.6z M771.6,31.3c0-1-0.7-1.8-2-1.8c-1.2,0-1.9,0.9-2,1.8
	H771.6z"/>
<path class="st0" d="M84.9,275v123.4c0,17,13.8,30.8,30.8,30.8h95.7"/>
<circle class="st2" cx="195.6" cy="429.1" r="15.7"/>
<circle class="st2" cx="148.7" cy="429.1" r="15.8"/>
<circle class="st20" cx="148.7" cy="306.1" r="15.8"/>
<path class="st0" d="M536.5,398.4c0,17-13.8,30.8-30.8,30.8 M706.7,398.4c0,17,13.8,30.8,30.8,30.8h23 M568.1,367.6h-0.9
	c-17,0-30.8,13.8-30.8,30.8 M706.7,398.4c0-17-13.8-30.8-30.8-30.8H568.1"/>
<circle class="st22" cx="645" cy="367.6" r="15.8"/>
<circle class="st22" cx="598.2" cy="367.6" r="15.8"/>
<path class="st0" d="M148.7,444.9v91.5 M164.5,305.7h47.2 M645,351.9v-91.5"/>
<g>
	<path class="st10" d="M8,5.2h96.2v50H8V5.2z"/>
	<g>
		<path class="st23" d="M40.9,36.3v-10h2l2.4,7.1c0.2,0.7,0.4,1.2,0.5,1.5c0.1-0.4,0.3-0.9,0.5-1.6l2.4-7h1.8v10h-1.3v-8.4l-2.9,8.4
			h-1.2l-2.9-8.5v8.5H40.9z"/>
		<path class="st23" d="M57.9,35.4c-0.5,0.4-0.9,0.7-1.3,0.8c-0.4,0.2-0.9,0.2-1.4,0.2c-0.8,0-1.4-0.2-1.8-0.6s-0.6-0.9-0.6-1.5
			c0-0.4,0.1-0.7,0.2-1c0.2-0.3,0.4-0.5,0.6-0.7c0.3-0.2,0.6-0.3,0.9-0.4c0.2-0.1,0.6-0.1,1.1-0.2c1-0.1,1.7-0.3,2.2-0.4
			c0-0.2,0-0.3,0-0.3c0-0.5-0.1-0.9-0.3-1.1c-0.3-0.3-0.8-0.4-1.4-0.4s-1,0.1-1.3,0.3c-0.3,0.2-0.5,0.6-0.6,1.1L53,31
			c0.1-0.5,0.3-0.9,0.5-1.2c0.3-0.3,0.6-0.6,1.1-0.7c0.5-0.2,1-0.3,1.6-0.3s1.1,0.1,1.5,0.2s0.7,0.3,0.9,0.6
			c0.2,0.2,0.3,0.5,0.4,0.8c0,0.2,0.1,0.6,0.1,1.1v1.6c0,1.1,0,1.9,0.1,2.2c0.1,0.3,0.2,0.6,0.3,0.9h-1.3
			C58,36.1,57.9,35.8,57.9,35.4z M57.8,32.7c-0.4,0.2-1.1,0.3-2,0.5c-0.5,0.1-0.9,0.2-1.1,0.2s-0.4,0.2-0.5,0.4S54,34.2,54,34.4
			c0,0.3,0.1,0.6,0.4,0.8c0.2,0.2,0.6,0.3,1.1,0.3s0.9-0.1,1.3-0.3c0.4-0.2,0.6-0.5,0.8-0.9c0.1-0.3,0.2-0.7,0.2-1.2L57.8,32.7
			L57.8,32.7z"/>
		<path class="st23" d="M61.6,27.7v-1.4h1.2v1.4H61.6z M61.6,36.3V29h1.2v7.3H61.6z"/>
		<path class="st23" d="M65.4,36.3V29h1.1v1c0.5-0.8,1.3-1.2,2.3-1.2c0.4,0,0.8,0.1,1.2,0.2c0.4,0.2,0.6,0.4,0.8,0.6
			c0.2,0.2,0.3,0.6,0.4,0.9c0,0.2,0.1,0.6,0.1,1.2v4.5h-1.2v-4.4c0-0.5,0-0.9-0.1-1.1s-0.3-0.4-0.5-0.6c-0.2-0.1-0.5-0.2-0.9-0.2
			c-0.5,0-1,0.2-1.4,0.5s-0.6,1-0.6,1.9v4H65.4z"/>
	</g>
</g>
</svg>
04 Hotfix branches.svg…]()
