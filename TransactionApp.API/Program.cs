using Microsoft.EntityFrameworkCore;
using TransactionApp.BUSINESS.Abstract;
using TransactionApp.BUSINESS.Concrete;
using TransactionApp.BUSINESS.MapperConfiguration.AutoMapper;
using TransactionApp.CORE.Middleware;
using TransactionApp.DAL.Abstract.EntityFramework;
using TransactionApp.DAL.Abstract.EntityFramework.Repositories;
using TransactionApp.DAL.Concrete.EntityFramework;
using TransactionApp.DAL.Concrete.EntityFramework.Contexts;
using TransactionApp.DAL.Concrete.EntityFramework.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbConnectionString = builder.Configuration["ConnectionStrings:DevelopmentDB"];

builder.Services.AddDbContext<TransactionManagerDbContext>(options =>
    options.UseSqlServer(dbConnectionString, b => b.MigrationsAssembly("TransactionApp.DAL")));

builder.Services.AddMemoryCache();

//Community licence key for automapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = "<eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzkwNjQwMDAwIiwiaWF0IjoiMTc1OTE0Mzg1MiIsImFjY291bnRfaWQiOiIwMTk5OTUxZWMzOWM3ZDZjOWI4ZDc0N2E2YmExMzAzMCIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazZhajl5MWhxa3pqdjhiZ2YyMWFyamE0Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.zt33jqAZfGYCH7BgaEIovJcMe33Q2fzXDi4XpzoaBZon-RAhdzL-rNkKKpO1iZOh8twT7Ee-X8tL4UKDfGxaxXUTbORK1UO0-geugXxwJUjML51dnZfvv8B4bwIegFqIkeVELdDOoQluF_sZcF5BsjMTmqoHcLWxA7oOW748YWqAMF13zcrJU6z-GlxCbaw5vpi17pNByM7V-VhfFybMld6mMcocb14uot8wZm1KaWc-o8O3K5LcmUuqghMAQrybUE2tAv5jMUfttrbw9NmUJ-ZSOyKRgIc50RTfcghiGMLCkO_ffdvyKKWkjfJ2kijDZl4ZKSw2msZBclRHeQU-5w>";
}, typeof(MapProfile));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

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
