# Sistema P2P con Backend .NET y Compresión en Rust

Sistema peer-to-peer (P2P) que implementa transferencia de archivos con un backend en C# (.NET) y un componente de compresión desarrollado en Rust.

## Estructura del Proyecto

```
├── csharp_backend/       # Backend en C# (.NET)
│   ├── p2p.data/        # Capa de acceso a datos
│   ├── p2p.services/    # Servicios de negocio
│   ├── p2p.web/         # API web y UI Blazor
│   └── BlazorApp1/      # Interfaz de usuario
└── rust_compressor/      # Componente de compresión en Rust
```

## Requisitos Previos

- .NET 6.0 o superior
- Rust (última versión estable)
- Visual Studio 2022 o VS Code con extensiones C# y Rust

## Instalación

1. Clonar el repositorio
2. Compilar el backend de C#:
```sh
cd csharp_backend
dotnet build
```

3. Compilar el componente Rust:
```sh
cd rust_compressor
cargo build
```

## Ejecución

1. Iniciar el backend:
```sh
cd csharp_backend/p2p.web
dotnet run
```

2. La aplicación estará disponible en `https://localhost:5001`

## Características

- Transferencia P2P de archivos
- Interfaz web con Blazor
- Compresión de archivos optimizada en Rust
- API REST para gestión de transferencias

## Tecnologías Utilizadas

- Backend: C# / .NET 6
- Frontend: Blazor
- Compresión: Rust
- Base de Datos: [Especificar DB]

## Contribución

1. Fork el proyecto
2. Crea tu rama de características (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request
