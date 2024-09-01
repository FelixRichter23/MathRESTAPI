using MathAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ICalculationRepository, CalculationRepository>();

builder.Services.AddSingleton<IRelationRepository, RelationRepository>();

builder.Services.AddTransient<QueryFactory>(options => 
{
    var connection = new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));

    return new QueryFactory(connection, new SqlServerCompiler());
});

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
