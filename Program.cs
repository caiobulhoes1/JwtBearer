using JwtBearer;
using JwtBearer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<JwtService>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.PrivateKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("PremiumPolicy", x => x.RequireRole("Premium"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (JwtService service) => service.Create(new JwtBearer.Models.User(1, "Caio Sabino", "teste@caio.sabino", "imagem", "12345", new[] {"Student","Premium"} )));

app.MapGet("/restrito", (ClaimsPrincipal user) => $"Autenticou com o usuário: {user.Identity.Name}").RequireAuthorization();

app.MapGet("/premium", (ClaimsPrincipal user) => $"Autenticou com o usuário: {user.Identity.Name}").RequireAuthorization("PremiumPolicy");

app.Run();