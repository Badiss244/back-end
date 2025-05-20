using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Extensions;
using Data.Context;
using Data.Repositories;
using Domain.Commands;
using Domain.Handlers;
using Domain.Interface;
using Domain.MailSender;
using Domain.Models;
using Domain.Queries;
using System.Text.Json.Serialization;
using Domain.Seeder;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];
var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,


        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<AppUser>, IEnumerable<AppUser>>, GetListGenericHandler<AppUser, int>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<AppUser, int>, AppUser>, GetGenericHandler<AppUser, int>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<AppUser, string>, AppUser>, GetGenericHandler<AppUser, string>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<AppUser>, AppUser>, AddGenericHandler<AppUser, int>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<AppUser>, AppUser>, PutGenericHandler<AppUser, int>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<AppUser, int>, bool>, DeleteGenericHandler<AppUser, int>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<AppUser, string>, bool>, DeleteGenericHandler<AppUser, string>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<AppUser>, IEnumerable<AppUser>>, FindGenericHandler<AppUser>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<Filiale, Guid>, Filiale>, GetGenericHandler<Filiale, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<Filiale>, IEnumerable<Filiale>>, GetListGenericHandler<Filiale, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<Filiale>, IEnumerable<Filiale>>, FindGenericHandler<Filiale>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<Filiale>, Filiale>, AddGenericHandler<Filiale, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<Filiale>, Filiale>, PutGenericHandler<Filiale, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<Filiale, Guid>, bool>, DeleteGenericHandler<Filiale, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<Factory>, IEnumerable<Factory>>, FindGenericHandler<Factory>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<Factory>, IEnumerable<Factory>>, GetListGenericHandler<Factory, Guid>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<Factory>, Factory>, AddGenericHandler<Factory, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<Factory>, Factory>, PutGenericHandler<Factory, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<Factory, Guid>, bool>, DeleteGenericHandler<Factory, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<Factory, Guid>, Factory>, GetGenericHandler<Factory, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<Audit, Guid>, Audit>, GetGenericHandler<Audit, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<Audit>, IEnumerable<Audit>>, GetListGenericHandler<Audit, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<Audit>, IEnumerable<Audit>>, FindGenericHandler<Audit>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<Audit>, Audit>, AddGenericHandler<Audit, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<Audit>, Audit>, PutGenericHandler<Audit, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<Audit, Guid>, bool>, DeleteGenericHandler<Audit, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<PlanAction, Guid>, PlanAction>, GetGenericHandler<PlanAction, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<PlanAction>, IEnumerable<PlanAction>>, GetListGenericHandler<PlanAction, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<PlanAction>, IEnumerable<PlanAction>>, FindGenericHandler<PlanAction>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<PlanAction>, PlanAction>, AddGenericHandler<PlanAction, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<PlanAction>, PlanAction>, PutGenericHandler<PlanAction, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<PlanAction, Guid>, bool>, DeleteGenericHandler<PlanAction, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<Nofication, Guid>, Nofication>, GetGenericHandler<Nofication, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<Nofication>, IEnumerable<Nofication>>, GetListGenericHandler<Nofication, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<Nofication>, IEnumerable<Nofication>>, FindGenericHandler<Nofication>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<Nofication>, Nofication>, AddGenericHandler<Nofication, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<Nofication>, Nofication>, PutGenericHandler<Nofication, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<Nofication, Guid>, bool>, DeleteGenericHandler<Nofication, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<Rapport, Guid>, Rapport>, GetGenericHandler<Rapport, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<Rapport>, IEnumerable<Rapport>>, GetListGenericHandler<Rapport, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<Rapport>, IEnumerable<Rapport>>, FindGenericHandler<Rapport>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<Rapport>, Rapport>, AddGenericHandler<Rapport, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<Rapport>, Rapport>, PutGenericHandler<Rapport, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<Rapport, Guid>, bool>, DeleteGenericHandler<Rapport, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<Evidence, Guid>, Evidence>, GetGenericHandler<Evidence, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<Evidence>, IEnumerable<Evidence>>, GetListGenericHandler<Evidence, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<Evidence>, IEnumerable<Evidence>>, FindGenericHandler<Evidence>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<Evidence>, Evidence>, AddGenericHandler<Evidence, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<Evidence>, Evidence>, PutGenericHandler<Evidence, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<Evidence, Guid>, bool>, DeleteGenericHandler<Evidence, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<Sx, Guid>, Sx>, GetGenericHandler<Sx, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<Sx>, IEnumerable<Sx>>, GetListGenericHandler<Sx, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<Sx>, IEnumerable<Sx>>, FindGenericHandler<Sx>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<Sx>, Sx>, AddGenericHandler<Sx, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<Sx>, Sx>, PutGenericHandler<Sx, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<Sx, Guid>, bool>, DeleteGenericHandler<Sx, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<Critaire, Guid>, Critaire>, GetGenericHandler<Critaire, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<Critaire>, IEnumerable<Critaire>>, GetListGenericHandler<Critaire, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<Critaire>, IEnumerable<Critaire>>, FindGenericHandler<Critaire>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<Critaire>, Critaire>, AddGenericHandler<Critaire, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<Critaire>, Critaire>, PutGenericHandler<Critaire, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<Critaire, Guid>, bool>, DeleteGenericHandler<Critaire, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<AppUser, Guid>, AppUser>, GetGenericHandler<AppUser, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<AppUser>, IEnumerable<AppUser>>, GetListGenericHandler<AppUser, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<AppUser>, IEnumerable<AppUser>>, FindGenericHandler<AppUser>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<AppUser>, AppUser>, AddGenericHandler<AppUser, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<AppUser>, AppUser>, PutGenericHandler<AppUser, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<AppUser, Guid>, bool>, DeleteGenericHandler<AppUser, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<Tache, Guid>, Tache>, GetGenericHandler<Tache, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<Tache>, IEnumerable<Tache>>, GetListGenericHandler<Tache, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<Tache>, IEnumerable<Tache>>, FindGenericHandler<Tache>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<Tache>, Tache>, AddGenericHandler<Tache, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<Tache>, Tache>, PutGenericHandler<Tache, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<Tache, Guid>, bool>, DeleteGenericHandler<Tache, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<CritereDefinition, Guid>, CritereDefinition>, GetGenericHandler<CritereDefinition, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<CritereDefinition>, IEnumerable<CritereDefinition>>, GetListGenericHandler<CritereDefinition, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<CritereDefinition>, IEnumerable<CritereDefinition>>, FindGenericHandler<CritereDefinition>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<CritereDefinition>, CritereDefinition>, AddGenericHandler<CritereDefinition, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<CritereDefinition>, CritereDefinition>, PutGenericHandler<CritereDefinition, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<CritereDefinition, Guid>, bool>, DeleteGenericHandler<CritereDefinition, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<SDefinition, Guid>, SDefinition>, GetGenericHandler<SDefinition, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<SDefinition>, IEnumerable<SDefinition>>, GetListGenericHandler<SDefinition, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<SDefinition>, IEnumerable<SDefinition>>, FindGenericHandler<SDefinition>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<SDefinition>, SDefinition>, AddGenericHandler<SDefinition, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<SDefinition>, SDefinition>, PutGenericHandler<SDefinition, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<SDefinition, Guid>, bool>, DeleteGenericHandler<SDefinition, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetGenericQuery<Parametres, Guid>, Parametres>, GetGenericHandler<Parametres, Guid>>();
builder.Services.AddTransient<IRequestHandler<GetListGenericQuery<Parametres>, IEnumerable<Parametres>>, GetListGenericHandler<Parametres, Guid>>();
builder.Services.AddTransient<IRequestHandler<FindGenericQuery<Parametres>, IEnumerable<Parametres>>, FindGenericHandler<Parametres>>();
builder.Services.AddTransient<IRequestHandler<AddGenericCommand<Parametres>, Parametres>, AddGenericHandler<Parametres, Guid>>();
builder.Services.AddTransient<IRequestHandler<PutGenericCommand<Parametres>, Parametres>, PutGenericHandler<Parametres, Guid>>();
builder.Services.AddTransient<IRequestHandler<DeleteGenericCommand<Parametres, Guid>, bool>, DeleteGenericHandler<Parametres, Guid>>();








builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterGenericHandlers = false;
    cfg.RegisterServicesFromAssemblies(new[] { typeof(GetListGenericQuery<AppUser>).Assembly });
});


builder.Services.AddSwaggerGenJwtAuth();





builder.Services.AddTransient<IEmailSender, SmtpEmailSender>(); //added by me




builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});





builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();  ///hatha ta3 seeder
    await RoleSeeder.SeedRolesAsync(roleManager);
}




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<MaintenanceMiddleware>();
app.UseAuthorization();

app.UseCors("AllowAngular");

app.MapControllers();

app.Run();
