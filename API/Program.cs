

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ReportApiProject.Authorize;

using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IClaimsTransformation, ReportClaimsTransformation>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000"
                )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var scheme = JwtBearerDefaults.AuthenticationScheme;

var keycloakSettings = builder.Configuration
    .GetSection(ReportApiProject.Authorize.KeycloakSettings.Name)
    .Get<ReportApiProject.Authorize.KeycloakSettings>()!;

builder.Services
    .AddAuthentication(scheme)
    .AddKeycloakJwtBearer(
        serviceName: keycloakSettings.ServiceName,
        realm: keycloakSettings.Realm,
        authenticationScheme: scheme,
        configureOptions: options =>
        {
            options.Authority = $"http://{keycloakSettings.ServiceName}/realms/{keycloakSettings.Realm}";

            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role
            };
        })
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("default");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
