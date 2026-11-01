using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Servicios
{
    public Guid IdServicio { get; set; }

    public Guid IdEquipoResponsable { get; set; }

    public string Nombre { get; set; } = null!;

    public Guid IdCategoriaServicio { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool RequiereAprobacion { get; set; }

    public bool Activo { get; set; }

    public virtual CategoriasServicios IdCategoriaServicioNavigation { get; set; } = null!;

    public virtual EquiposSoporte IdEquipoResponsableNavigation { get; set; } = null!;

    public virtual ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();
}
