﻿namespace API_TSU_PassTracker.Infrastructure
{
    public class JwtOptions
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpiresHours { get; set; }
        public string Issuer { get; set; } = string.Empty; 
        public string Audience { get; set; } = string.Empty; 
    }
}
