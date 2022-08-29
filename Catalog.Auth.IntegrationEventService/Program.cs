
using Catalog.Auth.IntegrationEventService;
using RabbitMQ.Client;
using System.Text;
using Topshelf;

public class Program
{
    IntegrationEventContext context = new IntegrationEventContext();

    //public integration event from the IntegrationEvent model
    public void Start()
    {
        var connectionFactory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@localhost:5672")
        };
        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        var evt = context.IntegrationEvent.Where(e => !e.IsPublished).OrderBy(e => e.Id);
        foreach (var e in evt)
        {
            //publish message to event bus (rabbitmq)

            channel.QueueDeclare(e.Queue, true, false, false, null);
            var body = Encoding.UTF8.GetBytes(e.Data);
            channel.BasicPublish("", e.Queue, null, body);

            e.IsPublished = true;
        }
        context.SaveChanges();
    }

    public void Stop()
    {

    }

    public static void Main()
    {
        HostFactory.Run(x =>
         {
             x.Service<Program>(s =>
             {
                 s.ConstructUsing(name => new Program());
                 s.WhenStarted(evt => evt.Start());
                 s.WhenStopped(evt => evt.Stop());
             });
             x.RunAsLocalSystem();
             x.SetDescription("Integration event service for Catalog.Auth project");
             x.SetDisplayName("CatalogAuth.IntegrationEvent");
             x.SetServiceName("CatalogAuth.IntegrationEvent");
         });
    } 
}