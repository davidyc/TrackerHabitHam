# Stop PostgreSQL for local testing

$composeFile = Join-Path (Split-Path $PSScriptRoot -Parent) "docker-compose.postgres.yml"

Write-Host "Stopping PostgreSQL..." -ForegroundColor Cyan
docker compose -f $composeFile down
