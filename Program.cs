using KYC360MockAPI.Controllers;
using KYC360MockAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSingleton<IDatabase, MockDatabase>();
builder.Services.AddSingleton<IEntityRepository>(provider =>
{
    var database = provider.GetRequiredService<IDatabase>();
    return new MockEntityRepository(database);
});
builder.Services.AddSingleton<ILogger<EntityController>, Logger<EntityController>>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
