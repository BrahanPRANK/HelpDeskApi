param([switch]$Sobrescribir)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$infrastructureProject = Join-Path $projectRoot 'HelpDesk.Infrastructure\HelpDesk.Infrastructure.csproj'
$apiProject = Join-Path $projectRoot 'HelpDesk.Api\HelpDesk.Api.csproj'
if (-not (Test-Path -LiteralPath $infrastructureProject)) {
    throw 'Guardar este script en HelpDeskBackend\scripts antes de ejecutarlo.'
}

# Lee ConnectionStrings:DefaultConnection desde la configuracion de la API.
# En desarrollo se utiliza el almacen User Secrets; no poner claves aqui.
# Esta operacion lee el esquema, pero no crea tablas ni aplica el archivo SQL.
# --Sobrescribir permite regenerar archivos: revisar primero los cambios manuales.
$previousAspNetEnvironment = $env:ASPNETCORE_ENVIRONMENT
$previousDotNetEnvironment = $env:DOTNET_ENVIRONMENT
try {
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    $env:DOTNET_ENVIRONMENT = 'Development'
    $scaffoldArguments = @(
        'ef','dbcontext','scaffold',
        'Name=ConnectionStrings:DefaultConnection',
        'Npgsql.EntityFrameworkCore.PostgreSQL',
        '--project',$infrastructureProject,
        '--startup-project',$apiProject,
        '--schema','helpdesk',
        '--context','HelpDeskDbContext',
        '--context-dir','Data',
        '--output-dir','..\HelpDesk.Domain\Models',
        '--namespace','HelpDesk.Domain.Models',
        '--context-namespace','HelpDesk.Infrastructure.Data',
        '--no-onconfiguring',
        '--no-pluralize'
    )
    if ($Sobrescribir) { $scaffoldArguments += '--force' }
    & dotnet @scaffoldArguments
    if ($LASTEXITCODE -ne 0) { throw 'No se pudo generar el modelo.' }
    & dotnet build (Join-Path $projectRoot 'HelpDeskBackend.sln') --verbosity minimal
    if ($LASTEXITCODE -ne 0) { throw 'El modelo fue generado, pero fallo la compilacion.' }
}
finally {
    $env:ASPNETCORE_ENVIRONMENT = $previousAspNetEnvironment
    $env:DOTNET_ENVIRONMENT = $previousDotNetEnvironment
}
