﻿
using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace CondominiumAlerts.Domain.Aggregates.Entities;
    public sealed class LevelOfPriority : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty!;
        public string Description { get; set; } = string.Empty!;
        public int Priority { get; set; }
        // if condominium is null then is a base level of priority that any condominium can use
        public Guid? CondominiumId { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        
        public Condominium? Condominium { get; set; }  
        
        public IReadOnlyCollection<Post>? Posts { get; set; }
        public IReadOnlyCollection<Notification>? Notifications { get; set; }
    }
