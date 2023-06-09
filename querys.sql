-- 1. Monto total de venta en los últimos 30 días
SELECT SUM([Total]) AS TotalVentas, COUNT(*) AS CantidadVentas
  FROM [Prueba].[dbo].[Venta]
 WHERE [Fecha] >= DATEADD(DAY, -30, GETDATE())
;


-- 2. Fecha y hora con la venta más alta. Monto de la venta más alta
SELECT TOP 1 [Fecha], [Total]
  FROM [Prueba].[dbo].[Venta]
 WHERE [Fecha] >= DATEADD(DAY, -30, GETDATE())
ORDER BY [Total] DESC
;


-- 3. Producto con el mayor monto de ventas
SELECT TOP 1 [producto].[ID_Producto]
	,[producto].[Nombre]
	,SUM([detalle].[TotalLinea]) AS MontoTotalVentas
FROM [Prueba].[dbo].[VentaDetalle] detalle
INNER JOIN [Prueba].[dbo].[Producto] producto ON [detalle].[ID_Producto] = [producto].[ID_Producto]
WHERE [detalle].[ID_Venta] IN (
		SELECT [Venta].[ID_Venta]
		FROM [Prueba].[dbo].[Venta] Venta
		WHERE [Venta].[Fecha] >= DATEADD(DAY, - 30, GETDATE())
		)
GROUP BY [producto].[ID_Producto], [producto].[Nombre]
ORDER BY MontoTotalVentas DESC
;

-- 4. Local con el mayor monto de ventas
SELECT TOP 1 [local].[ID_Local], [local].[Nombre], SUM([venta].[Total]) AS MontoTotalVentas
 FROM [Prueba].[dbo].[Venta] venta
       INNER JOIN [Prueba].[dbo].[Local] local ON [venta].[ID_Local] = [local].[ID_Local]
WHERE [venta].[Fecha] >= DATEADD(DAY, -30, GETDATE())
GROUP BY [local].[ID_Local], [local].[Nombre]
ORDER BY MontoTotalVentas DESC
;

-- 5. Marca con mayor margen de ganancias
SELECT 
    TOP 1 [marca].[ID_Marca],
	[marca].[Nombre],
	SUM([detalle].[TotalLinea] - [producto].[Costo_Unitario] * [detalle].[Cantidad]) AS MargenGanancias
FROM [Prueba].[dbo].[VentaDetalle] detalle
	 INNER JOIN [Prueba].[dbo].[Producto] producto ON [detalle].[ID_Producto] = [producto].[ID_Producto]
	 INNER JOIN [Prueba].[dbo].[Marca] marca ON [producto].[ID_Marca] = [marca].[ID_Marca]
WHERE [detalle].[ID_Venta] IN (
		SELECT [venta].[ID_Venta]
		FROM [Prueba].[dbo].[Venta] venta
		WHERE [venta].[Fecha] >= DATEADD(DAY, - 30, GETDATE())
		)
GROUP BY [marca].[ID_Marca], [marca].[Nombre]
ORDER BY MargenGanancias DESC
;

-- 6. Producto que mas se vende en cada local
SELECT 
	-- obtener totales de ventas de todos los productos por local
    LOCAL.Nombre AS LOCAL,
	producto.Nombre AS Producto,
	COUNT(*) AS TotalVentas
FROM 
	[Prueba].[dbo].[Venta] venta
	INNER JOIN [Prueba].[dbo].[Local] local ON venta.ID_Local = local.ID_Local
	INNER JOIN [Prueba].[dbo].[VentaDetalle] detalle ON venta.ID_Venta = detalle.ID_Venta
	INNER JOIN [Prueba].[dbo].[Producto] producto ON detalle.ID_Producto = producto.ID_Producto
WHERE venta.Fecha >= DATEADD(day, - 30, GETDATE())
-- agrupar por local y producto
GROUP BY local.Nombre, local.ID_Local, producto.Nombre
HAVING
	-- subconsulta para obtener recuento max de ventas por local y obtener solo los productos con maximo
	COUNT(*) = (
		SELECT MAX(ventaCount)
		FROM (
			SELECT COUNT(*) AS ventaCount
			FROM [Prueba].[dbo].[Venta] venta2
			INNER JOIN [Prueba].[dbo].[Local] local2 ON venta2.ID_Local = local2.ID_Local
			INNER JOIN [Prueba].[dbo].[VentaDetalle] detalle2 ON venta2.ID_Venta = detalle2.ID_Venta
			INNER JOIN [Prueba].[dbo].[Producto] producto2 ON detalle2.ID_Producto = producto2.ID_Producto
			WHERE venta2.Fecha >= DATEADD(day, - 30, GETDATE())
				AND local2.ID_Local = local.ID_Local
			GROUP BY local2.Nombre, local2.ID_Local, producto2.Nombre
			) AS aux
		)
ORDER BY LOCAL, TotalVentas desc
;