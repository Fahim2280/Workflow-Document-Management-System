using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WfDoc.Api.Application.Admins;
using WfDoc.Api.Application.Auth;
using WfDoc.Api.Application.Masters;
using WfDoc.Api.Application.Workflows;
using WfDoc.Api.Common;
using WfDoc.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IMasterService, MasterService>();
builder.Services.AddScoped<IWorkflowService, WorkflowService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var signingKey = jwtSection.GetValue<string>(nameof(JwtOptions.SigningKey)) ?? string.Empty;
var issuer = jwtSection.GetValue<string>(nameof(JwtOptions.Issuer)) ?? "WfDoc";
var audience = jwtSection.GetValue<string>(nameof(JwtOptions.Audience)) ?? "WfDoc";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.ReadWriteOnly, policy =>
        policy.RequireRole(AccessLevel.ReadWrite.ToString()));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
