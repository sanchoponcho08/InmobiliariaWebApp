# InmobiliariaWebApp

Proyecto final para la materia Laboratorio de Programación. Se trata de un sistema web completo para la gestión de una inmobiliaria, desarrollado con ASP.NET Core y ADO.NET, siguiendo el patrón arquitectónico MVC.

## Tecnologías Utilizadas

- **Backend:** ASP.NET Core MVC (.NET 8)
- **Lenguaje:** C#
- **Acceso a Datos:** ADO.NET con el proveedor `MySql.Data`
- **Base de Datos:** MySQL / MariaDB
- **Frontend:** HTML, CSS, Bootstrap, Razor Pages

## Funcionalidades Implementadas

- **Gestión de Entidades (ABM/CRUD):**
  - Propietarios
  - Inquilinos
  - Inmuebles (con relación a Propietarios y Tipos)
  - Tipos de Inmueble
  - Contratos (con relación a Inquilinos e Inmuebles)
  - Pagos (asociados a Contratos)
- **Sistema de Usuarios y Seguridad:**
  - Registro y Login de usuarios con sistema de cookies.
  - Roles de **Administrador** y **Empleado**.
  - Edición de perfil de usuario (datos personales, cambio de contraseña y gestión de avatar).
  - Autorización por roles (e.g., solo administradores pueden eliminar registros o gestionar usuarios).
- **Lógica de Negocio Avanzada:**
  - Validación para evitar superposición de fechas en contratos.
  - Función para terminar contratos anticipadamente con cálculo y registro de multa.
  - Función para renovar contratos existentes.
- **Búsquedas y Reportes:**
  - Buscador de inmuebles disponibles por rango de fechas y otros criterios.
  - Reporte de contratos vigentes.
  - Reporte de contratos próximos a vencer.
  - Reporte de inmuebles por propietario.
  - Reporte de contratos por inmueble.
- **Auditoría y Usabilidad:**
  - Registro de qué usuario realiza las acciones clave.
  - Visualización de auditoría solo para administradores.
  - Notificaciones de éxito/error para mejorar la experiencia de usuario.

## Guía de Instalación y Ejecución

1.  **Prerrequisitos:**

    - Tener instalado el **SDK de .NET 8** (o superior).
    - Tener un servidor de **MySQL** o **MariaDB** en funcionamiento (ej. XAMPP).
    - Git.

2.  **Clonar el Repositorio:**

    ```sh
    git clone [https://github.com/sanchoponcho08/InmobiliariaWebApp.git](https://github.com/sanchoponcho08/InmobiliariaWebApp.git)
    cd InmobiliariaWebApp
    ```

3.  **Configurar la Base de Datos:**

    - Usando una herramienta como phpMyAdmin, crear una base de datos vacía llamada `inmobiliaria_db`.
    - Importar la estructura y datos iniciales usando el archivo `inmobiliaria_db.sql` que se encuentra en la raíz del proyecto.

4.  **Configurar la Conexión:**
    - Crear un archivo `appsettings.Development.json` en la raíz del proyecto.
    - Pegar el siguiente contenido y ajustar las credenciales si es necesario (por defecto para XAMPP es `root` sin contraseña).
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=127.0.0.1;Database=inmobiliaria_db;User=root;Password=;"
      }
    }
    ```
5.  **Ejecutar la Aplicación:**
    - Abrir una terminal en la raíz del proyecto y ejecutar el comando recomendado para desarrollo:
    ```sh
    dotnet watch
    ```
    - La aplicación estará disponible en la URL que indique la consola (ej. `http://localhost:5237`).

## Credenciales de Prueba

Para la evaluación del proyecto, se pueden utilizar los siguientes usuarios:

- **Rol Administrador:**

  - **Email:** admin@inmobiliaria.com
  - **Contraseña:** admin123

- **Rol Empleado:**
  - **Email:** empleadodelmes@inmobiliaria.com
  - **Contraseña:** contraseña
