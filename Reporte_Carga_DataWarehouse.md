# Reporte de Proceso de Extracción, Transformación y Carga (ETL)
**Proyecto**: Sistema Análisis de Ventas
**Fase**: Carga de Data Warehouse (Hechos y Dimensiones)

---

## 1. Introducción
El presente documento describe el proceso funcional de la fase de carga (Load) hacia el Data Warehouse `VentasAnalitica`. Se implementó un patrón de arquitectura limpia (Clean Architecture) operando 100% en memoria, garantizando eficiencia al realizar la inserción de datos masivos. 

## 2. Proceso de Limpieza (Truncado) de Tablas
Para garantizar la integridad y evitar duplicidad de datos en cada ciclo del ETL, se creó un proceso de limpieza inicial. Este proceso requiere eliminar primero la tabla de hechos (por las llaves foráneas) y luego las dimensiones en cascada.

En el código se refleja a través de las siguientes instrucciones asíncronas de alto rendimiento (EF Core Bulk Updates):
```csharp
// 1. Limpieza de Tabla de Hechos (Fact)
await _context.Fact_Ventas.ExecuteDeleteAsync(cancellationToken);

// 2. Limpieza de Tablas de Dimensiones (Dimensions)
await _context.Dim_Producto.ExecuteDeleteAsync(cancellationToken);
await _context.Dim_Cliente.ExecuteDeleteAsync(cancellationToken);
await _context.Dim_Tiempo.ExecuteDeleteAsync(cancellationToken);
await _context.Dim_Sucursal.ExecuteDeleteAsync(cancellationToken);
await _context.Dim_Ubicacion.ExecuteDeleteAsync(cancellationToken);
await _context.Dim_Fuente_Datos.ExecuteDeleteAsync(cancellationToken);
```

## 3. Descripción de Tablas Cargadas

### Dimensiones Cargadas
Las dimensiones proveen el contexto empresarial al Data Warehouse, permitiendo filtrar y agrupar los datos de ventas:

1. **Dim_Cliente**: Almacena de forma centralizada la información del comprador. Aislada de los múltiples orígenes (API, Base de datos y CSV).
2. **Dim_Producto**: Catálogo unificado de los artículos, independientemente de si provienen del archivo de la web o de los registros de CSV, integrados por su `ProductoOrigenId`.
3. **Dim_Tiempo**: Dimensión crítica para realizar reportes cronológicos y de series de tiempo. Descompone la fecha exacta de las órdenes en `Día`, `Mes`, `Año`, `Trimestre`, etc.
4. **Dim_Sucursal**: Consolida la ubicación y el local físico o virtual desde donde se gestionó la venta.
5. **Dim_Ubicacion**: Desglosa las ubicaciones geográficas de las órdenes (País, Región, Ciudad) para permitir analítica sobre desempeño territorial.
6. **Dim_Fuente_Datos**: Identificador del origen del registro (rastreabilidad), marcando si fue obtenido mediante *Archivo CSV*, *API Web* o *Base de Datos Relacional*.

### Hecho (Fact Table) Cargado
**Fact_Ventas**:
Es la tabla central (granulada) del modelo estrella. Cada registro en `Fact_Ventas` equivale a una transacción en el sistema (el detalle de un producto dentro de una orden).

**Llaves Foráneas asignadas**:
* `IdProducto` (Conecta con Dim_Producto)
* `IdCliente` (Conecta con Dim_Cliente)
* `IdTiempo` (Conecta con Dim_Tiempo a través de formato numérico yyyyMMdd)
* `IdSucursal` (Conecta con Dim_Sucursal)
* `IdUbicacion` (Conecta con Dim_Ubicacion)
* `IdFuente` (Conecta con Dim_Fuente_Datos)

**Métricas (Medidas) Calculadas**:
* `Cantidad`: Número de unidades vendidas.
* `PrecioUnitario`: Valor unitario del producto al momento de corte.
* `TotalVenta`: Monto absoluto resultante de procesar (Cantidad * PrecioUnitario).

## 4. Flujo de Carga (Load) de Hechos en Memoria
Tras la carga e inserción paralela de las dimensiones, el sistema mapea internamente las identificaciones únicas originarias hacia sus correspondientes "Surrogate Keys" (Llaves Primarias autogeneradas del DW). 
Se crea el objeto `FactVenta` en iteración, asociando las métricas contra las dimensiones consolidadas, y finalmente insertando el bloque entero en un solo comando:
```csharp
await _context.Fact_Ventas.AddRangeAsync(factVentas);
await _context.SaveChangesAsync();
```
