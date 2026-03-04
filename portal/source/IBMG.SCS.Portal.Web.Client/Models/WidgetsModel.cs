// Copyright (c) IBMG. All rights reserved.

using System.Text.Json.Serialization;

namespace IBMG.SCS.Portal.Web.Client.Models
{
    public class WidgetsModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("isMandatory")]
        public bool IsMandatory { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; } = string.Empty;
    }
}