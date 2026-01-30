#!/bin/bash
set -euo pipefail

# Usage: set REPO_URL and optionally APP_DIR env vars, or edit below
REPO_URL="${REPO_URL:-https://github.com/your-username/your-repo.git}"
APP_DIR="${APP_DIR:-/opt/pcm}"
ENV_SOURCE="${ENV_SOURCE:-/root/.env}"

echo "==> Deploy script starting"
echo "Repo: $REPO_URL"
echo "App dir: $APP_DIR"

if [ "$EUID" -ne 0 ]; then
  echo "Please run as root (sudo)" >&2
  exit 1
fi

apt-get update
apt-get install -y ca-certificates curl gnupg lsb-release apt-transport-https software-properties-common

if ! command -v docker >/dev/null 2>&1; then
  echo "Installing Docker Engine..."
  mkdir -p /etc/apt/keyrings
  curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg
  echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" \
    | tee /etc/apt/sources.list.d/docker.list > /dev/null
  apt-get update
  apt-get install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin
  systemctl enable --now docker
else
  echo "Docker already installed"
fi

mkdir -p "$APP_DIR"
chown root:root "$APP_DIR"

if [ -d "$APP_DIR/.git" ]; then
  echo "Updating existing repo"
  cd "$APP_DIR"
  git pull --rebase
else
  echo "Cloning repository"
  git clone "$REPO_URL" "$APP_DIR"
  cd "$APP_DIR"
fi

if [ -f "$ENV_SOURCE" ]; then
  echo "Copying env from $ENV_SOURCE"
  cp "$ENV_SOURCE" "$APP_DIR/.env"
else
  echo "No env file found at $ENV_SOURCE. Create $APP_DIR/.env with SA_PASSWORD before running docker compose."
fi

echo "Opening firewall (UFW) ports 22,80,443,8080"
if ! command -v ufw >/dev/null 2>&1; then
  apt-get install -y ufw
fi
ufw allow OpenSSH
ufw allow 80/tcp
ufw allow 443/tcp
ufw allow 8080/tcp
ufw --force enable || true

echo "Building and starting containers"
cd "$APP_DIR"
docker compose build --pull
docker compose up -d

echo "Deployment finished. Check containers with: docker compose ps"
echo "View logs: docker compose logs -f api"

exit 0
