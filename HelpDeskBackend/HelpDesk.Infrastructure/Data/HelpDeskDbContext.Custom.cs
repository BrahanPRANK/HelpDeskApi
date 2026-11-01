using HelpDesk.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace HelpDesk.Infrastructure.Data;
public partial class HelpDeskDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // El scaffolding interpreta los indices unicos parciales como relaciones 1:1.
        // El historial es 1:N; el indice SQL solo limita la asignacion vigente.
        modelBuilder.Entity<Tickets>().Ignore(x=>x.AsignacionesTickets);
        modelBuilder.Entity<AsignacionesTickets>().HasOne(x=>x.IdTicketNavigation).WithMany()
            .HasForeignKey(x=>x.IdTicket).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Activos>().Ignore(x=>x.AsignacionesActivos);
        modelBuilder.Entity<AsignacionesActivos>().HasOne(x=>x.IdActivoNavigation).WithMany()
            .HasForeignKey(x=>x.IdActivo).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Tickets>().Property(x=>x.Version).IsConcurrencyToken();
        modelBuilder.Entity<RecuperacionesClave>().Property(x=>x.FechaUso).IsConcurrencyToken();
        modelBuilder.Entity<Aprobaciones>().Property(x=>x.Estado).IsConcurrencyToken();
        modelBuilder.Entity<Usuarios>().Property(x=>x.ClaveHash).IsConcurrencyToken();
    }
}
