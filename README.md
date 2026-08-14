# FunEvents - Sistema de Reserva de Entradas

Prueba técnica desarrollada en **.NET 8** para implementar un sistema de reserva de entradas para eventos.

La solución permite realizar reservas mediante una API REST y también incluye un cliente de consola que consume dicha API.

El proyecto fue desarrollado aplicando principios de **Clean Architecture**, separación de responsabilidades, persistencia con **Entity Framework Core**, base de datos **PostgreSQL** y ejecución del entorno mediante **Docker Compose**.

---

## Arquitectura

La solución está organizada siguiendo principios de Clean Architecture, separando las reglas de negocio, los casos de uso, la infraestructura y los mecanismos de entrada al sistema.

```text
                    ┌─────────────────────┐
                    │  Cliente Consola    │
                    └──────────┬──────────┘
                               │ HTTP
                               ▼
                    ┌─────────────────────┐
                    │    FunEvents.Api    │
                    │     Minimal API     │
                    └──────────┬──────────┘
                               │
                               ▼
                  ┌─────────────────────────┐
                  │  FunEvents.Application  │
                  │      Casos de uso       │
                  └────────────┬────────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │  FunEvents.Domain   │
                    │ Reglas de negocio   │
                    └─────────────────────┘
                               ▲
                               │
                  ┌────────────┴────────────┐
                  │ FunEvents.Infrastructure│
                  │ EF Core / PostgreSQL    │
                  └────────────┬────────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │    PostgreSQL 16    │
                    └─────────────────────┘
```

### Proyectos de la solución

- **FunEvents.Domain**  
  Contiene las entidades del dominio y las reglas principales del negocio.

- **FunEvents.Application**  
  Contiene los casos de uso, interfaces y lógica de aplicación necesaria para realizar las reservas.

- **FunEvents.Infrastructure**  
  Contiene la implementación de persistencia mediante Entity Framework Core y PostgreSQL.

- **FunEvents.Api**  
  API REST desarrollada con ASP.NET Core Minimal APIs.

- **FunEvents.ConsoleClient**  
  Aplicación de consola que consume la API para realizar reservas.

- **FunEvents.UnitTests**  
  Pruebas unitarias de las reglas del dominio.

- **FunEvents.IntegrationTests**  
  Pruebas de integración sobre el flujo HTTP de reservas.

---

## Tecnologías utilizadas

- .NET 8
- C#
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL 16
- Docker
- Docker Compose
- Swagger / OpenAPI
- xUnit

---

## Estructura del proyecto

```text
FunEvents-Ticketing-System/
│
├── src/
│   ├── FunEvents.Api/
│   ├── FunEvents.Application/
│   ├── FunEvents.Domain/
│   └── FunEvents.Infrastructure/
│
├── clients/
│   └── FunEvents.ConsoleClient/
│
├── tests/
│   ├── FunEvents.UnitTests/
│   └── FunEvents.IntegrationTests/
│
├── docker-compose.yml
├── README.md
└── .gitignore
```

---

## Requisitos previos

Para ejecutar la solución se requiere:

- .NET 8 SDK
- Docker Desktop
- Git

No es necesario instalar PostgreSQL directamente en el equipo, ya que la base de datos puede ejecutarse mediante Docker.

---

# Ejecución del proyecto

## 1. Clonar el repositorio

```bash
git clone <URL-DEL-REPOSITORIO>
```

Ingresar a la carpeta:

```bash
cd FunEvents-Ticketing-System
```

---

## 2. Levantar PostgreSQL

Desde la raíz del repositorio ejecutar:

```bash
docker compose up -d
```

Para verificar el estado:

```bash
docker compose ps
```

El contenedor debe aparecer en estado:

```text
healthy
```

La configuración local de PostgreSQL es:

```text
Host: localhost
Puerto: 5432
Base de datos: funevents
Usuario: postgres
Contraseña: postgres
```

Estas credenciales se utilizan únicamente para el entorno local de desarrollo.

---

## 3. Restaurar dependencias

```bash
dotnet restore
```

---

## 4. Ejecutar la API

```bash
dotnet run --project src/FunEvents.Api
```

Al iniciar la aplicación en ambiente de desarrollo se ejecutan automáticamente las migraciones pendientes de Entity Framework Core.

```csharp
await dbContext.Database.MigrateAsync();
```

También se ejecuta el proceso de carga de datos iniciales.

Por lo tanto, no es necesario crear manualmente las tablas de PostgreSQL.

---

## Base de datos

Las principales tablas utilizadas son:

```text
events
users
bookings
__EFMigrationsHistory
```

### Datos iniciales

El sistema incluye datos básicos para facilitar las pruebas.

Ejemplos:

```text
Eventos:
EVENT-001
EVENT-002

Usuario:
USER-001
```

---

# Swagger

Cuando la API se ejecuta en ambiente de desarrollo, Swagger permite consultar y probar los endpoints disponibles.

La dirección será similar a:

```text
https://localhost:<puerto>/swagger
```

El puerto exacto será mostrado por la aplicación al iniciar.

---

# API de reservas

## Reservar entradas

```http
POST /api/v1/bookings
```

### Ejemplo de solicitud

```json
{
  "eventCode": "EVENT-001",
  "userCode": "USER-001",
  "quantity": 2
}
```

### Ejemplo de respuesta exitosa

```json
{
  "bookingId": "9574d6ad-fbe5-477a-a260-24247eb9893b",
  "eventCode": "EVENT-001",
  "userCode": "USER-001",
  "quantity": 2,
  "remainingTickets": 98,
  "createdAt": "2026-08-12T17:19:24Z"
}
```

La reserva creada se almacena en PostgreSQL y se actualiza la disponibilidad del evento.

---

## Códigos de respuesta

| Código | Descripción |
|---|---|
| `201 Created` | Reserva creada correctamente |
| `400 Bad Request` | Los datos enviados no son válidos |
| `404 Not Found` | El evento o usuario solicitado no existe |
| `409 Conflict` | No existe disponibilidad suficiente o se presentó un conflicto de concurrencia |
| `500 Internal Server Error` | Se presentó un error inesperado |

---

# Cliente de consola

La solución incluye un cliente de consola independiente que consume la API REST.

Para utilizarlo, primero debe estar ejecutándose `FunEvents.Api`.

Luego se puede iniciar:

```bash
dotnet run --project clients/FunEvents.ConsoleClient
```

El cliente solicita:

```text
Código del evento
Código del usuario
Cantidad de entradas
```

Ejemplo:

```text
=================================
        FunEvents Client
=================================

Event code: EVENT-001
User code: USER-001
Number of tickets: 1

Reservation successful!

Booking ID:        7d1eea50-9824-41f7-9ecb-64361fd4b4da
Event:             EVENT-001
User:              USER-001
Tickets:           1
Remaining tickets: 87
```

El cliente de consola no accede directamente a PostgreSQL. Toda la comunicación se realiza mediante la API.

---

# Manejo de errores

La API implementa un manejo centralizado de excepciones.

Esto permite convertir los errores del dominio y de la aplicación en respuestas HTTP adecuadas sin exponer detalles internos del sistema.

Por ejemplo:

```text
Solicitud inválida
        ↓
400 Bad Request

Evento inexistente
        ↓
404 Not Found

Entradas insuficientes
        ↓
409 Conflict

Error inesperado
        ↓
500 Internal Server Error
```

---

# Manejo de concurrencia

Uno de los puntos importantes de un sistema de venta de entradas es evitar inconsistencias cuando varios usuarios intentan reservar entradas al mismo tiempo.

La solución implementa un mecanismo de **concurrencia optimista** utilizando Entity Framework Core y PostgreSQL.

Cuando dos operaciones intentan modificar simultáneamente la disponibilidad de un mismo evento, se valida que ninguna operación sobrescriba silenciosamente información que ya fue modificada por otra transacción.

En caso de detectar un conflicto, la API puede responder:

```text
409 Conflict
```

Esto ayuda a proteger la disponibilidad de las entradas ante solicitudes concurrentes.

---

# Pruebas automatizadas

La solución incluye pruebas unitarias y pruebas de integración.

Para ejecutar todas las pruebas:

```bash
dotnet test
```

Actualmente se tienen:

```text
8 pruebas
8 correctas
0 errores
```

## Pruebas unitarias

Las pruebas unitarias validan reglas del dominio como:

- Normalización del código del evento.
- Reserva correcta de entradas.
- Rechazo de cantidades iguales o menores a cero.
- Rechazo de reservas superiores a la disponibilidad.

## Pruebas de integración

Las pruebas de integración validan el comportamiento de la API:

- Reserva válida → `201 Created`
- Evento inexistente → `404 Not Found`
- Cantidad inválida → `400 Bad Request`
- Entradas insuficientes → `409 Conflict`

Las pruebas de integración utilizan una base de datos aislada para no modificar la información almacenada en PostgreSQL durante el desarrollo.

---

# Decisiones de diseño

## Clean Architecture

Se utilizó una arquitectura por capas para mantener separadas las responsabilidades del sistema.

La lógica principal del negocio se encuentra en el dominio y no depende directamente de ASP.NET Core, PostgreSQL o Entity Framework Core.

Esto facilita:

- mantenimiento;
- pruebas automatizadas;
- evolución del sistema;
- sustitución de componentes de infraestructura.

---

## Minimal APIs

Para el prototipo se utilizaron Minimal APIs debido a que el alcance HTTP requerido es pequeño.

Esto permite mantener una implementación sencilla sin agregar complejidad innecesaria mediante controladores tradicionales.

---

## PostgreSQL

Se seleccionó PostgreSQL como base de datos relacional debido a su soporte transaccional y sus mecanismos de concurrencia, características importantes para manejar disponibilidad de entradas.

---

## Docker

PostgreSQL se ejecuta mediante Docker Compose para proporcionar un ambiente reproducible.

Esto permite que otra persona pueda ejecutar:

```bash
docker compose up -d
```

sin necesidad de instalar o configurar PostgreSQL manualmente.

---

# Propuesta de arquitectura para un sistema productivo

Para un escenario real con aplicaciones web, móviles y puntos físicos de venta, se propone una arquitectura preparada para evolucionar según la demanda.

```text
 Aplicación Web     Aplicación Móvil     Punto de Venta
       │                   │                   │
       └───────────────────┼───────────────────┘
                           │
                           ▼
                      API Gateway
                           │
               ┌───────────┼───────────┐
               │           │           │
               ▼           ▼           ▼
            Eventos     Reservas     Usuarios
               │           │           │
               └───────────┼───────────┘
                           │
                           ▼
                       PostgreSQL
                           │
                           ▼
                    Broker de mensajes
                           │
                ┌──────────┼──────────┐
                ▼          ▼          ▼
              Pagos   Notificaciones Analítica
```

Para eventos de alta demanda podrían incorporarse mecanismos adicionales como:

- escalamiento horizontal;
- balanceadores de carga;
- caché distribuida;
- procesamiento mediante colas;
- idempotencia de solicitudes;
- rate limiting;
- observabilidad y trazabilidad distribuida;
- réplicas de lectura;
- particionamiento;
- patrón Transactional Outbox.

El prototipo actual se mantiene intencionalmente como una solución modular para evitar agregar complejidad de sistemas distribuidos que no es necesaria para el alcance de la prueba.

---

# Detener el ambiente

Para detener PostgreSQL:

```bash
docker compose down
```

Para detenerlo y eliminar también el volumen de datos:

```bash
docker compose down -v
```

---

# Autor

Ruben Hernandez