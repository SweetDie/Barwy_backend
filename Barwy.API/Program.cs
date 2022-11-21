using Barwy.API.Infrastructure.AutoMapper;
using Barwy.API.Infrastructure.Repositories;
using Barwy.API.Infrastructure.Services;
using Barwy.Data.Data.Context;
using Barwy.Data.Initializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add dababase context
builder.Services.AddDbContext<AppDbContext>();

// Add services to the container.
builder.Services.AddControllers();

// Jwt configuration
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Key"]);
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero,
    ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
    ValidAudience = builder.Configuration["JwtConfig:Audience"],
};

builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = tokenValidationParameters;
});

// Add AutoMapper configuration 
AutoMapperConfiguration.Config(builder.Services);

//Add Services configuration
ServicesConfiguration.Config(builder.Services);

// Add Repositories configuration
RepositoriesConfiguration.Config(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options
    .WithOrigins(new[] { "http://localhost:3000" })
    .AllowAnyHeader()
    .AllowCredentials()
    .AllowAnyMethod()
);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await AppDbInitializer.Seed(app);
app.Run();
