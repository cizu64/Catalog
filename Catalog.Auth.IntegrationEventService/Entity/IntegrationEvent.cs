﻿namespace Catalog.Auth.IntegrationEventService.Entity
{
    public class IntegrationEvent
    {
        public int Id { get; set; }
        public string Queue { get; set; }
        public string Data { get; set; }
        public bool IsPublished { get; set; } = false;
    }
}