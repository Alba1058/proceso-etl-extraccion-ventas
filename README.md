# Sistema de Análisis de Ventas - Proceso ETL

Proyecto desarrollado para la construcción y carga de un Data Warehouse orientado al análisis de ventas mediante procesos ETL (Extract, Transform and Load).

## Objetivo

Implementar un proceso ETL capaz de integrar información proveniente de múltiples fuentes de datos, transformarla y almacenarla en un Data Warehouse para facilitar el análisis histórico y la generación de reportes.

## Tecnologías Utilizadas

* C#
* .NET
* SQL Server
* ETL
* Data Warehouse
* Arquitectura por Capas

## Arquitectura del Proyecto

La solución fue organizada utilizando arquitectura por capas:

* API: exposición de endpoints y consultas.
* Application: coordinación de la lógica ETL.
* Domain: entidades y modelos de negocio.
* Infrastructure: extracción de datos y manejo de archivos CSV.
* Persistence: acceso y carga del Data Warehouse.
* Worker: ejecución automatizada del proceso ETL.

## Fuentes de Datos

El sistema fue diseñado para trabajar con:

* Archivos CSV.
* API REST.
* Base de datos relacional.

La implementación funcional utiliza datos provenientes de archivos CSV, manteniendo la estructura preparada para soportar otras fuentes.

## Proceso ETL

### Extracción

Lectura de información de:

* Clientes.
* Productos.
* Órdenes.
* Detalles de órdenes.

### Transformación

* Limpieza de datos.
* Validación de identificadores.
* Normalización de formatos.
* Eliminación de inconsistencias.

### Carga

Inserción de información procesada en un modelo dimensional tipo estrella.

## Modelo Analítico

### Dimensiones

* Dim_Producto
* Dim_Cliente
* Dim_Tiempo
* Dim_Sucursal
* Dim_Ubicacion
* Dim_Fuente_Datos

### Tabla de Hechos

* Fact_Ventas

La tabla Fact_Ventas almacena las métricas principales del negocio:

* Cantidad vendida.
* Precio unitario.
* Total de venta.

## Optimización de Carga

Para evitar duplicidad de información:

* Se realiza limpieza previa de dimensiones y hechos.
* Se mantiene el orden correcto de eliminación de registros.
* Se implementa carga por lotes de 5,000 registros para mejorar el rendimiento y reducir el consumo de memoria.

## Resultados

El proyecto permite consolidar información de ventas en una estructura analítica preparada para:

* Consultas históricas.
* Reportes empresariales.
* Análisis de tendencias.
* Apoyo a la toma de decisiones.

## Autor

Alba Marina Then Lugo
