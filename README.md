# Documentación
## ¿De que tratá?
El proyecto trata, a grandes rasgos, de una app que permitirá a los miembros de una comunidad comunicarse por canales en tiempo real, similar a Discord.
## ¿Por qué?
Para ahorrarnos tiempos y complicaciones, y que podamos desarrollar el proyecto de forma rápida y ordenada, cumpliendo con los aspectos que nos piden, es necesaria esta redacción para ponernos de acuerdo y estandarizar el como trabajaremos en el proyecto.
## Arquitectura y estructura del proyecto
### Tecnologías a utilizar

- .NET 9: Versión del SDK de .NET.
- ASP.NET Core: Framework utilizado para crear la Web Api en el backend.
- Angular 19: Framework utilizado para crear el frontend.
- PostgreSQL: Base de datos usada para persistir los datos NO RELACIONADOS a la autenticación.
- Firebase: Utilizaremos el servicio de autenticación **Firebase Auth** para simplificar todo el proceso del registro, autorización y autenticación de usuarios en la app, autenticación social (Google, etc.), de ser requerido u, opcionalmente funcionalidades como enviar correos de confirmación.
- Docker(opcional): No requerido pero recomendado, para las pruebas de integración y para montar contenedores de bases de datos de diversos ambientes tanto de desarrollo como la que usaremos al presentar el proyecto. Manteniendo una coherencia garantizada entre las versiones que usaremos al compartir todos un ambiente identico, evitando problemas y ahorrandonos tiempo debido a que alguno descargo la versión de SQL que no era, etc.
- Cloudinary: Servicio que usaremos para la gestión de subida, recuperación y borrado de imágenes.
  
### Backend
![Imagen de WhatsApp 2025-01-21 a las 13 28 18_11211e3b](https://github.com/user-attachments/assets/b80c1c45-6f55-48bb-89d2-1b0240ff2cd3)
Para desarrollar el backend utilizaremos C# con su framework ASP.NET Core para desarrollar la Web API que soportará la aplicación. A continuación, hablaremos más a detalle de que hara cada una de las capas que sale en la imagen:.

#### Patrones
Dado que se evaluarán estos aspectos, es necesario clarificar que patrones estaremos utilizando.
**Repository**: Patrón usado para abstraer el acceso a los datos.

**CQRS**: Patrón usado para segregar las operaciones de escritura (Commands) de las operaciones de lecture (Queries).

**Mediator**: Patrón usado para desacoplar el código de los endpoints a través de handlers.

**Middleware**: Patrón usado para ejecutar ciertas acciones antes o después de procesar una solicitud. Lo usaremos para ejecutar validaciones antes de procesar realmente una solicitud a la API.

**Strategy**: Patrón usado para tomar diferentes "estrategias" o "implementaciones" dependiendo de una elección tomada por el usuario. Lo podemos usar, por ejemplo, para diferenciar la publicación un comentario sin ninguna foto adjunta de un comentario con una foto adjunta.

**Inyección de dependencias**: Patrón usado para proveer las dependencias necesarias a una clase sin que esta se tenga que preocupar por eso.

#### **Domain**
Aquí residirá el dominio y lo relacionado a la lógica de negocio de la aplicación, componiendose, pero no limitandose a los siguientes elementos:

**Entities**: Son los modelos que representan objetos clave del dominio, por ejemplo, en nuestro caso serían **Users**, **Comments** o **Condominiums**. Estos objetos se identifican de forma única a través de un **Id** en lugar de por sus atributos, dígase, un **User** que se llame Pepito y tenga el **Id 1** es diferente de otro **User** que se llame también Pepito y tengo el **Id 2**.

**Value Objects**: Son clases u objetos que se identifican por su **contenido** en lugar de por su **Id**, por ejemplo, **Address** o **Phone**, por ejemplo, un **Phone** 123456789 es el mismo que otro **Phone** 123456789. Suelen ser inmutables.

**Enums**: Son objetos utilizados para representar estados de forma significativa, evitando **"números mágicos"** o **"palabras mágicas"**, de modo que en vez de usar el número 1 para representar el estado "muteado" de un canal, usamos el miembro **MUTED** del enum **ChannelStates**.

**Repositories**: En esta capa de dominio, los **repositories** son interfaces que definen los métodos de las abstracciones que usaremos para acceder a los datos en la base de datos cumpliendo el **Dependency Inversion Principle**, haciendo el código más flexible y escalable (aspecto que se evaluará en el proyecto final TDS). La implementación real de estas interfaces se harán en la capa **Infrastructure**.

**Aggregates**: Es una agrupación de **entities**, **agreggates roots** y **value objects** relacionadas que deben trabajar juntas.

**Aggregate Root**: Es la entidad principal de un **aggreggate**, a través de la cual se interactua con todos los demás elementos del **aggregate**, asegurandose que se cumplan las funcionalidades y reglas de la aplicación, por ejemplo, en nuestro caso, podría ser una entidad **Event** o **Channel**, toda la interacción que se dé con las entidades internas al **aggregate** deberían de darse a través del **aggregate root**, facilitando la implementación de funcionalidades y validaciones como validar que los eventos solo puedan ser creados por X tipo de usuarios que esten dentro del canal dado, o que no se superé el limite de usuarios en un canal.

**Domain Events**: Son eventos que se dispararán despues de una acción determinada en el dominio. Esto facilitará la implementación de funcionalidades como enviar correos de confirmación a los usuarios después de registrarse. Notése que esto es diferente y no necesariamente igual a la entidad **Event** que mencionamos anteriormente y podríamos o no usar **Domain Events** para desarrollar la funcionalidad de eventos que requiere la app.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

**Interfaces**: Abstracciones de servicios relevantes que serán implementados en la capa **infrastructure**, similares a los **repositories** pero orientados a aspectos no relacionados directamente a la persistencia en la base de datos.

##### Paquetes usados:

**MediatR**: Usaremos la librería **MediatR**, que es la única dependencia que tiene esta capa para disparar los **Domain Events**.

##### Dependencias:

- Ninguna

#### **Infrastructure**
Es la capa que proveerá todo el acceso a la infrastructura y librerías necesarias para que la aplicación realmente sea funcional. Contendrá los siguientes elementos:

**DbContext**: Es la clase personalizada que haremos heredar de la clase **DbContext** que nos ofrece la librería **Entity Framework Core**. Esta clase se encargará del acceso a la base de datos.

**Repositories**: Son las clases que implementarán las interfaces de la capa **domain**, haciendo uso del **DbContext** para acceder a los datos mediante los métodos. Las responsabilidades de estas clases se limitan a operaciones CRUD, nada más.

**Providers o Services**: Son clases que proveerán acceso a demás aspectos relacionados a **infrastructure** que **no sea la conexión a la base de datos** como por ejemplo una clase para gestionar los archivos (fotos) de la app y otra para gestionar la autenticación, los roles, etc.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

### Librerías

**Entity Framework Core**: Usaremos este ORM para conectarnos a la base de datos.

**Entity Framework Core NgPsql**: Usaremos este paquete para poder acceder a nuestro PostgreSQL.

**FirebaseAdmin**: Usaremos este paquete para conectarnos a Firebase.

**CloudinaryDotnet**: Usaremos este paquete para conectarnos con Cloudinary.

### Dependencias

- Domain
- CrossCutting

#### **Features**
Es la capa que hará uso de los elementos de la capa **domain** e **infrastructure** para hacer funcionalidades concretas, como registrar usuario o iniciar sesión.

Por cada **feature** en esta capa se creará una carpeta donde se agruparán todas las **features** relacionadas, por ejemplo, crearemos una carpeta **Users** que contendrá, por ejemplo, las **features** **Create y Login**. Notése que los nombres de las carpetas que agrupan las **features** debe de ser en **plural** y las **features** deben tener nombres signficativos, que ayuden a identificar rápidamente el código que estará almacenado dentro de ellas. 

En las carpetas que agrupan una **feature** debe contener todo el código relevante a esa **feature**, de modo que si se necesita hacer un cambio en esa funcionalidad, en esa carpeta este todo lo que necesito para hacerlo. Por ejemplo en una **feature Register** de **Users** debería de estar su **Command**, su **Command Handler**, **Result**, etc. De modo que no tenga que ir a ninguna otra parte si quiero cambiar el código, agregarle una nueva funcionalidad o simplemente leer y comprender la funcionalidad.

Comentado esto, veamos los demás elementos que compondrán esta capa:

**Commands**: Son records que contienen toda la información necesaria para hacer una operación de escritura (creación, actualización o borrado) en la infraestructura, como registrar un usuario o actualizar su nombre. Estos records deben heredar de la interfaz **ICommand** para poder ser procesados por el handler.

**Queries**: Son records que contienen toda la información necesaria para hacer una operación de lectura (query) en la infraestructura, como recuperar una foto o un usuario. Estos records deben heredar de la interfaz **IQuery** para poder ser procesados por el handler y siempre devuelven un resultado.

**Command Handlers**: Son handlers que proveé MediatR para responder y procesar las solicitudes hechas por los **Commands**, lo utilizaremos en operaciones de escritura. Deben heredar de la interfaz **ICommandHandler** en la que especificarás el tipo que recibe (requerido) y el tipo que devuelve (opcional, en caso de no especificarlo se devolvera **Unit**, que es igual a no devolver nada). Pueden o no devolver un resultado dependiendo de cual de las dos interfaces **ICommandHandler** se utilicé

**Query Handlers**: Son handlers que proveé MediatR para responder y procesar las solicitudes hechas por las **Queries**, lo utilizaremos para operaciones de escritura. Deben heredar de la interfaz **IQueryHandler**, en la que debemos especificar tanto el tipo que recibe como el tipo que retornará, ya que siempre devuelven un resultado.

**Event Handlers**: Son handlers que procesan y responden a los eventos que se disparán a través de MediatR después de cierta acción.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

### Librerías

**MediatR**: Usaremos esta librería para la gestión de CQRS y todo lo que tiene que ver con los events.

**FluentValidation**: Usaremos esta librería para facilitar las validaciones.

**Mapster**: Usaremos esta librería para mapear las distintas clases **sin necesidad de configuración como en AutoMapper de seguir ciertos estándares (como que las propiedades clases tengan los mismos nombres)**.

### Dependencias

- Domain
- CrossCutting
- Infrastructure
  
#### **API**
Esta es la capa que expondrá los endpoints correspondientes, utilizaremos las **Minimal APIs** de ASP.NET Core. Contiene los siguientes elementos:

**Endpoint**: Son los módulos o endpoints que expondrán el punto de entrada a todas las demás capas que comentamos anteriormente.

**Results**: Son las clases que utilizaremos para devolver los resultados finales en formato JSON al frontend, a petición de uno de los integrantes del grupo.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

### Librerías

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

### Dependencias

- Domain
- CrossCutting
- Infrastructure
- Features

#### **CrossCutting**
Es una capa que utilizaremos para colocar código transversal a las distintas capas. Se compone, pero no se limita a los siguientes elementos:

**Logging**: Las clases relevantes para proveer logs al backend de la app.

**Validators**: Las clases que harán las validaciones que se ejecutarán antes de procesar una solicitud.

**Error handlers**: Las clases que manejarán los errores, en caso de presentarse, devolviendo una respuesta adecuada.

**Middlewares**: Middlewares personalizados que crearemos, de ser necesarios, durante el desarrollo de la app.

**Results**: Resultados que pueden devolverse en caso de, por ejemplo, no pasar una validación.

**CQRS**: Aquí definiremos todas las interfaces relevantes a CQRS como **ICommand, IQuery, ICommandHandler, IQueryHandler y IEventHandler**

**Extension Methods**: Métodos de extensión relevantes a esta capa.

**Cualquier otro elemento que sea reutilizable y transversal a varias capas**

### Librerías

**MediatR**

**Mapster**

**FluentValidation**

**Serilog**: Paquete para gestionar los logs de la app.

**Serilog.Sinks.Console**: Paquete para gestionar los logs de la app a nivel de consola, dado que no es una app de producción no necesitaremos otros Sinks para centralizar los logs, basta con los de consola.

**Extension Methods**: Métodos de extensión relevantes a esta capa.

### Dependencias

- Domain
  
### **Tests**
Es la capa que utilizaremos para hacer las pruebas unitarias y de integración, requeridas en el proyecto final TDS. Se compone de estos elementos:

**Extension Methods**: Métodos de extensión relevantes a esta capa.

### Librerías
**xUnit**: Librería para hacer pruebas unitarias en C#.

**Moq**: Librería para mockear las dependencias en pruebas unitarias.

**TestContainers**: Librería para hacer tests de integración en C# usando contenedores. Debes tener **Docker** instalado para que funcioné.

### Dependencias

- Domain
- Features
- Infrastructure
- CrossCutting
- API
