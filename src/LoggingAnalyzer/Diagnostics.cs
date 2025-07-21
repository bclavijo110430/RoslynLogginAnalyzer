using Microsoft.CodeAnalysis;

namespace LoggingAnalyzer
{
    public static class Diagnostics
    {
        public const string Category = "Logging";

        // Console.WriteLine usage
        public static readonly DiagnosticDescriptor ConsoleWriteLineRule = new(
            id: "LA0001",
            title: "Usa logging estructurado en lugar de Console.WriteLine",
            messageFormat: "Utiliza logging estructurado en vez de Console.WriteLine para una mejor observabilidad",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Console.WriteLine debe ser reemplazado por logging estructurado para mejorar la observabilidad y las capacidades de depuración.",
            helpLinkUri: "https://dev.azure.com/quironsalud/Arquitectura-Devops/_git/RoslynLogginAnalyzer?path=/docs/LA0001.md",
            customTags: new[] { WellKnownDiagnosticTags.Telemetry });

        // Exception logging without details
        public static readonly DiagnosticDescriptor ExceptionLoggingRule = new(
            id: "LA0002",
            title: "Registra los detalles de la excepción correctamente",
            messageFormat: "Registra los detalles de la excepción usando ex.ToString() o ex.Message para una mejor depuración",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Al registrar excepciones, incluye los detalles usando ex.ToString() o ex.Message para mejorar la depuración.",
            helpLinkUri: "https://dev.azure.com/quironsalud/Arquitectura-Devops/_git/RoslynLogginAnalyzer?path=/docs/LA0002.md",
            customTags: new[] { WellKnownDiagnosticTags.Telemetry });

        // Sensitive information logging
        public static readonly DiagnosticDescriptor SensitiveInfoRule = new(
            id: "LA0003",
            title: "No registres información sensible",
            messageFormat: "Se detectó posible información sensible en el mensaje de log: '{0}'",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Evita registrar información sensible como contraseñas, tokens, claves API, etc.",
            helpLinkUri: "https://dev.azure.com/quironsalud/Arquitectura-Devops/_git/RoslynLogginAnalyzer?path=/docs/LA0003.md",
            customTags: new[] { WellKnownDiagnosticTags.Telemetry });

        // Incorrect log levels
        public static readonly DiagnosticDescriptor IncorrectLogLevelRule = new(
            id: "LA0004",
            title: "Usa el nivel de log apropiado",
            messageFormat: "Considera usar {0} en vez de {1} para este tipo de mensaje",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Utiliza niveles de log apropiados según el contenido y contexto del mensaje.",
            helpLinkUri: "https://dev.azure.com/quironsalud/Arquitectura-Devops/_git/RoslynLogginAnalyzer?path=/docs/LA0004.md",
            customTags: new[] { WellKnownDiagnosticTags.Telemetry });

        // Missing structured logging parameters
        public static readonly DiagnosticDescriptor MissingStructuredParamsRule = new(
            id: "LA0005",
            title: "Usa parámetros estructurados en el logging",
            messageFormat: "Considera usar parámetros estructurados en vez de concatenación de strings",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Utiliza parámetros estructurados en el logging para mejorar la consulta y el rendimiento.",
            helpLinkUri: "https://dev.azure.com/quironsalud/Arquitectura-Devops/_git/RoslynLogginAnalyzer?path=/docs/LA0005.md",
            customTags: new[] { WellKnownDiagnosticTags.Telemetry });
    }
} 