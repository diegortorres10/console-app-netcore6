using console_app_netcore6.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace console_app_netcore6.Core.Repositories
{
    public static class VentasRepository
    {
        public static IEnumerable<Ventum> ConsultaDetalleVentas(int days)
        {
            using (var context = new DefontanaDbContext())
            {
                var fechaLimite = DateTime.Now.AddDays(-days);
                var ventas = context.Venta
                    .Include(x => x.VentaDetalles)
                        .ThenInclude(v => v.IdProductoNavigation)
                        .ThenInclude(v => v.IdMarcaNavigation)
                    .Include(x => x.IdLocalNavigation)
                    .Where(v => v.Fecha >= fechaLimite)
                    .ToList();

                return ventas;
            }
        }
    }
}