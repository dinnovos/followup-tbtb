using System.Text.Json.Serialization;
using FollowUp.Api.ErrorHandling;
using FollowUp.Application.Repositories;
using FollowUp.Application.Services;
using FollowUp.Infrastructure.Data;
using FollowUp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

const string AngularDevClient = "AngularDevClient";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FollowUpDb")));

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();

builder.Services.AddExceptionHandler<DuplicatePatientExceptionHandler>();
builder.Services.AddExceptionHandler<DatabaseConstraintExceptionHandler>();
builder.Services.AddExceptionHandler<PatientNotFoundExceptionHandler>();
builder.Services.AddProblemDetails();

// The Angular dev server (localhost:4200) and this Api (localhost:5129) are
// different origins -- without this, the browser blocks every request from
// the form, even though tools like curl (which ignore CORS) work fine.
builder.Services.AddCors(options =>
    options.AddPolicy(AngularDevClient, policy =>
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors(AngularDevClient);

app.UseAuthorization();

app.MapControllers();

app.Run();
