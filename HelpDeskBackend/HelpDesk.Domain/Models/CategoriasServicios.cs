using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class CategoriasServicios
{
    public Guid IdCategoriaServicio { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Servicios> Servicios { get; set; } = new List<Servicios>();
}
