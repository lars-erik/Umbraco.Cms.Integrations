﻿using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class TokenDto
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("isAccessTokenAvailable")]
        public bool IsAccessTokenAvailable => !string.IsNullOrEmpty(AccessToken);
    }
}