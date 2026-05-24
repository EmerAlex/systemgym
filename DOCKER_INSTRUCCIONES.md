# 🐳 DESPLIEGUE CON DOCKER
## SystemGym — Solo necesitas Docker instalado

---

## REQUISITO ÚNICO

Instalar **Docker Desktop** en Windows:
```
https://www.docker.com/products/docker-desktop/
```
> Nada más. Sin .NET, PostgreSQL, Node.js, ni nada adicional.

---

## PASO A PASO

### 1. Copiar el proyecto
Copiar la carpeta completa `SystemGym` al computador destino.

### 2. Abrir terminal en la carpeta
```powershell
cd C:\ruta\SystemGym
```

### 2.1 Verificar Docker
```powershell
docker --version
docker compose version
```
Si `docker` no se reconoce, Docker Desktop no está instalado, no está abierto, o la terminal se abrió antes de instalarlo. Cierra y abre la terminal nuevamente.

### 3. Iniciar todo
```powershell
docker compose up -d --build
```
> Primera vez: ~10 minutos (descarga imágenes + compila código)
> Siguientes veces: ~2 minutos

Si tu instalación usa el binario antiguo de Compose, ejecuta:
```powershell
docker-compose up -d --build
```

### 4. Verificar que todo está corriendo
```powershell
docker compose ps
```
Debe mostrar 3 servicios en estado `Up (healthy)`:
```
systemgym-postgres    Up (healthy)
systemgym-backend     Up (healthy)
systemgym-frontend    Up (healthy)
```

### 5. Abrir en el navegador
| Servicio   | URL                            |
|------------|-------------------------------|
| Aplicación | http://localhost:3000          |
| API Swagger| http://localhost:5000/swagger  |

---

## COMANDOS DEL DÍA A DÍA

```powershell
# Iniciar la aplicación
docker compose up -d

# Detener la aplicación (conserva los datos)
docker compose down

# Ver logs en tiempo real
docker compose logs -f

# Ver logs solo del backend
docker compose logs backend -f

# Ver logs solo del frontend
docker compose logs frontend -f

# Reiniciar un servicio específico
docker compose restart backend
docker compose restart frontend

# Ver estado de los contenedores
docker compose ps
```

---

## ACTUALIZAR CUANDO HAY CAMBIOS EN EL CÓDIGO

```powershell
# Reconstruir y reiniciar con los cambios nuevos
docker compose up -d --build
```

---

## BORRAR TODO (incluyendo datos de la BD)

```powershell
# ⚠️ ESTO BORRA TODOS LOS DATOS DE LA BASE DE DATOS
docker compose down --volumes
```

---

## BACKUP DE LA BASE DE DATOS

```powershell
# Crear backup
docker compose exec postgres pg_dump -U systemgym_user systemgym_db > backup.sql

# Restaurar backup
docker compose exec -T postgres psql -U systemgym_user systemgym_db < backup.sql
```

---

## SOLUCIÓN DE PROBLEMAS

### El servicio no inicia
```powershell
# Ver el error específico
docker compose logs backend
docker compose logs postgres
```

### Puerto ocupado
```powershell
# Si el puerto 3000 o 5000 está en uso, detener lo que lo usa:
netstat -ano | findstr :3000
netstat -ano | findstr :5000
taskkill /PID <numero_de_pid> /F
```

### Reconstruir desde cero
```powershell
docker compose down --volumes
docker compose up -d --build
```

### Docker no reconoce el comando
Verificar que Docker Desktop esté instalado y abierto (ícono en la barra de tareas).

Si `docker compose up -d --build` muestra un error como `unknown short flag: 'd' in -d`:
```powershell
docker --version
docker compose version
docker-compose --version
```
Usa una de estas opciones según el resultado:

```powershell
# Docker Compose v2
docker compose up -d --build

# Docker Compose v1
docker-compose up -d --build
```

Si ninguno de los dos comandos existe, instala Docker Desktop y vuelve a abrir la terminal.

---

## ESTRUCTURA DE ARCHIVOS DOCKER

```
SystemGym/
├── Dockerfile              ← Compila y empaqueta el backend (.NET)
├── Dockerfile.frontend     ← Compila y empaqueta el frontend (React)
├── nginx.conf              ← Configuración del servidor web del frontend
├── docker-compose.yml      ← Orquesta los 3 servicios juntos
└── .dockerignore           ← Excluye archivos innecesarios del build
```

---

## ARQUITECTURA

```
Tu navegador
     │
     ▼
http://localhost:3000
     │
┌────┴────┐     ┌──────────────┐     ┌──────────────┐
│  Nginx  │────▶│  .NET 10 API │────▶│ PostgreSQL 15│
│ (React) │     │  :5000       │     │  :5432       │
└─────────┘     └──────────────┘     └──────────────┘
     Red interna Docker (los contenedores se ven entre sí)
```
