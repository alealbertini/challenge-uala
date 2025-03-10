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

Para verificar que la imágen de Docker este correctamente levantada, ejecutar el siguiente comando:
```
docker container ls
```
Luego, ejecutar el proyecto en Visual Studio. El archivo TwitterUala.sln para abrir la solución, se encuentra adentro de la carpeta TwitterUala. Se debe compilar y levantar con el profile Container (dockerfile) y se pueden consumir los endpoints a través del swagger que aparece al levantar la aplicación.

Si desea ver la base de datos con una interfaz visual, se puede conectar generando una conexión de PostgreSQL con los siguientes datos:
```
host: localhost
database: postgres
usuario: postgres
contraseña: uala123
puerto: 5432
```

## Ejecutar los tests
Para ejecutar todos los tests, hay que hacer click derecho en el proyecto TwiterUalaTest y seleccionar la opción Run Tests.
