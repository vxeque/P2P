#  P2P - Sistema de Transferencia de Archivos Peer-to-Peer

Sistema peer-to-peer (P2P) de alto rendimiento que implementa transferencia segura de archivos entre dispositivos en la red local, con un backend robusto en C# (.NET 10+ y .NET 8) y un componente optimizado de compresión en Rust.

**Estado:** En desarrollo activo  
**Versión:** 4.0.0  
**Licencia:** MIT

---

##  Tabla de Contenidos

- [Descripción General](#-descripción-general)
- [Arquitectura del Sistema](#-arquitectura-del-sistema)
- [Flujo de Datos P2P](#-flujo-de-datos-p2p)
- [Casos de Uso Reales](#-casos-de-uso-reales)
- [Características](#-características)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación](#-instalación)
- [Guía de Uso Paso a Paso](#-guía-de-uso-paso-a-paso)
- [Configuración](#-configuración)
- [API REST](#-api-rest)
- [Roadmap](#-roadmap)
- [Contribución](#-contribución)

---

##  Descripción General

### ¿Qué es P2P?

P2PV4 es una **solución de transferencia de archivos peer-to-peer** que permite compartir archivos entre dispositivos en la misma red local (LAN) de forma segura, rápida y eficiente. El sistema está diseñado para evolucionar y soportar redes WAN.

### ¿Para qué sirve?

 **Compartir archivos sin servidores centralizados** - Comunicación directa dispositivo a dispositivo  
 **Descubrimiento automático de peers** - Escaneo inteligente de la red local  
 **API REST moderna** - Integración fácil con otras aplicaciones  
 **Transferencias optimizadas** - Compresión integrada en Rust para máximo rendimiento  
 **Multiplataforma** - Windows, Linux y macOS  

### Ventajas Clave

| Ventaja | Descripción |
|---------|-------------|
|  **Rendimiento** | Transferencia directa P2P sin intermediarios |
|  **Seguridad** | Comunicación local, sin exposición a internet |
|  **Escalabilidad** | Preparado para redes WAN en futuras versiones |
|  **Simplicidad** | API intuitiva y fácil de usar |
|  **Multiplataforma** | Funciona en Windows, Linux y macOS |

---

##  Arquitectura del Sistema

### Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────────────────────┐
│                        RED LOCAL (LAN 192.168.x.x)                   │
└─────────────────────────────────────────────────────────────────────┘

┌──────────────────────┐         ┌──────────────────────┐
│   PEER A             │         │   PEER B             │
│  192.168.1.100       │         │  192.168.1.101       │
├──────────────────────┤         ├──────────────────────┤
│  Frontend Astro      │         │  Frontend Astro      │
│  (Puerto 3000)       │         │  (Puerto 3000)       │
└──────────────────────┘         └──────────────────────┘
         │                                 │
         │                                 │
┌────────▼─────────────┐         ┌────────▼─────────────┐
│  Backend .NET API    │         │  Backend .NET API    │
│  (Puerto 5000-5001)  │         │  (Puerto 5000-5001)  │
├──────────────────────┤         ├──────────────────────┤
│ • P2PController      │         │ • P2PController      │
│ • FileTransfer       │         │ • FileTransfer       │
│ • LanIpScanner       │         │ • LanIpScanner       │
└──────────────────────┘         └──────────────────────┘
         │                                 │
         │◄──────────────────────────────►│
         │      Conexión P2P Directa      │
         │  (Puerto 8080 - Transferencia) │
         │                                │
         └────────────┬───────────────────┘
                
```

### Componentes Clave

#### 1. **Frontend Astro** (Interfaz Web)
- Interface responsiva y moderna
- Comunicación con API REST
- Escaneo visual de dispositivos

#### 2. **Backend .NET API** (Gestión)
- API REST completa (CRUD)
- Escaneo de dispositivos en LAN
- Gestión de metadatos de transferencia

#### 3. **Servicio P2P** (Transferencia)
- Descubrimiento automático de peers
- Transferencia directa de archivos
- Puerto configurable (por defecto 8080)

#### 4. **Rust Compressor** (Optimización)
- Compresión de archivos
- Máximo rendimiento
- Interoperabilidad con .NET

---

##  Flujo de Datos P2P

### Proceso Completo de Transferencia

```
┌─────────────────────────────────────────────────────────────────────┐
│ PASO 1: DESCUBRIMIENTO DE DISPOSITIVOS                              │
├─────────────────────────────────────────────────────────────────────┤
│                  d                                                     │
│  Peer A                        Peer B                                 │
│  ┌──────────────┐             ┌──────────────┐                      │
│  │ LanIpScanner │────────────►│ Escucha      │                      │
│  │ 192.168.0.x  │             │ broadcast    │                      │
│  └──────────────┘             └──────────────┘                      │
│       │                              │                               │
│       └──────────────┬───────────────┘                              │
│                      │                                               │
│            ✓ Dispositivos detectados                                │
│            ✓ Almacenados en BD SQLite                               │
│            ✓ Disponibles en API                                     │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│ PASO 2: REGISTRO DE DISPOSITIVO EN BD                                │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  POST /api/p2p                                                       │
│  {                                                                    │
│    "deviceName": "PC-Oficina",      → Guardado en P2PContext        │
│    "deviceType": "Desktop",         → Modelo: P2PItems              │
│    "deviceIp": "192.168.1.100"      → DB: DB.db (SQLite)            │
│  }                                                                    │
│                                                                       │
│  ✓ ID autogenerado: 20250120120530123-a1b2c3d4-e5f6                │
│  ✓ Dispositivo listo para recibir transferencias                    │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│ PASO 3: INICIO DE TRANSFERENCIA (Peer A → Peer B)                   │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  Peer A (Remitente)            Peer B (Receptor)                     │
│  ┌──────────────────┐          ┌──────────────────┐                │
│  │ Archivo a enviar │          │ En espera (8080) │                │
│  │ documento.pdf    │          │                  │                │
│  └────────┬─────────┘          └──────────────────┘                │
│           │                              ▲                           │
│           ├─────────────────────────────┤                           │
│           │ TCP Socket Puerto 8080      │                           │
│           │ Conexión P2P Directa        │                           │
│           ▼                              │                           │
│  ┌──────────────────┐                   │                           │
│  │ Rust Compressor  │                   │                           │
│  │ Comprime archivo │                   │                           │
│  └────────┬─────────┘                   │                           │
│           │                              │                           │
│  Archivo comprimido (50% - 80%)         │                           │
│  document.zip (50KB → 10KB)             │                           │
│           │                              │                           │
│           └─────────────────────────────►│                           │
│              Transmisión                 ▼                           │
│                              ┌──────────────────┐                   │
│                              │ Descomprime      │                   │
│                              │ Verifica         │                   │
│                              │ Guarda en disco  │                   │
│                              └──────────────────┘                   │
│                                                                       │
│  ✓ Transferencia completada                                          │
│  ✓ Log en BD SQLite                                                 │
│  ✓ Notificación al Peer A                                           │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│ PASO 4: CONFIRMACIÓN Y FINALIZACIÓN                                  │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  Peer B responde al Peer A:                                          │
│  ✓ Hash del archivo (validación)                                    │
│  ✓ Timestamp de recepción                                           │
│  ✓ Confirmación de integridad                                       │
│                                                                       │
│  PUT /api/p2p/{id} → Actualizar estado en BD                        │
│  {                                                                    │
│    "status": "completed",                                            │
│    "receivedAt": "2025-01-20T12:05:30Z",                            │
│    "fileHash": "abc123def456..."                                     │
│  }                                                                    │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘
```

### Secuencia de Estados

```
Descubrimiento → Registro → Solicitud → Transferencia → Validación → Completado
     ↓              ↓           ↓             ↓               ↓           ↓
  Escaneo       API POST    Negociación   Transmisión   Hash Check   Confirmación
  LAN (.x)      /api/p2p    Conexión TCP  Compress.     BD Update    Notificación
```

---

##  Casos de Uso Reales

### Caso 1: Oficina - Compartir Documentos entre Departamentos

**Escenario:**
- Departamento de Diseño (PC1: 192.168.1.10)
- Departamento de Marketing (PC2: 192.168.1.20)
- Archivos: 200MB de recursos gráficos

**Ventajas P2P:**
- Transferencia: ~2 minutos sin compresión, ~30 segundos con Rust
- No usa ancho de banda del servidor central
- Sin exposición a internet
- Privacidad de datos garantizada

**Flujo:**
```
PC1 (Diseño) → App P2P → Escanea LAN → Detecta PC2 (Marketing)
    ↓
   Selecciona archivo (200MB)
    ↓
   Envía directamente a PC2
    ↓
   Rust comprime automaticamente (60% reducción)
    ↓
   Transferencia completada en 30 segundos
```

### Caso 2: Hogar - Sincronización de Multimedia

**Escenario:**
- Computadora principal (Desktop)
- Laptop
- Smart TV

**Requisitos:**
- Compartir fotos y videos de vacaciones
- Sincronización automática

**Ventajas:**
- No necesita almacenamiento en la nube
- Velocidad máxima (Gigabit LAN: 125 MB/s)
- Datos siempre bajo control local

### Caso 3: Eventos - Distribución en Vivo

**Escenario:**
- Conferencia con 50 participantes
- Necesidad de compartir presentaciones y recursos

**Ventajas P2P:**
- Distribuye carga entre peers
- Escalabilidad sin inversión en servidores
- Bajo costo de infraestructura

---

##  Características

-  **Descubrimiento Automático**: Escanea y detecta dispositivos en LAN (192.168.x.x)
-  **Transferencia P2P**: Conexión directa dispositivo-a-dispositivo sin intermediarios
-  **Detección de SO**: Identifica automáticamente Windows, Linux, macOS
-  **API REST Completa**: CRUD completo con validación de datos
-  **Compresión Integrada**: Optimización de archivos con Rust (50-80% reducción)
-  **Puerto Configurable**: Por defecto 8080, personalizable
-  **Base de Datos Local**: SQLite para persistencia de datos
-  **Interfaz Web Moderna**: Astro para experiencia responsiva
-  **Documentación XML**: Código autodocumentado
-  **Futuro: Soporte WAN**: Evolución para redes amplias

---

##  Estructura del Proyecto

```
P2P/
├──  readme.md                          # Este archivo
├──  LICENCE                            # Licencia MIT
├──  package.json                       # Dependencias monorepo
│
├── 🔧 csharp_backend/                   # Backend C# .NET 10/8
│   ├── p2p.sln                          # Solución Visual Studio
│   │
│   ├──  p2p.api/                      # Proyecto API REST
│   │   ├── Controllers/
│   │   │   └── P2PController.cs        # 6 endpoints REST documentados
│   │   ├── Properties/
│   │   │   └── launchSettings.json     # Configuración de puertos
│   │   ├── appsettings.json            # Configuración app
│   │   ├── appsettings.Development.json
│   │   ├── Program.cs                   # Configuración inicio (XML docs)
│   │   ├── p2p.api.csproj
│   │   └── Migrations/                  # Entity Framework
│   │       └── 20250415171457_InitialCreate.cs
│   │
│   ├──  p2p.Models/                   # Modelos de datos
│   │   ├── P2PContext.cs               # DbContext EF Core (XML docs)
│   │   ├── P2PItems.cs                 # Modelo & DTO (XML docs)
│   │   ├── P2PDtoDevice.cs             # DTO respuesta (XML docs)
│   │   └── p2p.models.csproj
│   │
│   ├──  p2p.services/                # Servicios de negocio
│   │   ├── FileTransfer.cs             # Transferencia archivos
│   │   ├── LanIpScanner.cs             # Escaneo de red local
│   │   ├── UniversalDeviceScanner.cs   # Detector de dispositivos
│   │   ├── IP2PService.cs              # Interfaz servicios
│   │   ├── Lan.cs                      # Utilidades LAN
│   │   ├── Program.cs                   # Entry point (XML docs)
│   │   └── p2p.services.csproj
│   │
│   └──  bin/                         # Compilación .NET
│       └── Debug/
│           ├── net10.0/                # .NET 10
│           └── net8.0/                 # .NET 8
│
├──  p2pWeb/                           # Frontend Astro
│   ├── src/
│   │   ├──  pages/
│   │   │   ├── index.astro             # Página inicio
│   │   │   └── devices.astro           # Lista dispositivos
│   │   ├──  components/
│   │   │   ├── NavBar.astro            # Navegación
│   │   │   ├── Getfile.astro           # Transferencia
│   │   │   └── main.astro              # Componente principal
│   │   ├──  layouts/
│   │   │   └── Layout.astro            # Layout principal
│   │   ├──  locales/
│   │   │   ├── en/common.json          # Inglés
│   │   │   └── es/commong.json         # Español
│   │   └──  styles/
│   │       └── global.css              # Estilos globales
│   ├── astro.config.mjs                # Config Astro
│   ├── package.json
│   ├── tsconfig.json
│   └──  public/                      # Archivos estáticos
│
├──  rust_compressor/                  # Compresión Rust
│   ├── Cargo.toml                      # Dependencias Rust
│   └── src/
│       └── main.rs                     # Lógica compresión
│
└──  Diagrama general
```

---

##  Requisitos Previos

| Requisito | Versión Mínima | Uso |
|-----------|-----------------|-----|
| **.NET** | 10.0 o 8.0 | Backend API + Servicios |
| **Rust** | 1.70+ | Compresión de archivos |
| **Node.js** | 18+ | Frontend Astro |
| **pnpm** | 7.0+ | Gestor paquetes (recomendado) |
| **SQL** | - | SQLite (incluido) |
| **Git** | - | Control versiones |
| **Visual Studio** | 2022+ | IDE recomendado (C#) |
| **VS Code** | - | IDE alternativa |

---

##  Instalación

###  Clonar Repositorio

```bash
git clone https://github.com/tu-usuario/p2pv4.git
cd p2pv4
```

###  Instalar .NET

**Windows** (Instalador):
```bash
# Descargar desde https://dotnet.microsoft.com/download
# Y ejecutar el instalador
```

**Linux (Ubuntu/Debian)**:
```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0
dotnet --version
```

**macOS (Homebrew)**:
```bash
brew install dotnet
dotnet --version
```

###  Compilar Backend

```bash
cd csharp_backend

# Restaurar dependencias
dotnet restore

# Compilar solución completa
dotnet build

# Ejecutar migraciones de base de datos
cd p2p.api
dotnet ef database update
cd ..
```

###  Instalar Rust (Opcional, para compresión nativa)

```bash
# Linux/macOS
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source $HOME/.cargo/env

# Windows PowerShell
irm https://sh.rustup.rs -outfile rustup-init.exe
.\rustup-init.exe

# Compilar
cd ../rust_compressor
cargo build --release
```

###  Frontend Astro (Opcional)

```bash
cd p2pWeb

# Instalar dependencias
pnpm install  # o npm install

# Compilar
pnpm run build

# Servidor desarrollo
pnpm run dev
```

### Verificar Instalación

```bash
# Verificar .NET
dotnet --version
# Salida esperada: 10.0.0 (o 8.0.x)

# Verificar Rust (si se instaló)
rustc --version
# Salida esperada: rustc 1.7x.x

# Verificar Node (si se usará frontend)
node --version
# Salida esperada: v18.x.x o superior
```

---

## Guía de Uso Paso a Paso

### Escenario: Transferir archivo de PC1 a PC2 en la misma LAN

#### **PASO 1: Iniciar Backend en PC1**

```bash
# Terminal 1 - PC1 (192.168.1.100)
cd csharp_backend/p2p.api
dotnet run
```

**Salida esperada:**
```
Building...
info: Microsoft.Hosting.Lifetime[13]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to quit.
```

#### **PASO 2: Iniciar Servicios P2P en PC1**

```bash
# Terminal 2 - PC1 (misma máquina)
cd csharp_backend/p2p.services
dotnet run
```

**Salida esperada:**
```
Dirección de ip de la maquina local: 192.168.1.100
IP: 192.168.1.100 | Nombre: DESKTOP-PC1 | SO: Windows
IP: 192.168.1.101 | Nombre: LAPTOP-PC2 | SO: Windows
IP: 192.168.1.102 | Nombre: UBUNTU-SERVER | SO: Linux
```

#### **PASO 3: Registrar Dispositivo PC1**

```bash
# Terminal 3 - Cualquier máquina
curl -X POST "http://localhost:5000/api/p2p" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "PC1-Oficina",
    "deviceType": "Desktop",
    "deviceIp": "192.168.1.100"
  }'
```

**Respuesta (201 Created):**
```json
{
  "deviceName": "PC1-Oficina",
  "deviceType": "Desktop",
  "deviceIp": "192.168.1.100"
}
```

#### **PASO 4: Verificar Dispositivos Detectados**

```bash
curl -X GET "http://localhost:5000/api/p2p/devices"
```

**Respuesta:**
```json
[
  {
    "ip": "192.168.1.100",
    "name": "DESKTOP-PC1",
    "osType": "Windows"
  },
  {
    "ip": "192.168.1.101",
    "name": "LAPTOP-PC2",
    "osType": "Windows"
  },
  {
    "ip": "192.168.1.102",
    "name": "UBUNTU-SERVER",
    "osType": "Linux"
  }
]
```

#### **PASO 5: Obtener Todos los Dispositivos Registrados**

```bash
curl -X GET "http://localhost:5000/api/p2p"
```

**Respuesta:**
```json
[
  {
    "id": "20250120120530123-a1b2c3d4-e5f6",
    "deviceName": "PC1-Oficina",
    "deviceType": "Desktop",
    "deviceIp": "192.168.1.100"
  }
]
```

#### **PASO 6: Iniciar PC2 (igual que PC1)**

```bash
# En PC2 (192.168.1.101) - Terminal 1
cd csharp_backend/p2p.api
dotnet run

# En PC2 - Terminal 2
cd csharp_backend/p2p.services
dotnet run
```

#### **PASO 7: Transferencia de Archivo (Código C#)**

```csharp
// En una aplicación C# o servicio
using p2p.services;
using System.Net;

// PC1 - Enviar archivo
var fileTransfer = new FileTransfer();
fileTransfer.Port = 8080;
fileTransfer.LocalIp = IPAddress.Parse("192.168.1.100");

// Enviar archivo a PC2
await fileTransfer.SendFileAsync(
    "192.168.1.101", 
    "C:\\archivos\\documento.pdf"
);

Console.WriteLine("Archivo enviado correctamente");
```

```csharp
// En PC2 - Recibir archivo
var fileTransfer = new FileTransfer();
fileTransfer.Port = 8080;
fileTransfer.LocalIp = IPAddress.Parse("192.168.1.101");

// Escuchar transferencias en el puerto 8080
await fileTransfer.ReceiveFileAsync("C:\\descargas\\");

Console.WriteLine("Esperando archivos...");
// Se recibe el archivo automáticamente
```

#### **PASO 8: Verificar Transferencia Completada**

```bash
# Actualizar estado en BD
curl -X PUT "http://localhost:5000/api/p2p/20250120120530123-a1b2c3d4-e5f6" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "20250120120530123-a1b2c3d4-e5f6",
    "deviceName": "PC1-Oficina",
    "deviceType": "Desktop",
    "deviceIp": "192.168.1.100",
    "transferStatus": "completed"
  }'
```

###  Resultado Final

```
┌─────────────────────────────────────────────┐
│  PC1: documento.pdf (5 MB)                 │
│  ├─ Comprimido con Rust (50% reducción)   │
│  └─ Enviado a 192.168.1.101:8080          │
│                                             │
│  - Tiempo transferencia: ~2 segundos       │
│  - Archivo recibido en PC2                │
│  - Verificación: HASH OK                  │
│  - BD actualizada                         │
└─────────────────────────────────────────────┘
```

---

##  Configuración

### Backend: appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DB.db"
  },
  "P2P": {
    "Port": 8080,
    "EnableCompression": true,
    "MaxFileSize": 5368709120,
    "Timeout": 30000
  }
}
```

### Variables de Entorno

```bash
# Desarrollo
set ASPNETCORE_ENVIRONMENT=Development
set ASPNETCORE_URLS=http://localhost:5000;https://localhost:5001
set P2P_PORT=8080

# Linux/macOS
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS=http://localhost:5000;https://localhost:5001
export P2P_PORT=8080
```

### Frontend: astro.config.mjs

```javascript
import { defineConfig } from 'astro/config';

export default defineConfig({
  vite: {
    server: {
      proxy: {
        '/api': {
          target: 'http://localhost:5000',
          changeOrigin: true,
          rewrite: (path) => path.replace(/^\/api/, '')
        }
      }
    }
  }
});
```

---

##  API REST

### Endpoints Disponibles

| Método | Endpoint | Descripción | Respuesta |
|--------|----------|-------------|-----------|
| `GET` | `/api/p2p` | Obtiene todos los dispositivos | `200 OK` - Array de P2PItems |
| `GET` | `/api/p2p/{id}` | Obtiene dispositivo por ID | `200 OK` - P2PItem único o `404 Not Found` |
| `GET` | `/api/p2p/devices` | Escanea LAN y detecta dispositivos | `200 OK` - Array con ip, name, osType |
| `POST` | `/api/p2p` | Crea nuevo dispositivo (genera ID) | `201 Created` - P2PItemsDto |
| `PUT` | `/api/p2p/{id}` | Actualiza dispositivo existente | `204 No Content` |
| `DELETE` | `/api/p2p/{id}` | Elimina dispositivo | `204 No Content` o `404 Not Found` |

### Modelos de Datos

#### Solicitud POST/PUT (P2PItems)
```json
{
  "id": "20250120120530123-a1b2c3d4-e5f6",
  "deviceName": "PC-Oficina",
  "deviceType": "Desktop",
  "deviceIp": "192.168.1.100"
}
```

#### Respuesta GET (Array)
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

#### Respuesta Escaneo Dispositivos
```json
[
  {
    "ip": "192.168.1.100",
    "name": "DESKTOP-PC1",
    "osType": "Windows"
  },
  {
    "ip": "192.168.1.101",
    "name": "LAPTOP-PC2",
    "osType": "Windows"
  }
]
```

---

##  Roadmap

### Versión 4.1 (Próxima)
- [ ] Soporte para transferencias WAN seguras (VPN)
- [ ] Encriptación de archivos (AES-256)
- [ ] Autenticación de usuarios
- [ ] Historial completo de transferencias

### Versión 4.2
- [ ] Interfaz gráfica de escritorio (WPF/Avalonia)
- [ ] Sincronización automática de carpetas
- [ ] Cuota de almacenamiento por dispositivo
- [ ] Analytics de transferencias

### Versión 5.0
- [ ] Soporte para redes WAN públicas
- [ ] Blockchain para integridad de archivos
- [ ] API GraphQL
- [ ] Mobile apps (iOS/Android)

---

##  Contribución

¡Contribuciones bienvenidas! Sigue estos pasos:

1. **Fork** el proyecto
   ```bash
   # En GitHub, haz clic en "Fork"
   ```

2. **Clona tu fork**
   ```bash
   git clone https://github.com/tu-usuario/p2pv4.git
   cd p2pv4
   ```

3. **Crea una rama para tu feature**
   ```bash
   git checkout -b feature/tu-feature-amazing
   ```

4. **Realiza tus cambios**
   - Cumple con la documentación XML
   - Mantén consistencia de código
   - Añade pruebas si es necesario

5. **Commit con mensaje descriptivo**
   ```bash
   git commit -m "feat: Descripción de cambios
   
   - Cambio específico 1
   - Cambio específico 2"
   ```

6. **Push a tu rama**
   ```bash
   git push origin feature/tu-feature-amazing
   ```

7. **Abre un Pull Request**
   - Título claro
   - Descripción detallada
   - Referencias a issues relacionados

### Directrices de Desarrollo

- **Lenguaje de Commits**: Español/Inglés consistente
- **Formato Código**: Seguir estándares C# y Rust
- **Documentación**: Obligatorio comentarios XML en C#
- **Pruebas**: Añadir para nuevas funcionalidades

---

##  Licencia

Este proyecto está bajo licencia **MIT**. Ver [LICENCE](./LICENCE) para más detalles.

---

##  Soporte y Contacto

-  **Issues**: Reporta bugs en [GitHub Issues](../../issues)
-  **Discusiones**: Participa en [GitHub Discussions](../../discussions)
-  **Email**: contacto@proyecto-p2p.dev
-  **Wiki**: Documentación ampliada en [Wiki](../../wiki)

---

##  Estadísticas del Proyecto

- **Lenguajes**: C# (.NET), Rust, TypeScript (Astro), HTML/CSS
- **Licencia**: MIT  
- **Estado**: En desarrollo activo
- **Última actualización**: Enero 2026
- **Versión**: 4.0.0

---

**Tecnologías Principales**:
- .NET 10  - Backend robusto
- Rust - Optimización y compresión - en proceso
- Astro - Frontend moderno
- SQLite - Persistencia de datos

---

```
┌──────────────────────────────────────────┐
│   ¡Gracias por usar P2P! 🚀            │
│                                          │
│   ¿Preguntas? Abre un issue              │
│   ¿Ideas? Contribuye al proyecto        │
└──────────────────────────────────────────┘
```
