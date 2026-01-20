# P2P Web - Interfaz de Gestión de Transferencia de Archivos P2P

Una aplicación web moderna construida con **Astro** y **Tailwind CSS** para gestionar la transferencia de archivos entre dispositivos en una red LAN (Local Area Network) de forma segura y eficiente.

## 📋 Descripción del Proyecto

P2P Web es la interfaz frontend de un sistema completo de transferencia de archivos punto a punto. Permite a los usuarios:

-  **Descubrir dispositivos** conectados en la red local
-  **Transferir archivos** entre dispositivos
-  **Gestionar carpetas** completas
-  **Soporte multiidioma** (Español e Inglés)
-  **Interfaz responsiva** optimizada para desktop, tablet y móvil

## 🏗️ Arquitectura del Proyecto

```
p2pWeb/
├── src/
│   ├── components/          # Componentes Astro reutilizables
│   │   ├── Getfile.astro    # Componente para seleccionar y enviar archivos
│   │   ├── main.astro       # Componente principal de bienvenida
│   │   └── NavBar.astro     # Barra de navegación
│   ├── layouts/
│   │   └── Layout.astro     # Layout base de la aplicación
│   ├── locales/
│   │   ├── en/common.json   # Traducciones en inglés
│   │   └── es/common.json   # Traducciones en español
│   ├── pages/
│   │   ├── index.astro      # Página de inicio
│   │   └── devices.astro    # Página de gestión de dispositivos
│   └── styles/
│       └── global.css       # Estilos globales
├── public/                  # Recursos estáticos
├── astro.config.mjs         # Configuración de Astro
├── tsconfig.json            # Configuración de TypeScript
└── package.json             # Dependencias y scripts
```

## 🔗 Conexión con el Backend

### Arquitectura General

El frontend **P2P Web** se comunica con un backend API REST desarrollado en **C# (.NET)** ubicado en `csharp_backend/p2p.api`.

```
┌─────────────┐                          ┌──────────────────┐
│  P2P Web    │  ◄──────HTTP/REST──────► │  Backend API C#  │
│  (Astro)    │                          │  (.NET Core)     │
└─────────────┘                          └──────────────────┘
                                                    │
                                                    ▼
                                         ┌──────────────────┐
                                         │  Base de Datos   │
                                         │  SQLite (DB.db)  │
                                         └──────────────────┘
```

### Endpoints API Disponibles

El backend expone los siguientes endpoints en `http://localhost:5000/api/p2p`:

#### 1. **GET /api/p2p/devices**
Obtiene la lista de dispositivos conectados en la red local.

```javascript
// Respuesta
{
  "ip": "192.168.1.100",
  "name": "Mi-PC",
  "osType": "Windows"
}
```

#### 2. **GET /api/p2p**
Obtiene todos los elementos/registros P2P almacenados.

```javascript
// Respuesta
[
  {
    "id": "1",
    "name": "archivo.txt",
    "size": 1024,
    ...
  }
]
```

#### 3. **GET /api/p2p/{id}**
Obtiene un elemento específico por ID.

#### 4. **PUT /api/p2p/{id}**
Actualiza un elemento específico.

### Configuración de la URL del Backend

La URL del backend se configura mediante variables de entorno:

**En el archivo `.env`:**
```
PUBLIC_URL=http://localhost:5000/api/p2p/devices
```

**En los componentes Astro:**
```astro
---
const URL_GET = import.meta.env.PUBLIC_URL;
---
```

### Ejemplo: Cargar Dispositivos

El archivo `src/pages/devices.astro` muestra cómo conectarse al backend:

```astro
<script>
  const URL = `${import.meta.env.PUBLIC_URL}`;

  async function loadDevices() {
    try {
      const response = await fetch(URL);
      const devices = await response.json();
      
      // Renderizar dispositivos en la UI
      const container = document.getElementById("devices-container");
      container.innerHTML = devices
        .map(device => `
          <div class="bg-blue-500 p-4 rounded">
            <p>Nombre: ${device.name}</p>
            <p>IP: ${device.ip}</p>
            <p>SO: ${device.osType}</p>
          </div>
        `)
        .join("");
    } catch (error) {
      console.error("Error al cargar dispositivos:", error);
    }
  }

  window.addEventListener("load", loadDevices);
</script>
```

## 🛠️ Stack Tecnológico

### Frontend
- **Astro 5.5.5** - Framework web moderno orientado al contenido
- **Tailwind CSS 4.0** - Framework CSS utilities
- **TypeScript** - Tipado estático opcional
- **astro-i18next** - Sistema de internacionalización multiidioma

### Backend
- **.NET 8+** - Framework web principal
- **Entity Framework Core** - ORM para acceso a datos
- **SQLite** - Base de datos embebida
- **ASP.NET Core Web API** - API REST

## 📦 Instalación y Configuración

### Requisitos Previos
- Node.js 18+ y pnpm
- Backend C# ejecutándose en `http://localhost:5000`

### Instalación

```bash
# Instalar dependencias
pnpm install

# Configurar variables de entorno
# Crear un archivo .env en la raíz del proyecto
echo "PUBLIC_URL=http://localhost:5000/api/p2p/devices" > .env
```

## 🚀 Comandos Disponibles

| Comando | Acción |
|---------|--------|
| `pnpm dev` | Inicia servidor de desarrollo en `http://localhost:3000` |
| `pnpm build` | Compila la aplicación para producción en `./dist/` |
| `pnpm preview` | Previsualiza el build localmente |
| `pnpm astro ...` | Ejecuta comandos Astro CLI |

## 🎨 Componentes Principales

### Getfile.astro
Componente para la selección y transferencia de archivos a dispositivos específicos. Permite:
- Seleccionar archivos individuales
- Seleccionar carpetas completas
- Especificar dispositivo de destino por IP

### NavBar.astro
Barra de navegación con:
- Links a páginas principales
- Selector de idioma
- Responsive design

### Layout.astro
Template base que proporciona:
- Estructura HTML
- Estilos globales
- Metadatos SEO
- Navbar común

## 🌍 Internacionalización

La aplicación soporta múltiples idiomas mediante `astro-i18next`:

- **Español** (`es/common.json`)
- **Inglés** (`en/common.json`)

Para añadir traducciones, edita los archivos JSON en `src/locales/`.

## 🔧 Configuración de Desarrollo

### astro.config.mjs
```javascript
export default defineConfig({
    vite: {
        plugins: [tailwindcss()],
        resolve: {
            alias: {
                "@styles": "../../src/styles"
            }
        },
    },
});
```

### Tailwind CSS
Configurado con `@tailwindcss/vite` para mejor rendimiento y desarrollo.

## 📱 Características de la UI

- **Responsive Design**: Funciona perfectamente en desktop, tablet y móvil
- **Grid Layout**: Sistema de grid para mostrar dispositivos
- **Transiciones Suaves**: Efectos hover y animaciones
- **Modo Oscuro Ready**: Estructura preparada para tema oscuro

## ⚙️ Backend - Endpoints Detallados

### CORS Configuration
El backend está configurado con CORS permisivo:
```csharp
policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
```

### Servicios Disponibles

1. **UniversalDeviceScanner**
   - Escanea la red LAN
   - Detecta dispositivos activos
   - Retorna IP, nombre y SO

2. **FileTransfer**
   - Gestiona transferencias de archivos
   - Compresión de archivos
   - Validación de integridad

3. **Lan**
   - Utilidades de red LAN
   - Obtención de IP local
   - Configuración de red

## 🔐 Seguridad

- CORS habilitado para desarrollo
- Validación de datos en el backend
- SQLite como BD local sin exposición
- API REST sin autenticación en desarrollo (considerar añadir en producción)

## 📊 Base de Datos

**SQLite** (`DB.db`):
- Tabla `P2PItems`: Registros de archivos transferidos
- Tabla `P2PContextModelSnapshot`: Snapshots de cambios

## 🚀 Despliegue

### Para Desarrollo
```bash
# Terminal 1: Backend
cd csharp_backend/p2p.api
dotnet run

# Terminal 2: Frontend
cd p2pWeb
pnpm dev
```

### Para Producción
```bash
# Compilar frontend
pnpm build

# Servir con servidor web (nginx, apache, etc.)
# Apuntar a la carpeta ./dist/
```

## 📝 Notas de Desarrollo

- El frontend espera que el backend esté accesible en `http://localhost:5000`
- Las variables de entorno se cargan en tiempo de build
- Astro genera HTML estático con island de JavaScript cuando es necesario
- La detección de dispositivos es en tiempo real desde la página de dispositivos

## 🐛 Troubleshooting

### Error de CORS
- Verificar que el backend está ejecutándose
- Verificar que la URL en `.env` es correcta
- Revisar que CORS está habilitado en Program.cs

### Dispositivos no se cargan
- Verificar conectividad con el backend
- Abrir DevTools (F12) para ver logs de error
- Verificar que el backend escanea correctamente la red LAN

## 📚 Recursos Útiles

- [Documentación de Astro](https://docs.astro.build)
- [Documentación de Tailwind CSS](https://tailwindcss.com/docs)
- [Documentación de astro-i18next](https://github.com/yassinedossaji/astro-i18next)
- [Documentación de .NET](https://learn.microsoft.com/dotnet/)

## 📄 Licencia

Consulta el archivo [LICENCE](../LICENCE) en la raíz del proyecto.
