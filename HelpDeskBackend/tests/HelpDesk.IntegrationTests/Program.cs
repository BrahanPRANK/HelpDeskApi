using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Npgsql;

var root=FindRoot();
var sourceSql=Path.Combine(Directory.GetParent(root)!.FullName,"001_helpdesk_36_tablas_es.sql");
var project=XDocument.Load(Path.Combine(root,"HelpDesk.Api","HelpDesk.Api.csproj"));
var sid=project.Descendants("UserSecretsId").Single().Value;
using var config=JsonDocument.Parse(File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"Microsoft","UserSecrets",sid,"secrets.json")));
var original=config.RootElement.GetProperty("ConnectionStrings").GetProperty("DefaultConnection").GetString();
var dbname="helpdesk_test_"+Guid.NewGuid().ToString("N")[..16];
if(!System.Text.RegularExpressions.Regex.IsMatch(dbname,"^helpdesk_test_[a-f0-9]{16}$"))throw new Exception("Unsafe test database name.");
var adminSettings=new NpgsqlConnectionStringBuilder(original){Database="postgres",Pooling=false};
var testSettings=new NpgsqlConnectionStringBuilder(original){Database=dbname,Pooling=false};
var adminPassword=Convert.ToBase64String(RandomNumberGenerator.GetBytes(24))+"Aa1!";
var userPassword=Convert.ToBase64String(RandomNumberGenerator.GetBytes(24))+"Aa1!";
var jwtKey=Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
var listener=new TcpListener(System.Net.IPAddress.Loopback,0);listener.Start();var port=((System.Net.IPEndPoint)listener.LocalEndpoint).Port;listener.Stop();
var client=new HttpClient{BaseAddress=new Uri($"http://127.0.0.1:{port}"),Timeout=TimeSpan.FromSeconds(30)};
var log=new List<string>();var logLock=new object();Process? server=null;var created=false;int checks=0;
var dll=Path.Combine(root,"HelpDesk.Api","bin","Debug","net10.0","HelpDesk.Api.dll");

Process Start(bool bootstrap)
{
    var start=new ProcessStartInfo("dotnet") {WorkingDirectory=Path.Combine(root,"HelpDesk.Api"),RedirectStandardOutput=true,RedirectStandardError=true,UseShellExecute=false,CreateNoWindow=true,WindowStyle=ProcessWindowStyle.Hidden};
    start.ArgumentList.Add(dll);if(bootstrap)start.ArgumentList.Add("--crear-admin");
    start.Environment["ASPNETCORE_ENVIRONMENT"]="Development";start.Environment["DOTNET_ENVIRONMENT"]="Development";
    start.Environment["ASPNETCORE_URLS"]=$"http://127.0.0.1:{port}";
    start.Environment["ConnectionStrings__DefaultConnection"]=testSettings.ConnectionString;
    start.Environment["Jwt__Key"]=jwtKey;start.Environment["Jwt__Issuer"]="HelpDesk.Api";start.Environment["Jwt__Audience"]="HelpDesk.Web";
    start.Environment["Bootstrap__NombreUsuario"]="admin_test";start.Environment["Bootstrap__Correo"]="admin@example.test";start.Environment["Bootstrap__Clave"]=adminPassword;
    var process=new Process{StartInfo=start};
    process.OutputDataReceived+=(_,e)=>{if(e.Data is not null)lock(logLock)log.Add(e.Data);};
    process.ErrorDataReceived+=(_,e)=>{if(e.Data is not null)lock(logLock)log.Add(e.Data);};
    process.Start();process.BeginOutputReadLine();process.BeginErrorReadLine();return process;
}
async Task<JsonObject> Request(HttpMethod method,string path,object? data,int expected,string? token=null)
{
    using var request=new HttpRequestMessage(method,path);
    if(token is not null)request.Headers.Authorization=new AuthenticationHeaderValue("Bearer",token);
    if(data is not null)request.Content=JsonContent.Create(data);
    using var response=await client.SendAsync(request);var content=await response.Content.ReadAsStringAsync();
    if((int)response.StatusCode!=expected)throw new Exception($"{method} {path}: {(int)response.StatusCode}, expected {expected}. {content[..Math.Min(content.Length,600)]}");
    checks++;return JsonNode.Parse(content)?.AsObject()??new();
}
async Task<JsonObject> Post(string path,object data,string token)=> (await Request(HttpMethod.Post,path,data,201,token))["data"]!.AsObject();
async Task<string> Login(string name,string password)=> (await Request(HttpMethod.Post,"/api/auth/login",new{nombreUsuario=name,clave=password},200))["data"]!["token"]!.GetValue<string>();
Guid Id(JsonObject obj,string key)=>Guid.Parse(obj[key]!.GetValue<string>());
JsonArray Items(JsonObject response)=>response["data"]!["items"]!.AsArray();
try
{
    await using(var admin=new NpgsqlConnection(adminSettings.ConnectionString))
    {await admin.OpenAsync();await using var cmd=new NpgsqlCommand($"CREATE DATABASE \"{dbname}\"",admin);await cmd.ExecuteNonQueryAsync();created=true;}
    await using(var test=new NpgsqlConnection(testSettings.ConnectionString))
    {await test.OpenAsync();await using var cmd=new NpgsqlCommand(File.ReadAllText(sourceSql),test){CommandTimeout=60};await cmd.ExecuteNonQueryAsync();}
    using(var bootstrap=Start(true))
    {await bootstrap.WaitForExitAsync();if(bootstrap.ExitCode!=0)throw new Exception("Administrator bootstrap failed.");}
    server=Start(false);
    for(int attempt=0;;attempt++)
    {
        try{using var response=await client.GetAsync("/swagger/v1/swagger.json");if(response.IsSuccessStatusCode)break;}
        catch(HttpRequestException){}
        if(attempt>=60||server.HasExited)throw new Exception("API did not start or Swagger failed.");
        await Task.Delay(500);
    }
    var swagger=await client.GetStringAsync("/swagger/v1/swagger.json");
    var spec=JsonNode.Parse(swagger)!;
    if(!swagger.Contains("Bearer")||!swagger.Contains("SedesInputDto"))throw new Exception("Swagger contracts/security missing.");checks++;
    await Request(HttpMethod.Get,"/api/Sedes",null,401);
    var adminToken=await Login("admin_test",adminPassword);
    await Request(HttpMethod.Post,"/api/auth/login",new{nombreUsuario="admin_test",clave="incorrecta"},401);
    var resources=JsonNode.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"resources.json")))!.AsArray();
    foreach(var resource in resources)await Request(HttpMethod.Get,"/api/"+resource!["name"]!.GetValue<string>(),null,200,adminToken);
    var roles=Items(await Request(HttpMethod.Get,"/api/Roles",null,200,adminToken));
    Guid Role(string name)=>Guid.Parse(roles.Single(r=>r!["nombre"]!.GetValue<string>()==name)!["idRol"]!.GetValue<string>());
    var site=await Post("/api/Sedes",new{nombre="Sede prueba",direccion="Prueba",activo=true},adminToken);var siteId=Id(site,"idSede");
    await Request(HttpMethod.Put,$"/api/Sedes/{siteId}",new{nombre="Sede actualizada",direccion="Prueba",activo=true},200,adminToken);
    await Request(HttpMethod.Get,$"/api/Sedes/{siteId}",null,200,adminToken);
    var area=await Post("/api/Areas",new{idSede=siteId,nombre="Area prueba",activo=true},adminToken);var areaId=Id(area,"idArea");
    await Request(HttpMethod.Delete,$"/api/Sedes/{siteId}",null,409,adminToken);
    var disposable=await Post("/api/Sedes",new{nombre="Eliminar",activo=true},adminToken);
    await Request(HttpMethod.Delete,$"/api/Sedes/{Id(disposable,"idSede")}",null,200,adminToken);
    var team=await Post("/api/EquiposSoporte",new{nombre="Soporte prueba",activo=true},adminToken);var teamId=Id(team,"idEquipo");
    var userIds=new List<Guid>();
    foreach(var name in new[]{"empleado1","empleado2","tecnico1"})
    {
        var user=await Post("/api/Usuarios",new{idArea=areaId,nombres=name,apellidos="Prueba",nombreUsuario=name,correo=name+"@example.test",clave=userPassword,activo=true},adminToken);
        if(user.ContainsKey("claveHash")||user.ContainsKey("clave"))throw new Exception("Password leaked in response.");checks++;
        var userId=Id(user,"idUsuario");userIds.Add(userId);
        await Post("/api/UsuariosRoles",new{idUsuario=userId,idRol=Role(name=="tecnico1"?"tecnico":"empleado")},adminToken);
    }
    await Post("/api/UsuariosRoles",new{idUsuario=userIds[2],idRol=Role("coordinador")},adminToken);
    await Post("/api/IntegrantesEquipo",new{idEquipo=teamId,idUsuario=userIds[2],esCoordinador=true,disponible=true,activo=true},adminToken);
    await Post("/api/PoliticasAtencion",new{prioridad="media",minutosPrimeraRespuesta=30,minutosResolucion=120,activo=true},adminToken);
    var categories=Items(await Request(HttpMethod.Get,"/api/CategoriasServicios",null,200,adminToken));
    var service=await Post("/api/Servicios",new{idEquipoResponsable=teamId,nombre="Servicio prueba",idCategoriaServicio=categories[0]!["idCategoriaServicio"]!.GetValue<string>(),descripcion="Soporte",requiereAprobacion=false,activo=true},adminToken);
    var serviceId=Id(service,"idServicio");
    var employee=await Login("empleado1",userPassword);var stranger=await Login("empleado2",userPassword);var technician=await Login("tecnico1",userPassword);
    await Request(HttpMethod.Post,"/api/UsuariosRoles",new{idUsuario=userIds[0],idRol=Role("administrador")},403,employee);
    await Request(HttpMethod.Get,"/api/Auditoria",null,403,employee);
    await Request(HttpMethod.Post,"/api/auth/registro",new{nombres="X",apellidos="Y",nombreUsuario="malicioso",correo="x@example.test",clave=userPassword,roles=new[]{"administrador"}},400);
    var ticket=await Post("/api/Tickets",new{idServicio=serviceId,asunto="Sin internet",descripcion="No conecta",tipo="incidente",prioridad="media"},employee);
    var ticketId=Id(ticket,"idTicket");
    await Request(HttpMethod.Get,$"/api/Tickets/{ticketId}",null,404,stranger);
    var strangers=Items(await Request(HttpMethod.Get,"/api/Tickets",null,200,stranger));if(strangers.Count!=0)throw new Exception("Ticket list exposed another user.");checks++;
    async Task<JsonObject> Action(string action,string token,int version,string? reason=null)=>
        (await Request(HttpMethod.Post,$"/api/Tickets/{ticketId}/{action}",new{version,motivo=reason??"Prueba",idEquipo=teamId,idTecnico=userIds[2]},200,token))["data"]!.AsObject();
    ticket=await Action("asignar",technician,ticket["version"]!.GetValue<int>());
    ticket=await Action("iniciar",technician,ticket["version"]!.GetValue<int>());
    var internalComment=await Post("/api/Comentarios",new{idTicket=ticketId,mensaje="Nota tecnica interna",esInterno=true},technician);
    await Request(HttpMethod.Get,$"/api/Comentarios/{Id(internalComment,"idComentario")}",null,404,employee);
    await Post("/api/Comentarios",new{idTicket=ticketId,mensaje="Estamos revisando",esInterno=false},technician);
    ticket=(await Request(HttpMethod.Get,$"/api/Tickets/{ticketId}",null,200,technician))["data"]!.AsObject();
    var stale=ticket["version"]!.GetValue<int>();ticket=await Action("resolver",technician,stale,"Conexion restablecida");
    await Request(HttpMethod.Post,$"/api/Tickets/{ticketId}/resolver",new{version=stale,motivo="Duplicada"},409,technician);
    ticket=await Action("cerrar",employee,ticket["version"]!.GetValue<int>());
    await Post("/api/Encuestas",new{idTicket=ticketId,calificacion=5,comentario="Resuelto"},employee);
    await Request(HttpMethod.Post,"/api/Encuestas",new{idTicket=ticketId,calificacion=5},409,employee);
    // Activos e historial 1:N: devolver y volver a asignar el mismo bien.
    var assetTypes=Items(await Request(HttpMethod.Get,"/api/TiposActivos",null,200,adminToken));
    var asset=await Post("/api/Activos",new{idSede=siteId,idArea=areaId,codigoPatrimonial="PC-TEST-001",nombre="PC prueba",idTipoActivo=assetTypes[0]!["idTipoActivo"]!.GetValue<string>(),estado="disponible"},adminToken);
    var assetId=Id(asset,"idActivo");
    var assetAssignment=await Post("/api/AsignacionesActivos",new{idActivo=assetId,idUsuarioResponsable=userIds[0]},adminToken);
    await Request(HttpMethod.Post,$"/api/AsignacionesActivos/{Id(assetAssignment,"idAsignacion")}/devolver",new{},200,adminToken);
    await Post("/api/AsignacionesActivos",new{idActivo=assetId,idUsuarioResponsable=userIds[1]},adminToken);
    await Post("/api/TicketsActivos",new{idTicket=ticketId,idActivo=assetId},adminToken);
    await Request(HttpMethod.Get,$"/api/TicketsActivos/{ticketId}/{assetId}",null,200,adminToken);
    await Request(HttpMethod.Delete,$"/api/TicketsActivos/{ticketId}/{assetId}",null,200,adminToken);
    // Aprobacion, permiso actualizado con el mismo JWT y tareas de un cambio.
    await Post("/api/UsuariosRoles",new{idUsuario=userIds[2],idRol=Role("aprobador")},adminToken);
    var change=await Post("/api/Cambios",new{idResponsable=userIds[2],titulo="Cambio prueba",descripcion="Actualizar equipo",riesgo="bajo",planEjecucion="Actualizar",planReversion="Restaurar",requiereAprobacion=true},adminToken);
    var changeId=Id(change,"idCambio");
    await Request(HttpMethod.Post,$"/api/Cambios/{changeId}/programar",new{fechaProgramada=DateTime.UtcNow.AddHours(1)},409,technician);
    var approval=await Post("/api/Aprobaciones",new{idCambio=changeId,idAprobador=userIds[2],orden=1,motivo="Autorizar"},adminToken);
    await Request(HttpMethod.Post,$"/api/Aprobaciones/{Id(approval,"idAprobacion")}/aprobar",new{motivo="Aprobado"},200,technician);
    await Request(HttpMethod.Post,$"/api/Aprobaciones/{Id(approval,"idAprobacion")}/aprobar",new{motivo="Repetido"},409,technician);
    await Request(HttpMethod.Post,$"/api/Cambios/{changeId}/programar",new{fechaProgramada=DateTime.UtcNow.AddHours(1)},200,technician);
    var task=await Post("/api/TareasCambios",new{idCambio=changeId,idResponsable=userIds[2],orden=1,titulo="Actualizar",estado="pendiente"},adminToken);
    await Request(HttpMethod.Post,$"/api/Cambios/{changeId}/ejecutar",new{},200,technician);
    await Request(HttpMethod.Post,$"/api/Cambios/{changeId}/finalizar",new{motivo="Hecho"},409,technician);
    await Request(HttpMethod.Put,$"/api/TareasCambios/{Id(task,"idTareaCambio")}",new{idCambio=changeId,idResponsable=userIds[2],orden=1,titulo="Actualizar",estado="completada"},200,adminToken);
    await Request(HttpMethod.Post,$"/api/Cambios/{changeId}/finalizar",new{motivo="Cambio realizado"},200,technician);
    // Publicacion de conocimiento y relacion con ticket.
    var knowledgeCategories=Items(await Request(HttpMethod.Get,"/api/CategoriasConocimiento",null,200,adminToken));
    var article=await Post("/api/ArticulosConocimiento",new{idCategoriaConocimiento=knowledgeCategories[0]!["idCategoriaConocimiento"]!.GetValue<string>(),titulo="Reiniciar red",contenido="Instrucciones de prueba",esInterno=false},adminToken);
    var articleId=Id(article,"idArticulo");
    await Request(HttpMethod.Get,$"/api/ArticulosConocimiento/{articleId}",null,404,employee);
    await Request(HttpMethod.Post,$"/api/ArticulosConocimiento/{articleId}/publicar",new{},200,technician);
    await Request(HttpMethod.Get,$"/api/ArticulosConocimiento/{articleId}",null,200,employee);
    await Post("/api/TicketsArticulos",new{idTicket=ticketId,idArticulo=articleId,idUsuario=userIds[0]},adminToken);
    await Request(HttpMethod.Get,"/api/Reportes/indicadores",null,200,employee);
    await Request(HttpMethod.Get,"/api/Sedes?tamano=1000",null,400,adminToken);
    await Request(HttpMethod.Get,"/api/Sedes",null,401,adminToken[..^10]+"invalidjwt");
    await Request(HttpMethod.Post,"/api/Auditoria",new{},405,adminToken);
    var recovery=(await Request(HttpMethod.Post,$"/api/auth/recuperacion/{userIds[0]}",new{},201,adminToken))["data"]!;
    var resetToken=recovery["token"]!.GetValue<string>();var nextPassword=userPassword+"New";
    await Request(HttpMethod.Post,"/api/auth/restablecer-clave",new{token=resetToken,claveNueva=nextPassword},200);
    await Request(HttpMethod.Get,"/api/Tickets",null,401,employee);
    await Request(HttpMethod.Post,"/api/auth/restablecer-clave",new{token=resetToken,claveNueva=nextPassword},400);
    await Login("empleado1",nextPassword);
    await using(var test=new NpgsqlConnection(testSettings.ConnectionString))
    {
        await test.OpenAsync();await using var cmd=new NpgsqlCommand("SELECT clave_hash FROM helpdesk.usuarios WHERE nombre_usuario='empleado2'",test);
        var hash=(string)(await cmd.ExecuteScalarAsync())!;if(hash==userPassword||!hash.StartsWith("AQAAAA"))throw new Exception("Password hashing not applied.");checks++;
    }
    Console.WriteLine($"PASS: {checks} HTTP/security/database checks. CRUD, scoped access, ticket workflow, recovery and hashing verified.");
    var reportPath=Environment.GetEnvironmentVariable("HELPDESK_TEST_RESULT");
    if(!string.IsNullOrWhiteSpace(reportPath))File.WriteAllText(reportPath,JsonSerializer.Serialize(new{Passed=checks,TemporaryDatabase=dbname,ProductionDataChanged=false}));
}
catch(Exception ex)
{
    Console.Error.WriteLine(ex.Message);
    lock(logLock)foreach(var line in log.TakeLast(40))Console.Error.WriteLine(line);
    Environment.ExitCode=1;
}
finally
{
    if(server is not null){if(!server.HasExited)server.Kill(true);await server.WaitForExitAsync();server.Dispose();}
    client.Dispose();NpgsqlConnection.ClearAllPools();
    if(created)
    {
        await using var admin=new NpgsqlConnection(adminSettings.ConnectionString);await admin.OpenAsync();
        await using var cmd=new NpgsqlCommand($"DROP DATABASE \"{dbname}\" WITH (FORCE)",admin);await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("Temporary test database removed. helpdeskdb was not modified.");
    }
}

static string FindRoot()
{
    foreach(var start in new[]{Directory.GetCurrentDirectory(),AppContext.BaseDirectory})
        for(var current=new DirectoryInfo(start);current is not null;current=current.Parent)
            if(File.Exists(Path.Combine(current.FullName,"HelpDeskBackend.sln")))return current.FullName;
    throw new InvalidOperationException("Ejecute desde HelpDeskBackend o una de sus subcarpetas.");
}
