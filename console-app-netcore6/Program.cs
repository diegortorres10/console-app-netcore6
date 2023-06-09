using System;
using System.Linq;
using console_app_netcore6.Core.Common;
using console_app_netcore6.Core.Repositories;

class Program
{
    static void Main(string[] args)
    {
        // 0. Get ventas de 30 dias
        var datos = VentasRepository.ConsultaDetalleVentas(30);

        // 1. Total ventas últimos 30 días
        double montoTotalVentas = datos.Sum(v => v.Total);
        int cantidadVentas = datos.Count();

        // 2. Dia y hora venta con el monto mas alto, y el monto.
        var ventaMaxima = datos.OrderByDescending(venta => venta.Total).First();
        DateTime fechaVentaMaxima = ventaMaxima.Fecha;
        int montoVentaMaxima = ventaMaxima.Total;

        // 3. Producto con mayor monto total de ventas.
        // Se crea una clase custom para registrar nombre + cantidad
        var productoMayorVenta = datos
            .SelectMany(venta => venta.VentaDetalles)
            .GroupBy(ventaDetalle => ventaDetalle.IdProducto)
            .OrderByDescending(group => group.Sum(ventaDetalle => ventaDetalle.TotalLinea))
            .Select(group => new CustomNombreValorDto
            {
                Nombre = group.First().IdProductoNavigation.Nombre,
                ValorTotal = group.Sum(ventaDetalle => ventaDetalle.TotalLinea)
            })
            .FirstOrDefault();

        /*var productoMayorVenta = datos
            .SelectMany(venta => venta.VentaDetalles)
            .GroupBy(ventaDetalle => ventaDetalle.IdProducto)
            .OrderByDescending(group => group.Sum(ventaDetalle => ventaDetalle.TotalLinea))
            .Select(group => group.First().IdProductoNavigation)
            // .Select(group => group.Key)
            .First();*/

        // 4. Local con mayor monto de ventas
        var localMayorVenta = datos
            .GroupBy(venta => venta.IdLocalNavigation)
            .OrderByDescending(group => group.Sum(venta => venta.Total))
            .Select(group => group.Key)
            .FirstOrDefault();

        // 5. Marca con mayor margen de ganancias
        var marcaMayorMargen = datos
            .SelectMany(venta => venta.VentaDetalles)
            .GroupBy(ventaDetalle => ventaDetalle.IdProductoNavigation.IdMarca)
            .OrderByDescending(group => group.Sum(ventaDetalle => ventaDetalle.TotalLinea - ventaDetalle.IdProductoNavigation.CostoUnitario * ventaDetalle.Cantidad))
            //.Select(group => group.First().IdProductoNavigation.IdMarcaNavigation)
            .Select(group => new CustomNombreValorDto
            {
                Nombre = group.First().IdProductoNavigation.IdMarcaNavigation.Nombre,
                ValorTotal = group.Sum(ventaDetalle => ventaDetalle.TotalLinea - ventaDetalle.IdProductoNavigation.CostoUnitario * ventaDetalle.Cantidad)
            })
            .FirstOrDefault();

        // 6. Producto que mas se vende en cada local
        var productosMasVendidosPorLocal = datos
            .SelectMany(venta => venta.VentaDetalles)
            // agrupar detalles por local y nombre
            .GroupBy(ventaDetalle => new { ventaDetalle.IdVentaNavigation.IdLocal, ventaDetalle.IdVentaNavigation.IdLocalNavigation, ventaDetalle.IdProductoNavigation.Nombre })
            .Select(group => new
            {
                // proyectar los resultados con el total de ventas
                Local = group.Key.IdLocal,
                LocalNombre = group.Key.IdLocalNavigation.Nombre,
                Producto = group.Key.Nombre,
                TotalVentas = group.Count()
            })
            // agrupar solo por local
            .GroupBy(res => res.Local)
            // seleccionar el registro con el mayor total de ventas en cada grupo
            .Select(group =>
            {
                // obtener el maximo de ventas en cada grupo
                int MaxVentas = group.Max(res => res.TotalVentas);
                // fiiltra para obtener solo los productos con maximo total de ventas
                return group.Where(res => res.TotalVentas == MaxVentas);
            })
            .ToList();

        // Obtener totales de venta de todos los productos por local (todo: max por local)
        /*var productosMasVendidosPorLocal = datos
            .SelectMany(venta => venta.VentaDetalles)
            .GroupBy(ventaDetalle => new { ventaDetalle.IdVentaNavigation.IdLocalNavigation, ventaDetalle.IdProducto })
            .Select(group => new
            {
                Local = group.Key.IdLocalNavigation,
                ProductoMasVendido = group
                    .OrderByDescending(ventaDetalle => ventaDetalle.Cantidad)
                    .Select(ventaDetalle => ventaDetalle.IdProductoNavigation)
                    .First()
            });*/

        Console.WriteLine($"1. Monto total de venta en los últimos 30 días: {montoTotalVentas}. Cantidad de ventas totales: {cantidadVentas}");
        Console.WriteLine($"2. Fecha y hora con la venta más alta: {fechaVentaMaxima}. Monto de la venta más alta: {montoVentaMaxima}");
        Console.WriteLine($"3. Producto con el mayor monto de ventas: {productoMayorVenta.Nombre}, con {productoMayorVenta.ValorTotal} ventas totales");
        Console.WriteLine($"4. Local con el mayor monto de ventas: {localMayorVenta.Nombre}");
        Console.WriteLine($"5. Marca con mayor margen de ganancias: {marcaMayorMargen.Nombre}, con {marcaMayorMargen.ValorTotal} ventas totales");
        Console.WriteLine($"6. Productos que más se vende en cada local:");
        Console.WriteLine($"Local \t\t Producto \t TotalVentas ");

        foreach (dynamic local in productosMasVendidosPorLocal)
        {
            foreach (var producto in local)
            {
                Console.WriteLine($"{producto.LocalNombre} \t\t {producto.Producto} \t {producto.TotalVentas}");
            }
        }

    }
}