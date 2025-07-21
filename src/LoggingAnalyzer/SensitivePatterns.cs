namespace LoggingAnalyzer
{
    public static class SensitivePatterns
    {
        public static readonly string[] SensitiveKeywords = {
            "password",
            "passwd",
            "pwd",
            "secret",
            "token",
            "key",
            "api_key",
            "apikey",
            "auth",
            "authorization",
            "credential",
            "private",
            "sensitive",
            "ssn",
            "social",
            "credit",
            "card",
            "cvv",
            "pin",
            "ssn",
            "social_security",
            "socialsecurity",
            "credit_card",
            "creditcard",
            "bank_account",
            "bankaccount",
            "account_number",
            "accountnumber",
            "routing",
            "swift",
            "iban",
            "cvv2",
            "cvc",
            "cid",
            "cvv_code",
            "cvvcode"
        };

        public static readonly string[] SensitivePatternsRegex = {
            @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", // Email
            @"\b\d{3}-\d{2}-\d{4}\b", // SSN format
            @"\b\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}\b", // Credit card
            @"\b\d{3}-\d{3}-\d{4}\b", // Phone number
            @"\b[A-Za-z0-9+/]{20,}={0,2}\b", // Base64 encoded strings
            @"\b[A-Fa-f0-9]{32}\b", // MD5 hash
            @"\b[A-Fa-f0-9]{40}\b", // SHA1 hash
            @"\b[A-Fa-f0-9]{64}\b", // SHA256 hash
            @"\b[A-Fa-f0-9]{128}\b" // SHA512 hash
        };
    }
} 