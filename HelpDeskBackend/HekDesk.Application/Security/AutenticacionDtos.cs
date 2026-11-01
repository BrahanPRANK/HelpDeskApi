using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using HelpDesk.Application.Common;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;

namespace HelpDesk.Application.Security;

public class RegistroDto
{
    [Required,StringLength(100)] public string Nombres {get;set;}=string.Empty;
    [Required,StringLength(150)] public string Apellidos {get;set;}=string.Empty;
    [Required,StringLength(100)] public string NombreUsuario {get;set;}=string.Empty;
    [Required,EmailAddress,StringLength(254)] public string Correo {get;set;}=string.Empty;
    [Required,StringLength(128,MinimumLength=10)] public string Clave {get;set;}=string.Empty;
}
public class LoginDto
{
    [Required,StringLength(100)] public string NombreUsuario {get;set;}=string.Empty;
    [Required,StringLength(128)] public string Clave {get;set;}=string.Empty;
}
public class CambioClaveDto
{
    [Required] public string ClaveActual {get;set;}=string.Empty;
    [Required,StringLength(128,MinimumLength=10)] public string ClaveNueva {get;set;}=string.Empty;
}
public class RestablecerClaveDto
{
    [Required] public string Token {get;set;}=string.Empty;
    [Required,StringLength(128,MinimumLength=10)] public string ClaveNueva {get;set;}=string.Empty;
}
public record UsuarioRegistradoDto(Guid IdUsuario,string NombreUsuario);
public record SesionDto(string Token,DateTime FechaVencimiento,Guid IdUsuario,string NombreUsuario,IReadOnlyList<string> Roles);
public record TokenRecuperacionDto(string Token,DateTime FechaVencimiento);
