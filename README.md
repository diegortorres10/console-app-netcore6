# console-app-netcore6
Proyecto net core 6, console app

## Key Features
Realizar consulta a bd y ejemplos prácticos con linq

## Pre requisito
Crear archivo `appsettings.json` dentro de la carpeta `\console-app-netcore6` con la siguiente información:

```console
{
  "ConnectionStrings": {
    "Default": "cadena-conexion"
  }
}
```
Proyecto con SqlServer, la cadena se forma de la siguiente manera: 
`Server=url-server,numero-puerto;Database=nombre-db;User Id=usuario;Password=contaseña-usuario;`

## Build and run

```console
dotnet build
dotnet run
```
