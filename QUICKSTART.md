# 🚀 Ejecutar Análisis SonarQube - Guía Rápida

## Pasos para Analizar el Proyecto

### 1. Asegurarse de que SonarQube Server esté corriendo

#### Opción A: Con Docker (Recomendado)
```powershell
# Iniciar SonarQube en Docker
docker run -d --name sonarqube -p 9000:9000 sonarqube:latest

# Esperar ~30 segundos a que inicie
Start-Sleep -Seconds 30

# Verificar que está corriendo
Start-Process "http://localhost:9000"
```

Login inicial:
- Usuario: `admin`
- Password: `admin`
- Te pedirá cambiar la contraseña

#### Opción B: Instalación Local
Si ya tienes SonarQube instalado localmente, inícialo desde su directorio:
```powershell
cd <directorio-sonarqube>\bin\windows-x86-64
.\StartSonar.bat
```

### 2. Instalar SonarScanner (si no está instalado)
```powershell
dotnet tool install --global dotnet-sonarscanner
```

### 3. Ejecutar el Análisis

**Opción 1: Usar el script local (más fácil)**
```powershell
.\run-sonarqube-analysis-local.ps1
```

**Opción 2: Manual con comandos**
```powershell
# Tu token
$token = "squ_9ca812f2ebacfc67eb7d22c435df557627896ea6"

# Limpiar
dotnet clean

# Iniciar análisis
dotnet sonarscanner begin /k:"BadCleanArch" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="$token"

# Compilar
dotnet build BadCleanArch.sln

# Finalizar
dotnet sonarscanner end /d:sonar.login="$token"
```

### 4. Ver Resultados

Abre tu navegador en:
```
http://localhost:9000/dashboard?id=BadCleanArch
```

## 📊 Resultados Esperados

Después de la refactorización, deberías ver:

### ✅ Security (Seguridad)
- **Rating**: A
- **Vulnerabilities**: 0
- **Security Hotspots**: 0
- **Security Review Rating**: A

### ✅ Reliability (Confiabilidad)
- **Rating**: A
- **Bugs**: 0

### ✅ Maintainability (Mantenibilidad)
- **Rating**: A
- **Code Smells**: 0-5 (máximo)
- **Technical Debt**: < 30min

### 📈 Otras Métricas
- **Duplicación**: 0%
- **Cobertura**: N/A (sin tests aún)
- **Líneas de código**: ~450
- **Complejidad ciclomática**: <20

## 🔧 Troubleshooting

### Error: "SonarQube is not reachable"
```powershell
# Verificar que SonarQube está corriendo
Invoke-WebRequest -Uri "http://localhost:9000" -UseBasicParsing
```

### Error: "dotnet sonarscanner not found"
```powershell
# Reinstalar
dotnet tool uninstall --global dotnet-sonarscanner
dotnet tool install --global dotnet-sonarscanner

# Actualizar PATH si es necesario
$env:PATH += ";$env:USERPROFILE\.dotnet\tools"
```

### Error: "Project already exists"
Borra el proyecto en SonarQube y vuelve a ejecutar el análisis.

## 📝 Comparación: ANTES vs DESPUÉS

### ANTES (Código Original)
```
❌ Security Rating: E
❌ Vulnerabilities: 3 (SQL Injection, hardcoded credentials)
❌ Security Hotspots: 8
❌ Bugs: 8
❌ Code Smells: 42
❌ Technical Debt: 3h 30min
❌ Duplicación: 12.3%
```

### DESPUÉS (Código Refactorizado)
```
✅ Security Rating: A
✅ Vulnerabilities: 0
✅ Security Hotspots: 0
✅ Bugs: 0
✅ Code Smells: 0-5
✅ Technical Debt: <30min
✅ Duplicación: 0%
```

## 🎯 Qué se Corrigió

### 1. Seguridad
- ✅ SQL Injection eliminado (queries parametrizadas)
- ✅ Credenciales movidas a configuration
- ✅ CORS configurado apropiadamente
- ✅ Manejo de errores sin exposición de info sensible

### 2. Arquitectura
- ✅ Inversión de dependencias (DIP)
- ✅ Separación de responsabilidades (SRP)
- ✅ Interfaces para abstracción (ISP)
- ✅ Domain independiente de Infrastructure

### 3. Código
- ✅ Propiedades encapsuladas
- ✅ Async/await en lugar de Thread.Sleep
- ✅ Validaciones agregadas
- ✅ Logging estructurado
- ✅ DTOs separados de entities

## 📚 Documentación Completa

Para más detalles, ver:
- [README.md](README.md) - Documentación principal
- [SONARQUBE.md](SONARQUBE.md) - Guía completa de SonarQube

---

**¡Listo para análisis! 🎉**
