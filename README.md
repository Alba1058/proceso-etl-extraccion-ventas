# SistemaAnalisisVentas

Arquitectura ETL en .NET orientada al proyecto `Sistema de Analisis de Ventas`.

La solucion incluye:

- `Api`: expone endpoints locales para clientes, productos, ordenes y detalle de orden.
- `Worker`: orquesta la extraccion, el staging y la preparacion de datos.
- `Application`: coordina el flujo ETL y la transformacion.
- `Infrastructure`: implementa extractores CSV, API y base de datos, junto con staging y preparacion para carga.

La solucion queda preparada para:

- Extraer datos desde CSV, API y base de datos.
- Guardar datos crudos en `StagingArea/raw`.
- Normalizar y depurar datos de clientes, productos, ordenes y detalle de orden.
- Guardar datasets preparados en `StagingArea/prepared`.
- Servir como base para la siguiente tarea de carga de dimensiones en el Data Warehouse.
