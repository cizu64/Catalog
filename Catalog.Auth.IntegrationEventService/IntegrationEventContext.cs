using Catalog.Auth.IntegrationEventService.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Auth.IntegrationEventService
{
    public  class IntegrationEventContext : DbContext 
    {
        public DbSet<IntegrationEvent> IntegrationEvent { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\v11.0;Initial Catalog=AuthDb;Integrated Security=True");
        }
    }
}
