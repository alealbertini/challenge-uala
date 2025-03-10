# Challenge Ualá

## Inicio

Este proyecto es una app simil Twitter para el challenge de Uala, donde se puede crear usuarios, seguir a otros usuarios, postear tweets y ver el timeline de los usuarios. 

## Requisitos para poder ejecutar el proyecto 

```
Docker Desktop para Windows
Visual Studio 
Docker
```

## Ejecutar el proyecto 

Abrir una terminal de Windows Powershell y ejecutar los siguientes comandos:

```
docker pull postgres
```
Este comando descarga la última versión de la imágen de docker de PostgreSQL.

```
docker run -d --name postgresDB -p 5432:5432 -e POSTGRES_PASSWORD=uala123 postgres
```
Este comando levanta la imágen de docker donde "postgresDB" es el nombre del container y "postgres" es el nombre de la imágen de Docker.

Luego, ejecutar el proyecto en Visual Studio. Se puede compilar y levantar con IIS Server o con Docker Container y consumir los endpoints a través del swagger que aparece al levantar la aplicación.

Ayuda:
Al levantar la aplicación, se ejecutarán los scripts de Migration que crearan el modelo de base de datos en el container de postgres. Si la base de datos no se creó al levantar el proyecto, ejecutar el siguiente comando en el Package Manager Console:

```
Add-Migration InitialCreate
```

## Ejecutar los tests
Para ejecutar todos los tests, hay que hacer click derecho en el proyecto TwiterUalaTest y seleccionar la opción Run Tests.
