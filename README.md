# Analizador de Logging

Un analizador Roslyn para proyectos .NET que detecta y previene malas prácticas de logging en tiempo de compilación. Este analizador ayuda a asegurar un logging consistente, seguro y efectivo en toda tu base de código.

## Características

El analizador detecta los siguientes anti-patrones de logging:

### 534 LA0001: Uso de Console.WriteLine
**Error** - Detecta el uso de `Console.WriteLine` y `Console.Write` y sugiere usar logging estructurado en su lugar.

```csharp
// 6ab Malo
Console.WriteLine("Usuario ha iniciado sesión");

// 705 Bueno
_logger.LogInformation("Usuario ha iniciado sesión");
```

### 534 LA0002: Logging Incorrecto de Excepciones
**Error** - Detecta cuando las excepciones se registran sin detalles adecuados usando `ex.ToString()` o `ex.Message`.

```csharp
// 6ab Malo
_logger.LogError(ex);

// 705 Bueno
_logger.LogError(ex, "Ocurrió un error durante el procesamiento");
_logger.LogError(ex.ToString());
_logger.LogError(ex.Message);
```

### 534 LA0003: Logging de Información Sensible
**Error** - Detecta posible información sensible en los mensajes de log (contraseñas, tokens, claves API, correos electrónicos, tarjetas de crédito, SSN, etc.).

```csharp
// 6ab Malo
_logger.LogInformation("Contraseña de usuario: mimiclave123");
_logger.LogInformation("Clave API: sk-1234567890abcdef");
_logger.LogInformation("Email: usuario@ejemplo.com");

// 705 Bueno
_logger.LogInformation("Autenticación de usuario completada");
_logger.LogInformation("Solicitud de API procesada");
_logger.LogInformation("Correo enviado exitosamente");
```

### 534 LA0004: Niveles de Log Incorrectos
**Error** - Detecta cuando los niveles de log no coinciden con el contenido del mensaje.

```csharp
// 6ab Malo
_logger.LogInformation("Ocurrió un error durante el procesamiento");
_logger.LogError("Usuario inició sesión exitosamente");

// 705 Bueno
_logger.LogError("Ocurrió un error durante el procesamiento");
_logger.LogInformation("Usuario inició sesión exitosamente");
```

### 7e1 LA0005: Falta de Parámetros de Logging Estructurado
**Advertencia** - Detecta concatenación de strings en logging y sugiere usar parámetros estructurados.

```csharp
// 6ab Malo
_logger.LogInformation("Usuario " + userId + " inició sesión");

// 705 Bueno
_logger.LogInformation("Usuario {UserId} inició sesión", userId);
_logger.LogInformation($"Usuario {userId} inició sesión");
```

## Instalación

### Paquete NuGet
```bash
###compilar
dotnet build -c Release (desde la raiz del proyecto)
###contruir nugget
dotnet pack src/LoggingAnalyzer/LoggingAnalyzer.csproj -c Release
### utilizar 
### desde el proyecto que se donde se quiera utilizar  agregar el artefacto desde la carpeta release (se puede extraer el artefacto y guardarlo solo cambiar el path de importacion de paquete) 
dotnet  dotnet add package LoggingAnalyzer --source ../src/LoggingAnalyzer/bin/Release/

```

### Instalación Manual
1. Clona este repositorio
2. Compila la solución: `dotnet build`
3. Referencia el analizador en tu proyecto

## Configuración

### .editorconfig
El analizador puede configurarse usando `.editorconfig`:

```ini
[*.cs]
# Uso de Console.WriteLine - Error
dotnet_diagnostic.LA0001.severity = error

# Logging de excepciones sin detalles - Error
dotnet_diagnostic.LA0002.severity = error

# Logging de información sensible - Error
dotnet_diagnostic.LA0003.severity = error

# Niveles de log incorrectos - Error
dotnet_diagnostic.LA0004.severity = error

# Falta de parámetros estructurados - Advertencia
dotnet_diagnostic.LA0005.severity = warning

# Suprimir en archivos generados
[*.{Generated,Designer}.cs]
dotnet_diagnostic.LA0001.severity = none
dotnet_diagnostic.LA0002.severity = none
dotnet_diagnostic.LA0003.severity = none
dotnet_diagnostic.LA0004.severity = none
dotnet_diagnostic.LA0005.severity = none
```

### Suprimir Reglas Específicas
Puedes suprimir reglas específicas usando directivas `#pragma`:

```csharp
#pragma warning disable LA0001
Console.WriteLine("Esto está permitido aquí");
#pragma warning restore LA0001
```

O usando atributos:

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Logging", "LA0001")]
public void AlgúnMétodo()
{
    Console.WriteLine("Esto está permitido aquí");
}
```

## Correcciones Automáticas

El analizador provee correcciones automáticas para algunos problemas:

### Reemplazo de Console.WriteLine
Cuando se detecta un uso de `Console.WriteLine`, el analizador puede reemplazarlo automáticamente por logging estructurado:

```csharp
// Antes
Console.WriteLine("Usuario {0} inició sesión", userName);

// Después (con corrección)
_logger.LogInformation("Usuario {0} inició sesión", userName);
```

## Buenas Prácticas

### 1. Usa Logging Estructurado
```csharp
// Bueno
_logger.LogInformation("Usuario {UserId} inició sesión desde {IPAddress}", userId, ipAddress);

// Evita
_logger.LogInformation("Usuario " + userId + " inició sesión desde " + ipAddress);
```

### 2. Registra Excepciones Correctamente
```csharp
try
{
    // Alguna operación
}
catch (Exception ex)
{
    // Bueno - incluye detalles de la excepción
    _logger.LogError(ex, "No se pudo procesar la solicitud del usuario");
    
    // También es válido
    _logger.LogError("No se pudo procesar la solicitud del usuario: {Error}", ex.ToString());
}
```

### 3. Usa Niveles de Log Apropiados
- **Trace**: Información detallada de depuración
- **Debug**: Información general de depuración
- **Information**: Flujo general de la aplicación
- **Warning**: Situaciones inesperadas que no detienen la ejecución
- **Error**: Condiciones de error que no detienen la ejecución
- **Critical**: Errores críticos que pueden causar fallo de la aplicación

### 4. Nunca registres información sensible
```csharp
// Nunca registres:
// - Contraseñas
// - Claves API
// - Tokens
// - Números de tarjeta de crédito
// - Números de seguridad social
// - Correos electrónicos (en algunos contextos)
// - Información personal identificable
```

## Desarrollo

### Compilar desde el código fuente
```bash
 