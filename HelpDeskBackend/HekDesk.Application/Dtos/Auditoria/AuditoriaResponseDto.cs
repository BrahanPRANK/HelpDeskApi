namespace HelpDesk.Application.Dtos.Auditoria;

public class AuditoriaResponseDto
{
    public Guid IdAuditoria
    { get; set; }
    public Guid? IdUsuario
    { get; set; }
    public string Accion
    { get; set; } = string.Empty;
    public string TablaAfectada
    { get; set; } = string.Empty;
    public Guid? IdRegistro
    { get; set; }
    public string Descripcion
    { get; set; } = string.Empty;
    public DateTime Fecha
    { get; set; }
}
