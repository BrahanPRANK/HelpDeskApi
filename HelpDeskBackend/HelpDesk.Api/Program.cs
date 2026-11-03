using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Domain.Dtos.Autenticacion;
using System.Threading.RateLimiting;
using System.Text.Json.Serialization;
using HelpDesk.Api.Middleware;
using HelpDesk.Api.Security;
using HelpDesk.Application.Common;
using HelpDesk.Application.Configuration;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Ports;
using HelpDesk.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;

var bootstrap = args.Contains("--crear-admin");
var builder = WebApplication.CreateBuilder(args.Where(x => x != "--crear-admin").ToArray());
// Cargar explicitamente los secretos del proyecto API solo en desarrollo.
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<HelpDesk.Api.Security.CurrentUser>(optional: true);
    // Las variables del proceso conservan prioridad para pruebas y despliegues locales.
    builder.Configuration.AddEnvironmentVariables();
}
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow);
builder.Services.Configure<ApiBehaviorOptions>(o => o.InvalidModelStateResponseFactory = context =>
    new BadRequestObjectResult(new ResponseDto<object>(400, null, string.Join(" ", context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)))));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "HelpDesk API - primera version", Version = "v1" });
    o.CustomSchemaIds(SchemaId);
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", Description = "Token obtenido en /api/auth/login." });
    o.AddSecurityRequirement(document => new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] });
});
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = 429;
    o.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "local",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 20, Window = TimeSpan.FromMinutes(1), QueueLimit = 0, AutoReplenishment = true }));
});
var app = builder.Build();
if (bootstrap)
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<AuthService>().Bootstrap(new RegistroDto
    {
        Nombres = builder.Configuration["Bootstrap:Nombres"] ?? "Administrador",
        Apellidos = builder.Configuration["Bootstrap:Apellidos"] ?? "Inicial",
        NombreUsuario = builder.Configuration["Bootstrap:NombreUsuario"] ?? string.Empty,
        Correo = builder.Configuration["Bootstrap:Correo"] ?? string.Empty,
        Clave = builder.Configuration["Bootstrap:Clave"] ?? string.Empty
    });
    Console.WriteLine("Administrador inicial creado. No se imprimieron credenciales.");
    return;
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePages(async status =>
{
    var response = status.HttpContext.Response;
    await response.WriteAsJsonAsync(new ResponseDto<object>(response.StatusCode, null,
        response.StatusCode switch { 401 => "Inicie sesion con un token valido.", 403 => "No tiene acceso.", 429 => "Demasiados intentos. Espere un minuto.", _ => "Solicitud no disponible." }));
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HelpDesk API v1");
        options.RoutePrefix = string.Empty;
    });
}
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.Run();

static string SchemaId(Type type) => type.IsGenericType ? type.Name.Split('`')[0] + "_" + string.Join("_", type.GetGenericArguments().Select(SchemaId)) : type.Name;
