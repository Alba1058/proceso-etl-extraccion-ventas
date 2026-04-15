# Proceso de Carga (Load) de Dimensiones - Sistema Analítico de Ventas

**Sustentante:** Alba Marina Then Lugo (2024-2278)  
**Asignatura:** Electiva I (Big Data)  
**Docente:** Francis Ramirez  
**República Dominicana**

---

## 1. Descripción General
El presente documento tiene como objetivo presentar y detallar la fase de **Carga (Load)** de datos hacia el **DataWarehouse** (`VentasAnalitica`) que forma parte del proceso ETL desarrollado para el Sistema de Análisis de Ventas. 

Se empleó íntegramente la capacidad Code-First (o auto-generada) de Entity Framework Core mediante `Database.EnsureCreated()`, con el objetivo de garantizar una base de datos escalable, que responda dinámicamente al código y exente de esquemas SQL manuales engorrosos. Luego de la extracción, este código de Worker Service se conecta y escribe masivamente (bulks) hacia las tablas dimensionales respetando estrictamente las dependencias de Base de Datos.

## 2. Arquitectura del Proceso Funcional de Carga
Para orquestar eficientemente la carga, se modificaron y crearon los siguientes componentes tomando de referencia el proyecto modelo provisto por el docente:

*   **ETLService:** Actúa como orquestador general, removiendo lógicas estáticas y logrando un flujo de ciclo completo: Extracción de los archivos `.csv`, normalización en memoria (Transformación), y envío directo y validado hacia el repositorio del DWH.
*   **VentasAnaliticaContext (EF Core Context):** Inicializa la conexión y establece el modelo y convenciones (Entity Framework) que se atarán milimétricamente con el *schema* de SQL Server local provisto por el SQL Script.
*   **DwhRepository (Método `LoadAnalyticsDataAsync`):** Corazón de la implementación de esta fase. El método limpia lógicamente las dimensiones y tablas de hechos en una cadencia controlada (evitando colisiones), mapea los datos de los objetos en la RAM del servicio hacia entidades funcionales y asegura que los datos insertados (ej. `DimCliente`, `DimProducto`, `DimTiempo`) existan antes de armar centralmente el `FactVenta` y guardarlo hacia la base de datos de destino.

## 3. Diccionario de Tablas Cargadas

A continuación, la descripción de cómo el proceso ETL Carga cada tabla, basada en el Modelo Estrella:

### `Dim_Cliente`
Dimensión encargada de los registros limpios y sin duplicados de nuestros compradores.
*   **Mapeo Funcional:** Los datos provienen del originador (Orders) para no crear falsos positivos numéricos.
*   **IdCliente (PK):** Generado autoincremental en la BD.
*   **ClienteOrigenId:** Llave de origen única.
*   **NombreCliente:** Concatenación de *First Name* y *Last Name* normalizados.

### `Dim_Producto`
Almacena todos los productos físicos involucrados en los CSVs transaccionales.
*   **Mapeo Funcional:** Los registros vienen con categoría base predeterminada pero en la carga el proceso normaliza los precios a `decimal(18,2)`.
*   **ProductoOrigenId (UK):** ID único del producto original.
*   **Categoría y EstadoProducto:** Generados o extraídos durante la carga.

### `Dim_Tiempo`
Considerada la dimensión de corte analítico más crítica.
*   **Mapeo Funcional:** Es _dinámica_. Nuestro aplicativo C# itera sobre las fechas extraídas (únicas) y genera algoritmos (en `DwhRepository`) para autocalcular el Trimestre, Mes, Día, y la Natural Key (`YYYYMMDD`).
*   **IdTiempo (PK):** Numérico, no Identity.

### `Dim_Sucursal`
Almacena tiendas y entes distribuidores.
*   **Mapeo Funcional:** Al carecer de datos explícitos de sucursales en los CSV, el proceso de transformación infiere una Sucursal Principal por defecto con la finalidad de validar integridad referencial estricta hacia el _DataWarehouse_.

### `Dim_Ubicacion`
Geografía unificada.
*   **Mapeo Funcional:** Deduce las regiones físicas de cada fact/movimiento leyendo el país y la ciudad desde `PreparedSalesData`.

### `Dim_Fuente_Datos`
Metadatos del linaje de ETL para separar la analítica por plataformas.
*   **Mapeo Funcional:** Inserta el origen (CSV).

### `Fact_Ventas` (Hechos)
El punto de entrada del Data Warehouse.
*   **Mapeo Funcional:** Contiene todos los métricas clave y las foráneas hacia las dimensiones creadas anteriormente, iterando con un validador de diccionarios en memoria en C# para garantizar inserción inmediata rápida en lugar de buscar por EF Core recursivamente.

---

## 4. Evidencia Técnica
El código fuente refactorizado y completado en C# (Worker Service) que realiza esta gestión se encuentra alojado en GitHub.

**URL de Revisión del Proyecto en GitHub:**  
[https://github.com/Alba1058/proceso-etl-extraccion-ventas](https://github.com/Alba1058/proceso-etl-extraccion-ventas)
