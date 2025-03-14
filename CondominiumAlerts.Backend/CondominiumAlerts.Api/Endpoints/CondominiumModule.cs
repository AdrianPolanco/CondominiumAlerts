using Carter;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Features.Condominiums.Add;
using CondominiumAlerts.Features.Features.Condominiums.Join;
using CondominiumAlerts.Features.Features.Condominiums.Summaries;
using FirebaseAdmin.Auth;
using LightResults;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace CondominiumAlerts.Api.Endpoints
{
    public class CondominiumModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/condominium/join",
                async (ISender sender, [FromForm] JoinCondominiumCommand command, CancellationToken cancellationToken) =>
                {
                    Result<JoinCondominiumResponce> result = await sender.Send(command, cancellationToken);
                    if (!result.IsSuccess) return Results.BadRequest(result);

                    var responce = new
                    {
                        IsSuccess = result.IsSuccess,
                        Data = result.Value.Adapt<JoinCondominiumResponce>()
                    };
                    return Results.Ok(responce);
                }).DisableAntiforgery();

            app.MapPost("/condominium",
                async (ISender sender, [FromForm] AddCondominiumCommand command,CancellationToken cancellationToken) =>
                {
              
                    Result<AddCondominiumResponse> result = await sender.Send(command, cancellationToken);
                    if (!result.IsSuccess) return Results.BadRequest(result);

                    var response = new
                    {
                        result.IsSuccess,
                        Data = result.Value
                    };
                    return Results.Ok(response);
                }
                // TODO: Add anti forgery token in frontend: https://stackoverflow.com/a/77191406
            ).DisableAntiforgery();

            app.MapPost("/condominiums/{condominiumId}/summary/{userId}", async (string condominiumId, string userId,
                ISender sender, CancellationToken CancellationToken) =>
            {
                GetSummaryCommand command = new(Guid.Parse(condominiumId), userId);

                var result = await sender.Send(command, CancellationToken);

                if (!result.IsSuccess) return Results.BadRequest(result);

                var response = new
                {
                    result.IsSuccess,
                    Data = result.Value
                };
                return Results.Ok(response);
            });

            app.MapPost("/condominiums/test", async (IRepository<Condominium, Guid> repository) =>
            {
                List<Condominium> condominiums = new()
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Condominio Las Palmas",
                        Address = "Av. Principal #123",
                        ImageUrl = "https://res.cloudinary.com/dmeixgsxo/image/upload/v1741917883/example1.png",
                        LinkToken = string.Empty,
                        InviteCode = "PALMAS2025",
                        TokenExpirationDate = DateTime.UtcNow.AddMonths(3)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Residencial Vista Hermosa",
                        Address = "Calle 45, Zona Norte",
                        ImageUrl = "https://res.cloudinary.com/dmeixgsxo/image/upload/v1741917883/example2.png",
                        LinkToken = string.Empty,
                        InviteCode = "VISTA2025",
                        TokenExpirationDate = DateTime.UtcNow.AddMonths(3),
                    }

                };

                condominiums = await repository.BulkInsertAsync(condominiums, new CancellationToken());
                
                return Results.Ok(condominiums);
            });

            app.MapPost("/messages/test", async (IRepository<Message, Guid> repository) =>
            {
                List<Message> messages = new()
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "¡Hola a todos! ¿Alguien sabe cuándo es la próxima reunión del condominio?",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-400)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Hola Adrian, la reunión es el jueves a las 7 PM en la sala de eventos.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-390)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Gracias Sofía, ¿se tratará el tema del mantenimiento de la piscina?",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-380)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Sí, está en la agenda. También hablaremos de la seguridad en la entrada principal.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-370)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Genial. El fin de semana hay una reunión social en la terraza. ¡Están todos invitados!",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-360)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "¿Podemos agregar el tema de los ascensores a la reunión? Están fallando mucho últimamente.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-350)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Sí, lo pondré en la agenda. Varios vecinos han reportado problemas.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-340)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "¿Alguien ha visto un paquete que debía llegarme ayer? El repartidor dice que lo dejó en recepción.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-330)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Revisé y no hay paquetes en la recepción. Pregunta con seguridad, a veces los guardan ahí.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-320)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "Gracias, voy a preguntar.", CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2",
                        CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-310)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Por favor, recuerden no dejar basura en los pasillos. Se han visto algunas bolsas cerca de los ascensores.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-300)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "El fin de semana haré una limpieza en la azotea. Si alguien quiere ayudar, avíseme.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-290)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "Cuenta conmigo para la limpieza.", CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2",
                        CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-280)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Aviso: El lunes cortarán el agua de 9 AM a 1 PM por mantenimiento.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-270)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "¡Gracias por el aviso! Tendré listo un poco de agua almacenada.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-260)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "¿Alguien ha visto el cartel de aviso sobre las reparaciones en el gimnasio?",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-250)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Sí, el gimnasio estará cerrado el miércoles por mantenimiento. El aviso está en el hall de entrada.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-240)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Gracias por la info. Aprovecharé para salir a correr en lugar de hacer ejercicio en el gimnasio.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-230)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "¿Han tenido problemas con la conexión a Internet? Está bastante lenta últimamente.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-220)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Sí, parece que hay una caída en el servicio de la empresa proveedora. Están trabajando en ello.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-210)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "¿Alguien sabe si el jardinero vendrá esta semana? El césped ya está muy largo.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-200)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Sí, está programado para el jueves en la mañana. Lo avisaron en la última reunión.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-190)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "Perfecto, gracias por la confirmación.", CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2",
                        CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-180)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Recuerden que este viernes se realizará la revisión de los sistemas de seguridad. Si tienen alguna sugerencia, pueden enviarla antes del jueves.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-170)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "¿Sabemos si se va a discutir la instalación de cámaras adicionales en el edificio?",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-160)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Sí, está en la agenda. Se va a considerar la instalación en los pasillos y en el estacionamiento.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-150)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "Me parece una buena idea. La seguridad es importante para todos.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-140)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Por favor, no olviden sacar la basura antes del viernes, el camión de basura pasará a las 9 AM.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-130)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "¿Alguien tiene un carro rojo estacionado en la zona de carga? Está bloqueando la entrada.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-120)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "Sí, ese es el mío. Perdón, moveré el carro en cuanto pueda.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-110)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Gracias. Asegurémonos de mantener las entradas libres de obstáculos para todos.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-100)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "¡Feliz lunes a todos! Recuerden que hay que estar al tanto de las reuniones de este mes.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-90)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Sí, ya vi que hay una reunión este jueves a las 7 PM. ¡Nos vemos allí!",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-80)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "No olviden que el acceso al gimnasio se limita para los residentes por la cantidad de personas, así que asegúrense de que tienen acceso antes de ir.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-70)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "¿Alguien ha tenido problemas con el agua caliente en los apartamentos? Llevo horas esperando que salga agua caliente.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-60)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "A mí también me pasó lo mismo. Parece que es un problema en el sistema central. Lo están arreglando.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-50)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "Gracias por la información. Espero que lo solucionen pronto.",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-40)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Alguien está dejando luces encendidas en las áreas comunes. ¡Por favor, apáguenlas cuando no las necesiten!",
                        CreatorUserId = "gqm3lFdtECVIek1p23aFD5SqSTs2", CondominiumId = Guid.Parse("2c697c1d-6507-4f1e-ba75-97624d65403e"),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                    }
                };

                messages = await repository.BulkInsertAsync(messages, new CancellationToken());

                return Results.Ok(messages);
            });
        }
    }
}
