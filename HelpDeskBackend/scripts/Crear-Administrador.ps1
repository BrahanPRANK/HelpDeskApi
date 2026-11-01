param(
    [Parameter(Mandatory=$true)][string]$NombreUsuario,
    [Parameter(Mandatory=$true)][string]$Correo
)
$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$project=Join-Path $root 'HelpDesk.Api\HelpDesk.Api.csproj'
if(-not(Test-Path -LiteralPath $project)){throw 'Ejecutar desde HelpDeskBackend\scripts.'}
$password=Read-Host 'Clave del administrador inicial (minimo 10 caracteres)' -AsSecureString
$pointer=[Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)
$saved=@{}
$names=@('Bootstrap__NombreUsuario','Bootstrap__Correo','Bootstrap__Clave','ASPNETCORE_ENVIRONMENT','DOTNET_ENVIRONMENT')
foreach($name in $names){$saved[$name]=[Environment]::GetEnvironmentVariable($name,'Process')}
try{
    $env:Bootstrap__NombreUsuario=$NombreUsuario
    $env:Bootstrap__Correo=$Correo
    $env:Bootstrap__Clave=[Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer)
    $env:ASPNETCORE_ENVIRONMENT='Development'
    $env:DOTNET_ENVIRONMENT='Development'
    & dotnet run --project $project --no-launch-profile -- --crear-admin
    if($LASTEXITCODE -ne 0){throw 'No se pudo crear el administrador. Revise si ya existe uno o si falta configurar la conexion.'}
}
finally{
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer)
    foreach($name in $names){[Environment]::SetEnvironmentVariable($name,$saved[$name],'Process')}
    $password.Dispose()
}
