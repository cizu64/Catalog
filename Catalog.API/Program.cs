using Catalog.API.Infrastructure;
using Catalog.API.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ListenForIntegrationEvents();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var Secret = builder.Configuration["jwt:secret"];
var key = Encoding.ASCII.GetBytes(Secret);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["jwt:issuer"],
        ValidAudience = builder.Configuration["jwt:audience"]
    };
});

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CatalogContext>(options => 
{
    options.UseSqlServer(builder.Configuration["ConnectionString"], options =>
    {
        //options.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
        
        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
        options.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
    });
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<DbContext, CatalogContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


void ListenForIntegrationEvents()
{
    var contextOptions = new DbContextOptionsBuilder<CatalogContext>()
                 .UseSqlServer(builder.Configuration["ConnectionString"])
                 .Options;

    var connectionFactory = new ConnectionFactory
    {
        Uri = new Uri("amqp://guest:guest@localhost:5672")
    };
    var connection = connectionFactory.CreateConnection();
    var channel = connection.CreateModel();

    channel.QueueDeclare("user.add", true, false, false, null);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (sender, e) =>
    {
        channel.BasicAck(e.DeliveryTag, false);
        var context = new CatalogContext(contextOptions);
        var body = e.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var data = JObject.Parse(message);
        int userId = data["UserId"].Value<int>();
        if (!context.Shop.Any(s => s.UserId == userId)) //just to make sure we don't have duplicates
        {
            context.Shop.Add(new Catalog.API.Model.Shop
            {
                UserId = data["UserId"].Value<int>()
            });
        }
        context.SaveChanges();
    };
    channel.BasicConsume("user.add", false, consumer);
}