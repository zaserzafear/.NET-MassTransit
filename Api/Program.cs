using Api.Extensions;
using SettingModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var rabbitMqSetting = builder.Configuration.GetSection(nameof(RabbitMqSetting)).Get<RabbitMqSetting>();
builder.Services.AddMassTransitExtension(rabbitMqSetting!.Host, rabbitMqSetting.Username, rabbitMqSetting.Password);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
