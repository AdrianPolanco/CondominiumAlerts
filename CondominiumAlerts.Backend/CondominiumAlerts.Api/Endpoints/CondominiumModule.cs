using Carter;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Features.Condominiums.Add;
using CondominiumAlerts.Features.Features.Condominiums.GetCondominiumsJoinedByUser;
using CondominiumAlerts.Features.Features.Condominiums.Get;
using CondominiumAlerts.Features.Features.Condominiums.Join;
using CondominiumAlerts.Features.Features.Condominiums.Summaries;
using CondominiumAlerts.Infrastructure.Persistence.Repositories;
using CondominiumAlerts.Infrastructure.Services.AI.MessagesSummary;
using Coravel.Queuing.Interfaces;
using LightResults;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Message = CondominiumAlerts.Domain.Aggregates.Entities.Message;

namespace CondominiumAlerts.Api.Endpoints
{
    public class CondominiumModule : ICarterModule
    {
        public static string CondominiumId = "22f0abf5-f9fc-4730-b982-674f80d9b712";
        public static string UserId = "gqm3lFdtECVIek1p23aFD5SqSTs2";
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/condominium/join",
                async (ISender sender, [FromForm] JoinCondominiumCommand command, CancellationToken cancellationToken) =>
                {
                    Result<JoinCondominiumResponse> result = await sender.Send(command, cancellationToken);
                    if (!result.IsSuccess) return Results.BadRequest(result);

                    var responce = new
                    {
                        IsSuccess = result.IsSuccess,
                        Data = result.Value.Adapt<JoinCondominiumResponse>()
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

            app.MapGet("/condominium/GetById",
                async (ISender sender, [AsParameters] GetCondominiumCommand command, CancellationToken cancellationToken) =>
                {
                    Result<GetCondominiumResponse> result = await sender.Send(command, cancellationToken);

                    if (!result.IsSuccess) return Results.BadRequest(result);

                    var responce = new
                    {
                        IsSuccess = result.IsSuccess,
                        Data = result.Value,
                    };
                    return Results.Ok(responce);    
                });

            app.MapGet("/condominium/GetCondominiumsJoinedByUser",
                async (ISender sender, [AsParameters] GetCondominiumsJoinedByUserCommand command, CancellationToken cancellationToken) =>
                {
                    Result<List<GetCondominiumsJoinedByUserResponse>> result = await sender.Send(command, cancellationToken);

                    if (!result.IsSuccess) return Results.BadRequest(result);

                    var responce = new
                    {
                        IsSuccess = result.IsSuccess,
                        Data = result.Value,
                    };

                    return Results.Ok(responce);    
                });

            app.MapGet("/condominiums/{condominiumId}/messages", async (Guid condominiumId, CancellationToken cancellationToken, IRepository<Message, Guid> messagesRepository) =>
            {
                var messages = await messagesRepository.GetAsync(
                    cancellationToken: cancellationToken,
                    filter: m => m.CondominiumId == condominiumId
                );

                return Results.Ok(new {
                    messages
                });
            });

            app.MapPost("/condominiums/{condominiumId}/summary/{userId}", 
                async (
                    Guid condominiumId, 
                    string userId, 
                    ISender sender,
                    IQueue queue,
                    CancellationToken CancellationToken
                    ) =>
            {
                MessagesSummarizationRequest request = new(condominiumId, userId);

                queue.QueueInvocableWithPayload<MessagesSummarizationJob, MessagesSummarizationRequest>(request);

                var response = new
                {
                    IsSuccess = "pending",
                    Message = "Tu solicitud para resumir los mensajesdel condominio fue recibida."
                };
                return Results.Accepted("/condominiums/hubs/summary", response);
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

            app.MapPost("/condominiums/users/test",
                async (IRepository<CondominiumUser, Guid> repository, CancellationToken cancellationToken) =>
                {
                    var row = new CondominiumUser()
                    {
                        UserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                    };

                    await repository.CreateAsync(row, cancellationToken);
                });

            app.MapPost("/messages/test", async (IRepository<Message, Guid> repository) =>
            {
                List<Message> messages = new()
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "¡Hola a todos! ¿Alguien sabe cuándo es la próxima reunión del condominio?",
                        CreatorUserId = UserId, 
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-400)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Hola Adrian, la reunión es el jueves a las 7 PM en la sala de eventos.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-390)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Gracias Sofía, ¿se tratará el tema del mantenimiento de la piscina?",
                        CreatorUserId = UserId, 
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-380)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Sí, está en la agenda. También hablaremos de la seguridad en la entrada principal.",
                        CreatorUserId = UserId, 
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-370)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Genial. El fin de semana hay una reunión social en la terraza. ¡Están todos invitados!",
                        CreatorUserId = UserId, 
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-360)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "¿Podemos agregar el tema de los ascensores a la reunión? Están fallando mucho últimamente.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-350)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Sí, lo pondré en la agenda. Varios vecinos han reportado problemas.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-340)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "¿Alguien ha visto un paquete que debía llegarme ayer? El repartidor dice que lo dejó en recepción.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-330)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Revisé y no hay paquetes en la recepción. Pregunta con seguridad, a veces los guardan ahí.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-320)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "Gracias, voy a preguntar.", 
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-310)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Por favor, recuerden no dejar basura en los pasillos. Se han visto algunas bolsas cerca de los ascensores.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-300)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "El fin de semana haré una limpieza en la azotea. Si alguien quiere ayudar, avíseme.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-290)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "Cuenta conmigo para la limpieza.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-280)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Aviso: El lunes cortarán el agua de 9 AM a 1 PM por mantenimiento.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-270)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "¡Gracias por el aviso! Tendré listo un poco de agua almacenada.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-260)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "¿Alguien ha visto el cartel de aviso sobre las reparaciones en el gimnasio?",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-250)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Sí, el gimnasio estará cerrado el miércoles por mantenimiento. El aviso está en el hall de entrada.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-240)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Gracias por la info. Aprovecharé para salir a correr en lugar de hacer ejercicio en el gimnasio.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-230)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "¿Han tenido problemas con la conexión a Internet? Está bastante lenta últimamente.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-220)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Sí, parece que hay una caída en el servicio de la empresa proveedora. Están trabajando en ello.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-210)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "¿Alguien sabe si el jardinero vendrá esta semana? El césped ya está muy largo.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-200)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Sí, está programado para el jueves en la mañana. Lo avisaron en la última reunión.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-190)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "Perfecto, gracias por la confirmación.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-180)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Recuerden que este viernes se realizará la revisión de los sistemas de seguridad. Si tienen alguna sugerencia, pueden enviarla antes del jueves.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-170)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "¿Sabemos si se va a discutir la instalación de cámaras adicionales en el edificio?",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-160)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Sí, está en la agenda. Se va a considerar la instalación en los pasillos y en el estacionamiento.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-150)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "Me parece una buena idea. La seguridad es importante para todos.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-140)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Por favor, no olviden sacar la basura antes del viernes, el camión de basura pasará a las 9 AM.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-130)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "¿Alguien tiene un carro rojo estacionado en la zona de carga? Está bloqueando la entrada.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-120)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "Sí, ese es el mío. Perdón, moveré el carro en cuanto pueda.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-110)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Gracias. Asegurémonos de mantener las entradas libres de obstáculos para todos.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-100)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "¡Feliz lunes a todos! Recuerden que hay que estar al tanto de las reuniones de este mes.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-90)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Sí, ya vi que hay una reunión este jueves a las 7 PM. ¡Nos vemos allí!",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-80)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "No olviden que el acceso al gimnasio se limita para los residentes por la cantidad de personas, así que asegúrense de que tienen acceso antes de ir.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-70)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "¿Alguien ha tenido problemas con el agua caliente en los apartamentos? Llevo horas esperando que salga agua caliente.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-60)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "A mí también me pasó lo mismo. Parece que es un problema en el sistema central. Lo están arreglando.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-50)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(), Text = "Gracias por la información. Espero que lo solucionen pronto.",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-40)
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =
                            "Alguien está dejando luces encendidas en las áreas comunes. ¡Por favor, apáguenlas cuando no las necesiten!",
                        CreatorUserId = UserId,
                        CondominiumId = Guid.Parse(CondominiumId),
                        CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                    }
                };

                messages = await repository.BulkInsertAsync(messages, new CancellationToken());

                return Results.Ok(messages);
            });

            app.MapHub<SummaryHub>("/condominiums/hubs/summary");
        }
    }
}
