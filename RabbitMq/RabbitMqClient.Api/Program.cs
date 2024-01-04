using MediatR;
using RabbitMqClient.Send.Domain;
using RabbitMqClient.Send.Events.Notifications;
using RabbitMqClient.Send.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddScoped<ISendService, SendService>();
services.Configure<ConnectionRabbit>(builder.Configuration.GetSection("ConnectionRabbit"));
services.AddMediatR(typeof(EventNotification));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
