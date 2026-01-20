# Sistema P2P con Backend .NET

Sistema peer-to-peer (P2P) de alto rendimiento que implementa transferencia segura de archivos entre dispositivos en la red local, con un backend robusto en C# (.NET 10+ y .NET 8) y un componente optimizado de compresión en Rust.

## Tabla de Contenidos

- [Descripción General](#descripción-general)
- [Características](#características)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Requisitos Previos](#requisitos-previos)
- [Instalación](#instalación)
- [Uso Básico](#uso-básico)
- [Configuración](#configuración)
- [API REST](#api-rest)
- [Contribución](#contribución)

---

##  Descripción General

### ¿Qué hace este proyecto?

P2P es una **solución de transferencia de archivos peer-to-peer** que permite compartir archivos entre dispositivos en la misma red local de forma segura y eficiente. - Se estara programando para se pueda usar en redes WAN

### ¿Para qué sirve?

 **Compartir archivos entre dispositivos** sin servidores centralizados  
 **Escaneo automático de dispositivos** en la red local  
 **API REST moderna** para integración con otras aplicaciones  
 **Interfaz web responsive** construida con Astro  

### Ventajas

-  **Rápido**: Transferencia directa peer-to-peer sin intermediarios
-  **Seguro**: Comunicación local sin exposición a internet
-  **Multiplataforma**: Soporta Windows, Linux y macOS
-  **Moderno**: Backend .NET 10 y frontend Astro

---

##  Características

-  Descubrimiento automático de dispositivos en LAN (192.168.x.x)
-  Transferencia de archivos P2P sin servidor central
-  Detección de sistema operativo de dispositivos remotos
-  Puerto configurable (por defecto 8080)
-  API REST completa con documentación Swagger
-  Compresión de archivos integrada (Rust)
-  Interfaz web moderna y responsiva
-  Gestión de dispositivos conectados

---

##  Estructura del Proyecto

```
P2P/
├── csharp_backend/              # Backend en C# .NET 10/8
│   ├── p2p.api/                 # Proyecto API REST
│   │   ├── Controllers/
│   │   │   └── P2PController.cs    # Endpoints REST
│   │   ├── appsettings.json
│   │   └── Program.cs
│   ├── p2p.Models/              # Modelos de datos
│   │   ├── P2PContext.cs           # DbContext (EF Core)
│   │   ├── P2PItems.cs             # Modelo de dispositivo
│   │   └── P2PDtoDevice.cs         # DTO de respuesta
│   ├── p2p.services/            # Servicios de negocio
│   │   ├── FileTransfer.cs         # Transferencia de archivos
│   │   ├── LanIpScanner.cs         # Escaneo de red
│   │   ├── UniversalDeviceScanner.cs
│   │   └── Program.cs
│   └── p2p.sln
├── p2pWeb/                      # Frontend en Astro
│   ├── src/
│   │   ├── components/
│   │   ├── layouts/
│   │   ├── pages/
│   │   └── styles/
│   └── package.json
├── rust_compressor/             # Compresión en Rust
│   ├── Cargo.toml
│   └── src/
│       └── main.rs
└── readme.md
```

---

##  Requisitos Previos

Antes de instalar, asegúrate de tener:

| Requisito | Versión | Descripción |
|-----------|---------|-------------|
| **.NET** | 10.0+    | Runtime de ejecución C# |
| **Rust** | 1.70+    | Para compilar compresión |
| **Node.js/pnpm** | 18+ | Para frontend Astro |
| **Visual Studio** | 2022+ | Recomendado para C# |
| **Git** | Cualquiera | Para clonar repositorio |

---

##  Instalación

###  Clonar el Repositorio

```bash
git clone https://github.com/tu-usuario/p2p.git
cd p2p
```

###  Backend C# (.NET)

#### Instalación de .NET

**Windows:**
```bash
# Descargar e instalar desde:
# https://dotnet.microsoft.com/download
```

**Linux/macOS:**
```bash
# Ubuntu/Debian
sudo apt-get install dotnet-sdk-10.0

# macOS (Homebrew)
brew install dotnet
```

#### Compilación del Backend

```bash
# Navegar a backend
cd csharp_backend

# Restaurar dependencias
dotnet restore

# Compilar proyecto
dotnet build

# Ejecutar migraciones de base de datos
cd p2p.api
dotnet ef database update
```

### Componente Rust (Compresión)

#### Instalación de Rust

```bash
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source $HOME/.cargo/env
```

#### Compilación de Rust

```bash
cd rust_compressor
cargo build --release
```

### Frontend Astro (Opcional)

```bash
cd p2pWeb

# Con pnpm
pnpm install
pnpm run dev

# O con npm
npm install
npm run dev
```

---

##  Uso Básico

### Iniciar el Servidor Backend

```bash
cd csharp_backend/p2p.api
dotnet run
```

**Salida esperada:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

### Iniciar los Servicios P2P

```bash
cd csharp_backend/p2p.services
dotnet run
```

**Salida esperada:**
```
Dirección de ip de la maquina local: 192.168.1.100
IP: 192.168.1.101 | Nombre: DESKTOP-PC2 | SO: Windows
IP: 192.168.1.102 | Nombre: laptop-linux | SO: Linux
```

### Acceder a la API

#### 1. Obtener todos los dispositivos registrados

```bash
curl -X GET "http://localhost:5000/api/p2p"
```

**Respuesta:**
```json
[
  {
    "id": "20250120120530123-a1b2c3d4-e5f6",
    "deviceName": "PC-Oficina",
    "deviceType": "Desktop",
    "deviceIp": "192.168.1.100"
  }
]
```

#### 2. Escanear dispositivos en la red local

```bash
curl -X GET "http://localhost:5000/api/p2p/devices"
```

**Respuesta:**
```json
[
  {
    "ip": "192.168.-.---",
    "name": "DESKTOP-WORK",
    "osType": "Windows"
  },
  {
    "ip": "192.168.-.---",
    "name": "ubuntu-server",
    "osType": "Linux"
  }
]
```

#### 3. Registrar un nuevo dispositivo

```bash
curl -X POST "http://localhost:5000/api/p2p" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "Mi-Laptop",
    "deviceType": "Laptop",
    "deviceIp": "192.168.1.105"
  }'
```

**Respuesta (201 Created):**
```json
{
  "deviceName": "Mi-Laptop",
  "deviceType": "Laptop",
  "deviceIp": "192.168.1.105"
}
```

#### 4. Obtener un dispositivo por ID

```bash
curl -X GET "http://localhost:5000/api/p2p/20250120120530123-a1b2c3d4-e5f6"
```

#### 5. Actualizar un dispositivo

```bash
curl -X PUT "http://localhost:5000/api/p2p/20250120120530123-a1b2c3d4-e5f6" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "20250120120530123-a1b2c3d4-e5f6",
    "deviceName": "Mi-Laptop-Actualizado",
    "deviceType": "Laptop",
    "deviceIp": "192.168.1.105"
  }'
```

#### 6. Eliminar un dispositivo

```bash
curl -X DELETE "http://localhost:5000/api/p2p/20250120120530123-a1b2c3d4-e5f6"
```

### Transferencia de Archivos (Ejemplo)

```csharp
// En tu código C#
var fileTransfer = new FileTransfer();
fileTransfer.Port = 8080;
fileTransfer.LocalIp = IPAddress.Any;

// Recibir archivos en puerto 8080
// await fileTransfer.ReceiveFileAsync("ruta/destino");

// Enviar archivo a otro peer
// await fileTransfer.SendFileAsync("192.168.1.101", "ruta/archivo.txt");
```

---

##  Configuración

### Archivo appsettings.json (Backend)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DB.db"
  }
}
```

### Variables de Entorno

```bash
# Para desarrollo
ASPNETCORE_ENVIRONMENT=Development

# Puerto del servidor
ASPNETCORE_URLS=http://localhost:5000;https://localhost:5001

# Puerto P2P (por defecto 8080)
P2P_PORT=8080
```

---

##  API REST

### Endpoints Disponibles

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/p2p` | Obtener todos los dispositivos |
| `GET` | `/api/p2p/{id}` | Obtener dispositivo por ID |
| `GET` | `/api/p2p/devices` | Escanear dispositivos en LAN |
| `POST` | `/api/p2p` | Crear nuevo dispositivo |
| `PUT` | `/api/p2p/{id}` | Actualizar dispositivo |
| `DELETE` | `/api/p2p/{id}` | Eliminar dispositivo |

---

##  Licencia

Este proyecto está bajo licencia MIT. Ver archivo [LICENCE](./LICENCE) para más detalles.

---

##  Contacto

Para preguntas o soporte, abre un issue en el repositorio.

**Última actualización:** Enero 2026

- API REST para gestión de transferencias

## Tecnologías Utilizadas

- Backend: C# / .NET 6
- Frontend: Astro
- Base de Datos: SQLLite

## Contribución

1. Fork el proyecto
2. Crea tu rama de características (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request
