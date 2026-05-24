#!/bin/bash
# =============================================================
# SystemGym — Script de configuración para Ubuntu
# Ejecutar UNA SOLA VEZ después del primer despliegue
# =============================================================

set -e

# Ruta del proyecto (ajusta si es diferente)
PROJECT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
CURRENT_USER="$(whoami)"
SERVICE_NAME="systemgym"

echo "================================================"
echo "  SystemGym — Configuración de inicio automático"
echo "  Proyecto: $PROJECT_DIR"
echo "  Usuario:  $CURRENT_USER"
echo "================================================"

# 1. Asegurarse de que Docker inicia con el sistema
echo ""
echo "[1/4] Habilitando Docker en el arranque del sistema..."
sudo systemctl enable docker
sudo systemctl start docker
echo "      ✓ Docker habilitado"

# 2. Agregar usuario al grupo docker (si no está ya)
if ! groups "$CURRENT_USER" | grep -q docker; then
  echo ""
  echo "[2/4] Agregando '$CURRENT_USER' al grupo docker..."
  sudo usermod -aG docker "$CURRENT_USER"
  echo "      ✓ Usuario agregado al grupo docker"
  echo "      ⚠ IMPORTANTE: necesitas cerrar sesión y volver a entrar"
  echo "        para que este cambio tome efecto. Luego vuelve a ejecutar este script."
  exit 0
else
  echo ""
  echo "[2/4] Usuario '$CURRENT_USER' ya está en el grupo docker ✓"
fi

# 3. Instalar el servicio systemd
echo ""
echo "[3/4] Instalando servicio systemd para inicio automático..."

# Reemplazar %i con el usuario actual en el archivo de servicio
SERVICE_FILE="$PROJECT_DIR/linux/systemgym.service"
TMP_SERVICE="/tmp/${SERVICE_NAME}.service"

sed "s|WorkingDirectory=.*|WorkingDirectory=$PROJECT_DIR|g" "$SERVICE_FILE" > "$TMP_SERVICE"

sudo cp "$TMP_SERVICE" "/etc/systemd/system/${SERVICE_NAME}.service"
sudo systemctl daemon-reload
sudo systemctl enable "$SERVICE_NAME"
echo "      ✓ Servicio instalado y habilitado"

# 4. Levantar los contenedores ahora
echo ""
echo "[4/4] Levantando contenedores..."
cd "$PROJECT_DIR"
docker compose up -d
echo "      ✓ Contenedores corriendo"

echo ""
echo "================================================"
echo "  ✅ Configuración completada"
echo ""
echo "  SystemGym arrancará automáticamente cada vez"
echo "  que enciendas el computador."
echo ""
echo "  Comandos útiles:"
echo "    Ver estado:    docker compose ps"
echo "    Ver logs:      docker compose logs -f"
echo "    Detener:       docker compose down"
echo "    Iniciar:       docker compose up -d"
echo "================================================"
