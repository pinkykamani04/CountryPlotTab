// Copyright (c) IBMG. All rights reserved.

using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IBMG.SCS.KerridgeApi.Server.Settings
{
    public sealed class NullableRefSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null)
            {
                return;
            }

            foreach (var prop in schema.Properties)
            {
                var propertyInfo = context.Type.GetProperty(
                    prop.Key,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (propertyInfo == null)
                {
                    continue;
                }

                if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null || propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string))
                {
                    prop.Value.Nullable = true;
                }
            }
        }
    }
}