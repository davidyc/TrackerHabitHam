# Start PostgreSQL in Docker for local testing
# Uses WorkoutConnection from appsettings.json

$composeFile = Join-Path (Split-Path $PSScriptRoot -Parent) "docker-compose.postgres.yml"

Write-Host "Starting PostgreSQL (TrackerHabiHamWorkouts)..." -ForegroundColor Cyan
docker compose -f $composeFile up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "PostgreSQL is running on localhost:5432" -ForegroundColor Green
    Write-Host "  - Database: TrackerHabiHamWorkouts"
    Write-Host "  - Username: postgres"
    Write-Host "  - Password: postgres"
    Write-Host ""
    Write-Host "To stop: docker compose -f docker-compose.postgres.yml down" -ForegroundColor Gray
} else {
    Write-Host "Startup failed. Make sure Docker is installed and running." -ForegroundColor Red
}
