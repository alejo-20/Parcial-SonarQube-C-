# Script de análisis SonarQube - EJEMPLO
# Copia este archivo como run-sonarqube-analysis.ps1 y configura tu token

$ErrorActionPreference = "Stop"

Write-Host "`n=== Iniciando análisis SonarQube ===" -ForegroundColor Green
Write-Host "Proyecto: BadCleanArch (Clean Architecture Refactored)" -ForegroundColor Cyan

# Configuración
$projectKey = "BadCleanArch"
$projectName = "Clean Architecture - Orders API"
$sonarUrl = "http://localhost:9000"

# OPCIÓN 1: Usar variable de entorno (RECOMENDADO)
# Ejecutar antes: $env:SONAR_TOKEN = "tu-token-aqui"
$sonarToken = $env:SONAR_TOKEN

# OPCIÓN 2: Hardcodear token (NO RECOMENDADO - Solo para desarrollo local)
if ([string]::IsNullOrEmpty($sonarToken)) {
    Write-Host "`nERROR: Token de SonarQube no configurado" -ForegroundColor Red
    Write-Host "`nConfigura el token usando una de estas opciones:" -ForegroundColor Yellow
    Write-Host "  1. Variable de entorno:" -ForegroundColor Cyan
    Write-Host "     `$env:SONAR_TOKEN = 'tu-token-aqui'" -ForegroundColor White
    Write-Host "     .\run-sonarqube-analysis.ps1" -ForegroundColor White
    Write-Host "`n  2. Pasar como parámetro:" -ForegroundColor Cyan
    Write-Host "     .\run-sonarqube-analysis.ps1 -Token 'tu-token-aqui'" -ForegroundColor White
    Write-Host "`n  3. Descomenta la línea en el script (NO recomendado):" -ForegroundColor Cyan
    Write-Host "     `$sonarToken = 'tu-token-aqui'" -ForegroundColor White
    exit 1
}

# Verificar que SonarScanner esté instalado
Write-Host "`nVerificando instalación de SonarScanner..." -ForegroundColor Yellow
try {
    $version = dotnet sonarscanner --version
    Write-Host "SonarScanner instalado: $version" -ForegroundColor Green
}
catch {
    Write-Host "SonarScanner no encontrado. Instalando..." -ForegroundColor Red
    dotnet tool install --global dotnet-sonarscanner
}

# Limpiar builds anteriores
Write-Host "`nLimpiando builds anteriores..." -ForegroundColor Yellow
dotnet clean BadCleanArch.sln

# Iniciar análisis
Write-Host "`nIniciando scanner SonarQube..." -ForegroundColor Yellow
dotnet sonarscanner begin `
    /k:"$projectKey" `
    /n:"$projectName" `
    /d:sonar.host.url="$sonarUrl" `
    /d:sonar.login="$sonarToken" `
    /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" `
    /d:sonar.coverage.exclusions="**/Program.cs,**/DTOs/**"

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nError al iniciar el scanner. Verifica que SonarQube esté corriendo en $sonarUrl" -ForegroundColor Red
    exit 1
}

# Compilar proyecto
Write-Host "`nCompilando proyecto..." -ForegroundColor Yellow
dotnet build BadCleanArch.sln --configuration Release --no-incremental

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nError al compilar el proyecto" -ForegroundColor Red
    exit 1
}

# Finalizar análisis
Write-Host "`nFinalizando análisis y enviando resultados..." -ForegroundColor Yellow
dotnet sonarscanner end /d:sonar.login="$sonarToken"

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nError al finalizar el análisis" -ForegroundColor Red
    exit 1
}

Write-Host "`n=== Análisis completado exitosamente ===" -ForegroundColor Green
Write-Host "`nResultados disponibles en: $sonarUrl/dashboard?id=$projectKey" -ForegroundColor Cyan
Write-Host "`nMétricas esperadas:" -ForegroundColor Yellow
Write-Host "  ✅ Security Rating: A (0 vulnerabilidades)" -ForegroundColor Green
Write-Host "  ✅ Reliability Rating: A (0 bugs)" -ForegroundColor Green
Write-Host "  ✅ Maintainability Rating: A (<5 code smells)" -ForegroundColor Green
Write-Host "  ✅ Security Hotspots: 0" -ForegroundColor Green
Write-Host "  ✅ Technical Debt: <30min" -ForegroundColor Green
Write-Host "`n"
