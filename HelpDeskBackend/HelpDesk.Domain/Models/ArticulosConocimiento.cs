using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class ArticulosConocimiento
{
    public Guid IdArticulo { get; set; }

    public Guid IdAutor { get; set; }

    public Guid? IdRevisor { get; set; }

    public Guid IdCategoriaConocimiento { get; set; }

    public string Titulo { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public bool EsInterno { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaPublicacion { get; set; }

    public virtual Usuarios IdAutorNavigation { get; set; } = null!;

    public virtual CategoriasConocimiento IdCategoriaConocimientoNavigation { get; set; } = null!;

    public virtual Usuarios? IdRevisorNavigation { get; set; }

    public virtual ICollection<TicketsArticulos> TicketsArticulos { get; set; } = new List<TicketsArticulos>();
}
